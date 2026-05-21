using Application.DTOs.Responses;

namespace Application.Interfaces;

public interface IRechargePlanRecommendationService
{
    Task<ApiResponse<RechargePlanRecommendationResponse>> RecommendForCustomerAsync(
        long customerId,
        int top = 3,
        int recentTransactionLimit = 20,
        CancellationToken cancellationToken = default);
}
