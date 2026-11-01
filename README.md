# Product Catalog & Order Microservices

Two ASP.NET Core Web API services:
- **ProductCatalog.Api**: product CRUD and search, SQL Server, JWT auth, Swagger
- **OrderService.Api**: places orders by calling the Catalog service over REST

## Tech
C#, ASP.NET Core, EF Core, LINQ, SQL Server, JWT, xUnit, Moq, Swagger

## Highlights
- Repository pattern and Dependency Injection
- Singleton in-memory cache service
- JWT authentication with role-based authorization
- Unit tests with xUnit and Moq

## How to run
1. Install .NET 10 SDK and SQL Server Express.
2. Set `ConnectionStrings:DefaultConnection` and a 32+ character `Jwt:Key` in `ProductCatalog.Api/appsettings.json`.
3. `cd ProductCatalog.Api` then `dotnet ef database update` then `dotnet run`
4. In another terminal: `cd OrderService.Api`, set `CatalogService:BaseUrl` to the Catalog API URL, then `dotnet run`
5. Open `/swagger` on the Catalog API.

## Tests
`dotnet test`