namespace AspNetCoreWebAPI.Models.Responses
{
    /// <summary>
    /// Response model for product queries
    /// </summary>
    public class ProductResponse
    {
        /// <summary>
        /// Product ID
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        /// <example>Laptop</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Product price
        /// </summary>
        /// <example>999.99</example>
        public decimal Price { get; set; }

        /// <summary>
        /// Category ID the product belongs to
        /// </summary>
        /// <example>1</example>
        public int CategoryId { get; set; }
    }
}
