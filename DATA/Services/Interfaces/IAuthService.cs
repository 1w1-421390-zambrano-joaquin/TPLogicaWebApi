using TPLogicaWebApi.DATA.DTOs.UsuariosDTOs;
using TPLogicaWebApi.DATA.Models;

namespace TPLogicaWebApi.DATA.Services.Interfaces
{
    public interface IAuthService 
    {
        Task<Usuario?> Logear(LoginRequestDto dto);
        Task<Usuario> Crear(RegisterRequestDto dto);
    }
}
