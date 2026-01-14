using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TPLogicaWebApi.DATA.DTOs.ProductoDTOs;
using TPLogicaWebApi.DATA.Services.Interfaces;


namespace TPLogicaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private IProductoService _service;
        public ProductosController(IProductoService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "admin,vendedor")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _service.TraerTodo());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "admin,vendedor")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest("El ID debe ser un numero positivo mayor a cero.");
                }
                return Ok(await _service.TraerID(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("buscar")]
        [Authorize(Roles = "admin,vendedor")]
        public async Task<IActionResult> GetNombre([FromQuery] string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return BadRequest("El término de búsqueda no puede estar vacío.");
                }
                return Ok(await _service.TraerNombre(nombre));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<ProductosController>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] ProductoInsertDto producto)
        {
            try
            {
                var result = await _service.Cargar(producto);
                return Ok(result);
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is SqlException sql &&
                (sql.Number == 2627 || sql.Number == 2601) // UNIQUE KEY
            )
            {
                return Conflict(new { message = "Ya existe un producto con ese nombre comercial." });
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }


        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductoUpdateDto producto)
        {
            try
            {
                if (producto == null)
                {
                    return BadRequest("Datos del producto inválidos.");
                }
              
                return Ok(await _service.Modificar(id, producto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
