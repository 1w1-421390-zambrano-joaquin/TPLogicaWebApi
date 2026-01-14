namespace TPLogicaWebApi.DATA.DTOs.FacturasDTOs
{
    public class FacturaPdfDto
    {
        public int IdFactura { get; set; }
        public string TipoFactura { get; set; } = default!;
        public DateTime Fecha { get; set; }
        public string ClienteNombre { get; set; } = default!;
        public string ClienteApellido { get; set; } = default!;
        public string EmpleadoNombre { get; set; } = default!;
        public string EmpleadoApellido { get; set; } = default!;
        public List<FacturaPdfDetalleDto> Detalles { get; set; } = new();
    }
}
