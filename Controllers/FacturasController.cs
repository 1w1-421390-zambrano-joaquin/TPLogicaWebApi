using Microsoft.AspNetCore.Authorization;
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
        [HttpGet("all")]
        public async Task<IActionResult> GetAllFactura()
        {
            try
            {
                return Ok(await _service.TraerAllFacturas());
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
        [HttpGet("ultima")]
        public async Task<IActionResult> GetUltimaFactura()
        {
            try
            {
                return Ok(await _service.TraerUltimaFactura());
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
        [Authorize(Roles ="admin,vendedor")]
        public async Task<IActionResult> CreateFactura([FromBody] FacturaInsertDto dto)
        {
            
            try
            {
               return Ok( await _service.CrearFactura(dto)); 
            }
            catch (KeyNotFoundException ex) 
            {
                return BadRequest(ex.Message); 
            }
            catch (InvalidOperationException ex) 
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, error);
            }
        }


    }
}
