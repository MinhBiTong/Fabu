using Application.DTOs.Responses;
using Application.DTOs;
using Application.DTOs.Requests;
using Application.DTOs.Requests.ServiceRequest;
using Application.DTOs.Response;
using Application.Interfaces;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServiceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ApiResponse<List<ServiceResponse>>> GetAllAsync()
        {
            var services = await _unitOfWork.Services.GetAllAsync();
            var result = _mapper.Map<List<ServiceResponse>>(services);
            return ApiResponse<List<ServiceResponse>>.Success(result);
        }

        public async Task<ApiResponse<ServiceResponse>> GetByIdAsync(long id)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(id);
            if (service == null)
                return ApiResponse<ServiceResponse>.Fail(404, "Service not found.");

            var result = _mapper.Map<ServiceResponse>(service);
            return ApiResponse<ServiceResponse>.Success(result);
        }

        public async Task<ApiResponse<ServiceResponse>> CreateAsync(ServiceCreateRequest request)
        {
            var service = _mapper.Map<Service>(request);
            await _unitOfWork.Services.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();
            var result = _mapper.Map<ServiceResponse>(service);
            return ApiResponse<ServiceResponse>.Success(result, "Service created successfully.");
        }


        public async Task<ApiResponse<ServiceResponse>> UpdateAsync(long id, ServiceCreateRequest request)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(id);

            if (service == null)
                return ApiResponse<ServiceResponse>.Fail(404, "Service not found.");

            _mapper.Map(request, service);

            _unitOfWork.Services.Update(service);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<ServiceResponse>(service);

            return ApiResponse<ServiceResponse>.Success(result, "Service updated successfully.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(long id)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(id);

            if (service == null)
                return ApiResponse<bool>.Fail(404, "Service not found.");

            _unitOfWork.Services.Remove(service);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Service deleted successfully.");
        }

        public Task<ApiResponse<List<ServiceResponse>>> GetActiveServicesByCategoryAsync(string category)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<ServiceResponse>>> GetPopularServicesAsync(int top)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ServiceResponse>> GetByCodeAsync(string code)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> IsServiceActiveAsync(long serviceId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<ServiceResponse>>> SearchServicesAsync(string keyword)
        {
            throw new NotImplementedException();
        }
    }


}