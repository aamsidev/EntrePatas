using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntrePatasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacunaController : ControllerBase
    {

        private readonly IVacuna vacunaDATA;

        public VacunaController(IVacuna vacuna)
        {

            vacunaDATA = vacuna;
        }

        [HttpGet]
        public IActionResult ListarVacunas()
        {
            return Ok(vacunaDATA.Listado());
        }


        [HttpGet]
        [Route("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var vacuna = vacunaDATA.ObtenerVacunaPorId(id);
            if (vacuna == null)
                return NotFound();

            return Ok(vacuna);
        }



        [HttpPost]
        public IActionResult Registrar(VacunaDTO vacuna)
        {
            var nuevaVacuna = vacunaDATA.Registrar(vacuna);
            if (nuevaVacuna == null)
        return BadRequest("No se pudo registrar la vacuna.");
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaVacuna.IdVacuna }, nuevaVacuna);
        }





    }
}
