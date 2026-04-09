using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class CustomerServiceRepository : BaseRepository<CustomerService, long>, ICustomerServiceRepository
    {
        private readonly AppDbContext _context;

        public CustomerServiceRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task CancelServiceAsync(long customerId, long serviceId)
        {
            var record = await _context.Set<CustomerService>()
                .FirstOrDefaultAsync(cs => cs.CustomerId == customerId && cs.ServiceId == serviceId);

            if (record != null)
            {
                // Giả sử bạn có cột Status. Sửa thành thuộc tính thực tế của bạn.
                // record.Status = "Cancelled"; 
                _context.Set<CustomerService>().Update(record);
            }
        }

        public async Task<List<CustomerService>> GetActiveServicesByCustomerAsync(long customerId)
        {
            return await _context.Set<CustomerService>()
                .Where(cs => cs.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<List<CustomerService>> GetExpiringServicesAsync()
        {
            var targetDate = DateTime.UtcNow.AddDays(7);
            return await _context.Set<CustomerService>()
                // .Where(cs => cs.EndDate <= targetDate) 
                .ToListAsync();
        }

        public async Task<bool> IsServiceRegisteredAsync(long customerId, long serviceId)
        {
            return await _context.Set<CustomerService>()
                .AnyAsync(cs => cs.CustomerId == customerId && cs.ServiceId == serviceId);
        }

        public async Task RegisterServiceAsync(long customerId, long serviceId)
        {
            var newRegistration = new CustomerService
            {
                CustomerId = customerId,
                ServiceId = serviceId,
                // StartDate = DateTime.UtcNow,
                // Status = "Active"
            };
            await _context.Set<CustomerService>().AddAsync(newRegistration);
        }
    }
}