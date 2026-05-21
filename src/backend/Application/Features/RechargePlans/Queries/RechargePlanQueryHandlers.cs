using Application.DTOs.Responses;
using Application.Features.RechargePlans.Dtos;
using AutoMapper;
using Domain.Abstractions;
using MediatR;

namespace Application.Features.RechargePlans.Queries;

public sealed class RechargePlanQueryHandlers :
    IRequestHandler<GetAllRechargePlansQuery, ApiResponse<List<RechargePlanReadDto>>>,
    IRequestHandler<GetRechargePlanByIdQuery, ApiResponse<RechargePlanReadDto>>,
    IRequestHandler<GetActiveRechargePlansQuery, ApiResponse<List<RechargePlanReadDto>>>,
    IRequestHandler<GetRechargePlanByAmountQuery, ApiResponse<RechargePlanReadDto>>,
    IRequestHandler<GetRechargePlansByPriceRangeQuery, ApiResponse<List<RechargePlanReadDto>>>,
    IRequestHandler<GetPopularRechargePlansQuery, ApiResponse<List<RechargePlanReadDto>>>,
    IRequestHandler<GetRechargePlansByProviderQuery, ApiResponse<List<RechargePlanReadDto>>>,
    IRequestHandler<IsRechargePlanActiveQuery, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RechargePlanQueryHandlers(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<RechargePlanReadDto>>> Handle(
        GetAllRechargePlansQuery request,
        CancellationToken cancellationToken)
    {
        var plans = await _unitOfWork.RechargePlans.GetAllAsync();
        return ApiResponse<List<RechargePlanReadDto>>.Success(_mapper.Map<List<RechargePlanReadDto>>(plans));
    }

    public async Task<ApiResponse<RechargePlanReadDto>> Handle(
        GetRechargePlanByIdQuery request,
        CancellationToken cancellationToken)
    {
        var plan = await _unitOfWork.RechargePlans.GetByIdAsync(request.Id);
        if (plan is null)
        {
            return ApiResponse<RechargePlanReadDto>.Fail(404, "Recharge plan not found.");
        }

        return ApiResponse<RechargePlanReadDto>.Success(_mapper.Map<RechargePlanReadDto>(plan));
    }

    public async Task<ApiResponse<List<RechargePlanReadDto>>> Handle(
        GetActiveRechargePlansQuery request,
        CancellationToken cancellationToken)
    {
        var plans = await _unitOfWork.RechargePlans.GetActivePlansAsync();
        return ApiResponse<List<RechargePlanReadDto>>.Success(_mapper.Map<List<RechargePlanReadDto>>(plans));
    }

    public async Task<ApiResponse<RechargePlanReadDto>> Handle(
        GetRechargePlanByAmountQuery request,
        CancellationToken cancellationToken)
    {
        var plan = await _unitOfWork.RechargePlans.GetByAmountAsync(request.Amount);
        if (plan is null)
        {
            return ApiResponse<RechargePlanReadDto>.Fail(404, "Recharge plan not found.");
        }

        return ApiResponse<RechargePlanReadDto>.Success(_mapper.Map<RechargePlanReadDto>(plan));
    }

    public async Task<ApiResponse<List<RechargePlanReadDto>>> Handle(
        GetRechargePlansByPriceRangeQuery request,
        CancellationToken cancellationToken)
    {
        var plans = await _unitOfWork.RechargePlans.GetPlansByPriceRangeAsync(request.Min, request.Max);
        return ApiResponse<List<RechargePlanReadDto>>.Success(_mapper.Map<List<RechargePlanReadDto>>(plans));
    }

    public async Task<ApiResponse<List<RechargePlanReadDto>>> Handle(
        GetPopularRechargePlansQuery request,
        CancellationToken cancellationToken)
    {
        var plans = await _unitOfWork.RechargePlans.GetPopularPlansAsync(request.Top);
        return ApiResponse<List<RechargePlanReadDto>>.Success(_mapper.Map<List<RechargePlanReadDto>>(plans));
    }

    public async Task<ApiResponse<List<RechargePlanReadDto>>> Handle(
        GetRechargePlansByProviderQuery request,
        CancellationToken cancellationToken)
    {
        var plans = await _unitOfWork.RechargePlans.GetPlansByProviderAsync(request.Provider);
        return ApiResponse<List<RechargePlanReadDto>>.Success(_mapper.Map<List<RechargePlanReadDto>>(plans));
    }

    public async Task<ApiResponse<bool>> Handle(IsRechargePlanActiveQuery request, CancellationToken cancellationToken)
    {
        var isActive = await _unitOfWork.RechargePlans.IsPlanActiveAsync(request.Id);
        return ApiResponse<bool>.Success(isActive);
    }
}
