
using Domain.Abstractions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class CustomerService : EntityAuditSoftDeleteBase<long>
    {
        public long CustomerId { get; set; }
        public long ServiceId { get; set; }

        public DateTime ActivatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsAutoRenewed { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service? Service { get; set; }
        public async Task<ApiResponse<CustomerResponse>> GetByMobileNumberAsync(string mobileNumber)
        {
            var customer = await _unitOfWork.Customers.GetByMobileNumberAsync(mobileNumber);
            if (customer == null) return ApiResponse<CustomerResponse>.Fail(404, "Không tìm thấy khách hàng.");
            return ApiResponse<CustomerResponse>.Success(_mapper.Map<CustomerResponse>(customer));
        }

        public async Task<ApiResponse<List<CustomerResponse>>> GetByUserIdAsync(long userId)
        {
            var customers = await _unitOfWork.Customers.GetByUserIdAsync(userId);
            return ApiResponse<List<CustomerResponse>>.Success(_mapper.Map<List<CustomerResponse>>(customers));
        }

        public async Task<ApiResponse<bool>> LinkUserToCustomerAsync(long customerId, long userId)
        {
            await _unitOfWork.Customers.LinkUserToCustomerAsync(customerId, userId);
            await _unitOfWork.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "Liên kết thành công.");
        }

        public async Task<ApiResponse<bool>> ExistsByMobileAsync(string mobile)
        {
            var exists = await _unitOfWork.Customers.ExistsByMobileAsync(mobile);
            return ApiResponse<bool>.Success(exists);
        }

        public async Task<ApiResponse<List<CustomerResponse>>> GetActiveCustomersAsync()
        {
            var customers = await _unitOfWork.Customers.GetActiveCustomersAsync();
            return ApiResponse<List<CustomerResponse>>.Success(_mapper.Map<List<CustomerResponse>>(customers));
        }

        public async Task<ApiResponse<CustomerResponse>> GetWithAccountAsync(long customerId)
        {
            var customer = await _unitOfWork.Customers.GetWithAccountAsync(customerId);
            if (customer == null) return ApiResponse<CustomerResponse>.Fail(404, "Không tìm thấy.");
            return ApiResponse<CustomerResponse>.Success(_mapper.Map<CustomerResponse>(customer));
        }

        public async Task<ApiResponse<List<CustomerResponse>>> GetTopCustomersBySpendingAsync(int top)
        {
            var customers = await _unitOfWork.Customers.GetTopCustomersBySpendingAsync(top);
            return ApiResponse<List<CustomerResponse>>.Success(_mapper.Map<List<CustomerResponse>>(customers));
        }

        public async Task<ApiResponse<List<CustomerResponse>>> GetCustomersWithUnpaidBillsAsync()
        {
            var customers = await _unitOfWork.Customers.GetCustomersWithUnpaidBillsAsync();
            return ApiResponse<List<CustomerResponse>>.Success(_mapper.Map<List<CustomerResponse>>(customers));
        }
    }
}
