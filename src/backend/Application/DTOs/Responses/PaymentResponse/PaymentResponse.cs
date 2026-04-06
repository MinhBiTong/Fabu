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

        public static PaymentResponse FromEntity(Payment payment, string? paymentUrl = null)
        {
            return new PaymentResponse
            {
                PaymentId = payment.Id,
                PaymentRef = payment.PaymentRef,
                Amount = payment.Amount,
                Status = payment.Status.ToString(),
                PaymentMethod = payment.PaymentMethod.ToString(),
                PaymentDate = payment.PaymentDate,
                PaymentUrl = paymentUrl,
                Message = payment.Status == StatusPayment.Completed ? "Payment Successfully" : "The payment is pending"
            };
        }
    }
}
