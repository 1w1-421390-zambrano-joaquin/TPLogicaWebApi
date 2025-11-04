using System.ComponentModel.DataAnnotations;

namespace TPLogicaWebApi.DATA.DTOs.ClientesDTOs
{
    public class ClienteInsertDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(30)]
        public string? NomCliente { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(30)]
        public string? ApeCliente { get; set; }

        [Required]
        [StringLength(60)]
        public string? Domicilio { get; set; }

        [Required]
        [Range(1, 99999999, ErrorMessage = "El DNI no es válido.")]
        public int Dni { get; set; }

        [Required]
        [StringLength(50)]
        public string? Telefono { get; set; }
    }
}
