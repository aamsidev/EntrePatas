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


        [HttpPost("Registrar")]
        public IActionResult Registrar(VacunaDTO vacuna)
        {
            var nuevoVacuna = vacunaDATA.Registrar(vacuna);
            if (nuevoVacuna == null)
                return NotFound();
            return Ok(nuevoVacuna);
        }



        [Route("update/{id}")]
        [HttpPut]
        public IActionResult Editar(int id, VacunaDTO c)
        {
            try
            {
                var vacuna = vacunaDATA.Actualizar(id, c);
                return Ok(vacuna);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }



        [HttpDelete("{id}")]
        public IActionResult EliminarVacuna(int id)
        {
            var result = vacunaDATA.Eliminar(id);
            if (result)
                return Ok(new { mensaje = "Usuario eliminado correctamente" });
            else
                return NotFound(new { mensaje = "Usuario no encontrado" });
        }
    }
}
