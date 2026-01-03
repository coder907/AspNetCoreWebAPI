using AspNetCoreWebAPI.Repositories;
using AspNetCoreWebAPI.Models;

namespace AspNetCoreWebAPI.Tests.Repositories
{
    /// <summary>
    /// Unit tests for InMemoryProductRepository focusing on filtering logic
    /// </summary>
    public class InMemoryProductRepositoryTests
    {
        private readonly InMemoryProductRepository _repository;

        public InMemoryProductRepositoryTests()
        {
            _repository = new InMemoryProductRepository();
        }

        #region GetFilteredProductsAsync Tests

        [Fact]
        public async Task GetFilteredProductsAsync_WithNoFilters_ReturnsAllProducts()
        {
            // Act
            var result = await _repository.GetFilteredProductsAsync(null, null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Theory]
        [InlineData("Laptop", 1)]
        [InlineData("laptop", 1)] // Case-insensitive
        [InlineData("LAPTOP", 1)] // Case-insensitive
        [InlineData("Mouse", 1)]
        [InlineData("Desk", 2)] // Partial match: "Desk Chair" and "Desk Lamp"
        [InlineData("NonExistent", 0)]
        public async Task GetFilteredProductsAsync_WithNameFilter_ReturnsMatchingProducts(string name, int expectedCount)
        {
            // Act
            var result = await _repository.GetFilteredProductsAsync(name, null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
            Assert.All(result, product => 
                Assert.Contains(name, product.Name, StringComparison.OrdinalIgnoreCase));
        }

        [Theory]
        [InlineData(1, 4)] // Electronics category
        [InlineData(2, 2)] // Furniture category
        [InlineData(999, 0)] // Non-existent category
        public async Task GetFilteredProductsAsync_WithCategoryFilter_ReturnsMatchingProducts(int categoryId, int expectedCount)
        {
            // Act
            var result = await _repository.GetFilteredProductsAsync(null, categoryId, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
            Assert.All(result, product => Assert.Equal(categoryId, product.CategoryId));
        }

        [Theory]
        [InlineData(0, 6)] // All products >= 0
        [InlineData(30, 5)] // Products >= 30
        [InlineData(50, 4)] // Products >= 50
        [InlineData(100, 3)] // Products >= 100
        [InlineData(500, 1)] // Products >= 500 (only Laptop)
        [InlineData(1000, 0)] // No products >= 1000
        public async Task GetFilteredProductsAsync_WithMinPriceFilter_ReturnsMatchingProducts(decimal minPrice, int expectedCount)
        {
            // Act
            var result = await _repository.GetFilteredProductsAsync(null, null, minPrice, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
            Assert.All(result, product => Assert.True(product.Price >= minPrice));
        }

        [Theory]
        [InlineData(0, 0)] // No products <= 0
        [InlineData(30, 1)] // Only Mouse
        [InlineData(50, 2)] // Mouse and Desk Lamp
        [InlineData(100, 3)] // Mouse, Desk Lamp, and Keyboard
        [InlineData(500, 5)] // All except Laptop
        [InlineData(1000, 6)] // All products
        public async Task GetFilteredProductsAsync_WithMaxPriceFilter_ReturnsMatchingProducts(decimal maxPrice, int expectedCount)
        {
            // Act
            var result = await _repository.GetFilteredProductsAsync(null, null, null, maxPrice);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
            Assert.All(result, product => Assert.True(product.Price <= maxPrice));
        }

        [Theory]
        [InlineData(50, 300, 3)] // Keyboard (79.99), Monitor (299.99), Desk Chair (199.99)
        [InlineData(29.99, 79.99, 3)] // Mouse, Keyboard, Desk Lamp
        [InlineData(100, 200, 1)] // Desk Chair (199.99)
        [InlineData(1000, 2000, 0)] // No products in this range
        public async Task GetFilteredProductsAsync_WithMinAndMaxPriceFilter_ReturnsMatchingProducts(
            decimal minPrice, decimal maxPrice, int expectedCount)
        {
            // Act
            var result = await _repository.GetFilteredProductsAsync(null, null, minPrice, maxPrice);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
            Assert.All(result, product => 
            {
                Assert.True(product.Price >= minPrice);
                Assert.True(product.Price <= maxPrice);
            });
        }

        [Fact]
        public async Task GetFilteredProductsAsync_WithMultipleFilters_ReturnsMatchingProducts()
        {
            // Arrange
            string name = "Desk";
            int categoryId = 2;
            decimal minPrice = 100m;
            decimal maxPrice = 250m;

            // Act
            var result = await _repository.GetFilteredProductsAsync(name, categoryId, minPrice, maxPrice);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Only "Desk Chair" matches all criteria
            var product = result.First();
            Assert.Contains(name, product.Name, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(categoryId, product.CategoryId);
            Assert.True(product.Price >= minPrice);
            Assert.True(product.Price <= maxPrice);
        }

        [Fact]
        public async Task GetFilteredProductsAsync_WithNameAndCategory_ReturnsMatchingProducts()
        {
            // Arrange
            string name = "Desk";
            int categoryId = 2;

            // Act
            var result = await _repository.GetFilteredProductsAsync(name, categoryId, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // "Desk Chair" and "Desk Lamp"
            Assert.All(result, product =>
            {
                Assert.Contains(name, product.Name, StringComparison.OrdinalIgnoreCase);
                Assert.Equal(categoryId, product.CategoryId);
            });
        }

        [Fact]
        public async Task GetFilteredProductsAsync_WithNameAndPriceRange_ReturnsMatchingProducts()
        {
            // Arrange
            string name = "Desk";
            decimal minPrice = 40m;
            decimal maxPrice = 60m;

            // Act
            var result = await _repository.GetFilteredProductsAsync(name, null, minPrice, maxPrice);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Only "Desk Lamp" (49.99 is in range)
        }

        [Fact]
        public async Task GetFilteredProductsAsync_WithCategoryAndPriceRange_ReturnsMatchingProducts()
        {
            // Arrange
            int categoryId = 1;
            decimal minPrice = 50m;
            decimal maxPrice = 300m;

            // Act
            var result = await _repository.GetFilteredProductsAsync(null, categoryId, minPrice, maxPrice);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // "Keyboard" and "Monitor"
            Assert.All(result, product =>
            {
                Assert.Equal(categoryId, product.CategoryId);
                Assert.True(product.Price >= minPrice);
                Assert.True(product.Price <= maxPrice);
            });
        }

        [Fact]
        public async Task GetFilteredProductsAsync_WithNoMatchingFilters_ReturnsEmptyList()
        {
            // Arrange
            string name = "NonExistent";
            int categoryId = 999;
            decimal minPrice = 10000m;

            // Act
            var result = await _repository.GetFilteredProductsAsync(name, categoryId, minPrice, null);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFilteredProductsAsync_WithEmptyName_IgnoresNameFilter()
        {
            // Act
            var result = await _repository.GetFilteredProductsAsync("", null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count()); // Should return all products
        }

        [Fact]
        public async Task GetFilteredProductsAsync_WithWhitespaceName_IgnoresNameFilter()
        {
            // Act
            var result = await _repository.GetFilteredProductsAsync("   ", null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count()); // Should return all products
        }

        #endregion

        #region GetProductByIdAsync Tests

        [Theory]
        [InlineData(1, "Laptop")]
        [InlineData(2, "Mouse")]
        [InlineData(3, "Keyboard")]
        [InlineData(4, "Monitor")]
        [InlineData(5, "Desk Chair")]
        [InlineData(6, "Desk Lamp")]
        public async Task GetProductByIdAsync_WithValidId_ReturnsProduct(int id, string expectedName)
        {
            // Act
            var result = await _repository.GetProductByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(expectedName, result.Name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(999)]
        [InlineData(int.MaxValue)]
        public async Task GetProductByIdAsync_WithInvalidId_ReturnsNull(int id)
        {
            // Act
            var result = await _repository.GetProductByIdAsync(id);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region UpdateProductAsync Tests

        [Fact]
        public async Task UpdateProductAsync_WithValidProduct_UpdatesSuccessfully()
        {
            // Arrange
            var productToUpdate = new Product
            {
                Id = 1,
                Name = "Gaming Laptop",
                Price = 1499.99m,
                CategoryId = 1
            };

            // Act
            var result = await _repository.UpdateProductAsync(productToUpdate);

            // Assert
            Assert.True(result);

            // Verify the update
            var updatedProduct = await _repository.GetProductByIdAsync(1);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Gaming Laptop", updatedProduct.Name);
            Assert.Equal(1499.99m, updatedProduct.Price);
            Assert.Equal(1, updatedProduct.CategoryId);
        }

        [Fact]
        public async Task UpdateProductAsync_OnlyNameChange_UpdatesNameOnly()
        {
            // Arrange
            var originalProduct = await _repository.GetProductByIdAsync(2);
            var originalPrice = originalProduct!.Price;
            var originalCategoryId = originalProduct.CategoryId;

            var productToUpdate = new Product
            {
                Id = 2,
                Name = "Wireless Mouse",
                Price = originalPrice,
                CategoryId = originalCategoryId
            };

            // Act
            var result = await _repository.UpdateProductAsync(productToUpdate);

            // Assert
            Assert.True(result);

            // Verify only name changed
            var updatedProduct = await _repository.GetProductByIdAsync(2);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Wireless Mouse", updatedProduct.Name);
            Assert.Equal(originalPrice, updatedProduct.Price);
            Assert.Equal(originalCategoryId, updatedProduct.CategoryId);
        }

        [Fact]
        public async Task UpdateProductAsync_OnlyPriceChange_UpdatesPriceOnly()
        {
            // Arrange
            var originalProduct = await _repository.GetProductByIdAsync(3);
            var originalName = originalProduct!.Name;
            var originalCategoryId = originalProduct.CategoryId;

            var productToUpdate = new Product
            {
                Id = 3,
                Name = originalName,
                Price = 89.99m,
                CategoryId = originalCategoryId
            };

            // Act
            var result = await _repository.UpdateProductAsync(productToUpdate);

            // Assert
            Assert.True(result);

            // Verify only price changed
            var updatedProduct = await _repository.GetProductByIdAsync(3);
            Assert.NotNull(updatedProduct);
            Assert.Equal(originalName, updatedProduct.Name);
            Assert.Equal(89.99m, updatedProduct.Price);
            Assert.Equal(originalCategoryId, updatedProduct.CategoryId);
        }

        [Fact]
        public async Task UpdateProductAsync_WithNonExistentProduct_ReturnsFalse()
        {
            // Arrange
            var productToUpdate = new Product
            {
                Id = 999,
                Name = "Non-existent Product",
                Price = 100m,
                CategoryId = 1
            };

            // Act
            var result = await _repository.UpdateProductAsync(productToUpdate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateProductAsync_MultipleUpdates_AllSucceed()
        {
            // Arrange & Act
            var firstUpdate = new Product { Id = 1, Name = "Updated Name 1", Price = 100m, CategoryId = 1 };
            var firstResult = await _repository.UpdateProductAsync(firstUpdate);

            var secondUpdate = new Product { Id = 1, Name = "Updated Name 2", Price = 200m, CategoryId = 1 };
            var secondResult = await _repository.UpdateProductAsync(secondUpdate);

            // Assert
            Assert.True(firstResult);
            Assert.True(secondResult);

            var finalProduct = await _repository.GetProductByIdAsync(1);
            Assert.NotNull(finalProduct);
            Assert.Equal("Updated Name 2", finalProduct.Name);
            Assert.Equal(200m, finalProduct.Price);
        }

        [Fact]
        public async Task UpdateProductAsync_ChangingCategoryId_UpdatesSuccessfully()
        {
            // Arrange
            var productToUpdate = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 999.99m,
                CategoryId = 2 // Change from category 1 to 2
            };

            // Act
            var result = await _repository.UpdateProductAsync(productToUpdate);

            // Assert
            Assert.True(result);

            var updatedProduct = await _repository.GetProductByIdAsync(1);
            Assert.NotNull(updatedProduct);
            Assert.Equal(2, updatedProduct.CategoryId);
        }

        #endregion

        #region Integration Tests - Filter After Update

        [Fact]
        public async Task FilteredProducts_AfterUpdate_ReflectsChanges()
        {
            // Arrange - Update a product's price
            var productToUpdate = new Product
            {
                Id = 2, // Mouse, originally 29.99
                Name = "Mouse",
                Price = 150m,
                CategoryId = 1
            };
            await _repository.UpdateProductAsync(productToUpdate);

            // Act - Filter by price range that now includes the updated product
            var result = await _repository.GetFilteredProductsAsync(null, null, 100m, 200m);

            // Assert - Should include the updated Mouse
            Assert.Contains(result, p => p.Id == 2 && p.Name == "Mouse");
        }

        [Fact]
        public async Task FilteredProducts_AfterNameUpdate_ReflectsNewName()
        {
            // Arrange - Update product name
            var productToUpdate = new Product
            {
                Id = 1,
                Name = "Gaming Laptop Pro",
                Price = 999.99m,
                CategoryId = 1
            };
            await _repository.UpdateProductAsync(productToUpdate);

            // Act - Filter by new name
            var result = await _repository.GetFilteredProductsAsync("Gaming", null, null, null);

            // Assert
            Assert.Single(result);
            Assert.Equal("Gaming Laptop Pro", result.First().Name);
        }

        #endregion
    }
}
