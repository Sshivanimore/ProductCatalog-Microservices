using System.Net;
using OrderService.Api.Models;

namespace OrderService.Api.Services;

public interface ICatalogClient
{
    Task<ProductInfo?> GetProductAsync(int id);
}

public class CatalogClient(HttpClient http) : ICatalogClient
{
    public async Task<ProductInfo?> GetProductAsync(int id)
    {
        var response = await http.GetAsync($"api/products/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductInfo>();
    }
}