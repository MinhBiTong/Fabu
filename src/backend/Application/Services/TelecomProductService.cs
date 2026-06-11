using Application.DTOs.Requests.ProductRequest;
using Application.DTOs.Responses.ProductResponse;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services
{
    public class TelecomProductService : ITelecomProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TelecomProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductResponse> CreateAsync(ProductCreateRequest request)
        {
            if (await _unitOfWork.TelecomProducts.ExistsByCodeAsync(request.ProductCode))
                throw new AppException(ErrorCode.INVALID_KEY, "Product code already exists.");

            var product = new TelecomProduct
            {
                ProductCode = request.ProductCode.Trim(),
                ProductName = request.ProductName.Trim(),
                Category = request.Category.Trim(),
                Brand = request.Brand.Trim(),
                Description = request.Description.Trim(),
                ImageUrl = request.ImageUrl,
                Tags = request.Tags,
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                StockQuantity = request.StockQuantity,
                WarrantyMonths = request.WarrantyMonths,
                IsActive = request.IsActive,
                IsFeatured = request.IsFeatured,
                IsPublished = request.IsPublished
            };

            await _unitOfWork.TelecomProducts.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return ProductResponse.FromEntity(product);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var product = await _unitOfWork.TelecomProducts.GetByIdAsync(id);
            if (product is null)
                throw new AppException(ErrorCode.INVALID_KEY, "Product not found.");

            _unitOfWork.TelecomProducts.Delete(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductResponse>> GetFeaturedAsync(int top = 8)
        {
            var products = await _unitOfWork.TelecomProducts.GetFeaturedAsync(Math.Clamp(top, 1, 50));
            return products.Select(ProductResponse.FromEntity).ToList();
        }

        public async Task<ProductResponse> GetByIdAsync(long id)
        {
            var product = await _unitOfWork.TelecomProducts.GetByIdAsync(id);
            if (product is null || product.IsDeleted)
                throw new AppException(ErrorCode.INVALID_KEY, "Product not found.");

            return ProductResponse.FromEntity(product);
        }

        public async Task<List<ProductResponse>> SearchAsync(string? keyword, string? category, bool includeInactive = false)
        {
            var products = await _unitOfWork.TelecomProducts.SearchAsync(keyword, category, includeInactive);
            return products.Select(ProductResponse.FromEntity).ToList();
        }

        public async Task<ProductResponse> UpdateAsync(long id, ProductUpdateRequest request)
        {
            var product = await _unitOfWork.TelecomProducts.GetByIdAsync(id);
            if (product is null || product.IsDeleted)
                throw new AppException(ErrorCode.INVALID_KEY, "Product not found.");

            product.ProductName = request.ProductName.Trim();
            product.Category = request.Category.Trim();
            product.Brand = request.Brand.Trim();
            product.Description = request.Description.Trim();
            product.ImageUrl = request.ImageUrl;
            product.Tags = request.Tags;
            product.Price = request.Price;
            product.OriginalPrice = request.OriginalPrice;
            product.StockQuantity = request.StockQuantity;
            product.WarrantyMonths = request.WarrantyMonths;
            product.IsActive = request.IsActive;
            product.IsFeatured = request.IsFeatured;
            product.IsPublished = request.IsPublished;

            await _unitOfWork.SaveChangesAsync();
            return ProductResponse.FromEntity(product);
        }
    }
}
