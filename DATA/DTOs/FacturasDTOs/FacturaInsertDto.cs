using System.ComponentModel.DataAnnotations;

namespace TPLogicaWebApi.DATA.DTOs.FacturasDTOs
{
    public class FacturaInsertDto
    {
        [Required]
        [StringLength(50)]
        public string? TipoFactura { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int IdEmpleado { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int IdCliente { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "La factura debe tener al menos un detalle.")]
        public List<DetalleFacturaInsertDto> Detalles { get; set; } = new();
    }
}
