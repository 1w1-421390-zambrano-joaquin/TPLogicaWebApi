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
                                NombreComercial = d.IdProductoNavigation.NombreComercial
                            }
                        })
                        .ToList()
                })
                .ToListAsync();
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
    }
}
