using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntrePatasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DenunciasController : ControllerBase
    {

        private readonly IDenuncia denunciaDATA;

        public DenunciasController(IDenuncia denuncia) {

            denunciaDATA = denuncia;
        
        
        }

        [HttpGet]
        public IActionResult ListarDenuncias() {

            return Ok(denunciaDATA.Listado());
        
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var denuncia = denunciaDATA.ObtenerDenunciaPorId(id);
            if (denuncia == null)
                return NotFound();

            return Ok(denuncia);
        }

        [HttpPost("Registrar")]
        public IActionResult Registrar(DenunciaDTO denuncia)
        {
            var nuevaDenuncia = denunciaDATA.Registrar(denuncia);
            if (nuevaDenuncia == null)
                return NotFound();
            return Ok(nuevaDenuncia);
        }


    }
}
