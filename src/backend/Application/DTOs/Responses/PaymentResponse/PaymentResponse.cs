using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses.PaymentResponse
{
    public class PaymentResponse
    {
        public long PaymentId { get; set; }
        public string PaymentRef { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string? PaymentUrl { get; set; }           // URL chuyển hướng đến gateway
        public string? Message { get; set; }
        public long? CustomerId { get; set; }
        public Guid? OrderId { get; set; }
        public long? ServiceId { get; set; }
        public string? TransactionRef { get; set; }
        public string? CustomerType { get; set; }
        public decimal? AccountBalanceBefore { get; set; }
        public decimal? AccountBalanceAfter { get; set; }
        public decimal DiscountApplied { get; set; }

        public static PaymentResponse FromEntity(Payment payment, string? paymentUrl = null)
        {
            var transaction = payment.Transactions?.FirstOrDefault();
            return new PaymentResponse
            {
                PaymentId = payment.Id,
                PaymentRef = payment.PaymentRef,
                Amount = payment.Amount,
                Status = payment.Status.ToString(),
                PaymentMethod = payment.PaymentMethod.ToString(),
                PaymentDate = payment.PaymentDate,
                PaymentUrl = paymentUrl,
                CustomerId = transaction?.CustomerId,
                OrderId = transaction?.OrderId,
                ServiceId = transaction?.ServiceId,
                TransactionRef = transaction?.TransactionRef,
                Message = payment.Status == StatusPayment.Completed ? "Payment Successfully" : "The payment is pending"
            };
        }
    }
}
