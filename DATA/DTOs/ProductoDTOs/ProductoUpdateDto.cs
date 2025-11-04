using System.ComponentModel.DataAnnotations;

namespace TPLogicaWebApi.DATA.DTOs.ProductoDTOs
{
    public class ProductoUpdateDto
    {
        public string NombreComercial { get; set; }
        public string PrincipioActivo { get; set; }
        public decimal ContenidoCantidad { get; set; }
        public string UnidadMedida { get; set; }
        public int NroLote { get; set; }
        public DateOnly FVencimiento { get; set; }
        public int Stock { get; set; }
        public string Proveedor { get; set; }
    }
}
