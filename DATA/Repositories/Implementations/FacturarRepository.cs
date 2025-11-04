using Microsoft.EntityFrameworkCore;
using TPLogicaWebApi.DATA.Models;
using TPLogicaWebApi.DATA.Repositories.Interfaces;

namespace TPLogicaWebApi.DATA.Repositories.Implementations
{
    public class FacturarRepository : IFacturaRepository
    {
        private FarmaciaTPLogica1Context _context;
        public FacturarRepository(FarmaciaTPLogica1Context context)
        {
            _context = context;
        }

        public async Task<Factura?> GetFactura(int nroFactura)
        {
            return await _context.Facturas.Include(f=>f.IdClienteNavigation).
                                Include(f => f.IdEmpleadoNavigation).
                                Include(f => f.DetalleFacturas).
                                ThenInclude(d => d.IdProducto).
                                AsNoTracking().FirstOrDefaultAsync(f => f.NroFactura == nroFactura);
        }

        public async Task<bool> Insert(Factura factura)
        {
            _context.Facturas.Add(factura);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
