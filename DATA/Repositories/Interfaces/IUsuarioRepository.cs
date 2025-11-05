using TPLogicaWebApi.DATA.Models;

namespace TPLogicaWebApi.DATA.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        // Busca un usuario por su email e incluye sus roles.
        Task<Usuario?> GetByEmailWithRolesAsync(string email);

        // Un validador rápido para el REGISTRO
        Task<bool> EmailExistsAsync(string email);
    }
}
