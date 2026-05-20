using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Features.RechargePlanRecommendations.Queries;
using Application.Features.RechargePlans.Commands;
using Application.Features.RechargePlans.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RechargePlansController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RechargePlansController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => ToActionResult(await _mediator.Send(new GetAllRechargePlansQuery()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
            => ToActionResult(await _mediator.Send(new GetRechargePlanByIdQuery(id)));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRechargePlanRequest request)
        {
            var command = new CreateRechargePlanCommand(
                request.Name,
                request.Price,
                request.Points,
                null,
                request.Description,
                true);

            return ToActionResult(await _mediator.Send(command));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateRechargePlanRequest request)
        {
            if (request.Id != 0 && request.Id != id)
            {
                return BadRequest(ApiResponse<bool>.Fail(400, "ID mismatch."));
            }

            var command = new UpdateRechargePlanCommand(
                id,
                request.PlanName,
                request.Amount,
                request.BonusAmount,
                request.ValidityDays,
                request.Description,
                request.IsActive);

            return ToActionResult(await _mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
            => ToActionResult(await _mediator.Send(new DeleteRechargePlanCommand(id)));

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
            => ToActionResult(await _mediator.Send(new GetActiveRechargePlansQuery()));

        [HttpGet("amount/{amount}")]
        public async Task<IActionResult> GetByAmount(decimal amount)
            => ToActionResult(await _mediator.Send(new GetRechargePlanByAmountQuery(amount)));

        [HttpGet("price-range")]
        public async Task<IActionResult> GetByPriceRange([FromQuery] decimal min, [FromQuery] decimal max)
            => ToActionResult(await _mediator.Send(new GetRechargePlansByPriceRangeQuery(min, max)));

        [HttpGet("popular/{top}")]
        public async Task<IActionResult> GetPopular(int top)
            => ToActionResult(await _mediator.Send(new GetPopularRechargePlansQuery(top)));

        [HttpGet("recommendations/{customerId:long}")]
        public async Task<IActionResult> GetRecommendations(
            long customerId,
            [FromQuery] int top = 3,
            [FromQuery] int recentTransactionLimit = 20,
            CancellationToken cancellationToken = default)
            => ToActionResult(await _mediator.Send(
                new GetPersonalizedRechargePlanRecommendationsQuery(customerId, top, recentTransactionLimit),
                cancellationToken));

        [HttpGet("provider/{provider}")]
        public async Task<IActionResult> GetByProvider(string provider)
            => ToActionResult(await _mediator.Send(new GetRechargePlansByProviderQuery(provider)));

        [HttpGet("{id}/is-active")]
        public async Task<IActionResult> IsActive(long id)
            => ToActionResult(await _mediator.Send(new IsRechargePlanActiveQuery(id)));

        private IActionResult ToActionResult<T>(ApiResponse<T> response)
        {
            return response.Code switch
            {
                200 => Ok(response),
                400 => BadRequest(response),
                404 => NotFound(response),
                _ => StatusCode(response.Code, response)
            };
        }
    }
}
