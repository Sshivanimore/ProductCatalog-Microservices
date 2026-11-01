using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Api.Dtos;

public record ProductDto(int Id, string Name, decimal Price, int Stock, string CategoryName);

public class CreateProductDto
{
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    [Range(0.01, 1000000)] public decimal Price { get; set; }
    [Range(0, 100000)] public int Stock { get; set; }
    [Required] public int CategoryId { get; set; }
}