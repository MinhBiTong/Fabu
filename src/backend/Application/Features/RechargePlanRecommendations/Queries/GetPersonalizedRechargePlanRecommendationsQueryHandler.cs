using Application.DTOs.Responses;
using Application.Interfaces;
using MediatR;

namespace Application.Features.RechargePlanRecommendations.Queries;

public sealed class GetPersonalizedRechargePlanRecommendationsQueryHandler
    : IRequestHandler<GetPersonalizedRechargePlanRecommendationsQuery, ApiResponse<RechargePlanRecommendationResponse>>
{
    private readonly IRechargePlanRecommendationService _recommendationService;

    public GetPersonalizedRechargePlanRecommendationsQueryHandler(
        IRechargePlanRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    public Task<ApiResponse<RechargePlanRecommendationResponse>> Handle(
        GetPersonalizedRechargePlanRecommendationsQuery request,
        CancellationToken cancellationToken)
    {
        return _recommendationService.RecommendForCustomerAsync(
            request.CustomerId,
            request.Top,
            request.RecentTransactionLimit,
            cancellationToken);
    }
}
