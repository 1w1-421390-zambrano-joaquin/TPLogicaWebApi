using TPLogicaWebApi.DATA.DTOs.ClientesDTOs;
using TPLogicaWebApi.DATA.Models;
using TPLogicaWebApi.DATA.Repositories.Interfaces;
using TPLogicaWebApi.DATA.Services.Interfaces;

namespace TPLogicaWebApi.DATA.Services.Implementations
{
    public class ClienteService : IClienteService
    {
        private IClienteRepository _repo;
        public ClienteService(IClienteRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Cargar(ClienteInsertDto cliente)
        {
            var entity = new Cliente
            {
                Dni = cliente.Dni,
                NomCliente = cliente.NomCliente,
                ApeCliente = cliente.ApeCliente,
                Domicilio=cliente.Domicilio,
                Teléfono = cliente.Telefono,
                Estado = true
            };
            return await _repo.Insert(entity);
        }

        public async Task<bool> Modificar(int id,ClienteUpdateDto cliente)
        {
            var entity = await TraerId(id);
            entity = new Cliente
            {
                IdCliente = id,
                Dni = cliente.Dni,
                NomCliente = cliente.NomCliente,
                ApeCliente = cliente.ApeCliente,
                Teléfono = cliente.Telefono,
                Domicilio=cliente.Domicilio,
                Estado = cliente.Estado
            };
            return await _repo.Update(entity);
        }

        public async Task<List<Cliente>> TraerActivo()
        {
            var todosClientes = await _repo.GetAll();
            return todosClientes.Where(x=>x.Estado==true).ToList();
        }

        public async Task<Cliente?> TraerDni(int dni)
        {
            var entity = await _repo.GetByDni(dni);
            if(entity==null)
                throw new ArgumentException($"El DNI {dni} del cliente no se encontro");
            return entity;
        }

        public async Task<Cliente?> TraerId(int id)
        {
            var entity = await _repo.GetById(id);
            if (entity==null)
                throw new ArgumentException($"El {id} del cliente no se encontro");
            return entity;
        }

        public async Task<List<Cliente>> TraerNombre(string nombre)
        {
            return await _repo.GetByName(nombre);
        }

        public async Task<List<Cliente>> TraerTodo()
        {
            return await _repo.GetAll();
        }
    }
}
