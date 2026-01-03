using AspNetCoreWebAPI.Interfaces;
using AspNetCoreWebAPI.Models.Requests;
using AspNetCoreWebAPI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreWebAPI.Controllers
{
    /// <summary>
    /// Manages product operations including filtering and updates
    /// </summary>
    [ApiController]
    [Route("api/products")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductsController> _logger;

        /// <summary>
        /// Initializes a new instance of the ProductsController
        /// </summary>
        /// <param name="productRepository">Product repository implementation</param>
        /// <param name="logger">Logger instance</param>
        public ProductsController(IProductRepository productRepository, ILogger<ProductsController> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets filtered products based on specified criteria
        /// </summary>
        /// <param name="request">Filter criteria including name, category, and price range</param>
        /// <returns>List of products matching the filter criteria</returns>
        /// <response code="200">Returns the filtered list of products</response>
        /// <response code="400">If the request parameters are invalid</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetFilteredProducts([FromQuery] FilterProductsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Additional validation: MinPrice should not exceed MaxPrice
            if (request.MinPrice.HasValue && request.MaxPrice.HasValue && request.MinPrice > request.MaxPrice)
            {
                ModelState.AddModelError(nameof(request.MinPrice), "Minimum price cannot be greater than maximum price");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation(
                    "Filtering products - Name: {Name}, CategoryId: {CategoryId}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}",
                    request.Name, request.CategoryId, request.MinPrice, request.MaxPrice);

                var products = await _productRepository.GetFilteredProductsAsync(
                    request.Name,
                    request.CategoryId,
                    request.MinPrice,
                    request.MaxPrice);

                var response = products.Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while filtering products");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Updates a product's name and/or price
        /// </summary>
        /// <param name="id">Product ID to update</param>
        /// <param name="request">Updated product information (name and/or price)</param>
        /// <returns>Result of the update operation</returns>
        /// <response code="200">Product updated successfully</response>
        /// <response code="400">If the request is invalid or no fields to update are provided</response>
        /// <response code="404">If the product is not found</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(UpdateProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UpdateProductResponse>> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid product ID" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate that at least one field is provided for update
            if (!request.Name.HasValue() && !request.Price.HasValue)
            {
                return BadRequest(new { message = "At least one field (Name or Price) must be provided for update" });
            }

            try
            {
                _logger.LogInformation("Updating product {ProductId} - Name: {Name}, Price: {Price}", 
                    id, request.Name, request.Price);

                // Get existing product
                var existingProduct = await _productRepository.GetProductByIdAsync(id);

                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", id);
                    return NotFound(new { message = $"Product with ID {id} not found" });
                }

                // Update only the provided fields
                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    existingProduct.Name = request.Name;
                }

                if (request.Price.HasValue)
                {
                    existingProduct.Price = request.Price.Value;
                }

                // Perform update
                var updateSuccess = await _productRepository.UpdateProductAsync(existingProduct);

                if (!updateSuccess)
                {
                    _logger.LogError("Failed to update product {ProductId}", id);
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { message = "Failed to update product" });
                }

                var response = new UpdateProductResponse
                {
                    Success = true,
                    Message = "Product updated successfully",
                    Product = new ProductResponse
                    {
                        Id = existingProduct.Id,
                        Name = existingProduct.Name,
                        Price = existingProduct.Price,
                        CategoryId = existingProduct.CategoryId
                    }
                };

                _logger.LogInformation("Product {ProductId} updated successfully", id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product {ProductId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while processing your request" });
            }
        }
    }

    /// <summary>
    /// Extension methods for validation helpers
    /// </summary>
    internal static class ValidationExtensions
    {
        public static bool HasValue(this string? value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
