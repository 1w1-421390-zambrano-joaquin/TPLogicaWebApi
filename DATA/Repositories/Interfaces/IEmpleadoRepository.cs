using TPLogicaWebApi.DATA.Models;

namespace TPLogicaWebApi.DATA.Repositories.Interfaces
{
    public interface IEmpleadoRepository
    {
        Task<List<Empleado>> GetAll();
        Task<List<Empleado>> GetEstado();
        Task<Empleado?> GetById(int id);
        Task<Empleado?> GetByNombre(string nombre);
        Task<Empleado?> GetByDni(int dni);
        Task<bool> Insert(Empleado empleado);
        Task<bool> Update(Empleado empleado);

    }
}
