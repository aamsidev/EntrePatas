using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntrePatasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetallePedidoController : ControllerBase
    {

        private readonly IDetallePedido detalleDATA;

        public DetallePedidoController(IDetallePedido detalle)
        {

            detalleDATA = detalle;


        }


        [HttpGet]
        public IActionResult ListarDetalles()
        {

            return Ok(detalleDATA.Listado());

        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var detalle = detalleDATA.ObtenerDetallePedidoPorId(id);
            if (detalle == null)
                return NotFound();

            return Ok(detalle);
        }

        [HttpPost("Registrar")]
        public IActionResult Registrar(DetallePedidoDTO detalle)
        {
            var nuevoDetalle = detalleDATA.Registrar(detalle);
            if (nuevoDetalle == null)
                return NotFound();
            return Ok(nuevoDetalle);


        }


        [Route("update/{id}")]
        [HttpPut]
        public IActionResult Editar(int id, DetallePedidoDTO c)
        {
            try
            {
                var detalle = detalleDATA.Editar(id, c);
                return Ok(detalle);
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
            var result = detalleDATA.Eliminar(id);
            if (result)
                return Ok(new { mensaje = "Detalle eliminado correctamente" });
            else
                return NotFound(new { mensaje = "Detalle no encontrado" });
        }





    }
}
