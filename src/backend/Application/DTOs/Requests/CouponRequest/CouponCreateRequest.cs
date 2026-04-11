namespace Application.DTOs.Requests.CouponRequest
{
    public class CouponCreateRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal MinRechargeAmount { get; set; }
        public decimal? MaxDiscount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int UsageLimitPerUser { get; set; }
        public int? UsageLimitTotal { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
