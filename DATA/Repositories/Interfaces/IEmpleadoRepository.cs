using TPLogicaWebApi.DATA.Models;

namespace TPLogicaWebApi.DATA.Repositories.Interfaces
{
    public interface IEmpleadoRepository
    {
        Task<List<Empleado>> GetAll();
        Task<Empleado?> GetById(int id);
        Task<Empleado?> GetByDni(int dni);
        Task<bool> Insert(Empleado empleado);
        Task<bool> Update(Empleado empleado);
    }
}
