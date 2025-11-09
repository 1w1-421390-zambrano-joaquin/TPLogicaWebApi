using Microsoft.EntityFrameworkCore;
using TPLogicaWebApi.DATA.DTOs.FacturasDTOs;
using TPLogicaWebApi.DATA.Models;
using TPLogicaWebApi.DATA.Repositories.Interfaces;

namespace TPLogicaWebApi.DATA.Repositories.Implementations
{
    public class FacturarRepository : IFacturaRepository
    {
        private FarmaciaTPLogica1Context _context;
        public FacturarRepository(FarmaciaTPLogica1Context context)
        {
            _context = context;
        }

        public async Task<List<FacturaGetDto>> GetAllFacturas()
        {
            return await _context.Facturas
                .AsNoTracking()
                .Select(f => new FacturaGetDto
                {
                    NroFactura = f.NroFactura,
                    TipoFactura = f.TipoFactura,
                    FechaFactura = f.FechaFactura,

                    Cliente = new ClienteGetDto
                    {
                        IdCliente = f.IdClienteNavigation.IdCliente,
                        NombreCompleto = f.IdClienteNavigation.NomCliente + " " + f.IdClienteNavigation.ApeCliente
                    },

                    Empleado = new EmpleadoGetDto
                    {
                        IdEmpleado = f.IdEmpleadoNavigation.IdEmpleado,
                        NombreCompleto = f.IdEmpleadoNavigation.NomEmp + " " + f.IdEmpleadoNavigation.ApeEmp
                    },

                    Detalles = f.DetalleFacturas
                        .Select(d => new DetalleFacturaGetDto
                        {
                            PrecioUnitario = d.PrecioUnitario,
                            Cantidad = d.Cantidad,
                            Producto = new ProductoGetDto
                            {
                                IdProducto = d.IdProductoNavigation.IdProducto,
                                NombreComercial = d.IdProductoNavigation.NombreComercial,
                                Precio= d.IdProductoNavigation.PrecioUnitario

                            }
                        })
                        .ToList()
                })
                .ToListAsync();
        }
        public async Task<Factura> GetUltimaFactura()
        {
            return await _context.Facturas.OrderByDescending(x => x.NroFactura).FirstOrDefaultAsync();
        }
        public async Task<FacturaGetDto?> GetFactura(int nroFactura)
        {
            return await _context.Facturas.AsNoTracking()
            .Where(f => f.NroFactura == nroFactura) 
            .Select(f => new FacturaGetDto 
            {
                NroFactura = f.NroFactura,
                TipoFactura = f.TipoFactura,
                FechaFactura = f.FechaFactura,


                Cliente = new ClienteGetDto
                {
                    IdCliente = f.IdClienteNavigation.IdCliente,
                    NombreCompleto = f.IdClienteNavigation.NomCliente + " " + f.IdClienteNavigation.ApeCliente
                },
                Empleado = new EmpleadoGetDto
                {
                    IdEmpleado = f.IdEmpleadoNavigation.IdEmpleado,
                    NombreCompleto = f.IdEmpleadoNavigation.NomEmp + " " + f.IdEmpleadoNavigation.ApeEmp
                },


                Detalles = f.DetalleFacturas.Select(d => new DetalleFacturaGetDto
                {
                    PrecioUnitario = d.PrecioUnitario,
                    Cantidad = d.Cantidad,
                    Producto = _context.Productos
                                .Where(p => p.IdProducto == d.IdProducto) 
                                .Select(p => new ProductoGetDto
                                {
                                    IdProducto = p.IdProducto,
                                    NombreComercial = p.NombreComercial
                                })
                                .FirstOrDefault()
                }).ToList()
            })
            .FirstOrDefaultAsync();
        }

        public async Task Insert(Factura factura)
        {
            
           await _context.Facturas.AddAsync(factura);
        }

        public async Task<FacturaPdfDto?> GetFacturaPdf(int idFactura)
        {
            {
                var factura = await _context.Facturas
                    .Include(f => f.IdClienteNavigation)
                    .Include(f => f.IdEmpleadoNavigation)
                    .Include(f => f.DetalleFacturas)
                        .ThenInclude(d => d.IdProductoNavigation)
                    .FirstOrDefaultAsync(f => f.NroFactura == idFactura);

                if (factura == null)
                    return null;

                return new FacturaPdfDto
                {
                    IdFactura = factura.NroFactura,
                    TipoFactura = factura.TipoFactura,
                    Fecha = factura.FechaFactura.ToDateTime(new TimeOnly(0, 0)),
                    ClienteNombre = factura.IdClienteNavigation?.NomCliente ?? "N/D",
                    EmpleadoNombre = factura.IdEmpleadoNavigation?.NomEmp ?? "N/D",
                    Detalles = factura.DetalleFacturas.Select(d => new FacturaPdfDetalleDto
                    {
                        ProductoNombre = d.IdProductoNavigation?.NombreComercial ?? $"Prod {d.IdProducto}",
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    }).ToList()
                };
            }
        }
    }
}
