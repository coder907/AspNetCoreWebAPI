namespace AspNetCoreWebAPI.Models.Responses
{
    /// <summary>
    /// Response model for update operations
    /// </summary>
    public class UpdateProductResponse
    {
        /// <summary>
        /// Indicates whether the update was successful
        /// </summary>
        /// <example>true</example>
        public bool Success { get; set; }

        /// <summary>
        /// Message describing the result of the operation
        /// </summary>
        /// <example>Product updated successfully</example>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Updated product details
        /// </summary>
        public ProductResponse? Product { get; set; }
    }
}
