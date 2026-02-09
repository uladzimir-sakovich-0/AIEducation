# FinanceTracker Backend

A complete .NET 10 backend solution for FinanceTracker with PostgreSQL database support.

## Project Structure

```
Backend/
├── src/
│   ├── FinanceTracker.slnx                    # Solution file (.NET 10 XML format)
│   ├── FinanceTracker.API/                    # ASP.NET Core Web API
│   │   ├── Controllers/
│   │   │   └── HealthController.cs            # Health check endpoint
│   │   ├── Program.cs                         # Application entry point
│   │   └── appsettings.json                   # Configuration with PostgreSQL connection string
│   ├── FinanceTracker.Infrastructure/         # Data access layer
│   │   └── Data/
│   │       └── FinanceTrackerDbContext.cs     # Entity Framework DbContext
│   └── FinanceTracker.DB/                     # Database entities
│       └── Entities/                          # Database entity classes (to be added)
└── tests/
    └── FinanceTracker.UnitTests/              # Unit tests with xUnit
        └── SampleTests.cs                     # Sample test class
```

## Technologies Used

- **.NET 10.0.102** - Latest .NET SDK
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 10.0.2** - ORM for data access
- **Npgsql.EntityFrameworkCore.PostgreSQL 10.0.0** - PostgreSQL provider for EF Core
- **xUnit** - Unit testing framework
- **Moq 4.20.72** - Mocking library for tests

## Project Dependencies

### FinanceTracker.API
- References: `FinanceTracker.Infrastructure`
- NuGet Packages:
  - Microsoft.EntityFrameworkCore (10.0.2)
  - Microsoft.EntityFrameworkCore.Design (10.0.2)
  - Npgsql.EntityFrameworkCore.PostgreSQL (10.0.0)

### FinanceTracker.Infrastructure
- References: `FinanceTracker.DB`
- NuGet Packages:
  - Microsoft.EntityFrameworkCore (10.0.2)
  - Npgsql.EntityFrameworkCore.PostgreSQL (10.0.0)

### FinanceTracker.DB
- NuGet Packages:
  - Microsoft.EntityFrameworkCore (10.0.2)
  - Npgsql.EntityFrameworkCore.PostgreSQL (10.0.0)

### FinanceTracker.UnitTests
- References: All three main projects
- NuGet Packages:
  - xUnit
  - xunit.runner.visualstudio
  - Moq (4.20.72)
  - Microsoft.NET.Test.Sdk

## Getting Started

### Prerequisites

- .NET 10 SDK
- PostgreSQL database server

### Database Configuration

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=financetracker;Username=postgres;Password=REPLACE_WITH_YOUR_PASSWORD"
  }
}
```

**Important Security Notes:**
- Never commit actual passwords to source control
- For development: Use [User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- For production: Use environment variables or a secure key vault

**To use User Secrets (recommended for development):**
```bash
cd Backend/src/FinanceTracker.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=financetracker;Username=postgres;Password=YOUR_ACTUAL_PASSWORD"
```

### Building the Solution

```bash
cd Backend/src
dotnet build FinanceTracker.slnx
```

### Running Tests

```bash
cd Backend/src
dotnet test FinanceTracker.slnx
```

### Running the API

```bash
cd Backend/src/FinanceTracker.API
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7088`
- HTTP: `http://localhost:5270`

### API Endpoints

#### Health Check
- **GET** `/api/health`
- Returns: `{ "status": "Healthy", "timestamp": "2024-..." }`

## Development

### Adding New Entities

1. Create entity classes in `FinanceTracker.DB/Entities/`
2. Add `DbSet<T>` properties to `FinanceTrackerDbContext`
3. Configure entity relationships in `OnModelCreating`
4. Create and apply migrations:
   ```bash
   cd Backend/src/FinanceTracker.API
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

### Running Migrations

```bash
# Add a new migration
dotnet ef migrations add <MigrationName> --project ../FinanceTracker.Infrastructure --startup-project .

# Update database
dotnet ef database update --project ../FinanceTracker.Infrastructure --startup-project .
```

## Testing

The project uses xUnit for testing with the following conventions:
- Test class naming: `[ClassName]Tests`
- Test method naming: `When[Condition]_Then[ExpectedResult]`
- Arrange-Act-Assert (AAA) pattern

Example test structure:
```csharp
[Fact]
public void WhenAddingNumbers_ThenResultIsCorrect()
{
    // Arrange
    var a = 2;
    var b = 3;
    var expected = 5;

    // Act
    var actual = a + b;

    // Assert
    Assert.Equal(expected, actual);
}
```

## Build Status

✅ **Build**: Successful  
✅ **Tests**: 3 passed, 0 failed

## Future Enhancements

- Add authentication and authorization
- Implement CRUD operations for financial transactions
- Add API versioning
- Implement repository pattern
- Add logging and monitoring
- Add API documentation with Swagger/OpenAPI
- Implement data seeding
- Add integration tests

## Notes

- The solution uses the new `.slnx` format introduced in .NET 10
- All projects target `net10.0` framework
- PostgreSQL connection string should be secured using User Secrets or environment variables in production
- The `HealthController` provides a basic health check endpoint for monitoring

## License

[Your License Here]
