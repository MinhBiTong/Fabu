namespace Application.DTOs.Requests.CustomerRequest
{
    public class CustomerUpdateRequest
    {
        public long Id { get; set; }
        public string MobileNumber { get; set; }
        public string CustomerType { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
    }
}