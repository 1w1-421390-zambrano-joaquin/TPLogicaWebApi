using TPLogicaWebApi.DATA.DTOs.ClientesDTOs;
using TPLogicaWebApi.DATA.Models;

namespace TPLogicaWebApi.DATA.Services.Interfaces
{
    public interface IClienteService
    {
        Task<List<Cliente>> TraerTodo();
        Task<Cliente?> TraerId(int id);
        Task<List<Cliente>> TraerNombre(string nombre);
        Task<bool> Cargar(ClienteInsertDto cliente);
        Task<bool> Modificar(int id,ClienteUpdateDto cliente);
        Task<Cliente?> TraerDni(int dni);
        Task<List<Cliente>> TraerActivo();
    }
}
