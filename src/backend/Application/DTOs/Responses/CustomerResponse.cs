namespace Application.DTOs.Responses
{
    public class CustomerResponse
    {
        public long Id { get; set; }
        public string MobileNumber { get; set; } = string.Empty;
        public string CustomerType { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public long? UserId { get; set; }
    }
}