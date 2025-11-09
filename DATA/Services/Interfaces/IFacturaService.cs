using TPLogicaWebApi.DATA.DTOs.FacturasDTOs;
using TPLogicaWebApi.DATA.Models;

namespace TPLogicaWebApi.DATA.Services.Interfaces
{
    public interface IFacturaService
    { 
        Task<FacturaGetDto> TraerFactura(int nroFactura);
        //Task<bool> CrearFactura(FacturaInsertDto factura);
        Task<List<FacturaGetDto>> TraerAllFacturas();
        Task<Factura> TraerUltimaFactura();
        Task<FacturaCreadaDto> CrearFactura(FacturaInsertDto factura);
        Task<FacturaPdfDto?> ObtenerFacturaPdf(int idFactura);

    }
}
