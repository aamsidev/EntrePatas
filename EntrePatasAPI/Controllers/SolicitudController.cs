using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntrePatasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudController : ControllerBase
    {

        private readonly ISolicitud solicitudDATA;

        public SolicitudController(ISolicitud solicitud)
        {

            solicitudDATA = solicitud;


        }

        [HttpGet]
        public IActionResult ListarSolicitudes()
        {

            return Ok(solicitudDATA.Listado());

        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var animal = solicitudDATA.ObtenerSolicitudPorId(id);
            if (animal == null)
                return NotFound();

            return Ok(animal);
        }

        [HttpPost("Registrar")]
        public IActionResult Registrar(SolicitudDTO animal)
        {
            var nuevoSolicitud = solicitudDATA.Registrar(animal);
            if (nuevoSolicitud == null)
                return NotFound();
            return Ok(nuevoSolicitud);
        }

        [Route("update/{id}")]
        [HttpPut]
        public IActionResult Editar(int id, SolicitudDTO c)
        {
            try
            {
                var solicitud = solicitudDATA.Editar(id, c);
                return Ok(solicitud);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarSolicitud(int id)
        {
            var result = solicitudDATA.Eliminar(id);
            if (result)
                return Ok(new { mensaje = "Solicitud eliminado correctamente" });
            else
                return NotFound(new { mensaje = "Solicitud no encontrado" });
        }




    }
}
