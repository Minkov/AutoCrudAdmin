# AutoCrudAdmin Integration Tests

This project contains comprehensive integration tests for the AutoCrudAdmin library.

## Test Coverage

The integration test suite includes tests for:

1. **CRUD Operations** (`CrudOperationsTests.cs`)
   - Index/List functionality
   - Create operations (GET and POST)
   - Edit operations (GET and POST)
   - Delete operations (GET and POST)
   - GetAll functionality
   - Validation error handling

2. **Authorization** (`AuthorizationTests.cs`)
   - Synchronous authorization filters
   - Asynchronous authorization filters
   - Multiple authorization filters
   - Authorization failure scenarios

3. **Form Generation** (`FormGenerationTests.cs`)
   - Form control generation for entities
   - Hidden field configuration
   - Shown field configuration
   - Read-only field configuration
   - Required field validation
   - MaxLength validation
   - Enum property handling

4. **Autocomplete and Grid** (`AutocompleteAndGridTests.cs`)
   - Autocomplete search functionality
   - Grid column configuration
   - Hidden/shown column management
   - Pagination
   - Custom page sizes

5. **Helper Methods** (`HelperMethodsTests.cs`)
   - ReflectionHelper functionality
   - Type extensions
   - Primary key detection (single and composite)
   - Expression builder
   - Simple/complex type detection

## Running the Tests

### Prerequisites
- .NET 6.0 SDK or later
- Entity Framework Core 6.0

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~CrudOperationsTests"
```

### Run with Coverage
```bash
dotnet test /p:CollectCoverage=true
```

## Test Infrastructure

- **TestDbContext**: In-memory database context for testing
- **DatabaseFixture**: Provides seeded test data for all tests
- **TestWebApplicationFactory**: ASP.NET Core test server factory
- **TestStartup**: Test application startup configuration

## Test Data

The `DatabaseFixture` seeds the following test data:
- 2 Employees (john.doe, jane.smith)
- 1 Project (Project Alpha)
- 1 Task (Task 1)

Each test class uses `IClassFixture<DatabaseFixture>` to ensure isolated test data.

## Dependencies

- **xUnit**: Test framework
- **FluentAssertions**: Assertion library for more readable tests
- **Moq**: Mocking framework
- **Microsoft.AspNetCore.Mvc.Testing**: Integration testing infrastructure
- **Microsoft.EntityFrameworkCore.InMemory**: In-memory database provider

## Notes

- All tests use an in-memory database to ensure fast execution and isolation
- The database is reset before each test class runs
- Tests are designed to be run in parallel
- No external dependencies or databases required
