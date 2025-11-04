namespace TPLogicaWebApi.DATA.DTOs.FacturasDTOs
{
    public class DetalleFacturaGetDto
    {
        public ProductoGetDto Producto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => PrecioUnitario * Cantidad;
    }
}
