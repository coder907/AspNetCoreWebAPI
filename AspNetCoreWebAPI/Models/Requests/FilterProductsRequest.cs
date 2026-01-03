using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWebAPI.Models.Requests
{
    /// <summary>
    /// Request model for filtering products
    /// </summary>
    public class FilterProductsRequest
    {
        /// <summary>
        /// Filter products by name (partial match, case-insensitive)
        /// </summary>
        /// <example>Laptop</example>
        public string? Name { get; set; }

        /// <summary>
        /// Filter products by category ID
        /// </summary>
        /// <example>1</example>
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a positive number")]
        public int? CategoryId { get; set; }

        /// <summary>
        /// Filter products with price greater than or equal to this value
        /// </summary>
        /// <example>50.00</example>
        [Range(0, double.MaxValue, ErrorMessage = "Minimum price must be non-negative")]
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Filter products with price less than or equal to this value
        /// </summary>
        /// <example>500.00</example>
        [Range(0, double.MaxValue, ErrorMessage = "Maximum price must be non-negative")]
        public decimal? MaxPrice { get; set; }
    }
}
