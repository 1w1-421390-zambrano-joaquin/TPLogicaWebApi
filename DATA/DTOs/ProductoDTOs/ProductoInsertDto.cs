using System.ComponentModel.DataAnnotations;

namespace TPLogicaWebApi.DATA.DTOs.ProductoDTOs
{
    public class ProductoInsertDto
    {
        [Required(ErrorMessage = "El nombre comercial es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        public string? NombreComercial { get; set; }

        [Required(ErrorMessage = "El principio activo es obligatorio.")]
        [StringLength(50, ErrorMessage = "El principio activo no puede tener más de 50 caracteres.")]
        public string? PrincipioActivo { get; set; }

        [Required(ErrorMessage = "El Contenido es obligatorio.")]
        [Range(0.01, 99999, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal ContenidoCantidad { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatorio.")]
        [StringLength(10, ErrorMessage = "La unidad de medida no puede tener más de 10 caracteres.")]
        public string? UnidadMedida { get; set; }

        [Required(ErrorMessage = "El Nro de lote es obligatorio.")]
        [Range(1, 99999999, ErrorMessage = "El número de lote debe ser mayor a 0")]
        public int NroLote { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        public DateOnly FVencimiento { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, 99999, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "El proveedor es obligatorio.")]
        [StringLength(100, ErrorMessage = "El proveedor no puede tener más de 100 caracteres.")]
        public string? Proveedor { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal Precio { get; set; }
    }
}
