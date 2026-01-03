# Product API Endpoints

This document describes the two main product API endpoints.

## Endpoints

### 1. GET /api/products - Filter Products

Returns a filtered list of products based on optional criteria.

**Query Parameters:**
- `name` (string, optional): Filter by product name (partial match, case-insensitive)
- `categoryId` (integer, optional): Filter by category ID (must be positive)
- `minPrice` (decimal, optional): Minimum price filter (must be non-negative)
- `maxPrice` (decimal, optional): Maximum price filter (must be non-negative)

**Example Requests:**
```
GET /api/products
GET /api/products?name=laptop
GET /api/products?categoryId=1&minPrice=50&maxPrice=500
GET /api/products?name=desk&minPrice=100
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

**To swap implementations:**
1. Create a new class implementing `IProductRepository`
2. Update the registration in `Program.cs`:
```csharp
builder.Services.AddSingleton<IProductRepository, YourNewImplementation>();
```

Example implementations could include:
- Database-backed repository (Entity Framework)
- External API repository
- Cached repository
- Mock repository for testing

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
1. Navigate to the root URL (e.g., `https://localhost:5001/`)
2. Swagger UI will display both endpoints with full documentation
3. Use the "Try it out" button to test each endpoint interactively
4. All XML documentation comments are visible in the Swagger UI
