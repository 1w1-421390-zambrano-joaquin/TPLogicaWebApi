using System.ComponentModel.DataAnnotations;

namespace TPLogicaWebApi.DATA.DTOs.ProductoDTOs
{
    public class ProductoInsertDto
    {
        [Required(ErrorMessage = "El nombre comercial es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        public string? NombreComercial { get; set; }

        [Required(ErrorMessage = "El principio activo es obligatorio.")]
        [StringLength(50)] // Puedes omitir el mensaje de error para usar el de por defecto
        public string? PrincipioActivo { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public decimal ContenidoCantidad { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "La unidad de medida no puede tener más de 10 caracteres.")]
        public string? UnidadMedida { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El número de lote debe ser mayor a 0.")]
        public int NroLote { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        public DateOnly FVencimiento { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Required]
        [StringLength(100)]
        public string? Proveedor { get; set; }
    }
}
