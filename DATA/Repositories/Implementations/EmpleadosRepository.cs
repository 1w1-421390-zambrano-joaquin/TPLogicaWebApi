using Microsoft.EntityFrameworkCore;
using TPLogicaWebApi.DATA.Models;
using TPLogicaWebApi.DATA.Repositories.Interfaces;

namespace TPLogicaWebApi.DATA.Repositories.Implementations
{
    public class EmpleadosRepository : IEmpleadoRepository
    {
        private FarmaciaTPLogica1Context _context;
        public EmpleadosRepository(FarmaciaTPLogica1Context context)
        {
            _context = context;
        }
        public async Task<List<Empleado>> GetAll()
        {
            return await _context.Empleados.AsNoTracking().ToListAsync();
        }

        public async Task<Empleado?> GetByDni(int dni)
        {
            return await _context.Empleados.AsNoTracking().FirstOrDefaultAsync(x=>x.Dni==dni);
        }

        public async Task<Empleado?> GetById(int id)
        {
            return await _context.Empleados.AsNoTracking().FirstOrDefaultAsync(x=>x.IdEmpleado==id);
        }

        public async Task<Empleado?> GetByNombre(string nombre)
        {
            return await _context.Empleados.AsNoTracking().FirstOrDefaultAsync(x => x.NomEmp == nombre);
        }

        public async Task<List<Empleado>> GetEstado()
        {
            return await _context.Empleados.Where(x=>x.Estado==true).ToListAsync();
        }

        public async Task<bool> Insert(Empleado empleado)
        {
            _context.Empleados.Add(empleado);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(Empleado empleado)
        {
            _context.Empleados.Update(empleado);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
