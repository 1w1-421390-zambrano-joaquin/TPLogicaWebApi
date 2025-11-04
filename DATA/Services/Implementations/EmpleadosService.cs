using TPLogicaWebApi.DATA.DTOs.EmpleadosDTOs;
using TPLogicaWebApi.DATA.Models;
using TPLogicaWebApi.DATA.Repositories.Interfaces;
using TPLogicaWebApi.DATA.Services.Interfaces;

namespace TPLogicaWebApi.DATA.Services.Implementations
{
    public class EmpleadosService : IEmpleadoService
    {
        private IEmpleadoRepository _repo;
        public EmpleadosService(IEmpleadoRepository repo)
        {
            _repo = repo;
        }
        public Task<bool> Crear(EmpleadoInsertDto empleado)
        {
            var entity = new Empleado
            {
                NomEmp = empleado.NomEmp,
                ApeEmp=empleado.ApeEmp,
                Domicilio=empleado.Domicilio,
                Dni=empleado.Dni,
                Teléfono=empleado.Telefono,
                Estado=true
            };
            return _repo.Insert(entity);
        }

        public async Task<bool> Modificar(int id,EmpleadoUpdateDto empleado)
        {
            var entity = await TraerId(id);
            entity = new Empleado
            {
                IdEmpleado = id,
                NomEmp = empleado.NomEmp,
                ApeEmp=empleado.ApeEmp,
                Domicilio=empleado.Domicilio,
                Dni=empleado.Dni,
                Teléfono=empleado.Telefono,
                Estado=empleado.Estado
            };
            return await _repo.Update(entity);
        }

        public async Task<Empleado?> TraerDni(int dni)
        {
            var entity = await _repo.GetByDni(dni);
            if(entity==null)
                throw new ArgumentException($"El DNI {dni} del empleado no se encontro");
            return entity;
        }

        public async Task<List<Empleado>> TraerEstado()
        {
            return await _repo.GetEstado();
        }

        public async Task<Empleado?> TraerId(int id)
        {
            var entity= await _repo.GetById(id);
            if(entity==null)
                throw new ArgumentException($"El ID {id} del empleado no se encontro");
            return entity;
        }

        public async Task<Empleado?> TraerNombre(string nombre)
        {
            return await _repo.GetByNombre(nombre);
        }

        public async Task<List<Empleado>> TraerTodo()
        {
            return await _repo.GetAll();
        }
    }
}
