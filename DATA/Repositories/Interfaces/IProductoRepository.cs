using TPLogicaWebApi.DATA.Models;

namespace TPLogicaWebApi.DATA.Repositories.Interfaces
{
    public interface IProductoRepository
    {
        Task<List<Producto>> GetAll();
        Task<Producto?> GetById(int id);
        Task<bool> Insert(Producto producto);
        Task<bool> Update(Producto producto);
        Task<List<Producto>> GetByName(string nombre);
    }
}
