using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntrePatasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvioController : ControllerBase
    {
        private readonly IEnvio envioDATA;

        public EnvioController(IEnvio envio)
        {

            envioDATA = envio;


        }

        [HttpGet]
        public IActionResult ListarEnvios()
        {

            return Ok(envioDATA.Listado());

        }


        [HttpGet]
        [Route("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var envio = envioDATA.ObtenerEnvioPorId(id);
            if (envio == null)
                return NotFound();

            return Ok(envio);
        }



        [HttpPost("Registrar")]
        public IActionResult Registrar(EnvioDTO envio)
        {
            var nuevoEnvio = envioDATA.Registrar(envio);
            if (nuevoEnvio == null)
                return NotFound();
            return Ok(nuevoEnvio);
        }


        [Route("update/{id}")]
        [HttpPut]
        public IActionResult Editar(int id, EnvioDTO c)
        {
            try
            {
                var envio = envioDATA.Editar(id, c);
                return Ok(envio);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }


        [HttpDelete("{id}")]
        public IActionResult EliminarEnvio(int id)
        {
            var result = envioDATA.Eliminar(id);
            if (result)
                return Ok(new { mensaje = "Envio eliminado correctamente" });
            else
                return NotFound(new { mensaje = "Envio no encontrado" });
        }



    }
}
