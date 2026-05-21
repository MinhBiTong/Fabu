using Application.DTOs.Responses;
using Application.Models.Recommendations;

namespace Application.Interfaces;

public interface IRechargePlanRecommendationEngine
{
    IReadOnlyList<RechargePlanRecommendationItemResponse> Recommend(RechargePlanRecommendationContext context);
}
