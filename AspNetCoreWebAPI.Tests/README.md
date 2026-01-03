# AspNetCoreWebAPI Tests

This project contains comprehensive unit and integration tests for the AspNetCoreWebAPI project.

## Test Framework

- **xUnit** - Test framework
- **Moq** - Mocking library for unit tests
- **Microsoft.AspNetCore.Mvc.Testing** - ASP.NET Core testing utilities

## Test Structure

### 1. Unit Tests - InMemoryProductRepository (`Repositories/InMemoryProductRepositoryTests.cs`)

Comprehensive tests for the `InMemoryProductRepository` class, focusing on:

#### Filtering Tests
- **No filters**: Returns all products
- **Name filtering**: Case-insensitive partial matching
- **Category filtering**: Filter by category ID
- **Price range filtering**: MinPrice, MaxPrice, and combined ranges
- **Multiple filters**: Combining name, category, and price filters
- **Edge cases**: Empty results, empty/whitespace names, invalid IDs

#### Product Retrieval Tests
- **Valid IDs**: Returns correct product
- **Invalid IDs**: Returns null for non-existent products

#### Update Tests
- **Name updates**: Updates product name only
- **Price updates**: Updates product price only
- **Combined updates**: Updates both name and price
- **Category updates**: Updates category ID
- **Multiple updates**: Sequential updates persist correctly
- **Non-existent products**: Returns false for invalid IDs

#### Integration Tests
- **Filter after update**: Verifies filtered results reflect updates
- **Name search after update**: New names are searchable

**Total Tests**: 37

### 2. Unit Tests - ProductsController (`Controllers/ProductsControllerTests.cs`)

Tests the `ProductsController` with mocked dependencies:

#### Constructor Tests
- Validates null parameter checks

#### GetFilteredProducts Tests
- **No filters**: Returns all products
- **Individual filters**: Name, category, price range
- **Multiple filters**: Combined filtering
- **Validation**: Invalid price ranges return BadRequest
- **Error handling**: Repository exceptions return 500
- **Empty results**: Returns empty list when no matches

#### UpdateProduct Tests
- **Valid updates**: Name and/or price updates
- **Partial updates**: Only name or only price
- **Validation**: Invalid IDs, missing fields, empty names
- **Not found**: Returns 404 for non-existent products
- **Error handling**: Repository failures return appropriate errors
- **Category preservation**: Category ID remains unchanged

**Total Tests**: 27

### 3. Integration Tests - ProductsController (`Integration/ProductsControllerIntegrationTests.cs`)

End-to-end tests with the actual `InMemoryProductRepository` implementation:

#### End-to-End Filtering Tests
- All filtering scenarios with real data
- Case-insensitive searches
- Price range filtering
- Multiple criteria filtering
- Complex filtering scenarios

#### End-to-End Update Tests
- Name-only updates
- Price-only updates
- Combined updates
- Updates to non-existent products
- Invalid input validation

#### End-to-End Workflow Tests
- **Update then filter**: Verifies updates appear in filtered results
- **Multiple updates**: Multiple sequential updates all persist
- **Update then retrieve**: Updated data is immediately available
- **Complex scenarios**: Real-world usage patterns

#### Validation Tests
- Invalid price ranges
- Invalid IDs
- Missing required fields

**Total Tests**: 30

## Running the Tests

### Run all tests
```bash
dotnet test
```

### Run tests with detailed output
```bash
dotnet test --verbosity normal
```

### Run tests from a specific class
```bash
dotnet test --filter "FullyQualifiedName~InMemoryProductRepositoryTests"
dotnet test --filter "FullyQualifiedName~ProductsControllerTests"
dotnet test --filter "FullyQualifiedName~ProductsControllerIntegrationTests"
```

### Run a specific test
```bash
dotnet test --filter "FullyQualifiedName~GetFilteredProducts_WithNoFilters_ReturnsAllProducts"
```

### Generate code coverage report
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Test Summary

| Test Suite | Tests | Focus Area |
|------------|-------|------------|
| InMemoryProductRepositoryTests | 37 | Repository logic, filtering, CRUD operations |
| ProductsControllerTests | 27 | Controller behavior with mocked dependencies |
| ProductsControllerIntegrationTests | 30 | End-to-end scenarios with real repository |
| **Total** | **94** | **Complete coverage of API functionality** |

## Test Coverage

The test suite provides comprehensive coverage of:

? **Repository Layer**
- All filtering combinations
- CRUD operations
- Edge cases and error conditions

? **Controller Layer**
- Request validation
- Response formatting
- Error handling
- Dependency injection

? **Integration**
- End-to-end workflows
- Data persistence
- Real-world scenarios

? **Validation**
- Input validation
- Business rule enforcement
- Error messages

## Key Testing Patterns

### 1. Arrange-Act-Assert (AAA)
All tests follow the AAA pattern for clarity and maintainability.

### 2. Theory Tests
Using `[Theory]` with `[InlineData]` for testing multiple scenarios with the same test logic.

### 3. Mocking with Moq
Unit tests use Moq to isolate the controller from repository dependencies.

### 4. Real Implementation Testing
Integration tests use the actual `InMemoryProductRepository` to verify end-to-end behavior.

### 5. Descriptive Test Names
Test names clearly describe what is being tested and expected outcome.

## Continuous Integration

These tests are designed to run in CI/CD pipelines:
- Fast execution (< 1 second for all 94 tests)
- No external dependencies
- Deterministic results
- Clear failure messages

## Future Enhancements

Potential additions to the test suite:
- Performance tests for large datasets
- Concurrent update scenarios
- Additional edge cases
- Database integration tests (when real DB is added)
- API endpoint tests with WebApplicationFactory
