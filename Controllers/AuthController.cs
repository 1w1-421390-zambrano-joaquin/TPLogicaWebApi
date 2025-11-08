using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TPLogicaWebApi.DATA.DTOs.UsuariosDTOs;
using TPLogicaWebApi.DATA.Models;
using TPLogicaWebApi.DATA.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

            
            var claims = new List<Claim>
            {
                
                new Claim(JwtRegisteredClaimNames.Sub, usuario.IdEmpleado.ToString()),
                
                new Claim(JwtRegisteredClaimNames.Email, usuario.IdCredencialNavigation.Email), 
                
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            
            foreach (var auth in usuario.Autenticacions)
            {
                claims.Add(new Claim(ClaimTypes.Role, auth.IdRolNavigation.Rol));
            }

            
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8), 
                signingCredentials: creds
            );

            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
