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

- **.NET 10** - Web API framework
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

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

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

The API will start and be available, depending on the selected launch profile, at:
- **AspNetCoreWebAPI (http)**: `http://localhost:5183`
- **AspNetCoreWebAPI (https)**: `https://localhost:7053`

### Access Swagger UI

The Swagger UI provides:
- Complete API documentation
- Interactive testing interface
- Request/response examples
- Model schemas with validation rules
- The API includes sample data for testing

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

## Architecture

### Dependency Injection
The API uses dependency injection for loose coupling:

```csharp
builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();
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

## Development

### Using Visual Studio

1. Open `AspNetCoreWebAPI.slnx`
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

## Testing

The project includes comprehensive tests covering unit and integration scenarios.

| Test Suite | Description |
|------------|-------------|
| Repository Unit Tests | Tests filtering, CRUD operations, edge cases |
| Controller Unit Tests | Tests with mocked dependencies |
| Integration Tests | End-to-end scenarios with real repository |

See [AspNetCoreWebAPI.Tests/README.md](AspNetCoreWebAPI.Tests/README.md) for detailed test documentation.

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
- [ ] Install .NET 10 SDK
- [ ] Run `dotnet restore`
- [ ] Run `dotnet build`
- [ ] Run `dotnet run --project AspNetCoreWebAPI`
- [ ] Open `https://localhost:7053` in browser
- [ ] Explore the API using Swagger UI
- [ ] Run tests with `dotnet test` or Visual Studio Test Explorer
