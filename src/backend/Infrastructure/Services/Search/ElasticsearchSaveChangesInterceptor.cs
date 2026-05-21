using Application.Interfaces;
using Domain.Abstractions.Entities;
using Domain.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Search;

public sealed class ElasticsearchSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ISearchIndexService _searchIndex;
    private readonly ISearchDocumentMapper _mapper;
    private readonly ElasticsearchConfiguration _configuration;
    private readonly ILogger<ElasticsearchSaveChangesInterceptor> _logger;
    private readonly List<PendingSearchChange> _pendingChanges = new();

    public ElasticsearchSaveChangesInterceptor(
        ISearchIndexService searchIndex,
        ISearchDocumentMapper mapper,
        IOptions<ElasticsearchConfiguration> options,
        ILogger<ElasticsearchSaveChangesInterceptor> logger)
    {
        _searchIndex = searchIndex;
        _mapper = mapper;
        _configuration = options.Value;
        _logger = logger;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        CaptureChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        CaptureChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        _ = SyncPendingChangesAsync(CancellationToken.None);
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await SyncPendingChangesAsync(cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        _pendingChanges.Clear();
        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        _pendingChanges.Clear();
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private void CaptureChanges(DbContext? context)
    {
        _pendingChanges.Clear();

        if (context is null ||
            !_configuration.Enabled ||
            !_configuration.SyncOnSave ||
            !_searchIndex.IsEnabled)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (!_mapper.CanMap(entry.Entity))
            {
                continue;
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                var shouldDelete = entry.Entity is ISoftDelete softDelete && softDelete.IsDeleted;
                _pendingChanges.Add(new PendingSearchChange(entry, shouldDelete));
                continue;
            }

            if (entry.State == EntityState.Deleted)
            {
                _pendingChanges.Add(new PendingSearchChange(entry, true));
            }
        }
    }

    private async Task SyncPendingChangesAsync(CancellationToken cancellationToken)
    {
        if (_pendingChanges.Count == 0)
        {
            return;
        }

        var changes = _pendingChanges.ToList();
        _pendingChanges.Clear();

        foreach (var change in changes)
        {
            try
            {
                if (change.ShouldDelete)
                {
                    var documentId = _mapper.GetDocumentId(change.Entry.Entity);
                    if (!string.IsNullOrWhiteSpace(documentId))
                    {
                        await _searchIndex.DeleteAsync(documentId, cancellationToken);
                    }

                    continue;
                }

                var document = _mapper.Map(change.Entry.Entity);
                if (document is not null)
                {
                    await _searchIndex.UpsertAsync(document, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Realtime Elasticsearch sync failed for entity {EntityType}",
                    change.Entry.Entity.GetType().Name);
            }
        }
    }

    private sealed record PendingSearchChange(EntityEntry Entry, bool ShouldDelete);
}
