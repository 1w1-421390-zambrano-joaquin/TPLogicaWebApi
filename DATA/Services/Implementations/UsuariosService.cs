using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using TPLogicaWebApi.DATA.DTOs.UsuariosDTOs;
using TPLogicaWebApi.DATA.Models;
using TPLogicaWebApi.DATA.Repositories.Interfaces;
using TPLogicaWebApi.DATA.Services.Interfaces;
using BCrypt.Net;

namespace TPLogicaWebApi.DATA.Services.Implementations
{
    public class UsuariosService : IAuthService
    {
        private IUsuarioRepository _usuarioRepo;
        private IEmpleadoRepository _empleadoRepo;
        private FarmaciaTPLogica1Context _context;
        public UsuariosService(IUsuarioRepository usuarioRepo, IEmpleadoRepository empleadoRepo, FarmaciaTPLogica1Context context)
        {
            _usuarioRepo = usuarioRepo;
            _empleadoRepo = empleadoRepo;
            _context = context;
        }
        public async Task<Usuario> Crear(RegisterRequestDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await _usuarioRepo.EmailExists(dto.Email))
                    throw new InvalidOperationException("El email ya está registrado.");
                var empleado = await _empleadoRepo.GetById(dto.IdEmpleado);
                if (empleado == null)
                {
                    throw new KeyNotFoundException("El empleado no existe.");
                }
                var nuevaCredencial = new Credenciale
                {
                    IdCredencial = Guid.NewGuid(),
                    Email = dto.Email,
                    Password = dto.Password
                };
                var nuevoUsuario = new Usuario
                {
                    IdUsuario = Guid.NewGuid(),
                    IdEmpleado = dto.IdEmpleado,
                    IdCredencialNavigation = nuevaCredencial
                };
                _context.Usuarios.Add(nuevoUsuario);
                var nuevaAutenticacion = new Autenticacion
                {
                    
                    IdRol = dto.IdRol,
                    
                    IdUsuarioNavigation = nuevoUsuario
                };
                _context.Autenticacions.Add(nuevaAutenticacion);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return nuevoUsuario;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task<Usuario?> Logear(LoginRequestDto dto)
        {
            var usuario = await _usuarioRepo.GetByEmailRoles(dto.Email);
            if (usuario == null)
               throw new KeyNotFoundException("Usuario o contraseña incorrectos.");
            if(usuario.IdCredencialNavigation.Password != dto.Password)
               throw new KeyNotFoundException("Usuario o contraseña incorrectos.");
            return usuario;
        }
    }
}
