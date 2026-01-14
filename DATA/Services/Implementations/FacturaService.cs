using TPLogicaWebApi.DATA.DTOs.FacturasDTOs;
using TPLogicaWebApi.DATA.Models;
using TPLogicaWebApi.DATA.Repositories.Interfaces;
using TPLogicaWebApi.DATA.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TPLogicaWebApi.DATA.Services.Implementations
{
    public class FacturaService : IFacturaService
    {
        
        private IFacturaRepository _facturaRepo;
        private IProductoRepository _productoRepo;
        private IClienteRepository _clienteRepo;
        private IEmpleadoRepository _empleadoRepo;
        private FarmaciaTPLogica1Context _context;

        public FacturaService(
            IFacturaRepository facturaRepo,
            IProductoRepository productoRepo,
            IClienteRepository clienteRepo,
            IEmpleadoRepository empleadoRepo,
            FarmaciaTPLogica1Context context)
        {
            _facturaRepo = facturaRepo;
            _productoRepo = productoRepo;
            _clienteRepo = clienteRepo;
            _empleadoRepo = empleadoRepo;
            _context = context;
        }
        public async Task<FacturaCreadaDto> CrearFactura(FacturaInsertDto factura)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var cliente = await _clienteRepo.GetById(factura.IdCliente);
                var empleado = await _empleadoRepo.GetById(factura.IdEmpleado);

                if (cliente == null || !cliente.Estado)
                    throw new KeyNotFoundException("Cliente no encontrado o inactivo.");

                if (empleado == null || !empleado.Estado)
                    throw new KeyNotFoundException("Empleado no encontrado o inactivo.");

                var nuevaFactura = new Factura
                {
                    TipoFactura = factura.TipoFactura!,
                    IdEmpleado = factura.IdEmpleado,
                    IdCliente = factura.IdCliente,
                    FechaFactura = DateOnly.FromDateTime(DateTime.Now),
                    DetalleFacturas = new List<DetalleFactura>()
                };

                foreach (var detalleDto in factura.Detalles)
                {
                    var producto = await _productoRepo.GetById2(detalleDto.IdProducto);

                    if (producto == null)
                        throw new KeyNotFoundException($"Producto ID {detalleDto.IdProducto} no existe.");

                    if (producto.Stock < detalleDto.Cantidad)
                        throw new InvalidOperationException($"Stock insuficiente para {producto.NombreComercial}.");

                    producto.Stock -= detalleDto.Cantidad;
                    var precioDeLaDB = producto.PrecioUnitario;

                    nuevaFactura.DetalleFacturas.Add(new DetalleFactura
                    {
                        IdProducto = detalleDto.IdProducto,
                        PrecioUnitario = precioDeLaDB,
                        Cantidad = detalleDto.Cantidad,
                        Observ = detalleDto.Observ
                    });
                }

                await _facturaRepo.Insert(nuevaFactura);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new FacturaCreadaDto
                {
                    IdFactura = nuevaFactura.NroFactura,
                    TipoFactura = nuevaFactura.TipoFactura
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<FacturaPdfDto?> ObtenerFacturaPdf(int idFactura)
        {
            return await _facturaRepo.GetFacturaPdf(idFactura);
        }

        public async Task<List<FacturaGetDto>> TraerAllFacturas()
        {
            return await _facturaRepo.GetAllFacturas();
        }

        public async Task<FacturaGetDto> TraerFactura(int nroFactura)
        {

            var facturaDto = await _facturaRepo.GetFactura(nroFactura);


            if (facturaDto == null)
            {
                throw new KeyNotFoundException($"Factura {nroFactura} no encontrada.");
            }


            return facturaDto;
        }

        public async Task<Factura> TraerUltimaFactura()
        {
            return await _facturaRepo.GetUltimaFactura();
        }
    }
}