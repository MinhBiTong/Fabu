using Application.DTOs.Requests.ProductRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.ProductResponse;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ITelecomProductService _productService;

        public ProductsController(ITelecomProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<ProductResponse>>>> Search(
            [FromQuery] string? keyword,
            [FromQuery] string? category,
            [FromQuery] bool includeInactive = false)
        {
            var products = await _productService.SearchAsync(keyword, category, includeInactive);
            return Ok(ApiResponse<List<ProductResponse>>.Success(products));
        }

        [HttpGet("featured")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<ProductResponse>>>> Featured([FromQuery] int top = 8)
        {
            var products = await _productService.GetFeaturedAsync(top);
            return Ok(ApiResponse<List<ProductResponse>>.Success(products));
        }

        [HttpGet("{id:long}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> GetById(long id)
        {
            var product = await _productService.GetByIdAsync(id);
            return Ok(ApiResponse<ProductResponse>.Success(product));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> Create([FromBody] ProductCreateRequest request)
        {
            var product = await _productService.CreateAsync(request);
            return Ok(ApiResponse<ProductResponse>.Success(product, "Product created successfully."));
        }

        [HttpPut("{id:long}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> Update(long id, [FromBody] ProductUpdateRequest request)
        {
            var product = await _productService.UpdateAsync(id, request);
            return Ok(ApiResponse<ProductResponse>.Success(product, "Product updated successfully."));
        }

        [HttpDelete("{id:long}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(long id)
        {
            try
            {
                var deleted = await _productService.DeleteAsync(id);
                return Ok(ApiResponse<bool>.Success(deleted, "Product deleted successfully."));
            }
            catch (AppException ex)
            {
                return BadRequest(ApiResponse<bool>.Fail((int)ex.GetErrorCode(), ex.Message));
            }
        }
    }
}
