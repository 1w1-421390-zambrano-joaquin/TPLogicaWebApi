using TPLogicaWebApi.DATA.DTOs.FacturasDTOs;

namespace TPLogicaWebApi.DATA.Services.Interfaces
{
    public interface IFacturaService
    { 
        Task<FacturaGetDto> TraerFactura(int nroFactura);
        Task<bool> CrearFactura(FacturaInsertDto factura);
    }
}
