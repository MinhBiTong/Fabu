namespace Application.DTOs.Requests.CouponRequest
{
    public class CouponUpdateRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MinRechargeAmount { get; set; }
        public decimal? MaxDiscount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int UsageLimitPerUser { get; set; }
        public int? UsageLimitTotal { get; set; }
        public bool IsActive { get; set; }
    }
}
