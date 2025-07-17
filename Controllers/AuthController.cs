using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TravelRequestApi.Data;

namespace TravelRequestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto dto)
    {
        if (_context.Users.Any(u => u.Email == dto.Email))
            return BadRequest("Email ya registrado.");

        var user = new User
        {
            Username = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok("Usuario registrado correctamente.");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Credenciales inválidas.");

        Console.WriteLine(user);

        var token = GenerateJwtToken(user);
        return Ok(new AuthResponseDto { Token = token, Role = user.Role });
    }

    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == dto.Email);
        if (user == null) return NotFound("Usuario no encontrado.");

        var code = new Random().Next(100000, 999999).ToString();
        user.ResetCode = code;
        user.ResetCodeExpiration = DateTime.UtcNow.AddMinutes(5);

        _context.SaveChanges();

        return Ok(new { Code = code });
    }

    [HttpPost("reset-password")]
    public IActionResult ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var user = _context.Users.SingleOrDefault(u =>
            u.Email == dto.Email &&
            u.ResetCode == dto.Code &&
            u.ResetCodeExpiration > DateTime.UtcNow
        );

        if (user == null)
            return BadRequest("Código inválido o expirado.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.ResetCode = null;
        user.ResetCodeExpiration = null;

        _context.SaveChanges();

        return Ok("Contraseña actualizada correctamente.");
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
