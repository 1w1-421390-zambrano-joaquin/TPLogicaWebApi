using TPLogicaWebApi.DATA.Models;

namespace TPLogicaWebApi.DATA.Services.Interfaces
{
    public interface IEmpleadoService
    {
        Task<List<Empleado>> TraerTodo();
        Task<List<Empleado>> TraerEstado();
        Task<Empleado?> TraerId(int id);
        Task<Empleado?> TraerDni(int dni);
        Task<bool> Crear(Empleado empleado);
        Task<bool> Modificar(Empleado empleado);
    }
}
