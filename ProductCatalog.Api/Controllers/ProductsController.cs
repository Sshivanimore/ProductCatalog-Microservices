using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Dtos;
using ProductCatalog.Api.Models;
using ProductCatalog.Api.Repositories;
using ProductCatalog.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository repo, ICacheService cache) : ControllerBase
{
    private const string AllKey = "products:all";

    private static ProductDto ToDto(Product p) =>
        new(p.Id, p.Name, p.Price, p.Stock, p.Category?.Name ?? "");

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll()
    {
        var cached = cache.Get<List<ProductDto>>(AllKey);
        if (cached is not null) return Ok(cached);

        var dtos = (await repo.GetAllAsync()).Select(ToDto).ToList();
        cache.Set(AllKey, dtos, TimeSpan.FromMinutes(2));
        return Ok(dtos);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<ProductDto>>> Search(string? name, decimal? minPrice, decimal? maxPrice) =>
        Ok((await repo.SearchAsync(name, minPrice, maxPrice)).Select(ToDto));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var p = await repo.GetByIdAsync(id);
        return p is null ? NotFound() : Ok(ToDto(p));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name, Price = dto.Price, Stock = dto.Stock, CategoryId = dto.CategoryId
        };
        await repo.AddAsync(product);
        cache.Remove(AllKey);
        var saved = await repo.GetByIdAsync(product.Id);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, ToDto(saved!));
    }

    [Authorize] 
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CreateProductDto dto)
    {
        var existing = await repo.GetByIdAsync(id);
        if (existing is null) return NotFound();

        existing.Name = dto.Name;
        existing.Price = dto.Price;
        existing.Stock = dto.Stock;
        existing.CategoryId = dto.CategoryId;

        await repo.UpdateAsync(existing);
        cache.Remove(AllKey);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await repo.DeleteAsync(id)) return NotFound();
        cache.Remove(AllKey);
        return NoContent();
    }
}