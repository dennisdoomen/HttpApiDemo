# API Comparison: Controllers vs Minimal APIs

This document demonstrates both Controller-based and Minimal API implementations of the same Package API.

## Endpoints Comparison

### Package List
- **Controller API**: `GET /api/v2.0/packages/`
- **Minimal API**: `GET /minimal-api/v2.0/packages/`

### Package Details
- **Controller API v1.0**: `GET /api/v1.0/packages/{id}` (limited fields)
- **Minimal API v1.0**: `GET /minimal-api/v1.0/packages/{id}` (limited fields)
- **Controller API v2.0**: `GET /api/v2.0/packages/{id}` (full fields)
- **Minimal API v2.0**: `GET /minimal-api/v2.0/packages/{id}` (full fields)

### Package Statistics
- **Controller API**: `GET /api/v2.0/packages/{id}/statistics`
- **Minimal API**: `GET /minimal-api/v2.0/packages/{id}/statistics`

## Key Differences

### Code Structure
1. **Controllers**: Class-based with methods decorated with attributes
   - Located in `PackageController.cs`
   - Uses `[HttpGet]`, `[Route]`, `[MapToApiVersion]` attributes
   - Inherits from `ControllerBase`
   - Action methods return `IActionResult`

2. **Minimal APIs**: Function-based with fluent configuration
   - Located in `Program.cs` as static methods
   - Uses `app.MapGet()` with chained configuration
   - Direct function delegates
   - Methods return `IResult`

### API Versioning
Both implementations use the same versioning approach:
- v1.0: Returns limited package information (4 fields per version)
- v2.0: Returns complete package information (8 fields per version)

### Response Consistency
Both approaches return identical JSON responses, ensuring API consumers can use either endpoint interchangeably.

## Testing Results

All tests pass with identical responses:
- ✅ Package list returns 4 items for both APIs
- ✅ v1.0 returns 4 fields per version object
- ✅ v2.0 returns 8 fields per version object  
- ✅ Statistics endpoint returns same download counts
- ✅ Error handling is consistent (404 for missing packages, 400 for invalid input)

## Performance Considerations

Minimal APIs generally offer:
- Slightly better performance (less overhead)
- Reduced memory allocation
- Faster startup time
- More concise code for simple scenarios

Controllers offer:
- Better organization for complex APIs
- More familiar pattern for MVC developers
- Better tooling support
- More flexible action filters and model binding