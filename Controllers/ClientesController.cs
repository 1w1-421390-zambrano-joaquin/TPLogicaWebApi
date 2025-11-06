using Microsoft.AspNetCore.Mvc;
using TPLogicaWebApi.DATA.DTOs.ClientesDTOs;
using TPLogicaWebApi.DATA.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TPLogicaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private IClienteService _service;
        public ClientesController(IClienteService service)
        {
            _service = service;
        }
       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _service.TraerTodo());
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("/buscar/{nombre}")]
        public async Task<IActionResult> GetNombre(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return BadRequest("El nombre no puede estar vacio.");
                return Ok(await _service.TraerNombre(nombre));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("Estado")]
        public async Task<IActionResult> GetActivo()
        {
            try
            {
                return Ok(await _service.TraerActivo());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<ClientesController>/5
        [HttpGet("{dni:int}")]
        public async Task<IActionResult> GetDNI(int dni)
        {
            try
            {
                if (dni <= 0)
                {
                    return BadRequest("El DNI debe ser un numero positivo mayor a cero.");
                }
                return Ok(await _service.TraerDni(dni));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<ClientesController>
        [HttpPost]
        public async Task<IActionResult> PostCliente([FromBody] ClienteInsertDto cliente)
        {
            try
            {
                return Ok(await _service.Cargar(cliente));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutCliente(int id,[FromBody] ClienteUpdateDto cliente)
        {
            try
            {
                return Ok(await _service.Modificar(id,cliente));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, ex.Message);
            }
        }


    }
}
