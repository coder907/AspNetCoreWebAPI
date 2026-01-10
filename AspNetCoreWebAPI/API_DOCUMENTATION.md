# Product API Endpoints

This document describes the two main product API endpoints.

## Endpoints

### 1. GET /api/products - Filter Products

Returns a filtered list of products based on optional criteria.

**Query Parameters:**
- `name` (string, optional): Filter by product name (partial match, case-insensitive)
- `categoryid` (integer, optional): Filter by category ID (must be positive)
- `minprice` (decimal, optional): Minimum price filter (must be non-negative)
- `maxprice` (decimal, optional): Maximum price filter (must be non-negative)

**Example Requests:**
```
GET /api/products
GET /api/products?name=laptop
GET /api/products?categoryid=1&minprice=50&maxprice=500
GET /api/products?name=desk&minprice=100
```

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "name": "Laptop",
    "price": 999.99,
    "categoryId": 1
  }
]
```

**Validation Rules:**
- CategoryId must be a positive number if provided
- MinPrice and MaxPrice must be non-negative if provided
- MinPrice cannot exceed MaxPrice

---

### 2. PATCH /api/products/{id} - Update Product

Updates a product's name and/or price.

**Path Parameters:**
- `id` (integer, required): Product ID to update

**Request Body:**
```json
{
  "name": "Gaming Laptop",
  "price": 1299.99
}
```

**Note:** At least one field (name or price) must be provided.

**Example Requests:**
```
PATCH /api/products/1
Content-Type: application/json

{
  "name": "Gaming Laptop"
}
```

```
PATCH /api/products/1
Content-Type: application/json

{
  "price": 1299.99
}
```

```
PATCH /api/products/1
Content-Type: application/json

{
  "name": "Gaming Laptop",
  "price": 1299.99
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Product updated successfully",
  "product": {
    "id": 1,
    "name": "Gaming Laptop",
    "price": 1299.99,
    "categoryId": 1
  }
}
```

**Response (404 Not Found):**
```json
{
  "message": "Product with ID 999 not found"
}
```

**Validation Rules:**
- Product ID must be positive
- Name must be 1-200 characters if provided
- Price must be greater than zero if provided
- At least one field must be provided for update

---

## Architecture

### Abstraction Layer

The controller depends on the `IProductRepository` interface, allowing different implementations to be swapped without changing controller code.

**Interface Location:** `Interfaces/IProductRepository.cs`

**Current Implementation:** `InMemoryProductRepository` (in-memory storage with sample data)

**Factory Method Pattern:**

The application uses a factory method to create and register repository instances, providing flexibility to switch implementations:

```csharp
// In Program.cs
services.AddScoped<IProductRepository>(serviceProvider => 
    CreateProductRepository(serviceProvider, configuration));

private static IProductRepository CreateProductRepository(
    IServiceProvider serviceProvider, 
    IConfiguration configuration)
{
    // Factory method - can select implementation based on configuration
    return new InMemoryProductRepository();
}
```

**To swap implementations:**
1. Create a new class implementing `IProductRepository`
2. Update the factory method in `Program.cs`:
```csharp
private static IProductRepository CreateProductRepository(
    IServiceProvider serviceProvider, 
    IConfiguration configuration)
{
    var repositoryType = configuration["RepositoryType"];
    return repositoryType switch
    {
        "Database" => new DatabaseProductRepository(
            serviceProvider.GetRequiredService<DbContext>()),
        "Cache" => new CachedProductRepository(
            serviceProvider.GetRequiredService<IMemoryCache>()),
        "InMemory" => new InMemoryProductRepository(),
        _ => new InMemoryProductRepository()
    };
}
```
3. Add configuration to `appsettings.json`:
```json
{
  "RepositoryType": "InMemory"
}
```

**Example implementations:**
- **Database-backed repository** - Persistent storage using Entity Framework
- **Cached repository** - Combines database with in-memory caching for performance
- **External API repository** - Integrates with external product services
- **Mock repository** - For testing purposes

**Benefits of Factory Method:**
- Switch implementations without modifying controller code
- Configuration-based selection (no code changes needed)
- Access to dependency injection container for complex dependencies
- Environment-specific implementations (development vs. production)

---

### PUT vs. PATCH: Design Decision

**PUT (Complete Replacement)** replaces the entire resource. It requires all fields to be sent, making it idempotent and predictable but verbose. If you omit a field, it gets cleared. Example: `PUT /api/products/1` requires `{ "id": 1, "name": "...", "price": ..., "categoryId": 1 }`.

**PATCH (Partial Update)** modifies only specified fields. It's more flexible and bandwidth-efficient, but requires careful handling of null values and partial validation. Example: `PATCH /api/products/1` with `{ "price": 1299.99 }` updates only the price.

**Why PATCH was chosen:**
1. **Semantic Accuracy** - Our use case is partial updates (name and/or price only), not full replacement
2. **Immutable Fields** - `id` and `categoryId` cannot be changed, making PUT somewhat verbose (sending fields you can't update)
3. **Flexibility** - Update one field (`{ "name": "..." }`) or both (`{ "name": "...", "price": ... }`) without sending readonly fields
4. **Better UX** - Simpler for clients; only send what needs to change
5. **Network Efficiency** - Smaller payloads, especially important for mobile apps
6. **Future-Proof** - Easy to add new updatable fields without breaking existing clients

**Trade-off:** PATCH requires more complex server-side validation (checking which fields are present, handling partial data) compared to PUT's all-or-nothing approach. However, the improved flexibility and alignment with business rules (only name/price are updatable) makes PATCH the better choice for this API.

**Implementation Note:** This API uses JSON Merge Patch (simple key-value updates). For complex scenarios involving nested objects or arrays, consider JSON Patch (RFC 6902) with explicit operations: `[{ "op": "replace", "path": "/price", "value": 1299.99 }]`.

---

## Error Handling

All endpoints handle errors gracefully:
- **400 Bad Request**: Invalid input, validation errors
- **404 Not Found**: Product not found (update endpoint)
- **500 Internal Server Error**: Unexpected server errors

All errors return descriptive messages to help identify the issue.

---

## Testing with Swagger

When running the application:
1. Navigate to the root URL (e.g., `https://localhost:7053`)
2. Swagger UI will display both endpoints with full documentation
3. Use the "Try it out" button to test each endpoint interactively
4. All XML documentation comments are visible in the Swagger UI
