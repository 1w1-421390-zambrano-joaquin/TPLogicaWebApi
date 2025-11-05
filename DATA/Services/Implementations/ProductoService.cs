using TPLogicaWebApi.DATA.DTOs.ProductoDTOs;
using TPLogicaWebApi.DATA.Models;
using TPLogicaWebApi.DATA.Repositories.Interfaces;
using TPLogicaWebApi.DATA.Services.Interfaces;

namespace TPLogicaWebApi.DATA.Services.Implementations
{
    public class ProductoService : IProductoService
    {
        private IProductoRepository _repo;
        public ProductoService(IProductoRepository repo)
        {
            _repo = repo;
        }
        public async Task<bool> Cargar(ProductoInsertDto producto)
        {
            var entity = new Producto
            {
                NombreComercial = producto.NombreComercial,
                PrincipioActivo = producto.PrincipioActivo,
                ContenidoCantidad = producto.ContenidoCantidad,
                UnidadMedida = producto.UnidadMedida,
                NroLote = producto.NroLote,
                FVencimiento = producto.FVencimiento,
                Stock = producto.Stock,
                Proveedor = producto.Proveedor,
                PrecioUnitario = producto.Precio
            };
            DateOnly hoy = DateOnly.FromDateTime(DateTime.Now);
            if (entity.FVencimiento < hoy)
            {
                throw new InvalidOperationException("No se puede cargar un producto vencido.");
            }
            return await _repo.Insert(entity);
        }

        public async Task<bool> Modificar(int id, ProductoUpdateDto producto)
        {
            var entity = await TraerID(id);
            entity = new Producto
            {
                IdProducto = id, 
                NombreComercial = producto.NombreComercial,
                PrincipioActivo = producto.PrincipioActivo,
                ContenidoCantidad = producto.ContenidoCantidad,
                UnidadMedida = producto.UnidadMedida,
                NroLote = producto.NroLote,
                FVencimiento = producto.FVencimiento,
                Stock = producto.Stock,
                Proveedor = producto.Proveedor
            };
            DateOnly hoy = DateOnly.FromDateTime(DateTime.Now);
            if (entity.FVencimiento < hoy)
            {
                throw new InvalidOperationException("No se puede modificar un producto vencido.");
            }
            return await  _repo.Update(entity);
        }

        public async Task<List<Producto>> TraerNombre(string nombre)
        {
            return await _repo.GetByName(nombre);
        }

        public Task<Producto?> TraerID(int id)
        {
            var entity = _repo.GetById(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"El producto con el ID {id} no se encontro");
            }
            return entity;
        }

        public async Task<List<Producto>> TraerTodo()
        {
           return await _repo.GetAll();
        }
    }
}
