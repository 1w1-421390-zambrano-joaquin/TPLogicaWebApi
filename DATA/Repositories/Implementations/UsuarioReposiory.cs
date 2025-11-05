using Microsoft.EntityFrameworkCore;
using TPLogicaWebApi.DATA.Models;
using TPLogicaWebApi.DATA.Repositories.Interfaces;

namespace TPLogicaWebApi.DATA.Repositories.Implementations
{
    public class UsuarioReposiory : IUsuarioRepository
    {
        private FarmaciaTPLogica1Context _context;
        public UsuarioReposiory(FarmaciaTPLogica1Context context)
        {
            _context = context;
        }

        public async Task<bool> EmailExists(string email)
        {
            return await _context.Credenciales.AnyAsync(c => c.Email == email);
        }

        public async Task<Usuario?> GetByEmailRoles(string email)
        {
            return await _context.Usuarios.Include(u => u.IdCredencialNavigation)
                .Include(u => u.Autenticacions)
                    .ThenInclude(ur => ur.IdRolNavigation)
                    .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdCredencialNavigation.Email == email);
        }
    }
}
