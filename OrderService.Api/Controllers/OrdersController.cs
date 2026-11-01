using Microsoft.AspNetCore.Mvc;
using OrderService.Api.Models;
using OrderService.Api.Services;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(ICatalogClient catalog) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderResult>> Place(PlaceOrderDto dto)
    {
        if (dto.Quantity <= 0) return BadRequest("Quantity must be greater than zero.");

        var product = await catalog.GetProductAsync(dto.ProductId);
        if (product is null) return NotFound("Product not found in catalog.");
        if (product.Stock < dto.Quantity)
            return Conflict($"Only {product.Stock} in stock.");

        return Ok(new OrderResult(product.Id, product.Name, dto.Quantity,
            product.Price, product.Price * dto.Quantity, "Confirmed"));
    }
}