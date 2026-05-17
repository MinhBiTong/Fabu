using Application.DTOs.Requests.CouponRequest;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace greenginger.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        // APPLY COUPON
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyCoupon(
            string couponCode,
            long customerId,
            decimal originalAmount,
            string transactionType)
        {
            var result = await _couponService.ApplyCouponAsync(
                couponCode, customerId, originalAmount, transactionType);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // GENERATE COUPON
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateCoupon(
            int userId,
            decimal discountAmount,
            DateTime expiryDate)
        {
            var code = await _couponService.GenerateCouponAsync(userId, discountAmount, expiryDate);
            return Ok(new { Code = code });
        }

        // VALIDATE COUPON
        [HttpGet("validate")]
        public async Task<IActionResult> ValidateCoupon(string couponCode, int userId)
        {
            var isValid = await _couponService.ValidateCouponAsync(couponCode, userId);
            return Ok(new { IsValid = isValid });
        }

        // CONSUME COUPON
        [HttpPost("consume")]
        public async Task<IActionResult> ConsumeCoupon(string couponCode, int userId)
        {
            await _couponService.ConsumeCouponAsync(couponCode, userId);
            return Ok(new { Message = "Coupon consumed successfully" });
        }

        // GET ACTIVE COUPONS
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCoupons()
        {
            var coupons = await _couponService.GetActiveCouponAsync();
            return Ok(coupons);
        }

        // GET EXPIRED COUPONS
        [HttpGet("expired")]
        public async Task<IActionResult> GetExpiredCoupons()
        {
            var coupons = await _couponService.GetExpiredCouponAsync();
            return Ok(coupons);
        }

        // GET BY CUSTOMER
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(long customerId)
        {
            var coupons = await _couponService.GetCouponsByCustomerIdAsync(customerId);
            return Ok(coupons);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CouponCreateRequest request)
        {
            try
            {
                var result = await _couponService.CreateCouponAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _couponService.GetAllCouponAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("coupon/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var result = await _couponService.GetByCouponIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _couponService.DeleteCouponAsync(id);
                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(long id, CouponUpdateRequest request)
        {
            var result = await _couponService.UpdateCouponAsync(id, request);
            return Ok(result);
        }
    }
}
