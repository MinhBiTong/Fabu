using Application.DTOs.Requests.CouponRequest;
using Application.DTOs.Responses.CouponResponse;
using Application.Interfaces;
using AutoMapper;
using Domain.Abstractions;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IResponseCacheService? _responseCacheService;
        private readonly ILogger<CouponService> _logger;

        public CouponService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CouponService> logger,
            IResponseCacheService? responseCacheService = null)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _responseCacheService = responseCacheService;
            _logger = logger;
        }


        public async Task<CouponApplyResult> ApplyCouponAsync(string couponCode, long customerId, decimal originalAmount, string transactionType)
        {
            var now = DateTime.UtcNow;
            var coupon = (await _unitOfWork.Coupons.GetAllAsync())
                .FirstOrDefault(item =>
                    item.Code == couponCode &&
                    item.IsActive &&
                    item.ValidFrom <= now &&
                    item.ValidTo >= now);

            if (coupon == null)
                return CouponApplyResult.Failed("Coupon is invalid or expired.");

            if (originalAmount < coupon.MinRechargeAmount)
                return CouponApplyResult.Failed("Order amount does not meet coupon minimum amount.");

            var totalUsage = await _unitOfWork.CouponUsages.CountUsageAsync(coupon.Id);
            if (totalUsage >= coupon.UsageLimitTotal)
                return CouponApplyResult.Failed("Coupon usage limit has been reached.");

            var customerUsage = await _unitOfWork.CouponUsages.GetUsageCountByUserAsync(customerId, coupon.Id);
            if (customerUsage >= coupon.UsageLimitPerUser)
                return CouponApplyResult.Failed("Customer has already used this coupon.");

            var discount = coupon.DiscountType switch
            {
                DiscountType.Percentage => originalAmount * coupon.DiscountValue / 100,
                DiscountType.FixedAmount => coupon.DiscountValue,
                _ => 0
            };

            if (coupon.MaxDiscount > 0)
            {
                discount = Math.Min(discount, coupon.MaxDiscount);
            }

            discount = Math.Min(discount, originalAmount);
            var finalAmount = originalAmount - discount;

            var usage = new CouponUsage
            {
                CouponId = coupon.Id,
                DiscountApplied = discount,
                UsedAt = now,
                Status = "Pending"
            };

            return CouponApplyResult.Success(finalAmount, usage);
        }

        public Task ConsumeCouponAsync(string couponCode, int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GenerateCouponAsync(int userId, decimal discountAmount, DateTime expiryDate)
        {
            var code_gen = $"CPN-{Guid.NewGuid().ToString()[..8].ToUpper()}";

            var coupon = new Coupon
            {
                Code = code_gen,
                Name = "Auto Generated",
                DiscountType = DiscountType.FixedAmount,
                DiscountValue = discountAmount,
                ValidFrom = DateTime.UtcNow,
                ValidTo = expiryDate,
                UsageLimitPerUser = 1,
                UsageLimitTotal = 1000,
                CreatedByUserId = userId,
                IsActive = true
            };

            await _unitOfWork.Coupons.AddAsync(coupon);
            await _unitOfWork.SaveChangesAsync();

            return code_gen;
        }

        public async Task<bool> ValidateCouponAsync(string couponCode, int userId) { 
            return true;
        }

        public async Task<CouponResponse> CreateCouponAsync(CouponCreateRequest request)
        {
            try
            {
                var coupon = new Coupon
                {
                    Code = request.Code,
                    Name = request.Name,
                    DiscountType = Enum.Parse<DiscountType>(request.DiscountType),
                    DiscountValue = request.DiscountValue,
                    MinRechargeAmount = request.MinRechargeAmount,
                    MaxDiscount = request.MaxDiscount ?? 0,
                    ValidFrom = request.ValidFrom,
                    ValidTo = request.ValidTo,
                    UsageLimitPerUser = request.UsageLimitPerUser,
                    UsageLimitTotal = request.UsageLimitTotal ?? 1000,
                    IsActive = request.IsActive,
                    CreatedByUserId = request.CreatedByUserId
                };

                var existing = await _unitOfWork.Coupons.GetAllAsync();
                if (existing.Any(x => x.Code == coupon.Code))
                    throw new Exception("This Coupon Code already exist");

                await _unitOfWork.Coupons.AddAsync(coupon);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<CouponResponse>(coupon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating new coupon");
                throw;
            }
        }

        public async Task<CouponResponse> UpdateCouponAsync(long id, CouponUpdateRequest request)
        {
            try
            {
                var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
                if (coupon == null)
                    throw new Exception("Coupon not found");

                coupon.Name = request.Name;
                coupon.DiscountType = Enum.Parse<DiscountType>(request.DiscountType);
                coupon.DiscountValue = request.DiscountValue;
                coupon.MinRechargeAmount = request.MinRechargeAmount;
                coupon.MaxDiscount = request.MaxDiscount ?? 0;
                coupon.ValidFrom = request.ValidFrom;
                coupon.ValidTo = request.ValidTo;
                coupon.UsageLimitPerUser = request.UsageLimitPerUser;
                coupon.UsageLimitTotal = request.UsageLimitTotal ?? 1000;
                coupon.IsActive = request.IsActive;

                _unitOfWork.Coupons.Update(coupon);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<CouponResponse>(coupon);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating new permission");
                throw;
            }

        }

        public async Task DeleteCouponAsync(long id)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);

            if (coupon == null)
                throw new Exception("Coupon not exsited");

            _unitOfWork.Coupons.Delete(coupon);
            await _unitOfWork.SaveChangesAsync();

            await _responseCacheService.RemoveCacheResponseByGroupAsync("CouponGroup");
        }

        public async Task<List<CouponResponse>> GetAllCouponAsync()
        {
            string cacheKey = "coupon:all";

            var cached = await _responseCacheService.GetCachedResponseAsync<List<CouponResponse>>(cacheKey);
            if (cached != null) return cached;

            var coupons = await _unitOfWork.Coupons.GetAllAsync();

            var result = _mapper.Map<List<CouponResponse>>(coupons);

            await _responseCacheService.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<CouponResponse> GetByCouponIdAsync(long id)
        {
            string cacheKey = $"coupon:{id}";

            if (_responseCacheService != null)
            {
                var cached = await _responseCacheService.GetCachedResponseAsync<CouponResponse>(cacheKey);
                if (cached != null) return cached;
            }

            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);

            if (coupon == null)
                throw new Exception("Coupon not found");

            var result = _mapper.Map<CouponResponse>(coupon);

            if (_responseCacheService != null)
                await _responseCacheService.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<List<CouponResponse>> GetActiveCouponAsync()
        {
            string cacheKey = $"coupon:active";

            if (_responseCacheService != null)
            {
                var cached = await _responseCacheService.GetCachedResponseAsync<List<CouponResponse>>(cacheKey);
                if (cached != null) return cached;
            }

            var coupons = await _unitOfWork.Coupons.GetActiveCouponAsync();

            var result = _mapper.Map<List<CouponResponse>>(coupons);

            if (_responseCacheService != null)
                await _responseCacheService.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<List<CouponResponse>> GetExpiredCouponAsync()
        {
            string cacheKey = $"coupon:expired";

            var cached = await _responseCacheService.GetCachedResponseAsync<List<CouponResponse>>(cacheKey);
            if (cached != null) return cached;

            var coupon = await _unitOfWork.Coupons.GetExpiredCouponAsync();

            var result = _mapper.Map<List<CouponResponse>>(coupon);

            await _responseCacheService.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<List<CouponResponse>> GetCouponsByCustomerIdAsync(long customerId)
        {
            string cacheKey = $"coupon_by_customer:{customerId}";

            if (_responseCacheService != null)
            {
                var cached = await _responseCacheService.GetCachedResponseAsync<List<CouponResponse>>(cacheKey);
                if (cached != null) return cached;
            }

            var coupon = await _unitOfWork.Coupons.GetCouponsByCustomerIdAsync(customerId);

            var result = _mapper.Map<List<CouponResponse>>(coupon);

            await _responseCacheService.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
}
