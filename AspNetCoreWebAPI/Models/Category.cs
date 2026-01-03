namespace AspNetCoreWebAPI.Models
{
    /// <summary>
    /// Represents a product category
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Gets or sets the unique identifier for the category
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the category
        /// </summary>
        /// <example>Electronics</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the category
        /// </summary>
        /// <example>Electronic devices and accessories</example>
        public string Description { get; set; } = string.Empty;
    }
}
