namespace Application.DTOs.Responses
{
    public class FeedbackResponse
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Content { get; set; }
        public int Rating { get; set; }
        public long CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}