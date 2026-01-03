using AspNetCoreWebAPI.Interfaces;
using AspNetCoreWebAPI.Models;

namespace AspNetCoreWebAPI.Repositories
{
    /// <summary>
    /// In-memory implementation of product repository for demonstration purposes
    /// </summary>
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly List<Product> _products;

        public InMemoryProductRepository()
        {
            // Sample data
            _products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Price = 999.99m, CategoryId = 1 },
                new Product { Id = 2, Name = "Mouse", Price = 29.99m, CategoryId = 1 },
                new Product { Id = 3, Name = "Keyboard", Price = 79.99m, CategoryId = 1 },
                new Product { Id = 4, Name = "Monitor", Price = 299.99m, CategoryId = 1 },
                new Product { Id = 5, Name = "Desk Chair", Price = 199.99m, CategoryId = 2 },
                new Product { Id = 6, Name = "Desk Lamp", Price = 49.99m, CategoryId = 2 }
            };
        }

        public Task<IEnumerable<Product>> GetFilteredProductsAsync(string? name, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _products.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            return Task.FromResult(query);
        }

        public Task<Product?> GetProductByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<bool> UpdateProductAsync(Product product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            
            if (existingProduct == null)
            {
                return Task.FromResult(false);
            }

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.CategoryId = product.CategoryId;

            return Task.FromResult(true);
        }
    }
}
