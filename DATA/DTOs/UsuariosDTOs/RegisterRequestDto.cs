using System.ComponentModel.DataAnnotations;

namespace TPLogicaWebApi.DATA.DTOs.UsuariosDTOs
{
    public class RegisterRequestDto
    {
        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        [MinLength(6)]
        public string? Password { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int IdEmpleado { get; set; } 

        [Required]
        [Range(1, int.MaxValue)]
        public int IdRol { get; set; }
    }
}
