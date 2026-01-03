using AspNetCoreWebAPI.Controllers;
using AspNetCoreWebAPI.Models.Requests;
using AspNetCoreWebAPI.Models.Responses;
using AspNetCoreWebAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspNetCoreWebAPI.Tests.Integration
{
    /// <summary>
    /// Integration tests for ProductsController with actual InMemoryProductRepository
    /// Verifies the controller works correctly when wired to a real in-memory data access implementation
    /// </summary>
    public class ProductsControllerIntegrationTests
    {
        private readonly InMemoryProductRepository _repository;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _controller;

        public ProductsControllerIntegrationTests()
        {
            _repository = new InMemoryProductRepository();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_repository, _mockLogger.Object);
        }

        #region End-to-End Filtering Tests

        [Fact]
        public async Task EndToEnd_GetAllProducts_ReturnsAllSixProducts()
        {
            // Arrange
            var request = new FilterProductsRequest();

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Equal(6, products.Count());
        }

        [Fact]
        public async Task EndToEnd_FilterByName_ReturnsMatchingProducts()
        {
            // Arrange
            var request = new FilterProductsRequest { Name = "Desk" };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Equal(2, products.Count());
            Assert.Contains(products, p => p.Name == "Desk Chair");
            Assert.Contains(products, p => p.Name == "Desk Lamp");
        }

        [Fact]
        public async Task EndToEnd_FilterByCategoryId_ReturnsElectronicsProducts()
        {
            // Arrange
            var request = new FilterProductsRequest { CategoryId = 1 };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Equal(4, products.Count());
            Assert.All(products, p => Assert.Equal(1, p.CategoryId));
        }

        [Fact]
        public async Task EndToEnd_FilterByPriceRange_ReturnsProductsInRange()
        {
            // Arrange
            var request = new FilterProductsRequest { MinPrice = 50m, MaxPrice = 300m };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Equal(3, products.Count());
            Assert.Contains(products, p => p.Name == "Keyboard");
            Assert.Contains(products, p => p.Name == "Monitor");
            Assert.Contains(products, p => p.Name == "Desk Chair");
        }

        [Fact]
        public async Task EndToEnd_FilterWithMultipleCriteria_ReturnsCorrectProduct()
        {
            // Arrange
            var request = new FilterProductsRequest 
            { 
                Name = "Desk",
                CategoryId = 2,
                MinPrice = 100m,
                MaxPrice = 250m
            };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Single(products);
            var product = products.First();
            Assert.Equal("Desk Chair", product.Name);
            Assert.Equal(199.99m, product.Price);
            Assert.Equal(2, product.CategoryId);
        }

        [Fact]
        public async Task EndToEnd_FilterWithNoMatches_ReturnsEmptyList()
        {
            // Arrange
            var request = new FilterProductsRequest { Name = "NonExistentProduct" };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Empty(products);
        }

        [Fact]
        public async Task EndToEnd_CaseInsensitiveNameSearch_FindsProducts()
        {
            // Arrange
            var request = new FilterProductsRequest { Name = "LAPTOP" };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Single(products);
            Assert.Equal("Laptop", products.First().Name);
        }

        [Fact]
        public async Task EndToEnd_FilterByMinPriceOnly_ReturnsExpensiveProducts()
        {
            // Arrange
            var request = new FilterProductsRequest { MinPrice = 200m };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Equal(2, products.Count());
            Assert.Contains(products, p => p.Name == "Laptop");
            Assert.Contains(products, p => p.Name == "Monitor");
        }

        [Fact]
        public async Task EndToEnd_FilterByMaxPriceOnly_ReturnsCheaperProducts()
        {
            // Arrange
            var request = new FilterProductsRequest { MaxPrice = 50m };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Equal(2, products.Count());
            Assert.Contains(products, p => p.Name == "Mouse");
            Assert.Contains(products, p => p.Name == "Desk Lamp");
        }

        #endregion

        #region End-to-End Update Tests

        [Fact]
        public async Task EndToEnd_UpdateProductName_SuccessfullyUpdates()
        {
            // Arrange
            int productId = 1;
            var updateRequest = new UpdateProductRequest { Name = "Gaming Laptop" };

            // Act
            var result = await _controller.UpdateProduct(productId, updateRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<UpdateProductResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Gaming Laptop", response.Product!.Name);
            Assert.Equal(999.99m, response.Product.Price); // Price unchanged

            // Verify the update persisted
            var updatedProduct = await _repository.GetProductByIdAsync(productId);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Gaming Laptop", updatedProduct.Name);
        }

        [Fact]
        public async Task EndToEnd_UpdateProductPrice_SuccessfullyUpdates()
        {
            // Arrange
            int productId = 2;
            var updateRequest = new UpdateProductRequest { Price = 39.99m };

            // Act
            var result = await _controller.UpdateProduct(productId, updateRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<UpdateProductResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Mouse", response.Product!.Name); // Name unchanged
            Assert.Equal(39.99m, response.Product.Price);

            // Verify the update persisted
            var updatedProduct = await _repository.GetProductByIdAsync(productId);
            Assert.NotNull(updatedProduct);
            Assert.Equal(39.99m, updatedProduct.Price);
        }

        [Fact]
        public async Task EndToEnd_UpdateProductNameAndPrice_SuccessfullyUpdates()
        {
            // Arrange
            int productId = 3;
            var updateRequest = new UpdateProductRequest 
            { 
                Name = "Mechanical Keyboard",
                Price = 129.99m
            };

            // Act
            var result = await _controller.UpdateProduct(productId, updateRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<UpdateProductResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Mechanical Keyboard", response.Product!.Name);
            Assert.Equal(129.99m, response.Product.Price);

            // Verify the update persisted
            var updatedProduct = await _repository.GetProductByIdAsync(productId);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Mechanical Keyboard", updatedProduct.Name);
            Assert.Equal(129.99m, updatedProduct.Price);
        }

        [Fact]
        public async Task EndToEnd_UpdateNonExistentProduct_ReturnsNotFound()
        {
            // Arrange
            int productId = 999;
            var updateRequest = new UpdateProductRequest { Name = "Test Product" };

            // Act
            var result = await _controller.UpdateProduct(productId, updateRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task EndToEnd_UpdateWithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var updateRequest = new UpdateProductRequest { Name = "Test Product" };

            // Act
            var result = await _controller.UpdateProduct(0, updateRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task EndToEnd_UpdateWithNoFields_ReturnsBadRequest()
        {
            // Arrange
            var updateRequest = new UpdateProductRequest();

            // Act
            var result = await _controller.UpdateProduct(1, updateRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        #endregion

        #region End-to-End Workflow Tests

        [Fact]
        public async Task EndToEnd_UpdateThenFilter_ReflectsChanges()
        {
            // Arrange - Update a product's price
            int productId = 5; // Desk Chair, originally 199.99
            var updateRequest = new UpdateProductRequest { Price = 149.99m };

            // Act - Update
            var updateResult = await _controller.UpdateProduct(productId, updateRequest);

            // Assert update succeeded
            var updateOkResult = Assert.IsType<OkObjectResult>(updateResult.Result);
            var updateResponse = Assert.IsType<UpdateProductResponse>(updateOkResult.Value);
            Assert.True(updateResponse.Success);

            // Act - Filter by new price range
            var filterRequest = new FilterProductsRequest { MinPrice = 140m, MaxPrice = 160m };
            var filterResult = await _controller.GetFilteredProducts(filterRequest);

            // Assert filter includes updated product
            var filterOkResult = Assert.IsType<OkObjectResult>(filterResult.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(filterOkResult.Value);
            Assert.Single(products);
            Assert.Equal("Desk Chair", products.First().Name);
            Assert.Equal(149.99m, products.First().Price);
        }

        [Fact]
        public async Task EndToEnd_UpdateProductName_ThenFilterByNewName()
        {
            // Arrange & Act - Update product name
            int productId = 4; // Monitor
            var updateRequest = new UpdateProductRequest { Name = "4K Monitor" };
            await _controller.UpdateProduct(productId, updateRequest);

            // Act - Filter by new name
            var filterRequest = new FilterProductsRequest { Name = "4K" };
            var filterResult = await _controller.GetFilteredProducts(filterRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(filterResult.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Single(products);
            Assert.Equal("4K Monitor", products.First().Name);
            Assert.Equal(productId, products.First().Id);
        }

        [Fact]
        public async Task EndToEnd_MultipleUpdates_AllPersist()
        {
            // Arrange
            var updates = new[]
            {
                new { Id = 1, Request = new UpdateProductRequest { Price = 1199.99m } },
                new { Id = 2, Request = new UpdateProductRequest { Name = "Wireless Mouse" } },
                new { Id = 3, Request = new UpdateProductRequest { Name = "RGB Keyboard", Price = 149.99m } }
            };

            // Act - Perform multiple updates
            foreach (var update in updates)
            {
                await _controller.UpdateProduct(update.Id, update.Request);
            }

            // Assert - Verify all updates persisted
            var product1 = await _repository.GetProductByIdAsync(1);
            Assert.Equal(1199.99m, product1!.Price);

            var product2 = await _repository.GetProductByIdAsync(2);
            Assert.Equal("Wireless Mouse", product2!.Name);

            var product3 = await _repository.GetProductByIdAsync(3);
            Assert.Equal("RGB Keyboard", product3!.Name);
            Assert.Equal(149.99m, product3.Price);
        }

        [Fact]
        public async Task EndToEnd_UpdateThenRetrieve_ReturnsUpdatedData()
        {
            // Arrange
            int productId = 6; // Desk Lamp
            var updateRequest = new UpdateProductRequest 
            { 
                Name = "LED Desk Lamp",
                Price = 59.99m
            };

            // Act - Update
            await _controller.UpdateProduct(productId, updateRequest);

            // Act - Retrieve via filter
            var filterRequest = new FilterProductsRequest { Name = "LED" };
            var filterResult = await _controller.GetFilteredProducts(filterRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(filterResult.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Single(products);
            var product = products.First();
            Assert.Equal(productId, product.Id);
            Assert.Equal("LED Desk Lamp", product.Name);
            Assert.Equal(59.99m, product.Price);
        }

        [Fact]
        public async Task EndToEnd_ComplexFilteringScenario_WorksCorrectly()
        {
            // Scenario: Find all category 1 products between $50-$500

            // Arrange
            var request = new FilterProductsRequest
            {
                CategoryId = 1,
                MinPrice = 50m,
                MaxPrice = 500m
            };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            
            // Should return Keyboard (79.99) and Monitor (299.99)
            // Not Mouse (29.99 - below min), not Laptop (999.99 - above max)
            Assert.Equal(2, products.Count());
            Assert.Contains(products, p => p.Name == "Keyboard" && p.Price == 79.99m);
            Assert.Contains(products, p => p.Name == "Monitor" && p.Price == 299.99m);
            Assert.All(products, p => Assert.Equal(1, p.CategoryId));
        }

        #endregion

        #region Validation Tests

        [Fact]
        public async Task EndToEnd_FilterWithInvalidPriceRange_ReturnsBadRequest()
        {
            // Arrange
            var request = new FilterProductsRequest 
            { 
                MinPrice = 500m,
                MaxPrice = 100m
            };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        #endregion
    }
}
