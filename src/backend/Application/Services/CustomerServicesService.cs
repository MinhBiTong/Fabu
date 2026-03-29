using Application.DTOs;
using Application.DTOs.Requests.CustomerServiceRequest;
using Application.DTOs.Responses;
using Application.Interfaces;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
	public class CustomerServicesService : ICustomerServicesService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public CustomerServicesService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ApiResponse<List<CustomerServiceResponse>>> GetAllAsync()
		{
			var customerServices = await _unitOfWork.CustomerServices.GetAllAsync();
			var result = _mapper.Map<List<CustomerServiceResponse>>(customerServices);
			return ApiResponse<List<CustomerServiceResponse>>.Success(result);
		}

		public async Task<ApiResponse<CustomerServiceResponse>> GetByIdAsync(long id)
		{
			var customerService = await _unitOfWork.CustomerServices.GetByIdAsync(id);
			if (customerService == null)
				return ApiResponse<CustomerServiceResponse>.Fail(404, "Customer Service record not found.");

			var result = _mapper.Map<CustomerServiceResponse>(customerService);
			return ApiResponse<CustomerServiceResponse>.Success(result);
		}

		public async Task<ApiResponse<CustomerServiceResponse>> CreateAsync(CustomerServiceCreateRequest request)
		{
			var customerService = _mapper.Map<CustomerService>(request);
			await _unitOfWork.CustomerServices.AddAsync(customerService);
			await _unitOfWork.SaveChangesAsync();
			var result = _mapper.Map<CustomerServiceResponse>(customerService);
			return ApiResponse<CustomerServiceResponse>.Success(result, "Customer Service mapped successfully.");
		}
	}
}