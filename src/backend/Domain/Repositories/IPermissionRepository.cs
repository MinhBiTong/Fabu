using Domain.Abstractions;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IPermissionRepository : IRepositoryBase<Permission, int>
    {

    }
}
