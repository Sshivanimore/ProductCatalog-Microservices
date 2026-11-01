using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Data;
using ProductCatalog.Api.Models;

namespace ProductCatalog.Api.Repositories;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public async Task<List<Product>> GetAllAsync() =>
        await db.Products.AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();

    public async Task<Product?> GetByIdAsync(int id) =>
        await db.Products.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Product>> SearchAsync(string? name, decimal? minPrice, decimal? maxPrice)
    {
        var query = db.Products.AsNoTracking().Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Contains(name));
        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        return await query.OrderBy(p => p.Price).ToListAsync();
    }

    public async Task<Product> AddAsync(Product product)
    {
        db.Products.Add(product);
        await db.SaveChangesAsync();
        return product;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        db.Products.Update(product);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return false;
        db.Products.Remove(product);
        return await db.SaveChangesAsync() > 0;
    }
}