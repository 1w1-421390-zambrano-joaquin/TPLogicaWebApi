using TPLogicaWebApi.DATA.DTOs.FacturasDTOs;
using TPLogicaWebApi.DATA.Models; // <-- AÑADE ESTA LÍNEA
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

        public async Task<bool> CrearFactura(FacturaInsertDto factura)
        {
            await using var transaction= await _context.Database.BeginTransactionAsync();
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
                    TipoFactura = factura.TipoFactura,
                    IdEmpleado = factura.IdEmpleado,
                    IdCliente = factura.IdCliente,
                    FechaFactura = DateOnly.FromDateTime(DateTime.Now)
                };

                foreach (var detalleDto in factura.Detalles)
                {
                    var producto = await _productoRepo.GetById2(detalleDto.IdProducto);

                    if (producto == null)
                        throw new KeyNotFoundException($"Producto ID {detalleDto.IdProducto} no existe.");
                    if (producto.Stock < detalleDto.Cantidad)
                        throw new InvalidOperationException($"Stock insuficiente para {producto.NombreComercial}.");
                    producto.Stock -= detalleDto.Cantidad;

                   
                    var nuevoDetalle = new DetalleFactura
                    {
                        IdProducto = detalleDto.IdProducto,
                        PrecioUnitario = detalleDto.PrecioUnitario,
                        Cantidad = detalleDto.Cantidad,
                        Observ = detalleDto.Observ
                        
                    };

                    // C.4. Agregamos el detalle a la lista del maestro
                    nuevaFactura.DetalleFacturas.Add(nuevoDetalle);
                }
                // --- D. GUARDADO ---

                // D.1. Agregamos la factura (y sus detalles) al contexto
                await _facturaRepo.Add(nuevaFactura);

                // D.3. ¡ÉXITO! Confirmamos la transacción
                await transaction.CommitAsync();

                // D.4. Devolvemos el DTO de respuesta
                return await _facturaRepo.SaveChangesAsync()>0;

            }
            catch(Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<FacturaGetDto> TraerFactura(int nroFactura)
        {
            var factura = await _facturaRepo.Get(nroFactura);
            if (factura == null)
            {
                throw new KeyNotFoundException($"Factura {nroFactura} no encontrada.");
            }

            // B. Mapeo de Entidad -> DTO (Aquí rompemos la referencia circular)
            return new FacturaGetDto
            {
                NroFactura = factura.NroFactura,
                TipoFactura = factura.TipoFactura,
                FechaFactura = factura.FechaFactura,
                Cliente = new ClienteGetDto
                {
                    IdCliente = factura.IdClienteNavigation.IdCliente,
                    NombreCompleto = $"{factura.IdClienteNavigation.NomCliente} {factura.IdClienteNavigation.ApeCliente}"
                },
                Empleado = new EmpleadoGetDto
                {
                    IdEmpleado = factura.IdEmpleadoNavigation.IdEmpleado,
                    NombreCompleto = $"{factura.IdEmpleadoNavigation.NomEmp} {factura.IdEmpleadoNavigation.ApeEmp}"
                },
                Detalles = factura.DetalleFacturas.Select(d => new DetalleFacturaGetDto
                {
                    PrecioUnitario = d.PrecioUnitario,
                    Cantidad = d.Cantidad,
                    Producto = new ProductoGetDto
                    {
                        IdProducto = d.IdProductoNavigation.IdProducto,
                        NombreComercial = d.IdProductoNavigation.NombreComercial
                    }
                }).ToList()
            };
        }
    }
}
