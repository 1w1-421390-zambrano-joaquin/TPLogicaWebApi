using TPLogicaWebApi.DATA.Models;

namespace TPLogicaWebApi.DATA.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {

        Task<Usuario?> GetByEmailRoles(string email);

        // Un validador rápido para el REGISTRO
        Task<bool> EmailExists(string email);
    }
}
