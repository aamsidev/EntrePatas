using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntrePatasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudVacunaController : ControllerBase
    {


        private readonly ISolicitudVacuna solicitudDATA;

        public SolicitudVacunaController(ISolicitudVacuna solicitud)
        {

            solicitudDATA = solicitud;
        }

        [HttpGet]
        public IActionResult ListarSolicitudVacuna()
        {
            return Ok(solicitudDATA.Listado());
        }


        [HttpGet]
        [Route("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var solicitud = solicitudDATA.ObtenerSolicitudVacunaPorId(id);
            if (solicitud == null)
                return NotFound();

            return Ok(solicitud);
        }



        [HttpPost("Registrar")]
        public IActionResult Registrar(SolicitudVacunaDTO solicitud)
        {
            var nuevaSolicitud = solicitudDATA.Registrar(solicitud);
            if (nuevaSolicitud == null)
                return NotFound();
            return Ok(nuevaSolicitud);
        }

       













    }
}
