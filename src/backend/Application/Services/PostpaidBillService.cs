using Application.DTOs.Requests.PaymentRequest;
using Application.DTOs.Requests.PostpaidRequest;
using Application.DTOs.Responses.PaymentResponse;
using Application.DTOs.Responses.PostpaidResponse;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Application.Services
{
    public class PostpaidBillService : IPostpaidBillService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public PostpaidBillService(IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        public async Task<PostpaidBillResponse> CreateAsync(PostpaidCreateRequest request)
        {
            if (request.TotalAmount <= 0)
                throw new AppException(ErrorCode.INVALID_AMOUNT);

            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId);
            if (customer is null)
                throw new AppException(ErrorCode.CUSTOMER_NOT_FOUND);

            var bill = new PostpaidBill
            {
                CustomerId = request.CustomerId,
                BillMonth = new DateTime(request.BillMonth.Year, request.BillMonth.Month, 1),
                DueDate = request.DueDate,
                TotalAmount = request.TotalAmount,
                PaidAmount = 0,
                Status = StatusPostpaid.Unpaid
            };

            await _unitOfWork.PostpaidBills.AddAsync(bill);
            await _unitOfWork.SaveChangesAsync();

            return PostpaidBillResponse.FromEntity(bill);
        }

        public async Task<PostpaidBillResponse> GetLatestBillAsync(long customerId)
        {
            var bill = await _unitOfWork.PostpaidBills.GetLatestBillAsync(customerId);
            if (bill is null)
                throw new AppException(ErrorCode.INVALID_KEY, "Postpaid bill not found.");

            return PostpaidBillResponse.FromEntity(bill);
        }

        public async Task<List<PostpaidBillResponse>> GetOverdueBillsAsync()
        {
            var bills = await _unitOfWork.PostpaidBills.GetOverdueBillsAsync();
            return bills.Select(PostpaidBillResponse.FromEntity).ToList();
        }

        public async Task<List<PostpaidBillResponse>> GetUnpaidBillsByCustomerAsync(long customerId)
        {
            var bills = await _unitOfWork.PostpaidBills.GetUnpaidBillsByCustomerAsync(customerId);
            return bills.Select(PostpaidBillResponse.FromEntity).ToList();
        }

        public async Task<PaymentResponse> PayBillAsync(long billId, PostpaidPaymentRequest request)
        {
            var bill = await _unitOfWork.PostpaidBills.GetByIdAsync(billId);
            if (bill is null)
                throw new AppException(ErrorCode.INVALID_KEY, "Postpaid bill not found.");

            if (bill.Status == StatusPostpaid.Paid)
                throw new AppException(ErrorCode.INVALID_AMOUNT, "Postpaid bill has already been paid.");

            var remaining = bill.TotalAmount - bill.PaidAmount;
            if (request.Amount > remaining)
                throw new AppException(ErrorCode.INVALID_AMOUNT, "Payment amount is greater than remaining bill amount.");

            return await _paymentService.CreatePaymentAsync(new PaymentCreateRequest
            {
                CustomerId = bill.CustomerId,
                BillId = bill.Id,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                UseAccountBalance = request.UseAccountBalance,
                CouponCode = request.CouponCode,
                TransactionType = TransactionType.BillPayment,
                OrderInfo = $"Fabu postpaid bill {bill.Id:000000}"
            });
        }

        public async Task UpdateBillStatusAfterPayment(long billId, decimal paidAmount)
        {
            var bill = await _unitOfWork.PostpaidBills.GetByIdAsync(billId);
            if (bill == null) return;

            bill.PaidAmount += paidAmount;

            if (bill.PaidAmount >= bill.TotalAmount)
                bill.Status = StatusPostpaid.Paid;
            else if (bill.PaidAmount > 0)
                bill.Status = StatusPostpaid.Partial;
        }
    }
}
