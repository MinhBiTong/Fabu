using Application.DTOs;
using Application.DTOs.Requests;
using Application.DTOs.Requests.CustomerRequest;
using Application.DTOs.Responses;
using Application.Interfaces;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CustomerService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
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
            if (await _unitOfWork.Customers.ExistsByMobileAsync(request.MobileNumber))
                return ApiResponse<CustomerResponse>.Fail(400, "Phone number really exists");
            var customer = _mapper.Map<Customer>(request);
            if (customer == null)
                return ApiResponse<CustomerResponse>.Fail(404, "Customer not found.");
            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            var result = _mapper.Map<CustomerResponse>(customer);
            return ApiResponse<CustomerResponse>.Success(result, "Customer created successfully.");
        }

        public async Task<ApiResponse<bool>> UpdateAsync(long id, CustomerUpdateRequest request)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                return ApiResponse<bool>.Fail(404, "Customer not found");

            _mapper.Map(request, customer);
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Update customer successfully");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(long id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                return ApiResponse<bool>.Fail(404, "Customer not found");

            _unitOfWork.Customers.Delete(customer);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Xóa khách hàng thành công");
        }

        public async Task<ApiResponse<CustomerResponse>> VerifyOtpAndCreateCustomerAsync(long userId, string otp)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Kiểm tra User và OTP (giả sử đã validate OTP ở AuthService trước đó)
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    throw new AppException(ErrorCode.USER_NOT_EXISTED, "User really not exists");

                if (!user.IsActive)
                {
                    user.IsActive = true;
                    _unitOfWork.Users.Update(user);
                }

                // 2. Kiểm tra Customer đã tồn tại chưa (tránh tạo trùng)
                var existingCustomer = await _unitOfWork.Customers.GetByMobileNumberAsync(user.PhoneNumber);
                if (existingCustomer != null)
                {
                    await transaction.CommitAsync();
                    return ApiResponse<CustomerResponse>.Success(
                        _mapper.Map<CustomerResponse>(existingCustomer),
                        "Customer really exists.");
                }

                // 3. Tạo Customer mới từ thông tin User
                var customer = new Customer
                {
                    MobileNumber = user.PhoneNumber,
                    FullName = user.FullName,
                    CustomerType = "Prepaid",           // Mặc định là Prepaid khi mới tạo
                    UserId = user.Id,
                    Address = ""                        // Có thể cập nhật sau
                };

                await _unitOfWork.Customers.AddAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                // 4. Tạo Account cho Customer
                var account = new Account
                {
                    CustomerId = customer.Id,
                    Balance = 0,
                    CreditLimit = 0,
                    Status = StatusAccount.Active,
                    LastRechargeDate = null
                };

                await _unitOfWork.Accounts.AddAsync(account);
                await _unitOfWork.SaveChangesAsync();

                // 5. Cập nhật User.IsActive = true
                user.IsActive = true;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Successfully created Customer and Account for UserId: {UserId}", userId);

                var result = _mapper.Map<CustomerResponse>(customer);
                return ApiResponse<CustomerResponse>.Success(result, "OTP verification successful. Customer account created.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to verify OTP and create customer for UserId: {UserId}", userId);
                throw;
            }
        }

        public Task<ApiResponse<CustomerResponse>> GetByMobileNumberAsync(string mobileNumber)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<CustomerResponse>>> GetByUserIdAsync(long userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> LinkUserToCustomerAsync(long customerId, long userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> ExistsByMobileAsync(string mobile)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<CustomerResponse>>> GetActiveCustomersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<CustomerResponse>> GetWithAccountAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<CustomerResponse>>> GetTopCustomersBySpendingAsync(int top)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<CustomerResponse>>> GetCustomersWithUnpaidBillsAsync()
        {
            throw new NotImplementedException();
        }
    }
}