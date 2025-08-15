using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntrePatasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly IUsuario usuarioDATA;

        public UsuariosController(IUsuario usuario)
        { 
        
        usuarioDATA = usuario;
        }

        [HttpGet]
        public IActionResult ListarUsarios()
        {
            return Ok(usuarioDATA.Listado());
        }


        [HttpGet]
        [Route("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var usuario = usuarioDATA.ObtenerUsuarioPorId(id);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }



        [HttpPost("Registrar")]
        public IActionResult Registrar(UsuarioDTO usuario)
        {
            var nuevoUsuario = usuarioDATA.Registrar(usuario);
            if (nuevoUsuario == null)
                return NotFound();
            return Ok(nuevoUsuario);
        }

        [HttpPost("RegistrarAdmin")]
        public IActionResult RegistrarAdmin(UsuarioDTO usuario)
        {
            var nuevoUsuario = usuarioDATA.RegistrarAdmin(usuario);
            if (nuevoUsuario == null)
                return NotFound();
            return Ok(nuevoUsuario);
        }


    }
}
