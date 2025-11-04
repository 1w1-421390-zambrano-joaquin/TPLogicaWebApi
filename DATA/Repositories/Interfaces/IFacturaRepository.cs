using TPLogicaWebApi.DATA.Models;

namespace TPLogicaWebApi.DATA.Repositories.Interfaces
{
    public interface IFacturaRepository
    {
        Task<Factura?> GetFactura(int nroFactura);
        Task<bool> Insert(Factura factura);
    }
}
