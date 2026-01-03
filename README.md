# AspNetCore Web API

A sample ASP.NET Core Web API project demonstrating RESTful API design with OpenAPI/Swagger documentation, filtering capabilities, and comprehensive test coverage.

## Features

- RESTful API endpoints for product management
- Advanced filtering (name, category, price range)
- OpenAPI/Swagger UI for interactive API documentation
- Comprehensive test suite
- Clean architecture with dependency injection
- Input validation and error handling

## Tech Stack

- **.NET 9** - Web API framework
- **Swashbuckle.AspNetCore** - OpenAPI/Swagger implementation
- **xUnit** - Testing framework
- **Moq** - Mocking library for unit tests

## API Endpoints

### GET /api/products
Filter products by optional criteria:
- `name` - Filter by product name (case-insensitive, partial match)
- `categoryId` - Filter by category ID
- `minPrice` - Minimum price filter
- `maxPrice` - Maximum price filter

**Example**: `/api/products?name=laptop&minPrice=500&maxPrice=2000`

### PATCH /api/products/{id}
Update a product's name and/or price.

**Request Body**:
```json
{
  "name": "Gaming Laptop",
  "price": 1299.99
}
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later

### Clone the Repository

```bash
git clone https://github.com/yourusername/AspNetCoreWebAPI.git
cd AspNetCoreWebAPI
```

### Restore Dependencies

```bash
dotnet restore
```

### Build the Project

```bash
dotnet build
```

### Run the API

```bash
cd AspNetCoreWebAPI
dotnet run
```

The API will start and be available at:
- **HTTP**: `http://localhost:5183`
- **HTTPS**: `https://localhost:7053`

### Access Swagger UI

Once the API is running, open your browser and navigate to:

```
https://localhost:7053
```

The Swagger UI provides:
- Complete API documentation
- Interactive testing interface
- Request/response examples
- Model schemas with validation rules

## Running Tests

The project includes comprehensive tests covering unit and integration scenarios.

### Run All Tests

```bash
dotnet test
```

### Run with Detailed Output

```bash
dotnet test --verbosity normal
```

### Run Specific Test Class

```bash
# Repository tests
dotnet test --filter "FullyQualifiedName~InMemoryProductRepositoryTests"

# Controller tests
dotnet test --filter "FullyQualifiedName~ProductsControllerTests"

# Integration tests
dotnet test --filter "FullyQualifiedName~ProductsControllerIntegrationTests"
```

## Project Structure

```
AspNetCoreWebAPI/
├── AspNetCoreWebAPI/                           # Main API project
│   ├── Controllers/                            # API controllers
│   │   └── ProductsController.cs
│   ├── Interfaces/                             # Repository interfaces
│   │   └── IProductRepository.cs
│   ├── Models/                                 # Data models and DTOs
│   │   ├── Category.cs
│   │   ├── Product.cs
│   │   ├── Requests/                           # Request DTOs
│   │   └── Responses/                          # Response DTOs
│   ├── Repositories/                           # Data access implementations
│   │   └── InMemoryProductRepository.cs
│   └── Program.cs                              # Application entry point
│
└── AspNetCoreWebAPI.Tests/                     # Test project
    ├── Controllers/                            # Controller unit tests
    ├── Integration/                            # Integration tests
    └── Repositories/                           # Repository unit tests
```

## Sample Data

The API includes sample data for testing:

| Product | Price | Category |
|---------|-------|----------|
| Laptop | $999.99 | Electronics |
| Mouse | $29.99 | Electronics |
| Keyboard | $79.99 | Electronics |
| Monitor | $299.99 | Electronics |
| Desk Chair | $199.99 | Furniture |
| Desk Lamp | $49.99 | Furniture |

## API Usage Examples

### Filter All Products
```bash
curl https://localhost:7053/api/products
```

### Filter by Category
```bash
curl https://localhost:7053/api/products?categoryId=1
```

### Filter by Price Range
```bash
curl https://localhost:7053/api/products?minPrice=50&maxPrice=300
```

### Filter with Multiple Criteria
```bash
curl https://localhost:7053/api/products?name=desk&categoryId=2&minPrice=100
```

### Update Product
```bash
curl -X PATCH https://localhost:7053/api/products/1 \
  -H "Content-Type: application/json" \
  -d '{"name":"Gaming Laptop","price":1299.99}'
```

## Development

### Using Visual Studio

1. Open `AspNetCoreWebAPI.sln`
2. Select launch profile: **AspNetCoreWebAPI (https)** or **AspNetCoreWebAPI (http)**
3. Press **F5** to run with debugging
4. Browser will automatically open to Swagger UI

### Using VS Code

1. Open the project folder
2. Press **F5** to run
3. Navigate to `https://localhost:7053` in your browser

### Using Command Line

```bash
# Development mode
dotnet run --project AspNetCoreWebAPI

# Watch mode (auto-reload on changes)
dotnet watch run --project AspNetCoreWebAPI
```

## Architecture

### Dependency Injection
The API uses dependency injection for loose coupling:

```csharp
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
```

### Repository Pattern
Controllers depend on `IProductRepository` interface, allowing easy swapping of implementations:
- Current: `InMemoryProductRepository` (for demonstration)
- Future: Database-backed repository, external API, etc.

### Validation
Input validation using Data Annotations and custom validation logic:
- Required fields
- Range validation
- String length limits
- Business rule validation

## Testing

The project includes comprehensive test coverage:

| Test Suite | Description |
|------------|-------------|
| Repository Unit Tests | Tests filtering, CRUD operations, edge cases |
| Controller Unit Tests | Tests with mocked dependencies |
| Integration Tests | End-to-end scenarios with real repository |

See [AspNetCoreWebAPI.Tests/README.md](AspNetCoreWebAPI.Tests/README.md) for detailed test documentation.

## Configuration

### Launch Profiles

Two launch profiles are available in `Properties/launchSettings.json`:

- **AspNetCoreWebAPI (http)** - HTTP on port 5183
- **AspNetCoreWebAPI (https)** - HTTPS on port 7053

Both profiles automatically open Swagger UI on launch.

### Swagger Configuration

Swagger UI is configured to:
- Display at the root URL (`/`)
- Include XML documentation comments
- Show example values
- Provide interactive testing

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License.

## Documentation

- [API Documentation](AspNetCoreWebAPI/API_DOCUMENTATION.md) - Detailed endpoint documentation
- [Test Documentation](AspNetCoreWebAPI.Tests/README.md) - Test suite overview
- [Test Summary](AspNetCoreWebAPI.Tests/DETAILS.md) - Test suite details

## Quick Start Checklist

- [ ] Clone the repository
- [ ] Install .NET 9 SDK
- [ ] Run `dotnet restore`
- [ ] Run `dotnet build`
- [ ] Run `dotnet run --project AspNetCoreWebAPI`
- [ ] Open `https://localhost:7053` in browser
- [ ] Explore the API using Swagger UI
- [ ] Run tests with `dotnet test` or Visual Studio Test Explorer

