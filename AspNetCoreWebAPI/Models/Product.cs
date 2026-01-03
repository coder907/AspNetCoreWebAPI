namespace AspNetCoreWebAPI.Models
{
    /// <summary>
    /// Represents a product in the catalog
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Gets or sets the unique identifier for the product
        /// </summary>
        /// <example>1</example>
        public required int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the product
        /// </summary>
        /// <example>Laptop</example>
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price of the product
        /// </summary>
        /// <example>999.99</example>
        public required decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the category identifier that this product belongs to
        /// </summary>
        /// <example>1</example>
        public required int CategoryId { get; set; }
    }
}
