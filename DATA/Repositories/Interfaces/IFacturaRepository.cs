using TPLogicaWebApi.DATA.DTOs.FacturasDTOs;
using TPLogicaWebApi.DATA.Models;

namespace TPLogicaWebApi.DATA.Repositories.Interfaces
{
    public interface IFacturaRepository
    {
        Task<FacturaGetDto?> GetFactura(int nroFactura);
        Task Insert(Factura factura);
        Task<List<FacturaGetDto?>> GetAllFacturas();
        Task<Factura> GetUltimaFactura();
    }
}
