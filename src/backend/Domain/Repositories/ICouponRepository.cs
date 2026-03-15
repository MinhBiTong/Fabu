using Domain.Abstractions.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface ICouponRepository : IRepositoryBase<Coupon, long>
    {
        IQueryable<Coupon> GetAll();
    }
}
