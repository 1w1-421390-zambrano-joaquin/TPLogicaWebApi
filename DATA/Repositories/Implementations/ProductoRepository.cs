using Microsoft.EntityFrameworkCore;
using TPLogicaWebApi.DATA.Models;
using TPLogicaWebApi.DATA.Repositories.Interfaces;

namespace TPLogicaWebApi.DATA.Repositories.Implementations
{
    public class ProductoRepository : IProductoRepository
    {
        private FarmaciaTPLogica1Context _context;
        public ProductoRepository(FarmaciaTPLogica1Context context)
        {
            _context = context;
        }

        public async Task<bool> Delete(Producto producto)
        {
            _context.Productos.ExecuteDelete(producto);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Producto>> GetAll()
        {
            return await _context.Productos.AsNoTracking().ToListAsync();
        }

        public async Task<Producto?> GetById(int id)
        {
            return await _context.Productos.AsNoTracking().FirstOrDefaultAsync(x=>x.IdProducto==id);
        }

        public async Task<Producto?> GetById2(int id)
        {
            return await _context.Productos.FindAsync(id);
        }

        public async Task<List<Producto>> GetByName(string nombre)
        {
            return await _context.Productos.AsNoTracking().Where(x=>x.NombreComercial.Contains(nombre) || x.PrincipioActivo.Contains(nombre)).ToListAsync();
        }

        public async Task<bool> Insert(Producto producto)
        {
            _context.Productos.Add(producto);
            return await _context.SaveChangesAsync()> 0;
        }

        public async Task<bool> Update(Producto producto)
        {
            _context.Productos.Update(producto);
            return await _context.SaveChangesAsync()> 0;
        }
    }
}
