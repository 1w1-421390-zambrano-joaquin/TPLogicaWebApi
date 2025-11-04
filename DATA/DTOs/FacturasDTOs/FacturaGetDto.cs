namespace TPLogicaWebApi.DATA.DTOs.FacturasDTOs
{
    public class FacturaGetDto
    {
        public int NroFactura { get; set; }
        public string? TipoFactura { get; set; }
        public DateOnly FechaFactura { get; set; }

        public ClienteGetDto Cliente { get; set; }
        public EmpleadoGetDto Empleado { get; set; }

        public List<DetalleFacturaGetDto> Detalles { get; set; }

        public decimal TotalFactura => Detalles?.Sum(d => d.Subtotal) ?? 0;
    }
}
