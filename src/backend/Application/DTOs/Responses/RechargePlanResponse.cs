namespace Application.DTOs.Responses
{
    public class RechargePlanResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Points { get; set; }
        public string? Description { get; set; }
    }
}