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
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the product
        /// </summary>
        /// <example>Laptop</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price of the product
        /// </summary>
        /// <example>999.99</example>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the category identifier that this product belongs to
        /// </summary>
        /// <example>1</example>
        public int CategoryId { get; set; }
    }
}
