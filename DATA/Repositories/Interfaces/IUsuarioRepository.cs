using TPLogicaWebApi.DATA.Models;

namespace TPLogicaWebApi.DATA.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {

        Task<Usuario?> GetByEmailRoles(string email);

        
        Task<bool> EmailExists(string email);
    }
}
