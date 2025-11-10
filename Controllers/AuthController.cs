using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TPLogicaWebApi.DATA.DTOs.UsuariosDTOs;
using TPLogicaWebApi.DATA.Models;
using TPLogicaWebApi.DATA.Services.Interfaces;

namespace TPLogicaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _service;
        private IConfiguration _config;
        public AuthController(IAuthService service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            try
            {
                var nuevoUsuario = await _service.Crear(dto);
               
                return Ok(new { message = "Usuario registrado exitosamente." });
            }
            catch (KeyNotFoundException ex)
            {
                
                return BadRequest(ex.Message); 
            }
            catch (InvalidOperationException ex)
            {
                
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, error);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
               
                var usuario = await _service.Logear(dto);

                
                if (usuario == null)
                {
                    return Unauthorized("Email o contraseña incorrectos."); 
                }

                
                var token = GenerateJwtToken(usuario);

               
                return Ok(new { token = token });
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, error);
            }
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var claims = new List<Claim>
            {

                new Claim(JwtRegisteredClaimNames.Sub, usuario.IdEmpleado.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.IdCredencialNavigation.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
       
                new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                  ClaimValueTypes.Integer64)
            };

            
            foreach (var auth in usuario.Autenticacions)
            {
                claims.Add(new Claim(ClaimTypes.Role, auth.IdRolNavigation.Rol));
            }


            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                notBefore: now,
                expires: now.AddHours(8),
                signingCredentials: creds
                 );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
