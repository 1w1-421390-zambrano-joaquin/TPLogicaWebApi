using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TPLogicaWebApi.DATA.DTOs.FacturasDTOs;
using TPLogicaWebApi.DATA.Services.Implementations;
using TPLogicaWebApi.DATA.Services.Interfaces;
using TPLogicaWebApi.Utils;

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
        //[Authorize(Roles = "admin")]
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
        //[Authorize(Roles = "admin,vendedor")]
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
        public async Task<IActionResult> CrearFactura([FromBody] FacturaInsertDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var facturaCreada = await _service.CrearFactura(dto);

            return Ok(facturaCreada);
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetFacturaPdf(int id)
        {
            var factura = await _service.ObtenerFacturaPdf(id);
            if (factura == null)
                return NotFound("Factura no encontrada");

            var pdfBytes = FacturaPdfGenerator.Generar(factura);

            // ACA FLACO!!
            return File(pdfBytes,
                "application/pdf",
                $"Factura_{factura.IdFactura}.pdf");
        }
        //[HttpPost]
        ////[Authorize(Roles ="admin,vendedor")]
        //public async Task<IActionResult> CreateFactura([FromBody] FacturaInsertDto dto)
        //{

        //    try
        //    {
        //       return Ok( await _service.CrearFactura(dto)); 
        //    }
        //    catch (KeyNotFoundException ex) 
        //    {
        //        return BadRequest(ex.Message); 
        //    }
        //    catch (InvalidOperationException ex) 
        //    {
        //        return BadRequest(ex.Message); 
        //    }
        //    catch (Exception ex)
        //    {
        //        var error = ex.InnerException?.Message ?? ex.Message;
        //        return StatusCode(500, error);
        //    }
        //}


    }
}
