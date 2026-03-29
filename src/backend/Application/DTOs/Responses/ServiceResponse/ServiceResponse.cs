namespace Application.DTOs.Response
{
	public class ServiceResponse
	{
		public int Id { get; set; }
		public string ServiceName { get; set; } = string.Empty;
		public string ServiceCode { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public string? Description { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}