using Application.DTOs.Responses;

namespace Application.DTOs.Requests;

public class CreateRechargePlanRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Points { get; set; } 
    public string? Description { get; set; }
}