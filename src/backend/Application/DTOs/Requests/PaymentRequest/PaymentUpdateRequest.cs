using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.PaymentRequest
{
    //dung cho callbac tu payment gateway sau khi user thanh toan xong, update trang thai payment va transaction tuong ung
    public class PaymentUpdateRequest
    {
        public string PaymentRef { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;           // "Success", "Failed", "Cancelled"
        public string? GatewayResponseCode { get; set; }
        public string? GatewayTransactionId { get; set; }
        public Dictionary<string, string> RawData { get; set; } = new();
    }
}
