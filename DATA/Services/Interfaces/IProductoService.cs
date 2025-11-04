using TPLogicaWebApi.DATA.DTOs.ProductoDTOs;
using TPLogicaWebApi.DATA.Models;

namespace TPLogicaWebApi.DATA.Services.Interfaces
{
    public interface IProductoService
    {
        Task<List<Producto>> TraerTodo();
        Task<Producto?> TraerID(int id);
        Task<bool> Cargar(ProductoInsertDto producto);
        Task<bool> Modificar(int id,ProductoUpdateDto producto);
        Task<List<Producto>> TraerNombre(string nombre);
    }
}
