//using Domain.Abstractions;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Domain.Entities
//{
//    public class CustomerService : EntityAuditBase<long>
//    {
//        public long? CustomerId { get; set; }
//        public virtual Customer? Customer { get; set; }

//        public long? ServiceId { get; set; }
//        public virtual Service? Service { get; set; }

//        public DateTime ActivatedAt { get; set; } = DateTime.UtcNow;

//        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;

//        public bool IsAutoRenewed { get; set; } // 0: No, 1: Yes
//    }
//}

using Application.DTOs;
using Application.DTOs.Requests.CustomerRequest;
using Application.DTOs.Responses;
using Application.Interfaces;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CustomersService : ICustomersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomersService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<CustomerResponse>>> GetAllAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            var result = _mapper.Map<List<CustomerResponse>>(customers);
            return ApiResponse<List<CustomerResponse>>.Success(result);
        }

        public async Task<ApiResponse<CustomerResponse>> GetByIdAsync(long id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null) return ApiResponse<CustomerResponse>.Fail(404, "Customer not found.");
            var result = _mapper.Map<CustomerResponse>(customer);
            return ApiResponse<CustomerResponse>.Success(result);
        }

        public async Task<ApiResponse<CustomerResponse>> CreateAsync(CustomerCreateRequest request)
        {
            var customer = _mapper.Map<Customer>(request);
            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            var result = _mapper.Map<CustomerResponse>(customer);
            return ApiResponse<CustomerResponse>.Success(result, "Customer created successfully.");
        }
        [HttpGet("category/{category}/active")]
        public async Task<IActionResult> GetActiveByCategory(string category) => Ok(await _serviceService.GetActiveServicesByCategoryAsync(category));

        [HttpGet("popular/{top}")]
        public async Task<IActionResult> GetPopular(int top) => Ok(await _serviceService.GetPopularServicesAsync(top));

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code) => Ok(await _serviceService.GetByCodeAsync(code));

        [HttpGet("{id}/is-active")]
        public async Task<IActionResult> IsActive(long id) => Ok(await _serviceService.IsServiceActiveAsync(id));

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword) => Ok(await _serviceService.SearchServicesAsync(keyword));

    }
}