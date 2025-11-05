using Microsoft.AspNetCore.Mvc;
using TPLogicaWebApi.DATA.DTOs.FacturasDTOs;
using TPLogicaWebApi.DATA.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TPLogicaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturasController : ControllerBase
    {
        private IFacturaService _service;
        public FacturasController(IFacturaService service)
        {
            _service = service;
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFactura(int id)
        {
            try
            {
                return Ok(await _service.TraerFactura(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Para cualquier otro error
                var error = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFactura([FromBody] FacturaInsertDto dto)
        {
            // (La validación de DTOs [Required] es automática gracias a [ApiController])
            try
            {
                // 3. Llama al servicio (que recibe el DTO de creación)
               return Ok( await _service.CrearFactura(dto)); 
            }
            catch (KeyNotFoundException ex) // Ej. Cliente o Producto no existe
            {
                return BadRequest(ex.Message); // 400
            }
            catch (InvalidOperationException ex) // Ej. Sin stock
            {
                return BadRequest(ex.Message); // 400
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, error);
            }
        }


    }
}
