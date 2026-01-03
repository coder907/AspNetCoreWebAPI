using AspNetCoreWebAPI.Controllers;
using AspNetCoreWebAPI.Interfaces;
using AspNetCoreWebAPI.Models;
using AspNetCoreWebAPI.Models.Requests;
using AspNetCoreWebAPI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspNetCoreWebAPI.Tests.Controllers
{
    /// <summary>
    /// Integration tests for ProductsController with mocked dependencies
    /// </summary>
    public class ProductsControllerTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_mockRepository.Object, _mockLogger.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new ProductsController(null!, _mockLogger.Object));
            Assert.Equal("productRepository", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new ProductsController(_mockRepository.Object, null!));
            Assert.Equal("logger", exception.ParamName);
        }

        #endregion

        #region GetFilteredProducts Tests

        [Fact]
        public async Task GetFilteredProducts_WithNoFilters_ReturnsAllProducts()
        {
            // Arrange
            var products = GetSampleProducts();
            _mockRepository
                .Setup(r => r.GetFilteredProductsAsync(null, null, null, null))
                .ReturnsAsync(products);

            var request = new FilterProductsRequest();

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Equal(6, returnedProducts.Count());

            _mockRepository.Verify(r => r.GetFilteredProductsAsync(null, null, null, null), Times.Once);
        }

        [Fact]
        public async Task GetFilteredProducts_WithNameFilter_ReturnsFilteredProducts()
        {
            // Arrange
            var products = GetSampleProducts().Where(p => p.Name.Contains("Laptop"));
            _mockRepository
                .Setup(r => r.GetFilteredProductsAsync("Laptop", null, null, null))
                .ReturnsAsync(products);

            var request = new FilterProductsRequest { Name = "Laptop" };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Single(returnedProducts);
            Assert.Equal("Laptop", returnedProducts.First().Name);

            _mockRepository.Verify(r => r.GetFilteredProductsAsync("Laptop", null, null, null), Times.Once);
        }

        [Fact]
        public async Task GetFilteredProducts_WithCategoryFilter_ReturnsFilteredProducts()
        {
            // Arrange
            var products = GetSampleProducts().Where(p => p.CategoryId == 1);
            _mockRepository
                .Setup(r => r.GetFilteredProductsAsync(null, 1, null, null))
                .ReturnsAsync(products);

            var request = new FilterProductsRequest { CategoryId = 1 };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Equal(4, returnedProducts.Count());
            Assert.All(returnedProducts, p => Assert.Equal(1, p.CategoryId));

            _mockRepository.Verify(r => r.GetFilteredProductsAsync(null, 1, null, null), Times.Once);
        }

        [Fact]
        public async Task GetFilteredProducts_WithPriceRange_ReturnsFilteredProducts()
        {
            // Arrange
            decimal minPrice = 50m;
            decimal maxPrice = 300m;
            var products = GetSampleProducts()
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice);
            
            _mockRepository
                .Setup(r => r.GetFilteredProductsAsync(null, null, minPrice, maxPrice))
                .ReturnsAsync(products);

            var request = new FilterProductsRequest { MinPrice = minPrice, MaxPrice = maxPrice };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Equal(3, returnedProducts.Count());
            Assert.All(returnedProducts, p =>
            {
                Assert.True(p.Price >= minPrice);
                Assert.True(p.Price <= maxPrice);
            });
        }

        [Fact]
        public async Task GetFilteredProducts_WithMultipleFilters_ReturnsFilteredProducts()
        {
            // Arrange
            string name = "Desk";
            int categoryId = 2;
            decimal minPrice = 100m;
            decimal maxPrice = 250m;

            var products = GetSampleProducts()
                .Where(p => p.Name.Contains(name) && p.CategoryId == categoryId 
                    && p.Price >= minPrice && p.Price <= maxPrice);

            _mockRepository
                .Setup(r => r.GetFilteredProductsAsync(name, categoryId, minPrice, maxPrice))
                .ReturnsAsync(products);

            var request = new FilterProductsRequest 
            { 
                Name = name, 
                CategoryId = categoryId, 
                MinPrice = minPrice, 
                MaxPrice = maxPrice 
            };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Single(returnedProducts);
            Assert.Equal("Desk Chair", returnedProducts.First().Name);
        }

        [Fact]
        public async Task GetFilteredProducts_WithInvalidMinMaxPrice_ReturnsBadRequest()
        {
            // Arrange
            var request = new FilterProductsRequest { MinPrice = 500m, MaxPrice = 100m };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("MinPrice"));

            _mockRepository.Verify(r => r.GetFilteredProductsAsync(
                It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>()), 
                Times.Never);
        }

        [Fact]
        public async Task GetFilteredProducts_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetFilteredProductsAsync(null, null, null, null))
                .ThrowsAsync(new Exception("Database error"));

            var request = new FilterProductsRequest();

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetFilteredProducts_WithNoResults_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetFilteredProductsAsync("NonExistent", null, null, null))
                .ReturnsAsync(new List<Product>());

            var request = new FilterProductsRequest { Name = "NonExistent" };

            // Act
            var result = await _controller.GetFilteredProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Empty(returnedProducts);
        }

        #endregion

        #region UpdateProduct Tests

        [Fact]
        public async Task UpdateProduct_WithValidNameAndPrice_ReturnsSuccess()
        {
            // Arrange
            int productId = 1;
            var existingProduct = new Product 
            { 
                Id = productId, 
                Name = "Laptop", 
                Price = 999.99m, 
                CategoryId = 1 
            };

            _mockRepository
                .Setup(r => r.GetProductByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _mockRepository
                .Setup(r => r.UpdateProductAsync(It.IsAny<Product>()))
                .ReturnsAsync(true);

            var request = new UpdateProductRequest 
            { 
                Name = "Gaming Laptop", 
                Price = 1299.99m 
            };

            // Act
            var result = await _controller.UpdateProduct(productId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<UpdateProductResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Product updated successfully", response.Message);
            Assert.NotNull(response.Product);
            Assert.Equal("Gaming Laptop", response.Product.Name);
            Assert.Equal(1299.99m, response.Product.Price);

            _mockRepository.Verify(r => r.GetProductByIdAsync(productId), Times.Once);
            _mockRepository.Verify(r => r.UpdateProductAsync(It.Is<Product>(p => 
                p.Id == productId && p.Name == "Gaming Laptop" && p.Price == 1299.99m)), 
                Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_WithOnlyName_UpdatesNameOnly()
        {
            // Arrange
            int productId = 1;
            var existingProduct = new Product 
            { 
                Id = productId, 
                Name = "Laptop", 
                Price = 999.99m, 
                CategoryId = 1 
            };

            _mockRepository
                .Setup(r => r.GetProductByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _mockRepository
                .Setup(r => r.UpdateProductAsync(It.IsAny<Product>()))
                .ReturnsAsync(true);

            var request = new UpdateProductRequest { Name = "Gaming Laptop" };

            // Act
            var result = await _controller.UpdateProduct(productId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<UpdateProductResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Gaming Laptop", response.Product!.Name);
            Assert.Equal(999.99m, response.Product.Price); // Price unchanged

            _mockRepository.Verify(r => r.UpdateProductAsync(It.Is<Product>(p => 
                p.Name == "Gaming Laptop" && p.Price == 999.99m)), 
                Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_WithOnlyPrice_UpdatesPriceOnly()
        {
            // Arrange
            int productId = 1;
            var existingProduct = new Product 
            { 
                Id = productId, 
                Name = "Laptop", 
                Price = 999.99m, 
                CategoryId = 1 
            };

            _mockRepository
                .Setup(r => r.GetProductByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _mockRepository
                .Setup(r => r.UpdateProductAsync(It.IsAny<Product>()))
                .ReturnsAsync(true);

            var request = new UpdateProductRequest { Price = 1299.99m };

            // Act
            var result = await _controller.UpdateProduct(productId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<UpdateProductResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Laptop", response.Product!.Name); // Name unchanged
            Assert.Equal(1299.99m, response.Product.Price);

            _mockRepository.Verify(r => r.UpdateProductAsync(It.Is<Product>(p => 
                p.Name == "Laptop" && p.Price == 1299.99m)), 
                Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var request = new UpdateProductRequest { Name = "Test" };

            // Act
            var result = await _controller.UpdateProduct(0, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);

            _mockRepository.Verify(r => r.GetProductByIdAsync(It.IsAny<int>()), Times.Never);
            _mockRepository.Verify(r => r.UpdateProductAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProduct_WithNegativeId_ReturnsBadRequest()
        {
            // Arrange
            var request = new UpdateProductRequest { Name = "Test" };

            // Act
            var result = await _controller.UpdateProduct(-1, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateProduct_WithNoFieldsToUpdate_ReturnsBadRequest()
        {
            // Arrange
            var request = new UpdateProductRequest(); // No Name or Price

            // Act
            var result = await _controller.UpdateProduct(1, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);

            _mockRepository.Verify(r => r.GetProductByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProduct_WithEmptyName_ReturnsBadRequest()
        {
            // Arrange
            var request = new UpdateProductRequest { Name = "" };

            // Act
            var result = await _controller.UpdateProduct(1, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateProduct_WithWhitespaceName_ReturnsBadRequest()
        {
            // Arrange
            var request = new UpdateProductRequest { Name = "   " };

            // Act
            var result = await _controller.UpdateProduct(1, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateProduct_ProductNotFound_ReturnsNotFound()
        {
            // Arrange
            int productId = 999;
            _mockRepository
                .Setup(r => r.GetProductByIdAsync(productId))
                .ReturnsAsync((Product?)null);

            var request = new UpdateProductRequest { Name = "Test Product" };

            // Act
            var result = await _controller.UpdateProduct(productId, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.NotNull(notFoundResult.Value);

            _mockRepository.Verify(r => r.GetProductByIdAsync(productId), Times.Once);
            _mockRepository.Verify(r => r.UpdateProductAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProduct_RepositoryUpdateFails_ReturnsInternalServerError()
        {
            // Arrange
            int productId = 1;
            var existingProduct = new Product 
            { 
                Id = productId, 
                Name = "Laptop", 
                Price = 999.99m, 
                CategoryId = 1 
            };

            _mockRepository
                .Setup(r => r.GetProductByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _mockRepository
                .Setup(r => r.UpdateProductAsync(It.IsAny<Product>()))
                .ReturnsAsync(false);

            var request = new UpdateProductRequest { Name = "Gaming Laptop" };

            // Act
            var result = await _controller.UpdateProduct(productId, request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            int productId = 1;
            var existingProduct = new Product 
            { 
                Id = productId, 
                Name = "Laptop", 
                Price = 999.99m, 
                CategoryId = 1 
            };

            _mockRepository
                .Setup(r => r.GetProductByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _mockRepository
                .Setup(r => r.UpdateProductAsync(It.IsAny<Product>()))
                .ThrowsAsync(new Exception("Database error"));

            var request = new UpdateProductRequest { Name = "Gaming Laptop" };

            // Act
            var result = await _controller.UpdateProduct(productId, request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_PreservesOriginalCategoryId()
        {
            // Arrange
            int productId = 1;
            var existingProduct = new Product 
            { 
                Id = productId, 
                Name = "Laptop", 
                Price = 999.99m, 
                CategoryId = 1 
            };

            _mockRepository
                .Setup(r => r.GetProductByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _mockRepository
                .Setup(r => r.UpdateProductAsync(It.IsAny<Product>()))
                .ReturnsAsync(true);

            var request = new UpdateProductRequest { Name = "Gaming Laptop", Price = 1299.99m };

            // Act
            var result = await _controller.UpdateProduct(productId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<UpdateProductResponse>(okResult.Value);
            Assert.Equal(1, response.Product!.CategoryId); // CategoryId unchanged

            _mockRepository.Verify(r => r.UpdateProductAsync(It.Is<Product>(p => 
                p.CategoryId == 1)), 
                Times.Once);
        }

        #endregion

        #region Helper Methods

        private static List<Product> GetSampleProducts()
        {
            return new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Price = 999.99m, CategoryId = 1 },
                new Product { Id = 2, Name = "Mouse", Price = 29.99m, CategoryId = 1 },
                new Product { Id = 3, Name = "Keyboard", Price = 79.99m, CategoryId = 1 },
                new Product { Id = 4, Name = "Monitor", Price = 299.99m, CategoryId = 1 },
                new Product { Id = 5, Name = "Desk Chair", Price = 199.99m, CategoryId = 2 },
                new Product { Id = 6, Name = "Desk Lamp", Price = 49.99m, CategoryId = 2 }
            };
        }

        #endregion
    }
}
