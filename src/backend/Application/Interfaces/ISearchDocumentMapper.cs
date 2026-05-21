using Application.DTOs.Responses.Search;

namespace Application.Interfaces;

public interface ISearchDocumentMapper
{
    bool CanMap(object entity);
    SearchDocument? Map(object entity);
    string? GetDocumentId(object entity);
}
