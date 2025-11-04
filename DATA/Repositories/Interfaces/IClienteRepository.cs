using TPLogicaWebApi.DATA.Models;

namespace TPLogicaWebApi.DATA.Repositories.Interfaces
{
    public interface IClienteRepository
    {
        Task<List<Cliente>> GetAll();
        Task<Cliente?> GetById(int id);
        Task<List<Cliente>> GetByName(string nombre);
        Task<bool> Insert(Cliente cliente);
        Task<bool> Update(Cliente cliente);
        Task<Cliente?> GetByDni(int dni);
    }
}
