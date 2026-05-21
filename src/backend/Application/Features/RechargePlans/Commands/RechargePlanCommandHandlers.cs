using Application.DTOs.Responses;
using Application.Features.RechargePlans.Dtos;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Features.RechargePlans.Commands;

public sealed class RechargePlanCommandHandlers :
    IRequestHandler<CreateRechargePlanCommand, ApiResponse<RechargePlanReadDto>>,
    IRequestHandler<UpdateRechargePlanCommand, ApiResponse<bool>>,
    IRequestHandler<DeleteRechargePlanCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RechargePlanCommandHandlers(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<RechargePlanReadDto>> Handle(
        CreateRechargePlanCommand request,
        CancellationToken cancellationToken)
    {
        var plan = _mapper.Map<RechargePlan>(request);
        await _unitOfWork.RechargePlans.AddAsync(plan);

        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<RechargePlanReadDto>(plan);
        return ApiResponse<RechargePlanReadDto>.Success(dto, "Created successfully.");
    }

    public async Task<ApiResponse<bool>> Handle(UpdateRechargePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _unitOfWork.RechargePlans.GetByIdAsync(request.Id);
        if (plan is null)
        {
            return ApiResponse<bool>.Fail(404, "Recharge plan not found.");
        }

        _mapper.Map(request, plan);
        return ApiResponse<bool>.Success(true, "Updated successfully.");
    }

    public async Task<ApiResponse<bool>> Handle(DeleteRechargePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _unitOfWork.RechargePlans.GetByIdAsync(request.Id);
        if (plan is null)
        {
            return ApiResponse<bool>.Fail(404, "Recharge plan not found.");
        }

        _unitOfWork.RechargePlans.Delete(plan);
        return ApiResponse<bool>.Success(true, "Deleted successfully.");
    }
}
