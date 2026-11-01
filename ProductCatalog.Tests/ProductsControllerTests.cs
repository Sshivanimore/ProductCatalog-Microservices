using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductCatalog.Api.Controllers;
using ProductCatalog.Api.Dtos;
using ProductCatalog.Api.Models;
using ProductCatalog.Api.Repositories;
using ProductCatalog.Api.Services;

namespace ProductCatalog.Tests;

public class ProductsControllerTests
{
    private readonly Mock<IProductRepository> _repo = new();
    private readonly Mock<ICacheService> _cache = new();

    private ProductsController CreateController() => new(_repo.Object, _cache.Object);

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductMissing()
    {
        _repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        var result = await CreateController().GetById(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAll_ReturnsCachedData_WithoutHittingRepository()
    {
        var cached = new List<ProductDto> { new(1, "Pen", 10, 5, "Books") };
        _cache.Setup(c => c.Get<List<ProductDto>>("products:all")).Returns(cached);

        var result = await CreateController().GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(cached, ok.Value);
        _repo.Verify(r => r.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAll_LoadsFromRepository_AndStoresInCache_WhenCacheEmpty()
    {
        _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>
        {
            new() { Id = 1, Name = "Pen", Price = 10, Stock = 5, Category = new Category { Name = "Books" } }
        });

        var result = await CreateController().GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsType<List<ProductDto>>(ok.Value);
        Assert.Single(list);
        _cache.Verify(c => c.Set("products:all", It.IsAny<List<ProductDto>>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsCreated_AndClearsCache()
    {
        var dto = new CreateProductDto { Name = "Pen", Price = 10, Stock = 5, CategoryId = 2 };
        _repo.Setup(r => r.AddAsync(It.IsAny<Product>()))
             .ReturnsAsync((Product p) => { p.Id = 7; return p; });
        _repo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(new Product
        {
            Id = 7, Name = "Pen", Price = 10, Stock = 5, CategoryId = 2,
            Category = new Category { Id = 2, Name = "Books" }
        });

        var result = await CreateController().Create(dto);

        Assert.IsType<CreatedAtActionResult>(result.Result);
        _cache.Verify(c => c.Remove("products:all"), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenProductMissing()
    {
        _repo.Setup(r => r.DeleteAsync(5)).ReturnsAsync(false);

        var result = await CreateController().Delete(5);

        Assert.IsType<NotFoundResult>(result);
    }
}