using Microsoft.EntityFrameworkCore;
using System.Net;
using TPLogicaWebApi.DATA.Models;
using TPLogicaWebApi.DATA.Repositories.Interfaces;

namespace TPLogicaWebApi.DATA.Repositories.Implementations
{
    public class ClienteRepository : IClienteRepository
    {
        private FarmaciaTPLogica1Context _context;
        public ClienteRepository(FarmaciaTPLogica1Context context)
        {
            _context = context;
        }
        public async Task<List<Cliente>> GetAll()
        {
            return await _context.Clientes.AsNoTracking().ToListAsync();
        }

        public async Task<Cliente?> GetByDni(int dni)
        {
            return await _context.Clientes.AsNoTracking().Where(x=>x.Dni==dni).FirstOrDefaultAsync();
        }

        public async Task<Cliente?> GetById(int id)
        {
            return await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(x=>x.IdCliente==id);
        }

        public async Task<List<Cliente>> GetByName(string nombre)
        {
            return await _context.Clientes.Where(x => x.NomCliente.Contains(nombre)).ToListAsync();
        }

        public async Task<bool> Insert(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
