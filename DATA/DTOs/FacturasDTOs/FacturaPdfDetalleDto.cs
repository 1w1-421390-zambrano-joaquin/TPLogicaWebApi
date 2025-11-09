namespace TPLogicaWebApi.DATA.DTOs.FacturasDTOs
{
    public class FacturaPdfDetalleDto
    {
        public string ProductoNombre { get; set; } = default!;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
