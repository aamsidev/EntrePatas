using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntrePatasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : ControllerBase
    {

        private readonly IPago pagoDATA;

        public PagoController(IPago pago)
        {

            pagoDATA = pago;


        }



        [HttpGet]
        public IActionResult ListarPagos()
        {

            return Ok(pagoDATA.Listado());

        }


        [HttpGet]
        [Route("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var pago = pagoDATA.ObtenerPagoPorId(id);
            if (pago == null)
                return NotFound();

            return Ok(pago);
        }



        [HttpPost("Registrar")]
        public IActionResult Registrar(PagoDTO pago)
        {
            var nuevoPago = pagoDATA.Registrar(pago);

            if (nuevoPago == null)
                return BadRequest("No se pudo registrar el pago");

            // Devuelve 201 Created + el objeto en JSON
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoPago.IdPago }, nuevoPago);
        }

        [Route("update/{id}")]
        [HttpPut]
        public IActionResult Editar(int id, PagoDTO c)
        {
            try
            {
                var pago = pagoDATA.Editar(id, c);
                return Ok(pago);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }



        [HttpDelete("{id}")]
        public IActionResult EliminarAnimal(int id)
        {
            var result = pagoDATA.Eliminar(id);
            if (result)
                return Ok(new { mensaje = "Pago eliminado correctamente" });
            else
                return NotFound(new { mensaje = "Pago no encontrado" });
        }






    }
}
