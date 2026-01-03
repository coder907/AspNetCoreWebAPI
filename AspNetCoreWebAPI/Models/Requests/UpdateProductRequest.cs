using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWebAPI.Models.Requests
{
    /// <summary>
    /// Request model for updating a product
    /// </summary>
    public class UpdateProductRequest
    {
        /// <summary>
        /// New name for the product (optional)
        /// </summary>
        /// <example>Gaming Laptop</example>
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 200 characters")]
        public string? Name { get; set; }

        /// <summary>
        /// New price for the product (optional)
        /// </summary>
        /// <example>1299.99</example>
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal? Price { get; set; }
    }
}
