using System.ComponentModel.DataAnnotations;

namespace TPLogicaWebApi.DATA.DTOs.ProductoDTOs
{
    public class ProductoUpdateDto
    {

        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        public string? NombreComercial { get; set; }

        [StringLength(50, ErrorMessage ="El principio activo no puede tener más de 50 caracteres.")]
        public string? PrincipioActivo { get; set; }

        [Range(0.01, 500, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal ContenidoCantidad { get; set; }

        [StringLength(10, ErrorMessage = "La unidad de medida no puede tener más de 10 caracteres.")]
        public string? UnidadMedida { get; set; }

        [Range(1, 500, ErrorMessage = "El número de lote debe ser mayor a 0")]
        public int NroLote { get; set; }

        public DateOnly FVencimiento { get; set; }

        [Range(0, 500, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [StringLength(100, ErrorMessage = "El proveedor no puede tener más de 100 caracteres.")]
        public string? Proveedor { get; set; }
    }
}
