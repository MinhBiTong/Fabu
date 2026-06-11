using Application.DTOs.Requests.ProductRequest;
using Application.DTOs.Responses.ProductResponse;

namespace Application.Interfaces
{
    public interface ITelecomProductService
    {
        Task<List<ProductResponse>> SearchAsync(string? keyword, string? category, bool includeInactive = false);
        Task<List<ProductResponse>> GetFeaturedAsync(int top = 8);
        Task<ProductResponse> GetByIdAsync(long id);
        Task<ProductResponse> CreateAsync(ProductCreateRequest request);
        Task<ProductResponse> UpdateAsync(long id, ProductUpdateRequest request);
        Task<bool> DeleteAsync(long id);
    }
}
