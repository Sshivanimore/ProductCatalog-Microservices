using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Api.Dtos;

public class RegisterDto
{
    [Required, MaxLength(50)] public string Username { get; set; } = string.Empty;
    [Required, MinLength(6)] public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";   // demo only
}

public class LoginDto
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}