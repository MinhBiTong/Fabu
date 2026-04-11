namespace Application.DTOs.Response
{
	public class ServiceResponse
	{
        public int Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceCode { get; set; } = string.Empty;

        public string Category { get; set; }
        public int DataAmountMB { get; set; }
        public int ValidityDays { get; set; }

        public decimal Price { get; set; }
        public string Description { get; set; }

        public bool IsAutoRenew { get; set; }
        public int MaxActivationsPerMonth { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}