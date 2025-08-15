using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntrePatasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudAdopcionController : ControllerBase
    {

        private readonly ISolicitudAdopcion solicitudDATA;

        public SolicitudAdopcionController(ISolicitudAdopcion solicitud)
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
            var solicitud = solicitudDATA.ObtenerSolicitudAdopcionPorId(id);
            if (solicitud == null)
                return NotFound();

            return Ok(solicitud);
        }

        [HttpPost("Registrar")]
        public IActionResult Registrar(SolicitudAdopcionDTO solicitud)
        {
            var nuevaSolicitud = solicitudDATA.Registrar(solicitud);
            if (nuevaSolicitud == null)
                return NotFound();
            return Ok(nuevaSolicitud);
        }




    }
}
