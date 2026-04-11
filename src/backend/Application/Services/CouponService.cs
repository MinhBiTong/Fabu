using Application.DTOs.Requests.CouponRequest;
using Application.DTOs.Responses.CouponResponse;
using Application.Interfaces;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Persistence.Data.Contexts;

namespace Application.Services
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;
        private readonly IResponseCacheService _cache;
        private readonly ILogger<CouponService> _logger;
        private readonly AppDbContext _context;

        public CouponService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserContext userContext,
            IResponseCacheService cache,
            ILogger<CouponService> logger,
            AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userContext = userContext;
            _cache = cache;
            _logger = logger;
            _context = context;
        }

        public async Task<CouponApplyResult> ApplyCouponAsync(
            string couponCode,
            long customerId,
            decimal originalAmount,
            string transactionType)
        {
            var now = DateTime.UtcNow;

            // 1. lay coupon
            var coupon = await _unitOfWork.Coupons
                .FindAsync(x => x.Code == couponCode);

            var couponEntity = coupon.FirstOrDefault();

            if (couponEntity == null)
                return CouponApplyResult.Failed("Coupon không tồn tại");

            // 2. validate co ban
            if (!couponEntity.IsActive || now < couponEntity.ValidFrom || now > couponEntity.ValidTo)
                return CouponApplyResult.Failed("Coupon không hợp lệ hoặc đã hết hạn");

            if (originalAmount < couponEntity.MinRechargeAmount)
                return CouponApplyResult.Failed("Không đủ điều kiện áp dụng coupon");

            // 3. check usage
            var usages = await _unitOfWork.CouponUsages.FindAsync(x => x.CouponId == couponEntity.Id);

            var successUsages = usages.Where(x => x.Status == "Success").ToList();

            if (successUsages.Count >= couponEntity.UsageLimitTotal)
                return CouponApplyResult.Failed("Coupon đã hết lượt sử dụng");

            var userUsed = successUsages.Count(x =>
                x.Transaction != null && x.Transaction.CustomerId == customerId);

            if (userUsed >= couponEntity.UsageLimitPerUser)
                return CouponApplyResult.Failed("Bạn đã dùng coupon này rồi");

            // 4. tinh discount
            decimal discount = 0;

            if (couponEntity.DiscountType == DiscountType.Percentage)
            {
                discount = originalAmount * couponEntity.DiscountValue / 100;

                if (couponEntity.MaxDiscount > 0)
                    discount = Math.Min(discount, couponEntity.MaxDiscount);
            }
            else
            {
                discount = couponEntity.DiscountValue;
            }

            var finalAmount = Math.Max(0, originalAmount - discount);

            // 5. tao transaction (gia lap)
            var transaction = new Transaction
            {
                CustomerId = customerId,
                Amount = originalAmount,
                TransactionType = transactionType
            };

            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            // 6. tao usage
            var usage = new CouponUsage
            {
                CouponId = couponEntity.Id,
                TransactionId = transaction.Id,
                DiscountApplied = discount,
                Status = "Success"
            };

            await _unitOfWork.CouponUsages.AddAsync(usage);
            await _unitOfWork.SaveChangesAsync();

            return CouponApplyResult.Success(finalAmount, usage);
        }

        public async Task ConsumeCouponAsync(string couponCode, int userId)
        {
            var coupon = (await _unitOfWork.Coupons
                .FindAsync(x => x.Code == couponCode))
                .FirstOrDefault();

            if (coupon == null)
                throw new Exception("Coupon không tồn tại");

            var transaction = new Transaction
            {
                CustomerId = userId,
                Amount = 0,
                TransactionType = "ConsumeOnly"
            };

            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            var usage = new CouponUsage
            {
                CouponId = coupon.Id,
                TransactionId = transaction.Id,
                DiscountApplied = 0,
                Status = "Success"
            };

            await _unitOfWork.CouponUsages.AddAsync(usage);
            await _unitOfWork.SaveChangesAsync();
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

        public async Task<bool> ValidateCouponAsync(string couponCode, int userId)
        {
            var now = DateTime.UtcNow;

            var coupon = (await _unitOfWork.Coupons
                .FindAsync(x => x.Code == couponCode))
                .FirstOrDefault();

            if (coupon == null) return false;

            if (!coupon.IsActive || now < coupon.ValidFrom || now > coupon.ValidTo)
                return false;

            var usages = await _unitOfWork.CouponUsages
                .FindAsync(x => x.CouponId == coupon.Id);

            var successUsages = usages.Where(x => x.Status == "Success").ToList();

            if (successUsages.Count >= coupon.UsageLimitTotal)
                return false;

            var userUsed = successUsages.Count(x =>
                x.Transaction != null && x.Transaction.CustomerId == userId);

            if (userUsed >= coupon.UsageLimitPerUser)
                return false;

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
                    IsActive = true
                };

                var existing = await _unitOfWork.Coupons
                    .FindAsync(x => x.Code == coupon.Code);

                if (existing.Any())
                    throw new Exception("This Coupon Code already exist");

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

            await _cache.RemoveCacheResponseByGroupAsync("CouponGroup");
        }

        public async Task<List<CouponResponse>> GetAllCouponAsync()
        {
            string cacheKey = "coupon:all";

            var cached = await _cache.GetCachedResponseAsync<List<CouponResponse>>(cacheKey);
            if (cached != null) return cached;

            var coupons = await _unitOfWork.Coupons.GetAllAsync();

            var result = _mapper.Map<List<CouponResponse>>(coupons);

            await _cache.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<CouponResponse> GetByCouponIdAsync(long id)
        {
            string cacheKey = $"coupon:{id}";

            if (_cache != null)
            {
                var cached = await _cache.GetCachedResponseAsync<CouponResponse>(cacheKey);
                if (cached != null) return cached;
            }

            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);

            if (coupon == null)
                throw new Exception("Coupon not found");

            var result = _mapper.Map<CouponResponse>(coupon);

            if (_cache != null)
                await _cache.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<List<CouponResponse>> GetActiveCouponAsync()
        {
            string cacheKey = $"coupon:active";

            if (_cache != null)
            {
                var cached = await _cache.GetCachedResponseAsync<List<CouponResponse>>(cacheKey);
                if (cached != null) return cached;
            }

            var coupons = await _unitOfWork.Coupons.GetActiveCouponAsync();

            var result = _mapper.Map<List<CouponResponse>>(coupons);

            if (_cache != null)
                await _cache.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<List<CouponResponse>> GetExpiredCouponAsync()
        {
            string cacheKey = $"coupon:expired";

            var cached = await _cache.GetCachedResponseAsync<List<CouponResponse>>(cacheKey);
            if (cached != null) return cached;

            var coupon = await _unitOfWork.Coupons.GetExpiredCouponAsync();

            var result = _mapper.Map<List<CouponResponse>>(coupon);

            await _cache.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<List<CouponResponse>> GetCouponsByCustomerIdAsync(long customerId)
        {
            string cacheKey = $"coupon_by_customer:{customerId}";

            if (_cache != null)
            {
                var cached = await _cache.GetCachedResponseAsync<List<CouponResponse>>(cacheKey);
                if (cached != null) return cached;
            }

            var coupon = await _unitOfWork.Coupons.GetCouponsByCustomerIdAsync(customerId);

            var result = _mapper.Map<List<CouponResponse>>(coupon);

            await _cache.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
}
