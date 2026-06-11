using Domain.Entities;

namespace Application.DTOs.Responses.PostpaidResponse
{
    public class PostpaidBillResponse
    {
        public long Id { get; set; }
        public long? CustomerId { get; set; }
        public DateTime BillMonth { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Status { get; set; } = string.Empty;

        public static PostpaidBillResponse FromEntity(PostpaidBill bill)
        {
            return new PostpaidBillResponse
            {
                Id = bill.Id,
                CustomerId = bill.CustomerId,
                BillMonth = bill.BillMonth,
                DueDate = bill.DueDate,
                TotalAmount = bill.TotalAmount,
                PaidAmount = bill.PaidAmount,
                RemainingAmount = Math.Max(0, bill.TotalAmount - bill.PaidAmount),
                Status = bill.Status.ToString()
            };
        }
    }
}
