namespace OrderService.Api.Models;

public record ProductInfo(int Id, string Name, decimal Price, int Stock, string CategoryName);
public record PlaceOrderDto(int ProductId, int Quantity);
public record OrderResult(int ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal Total, string Status);