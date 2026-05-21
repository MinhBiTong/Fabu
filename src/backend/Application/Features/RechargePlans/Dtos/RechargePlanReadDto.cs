namespace Application.Features.RechargePlans.Dtos;

public sealed class RechargePlanReadDto
{
    public long Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BonusAmount { get; set; }
    public int? ValidityDays { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
