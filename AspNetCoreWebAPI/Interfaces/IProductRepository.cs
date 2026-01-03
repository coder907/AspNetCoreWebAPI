using AspNetCoreWebAPI.Models;

namespace AspNetCoreWebAPI.Interfaces
{
    /// <summary>
    /// Defines the contract for product data access operations
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Gets filtered products based on the provided criteria
        /// </summary>
        /// <param name="name">Optional product name filter</param>
        /// <param name="categoryId">Optional category ID filter</param>
        /// <param name="minPrice">Optional minimum price filter</param>
        /// <param name="maxPrice">Optional maximum price filter</param>
        /// <returns>List of products matching the filters</returns>
        Task<IEnumerable<Product>> GetFilteredProductsAsync(string? name, int? categoryId, decimal? minPrice, decimal? maxPrice);

        /// <summary>
        /// Gets a product by its ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product if found, null otherwise</returns>
        Task<Product?> GetProductByIdAsync(int id);

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="product">Product with updated information</param>
        /// <returns>True if update was successful, false otherwise</returns>
        Task<bool> UpdateProductAsync(Product product);
    }
}
