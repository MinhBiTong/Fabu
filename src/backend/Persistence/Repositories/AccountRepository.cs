using Domain.Entities;
using Domain.Repositories;
using Microsoft.Identity.Client;
using Persistence.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class AccountRepository : BaseRepository<Account, long>, IAccountRepository
    {
        public AccountRepository(AppDbContext context) : base(context)
        {
        }

        public string Username => throw new NotImplementedException();

        public string Environment => throw new NotImplementedException();

        public AccountId HomeAccountId => throw new NotImplementedException();
    }
}
