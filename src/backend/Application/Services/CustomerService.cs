//using Application.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Application.Services
//{
//    public class CustomerService : ICustomerService
//    {
//    }
//}
using Application.DTOs;
using Application.DTOs.Requests;
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
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
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

        public Task<ApiResponse<bool>> UpdateAsync(long id, CustomerUpdateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}