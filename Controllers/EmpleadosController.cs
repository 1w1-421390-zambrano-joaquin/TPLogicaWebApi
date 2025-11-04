using Microsoft.AspNetCore.Mvc;
using TPLogicaWebApi.DATA.DTOs.EmpleadosDTOs;
using TPLogicaWebApi.DATA.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TPLogicaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private IEmpleadoService _service;
        public EmpleadosController(IEmpleadoService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok( await _service.TraerTodo());
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("Estado")]
        public async Task<IActionResult> GetEstado()
        {
            try
            {
                return Ok(await _service.TraerEstado());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpGet("{dni:int}")]
        public async Task<IActionResult> GetByDni(int dni)
        {
            try
            {
                if(dni<=0)
                    return BadRequest("El DNI debe ser un numero positivo mayor a cero.");
                return Ok(await _service.TraerEstado());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("{nombre}")]
        public async Task<IActionResult> GetNombre(string nombre)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(nombre))
                    return BadRequest("El nombre no puede estar vacio.");
                return Ok(await _service.TraerNombre(nombre));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // POST api/<EmpleadosController>
        [HttpPost]
        public async Task<IActionResult> PostEmpleado([FromBody] EmpleadoInsertDto empleado)
        {
            try
            {
                return Ok(await _service.Crear(empleado));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/<EmpleadosController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpleadp(int id, [FromBody] EmpleadoUpdateDto empleado)
        {
            try
            {
                return Ok(await _service.Modificar(id,empleado));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

      
    }
}
