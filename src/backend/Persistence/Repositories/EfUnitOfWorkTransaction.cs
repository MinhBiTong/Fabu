using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class EfUnitOfWorkTransaction : IUnitOfWorkTransaction
    {
        private readonly Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction _transaction;
        private readonly IUnitOfWork _unitOfWork;

        public EfUnitOfWorkTransaction(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction, IUnitOfWork unitOfWork)
        {
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task CommitAsync()
        {
            await _unitOfWork.CommitAsync();
            await _transaction.CommitAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _transaction.DisposeAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaction.DisposeAsync();
            await _unitOfWork.RollbackAsync();
        }
    }
}
