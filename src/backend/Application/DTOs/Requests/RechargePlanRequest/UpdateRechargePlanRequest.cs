namespace Application.DTOs.Requests;

public class UpdateRechargePlanRequest
{
    public long Id { get; set; }
    public string PlanName { get; set; }
    public decimal Amount { get; set; }
    public decimal BonusAmount { get; set; }
    public int? ValidityDays { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
}