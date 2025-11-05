using System.ComponentModel.DataAnnotations;

namespace TPLogicaWebApi.DATA.DTOs.FacturasDTOs
{
    public class DetalleFacturaInsertDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int IdProducto { get; set; }

        

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [StringLength(70)]
        public string? Observ { get; set; }
    }
}
