using Domain.ValueObjects;

namespace Application.DTOs.Requests.FeedbackRequest
{
    public class FeedbackUpdateRequest
    {
        public string Content { get; set; }
        public int Rating { get; set; }
        public StatusFeedback Status { get; set; }
    }
}