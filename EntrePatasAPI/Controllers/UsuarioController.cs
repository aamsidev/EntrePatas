using EntrePatasAPI.Data;
using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntrePatasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        private readonly IUsuario usuarioDATA;

        public UsuarioController(IUsuario usuario)
        {

            usuarioDATA = usuario;
        }

        [HttpGet]
        public IActionResult ListarUsuarios()
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

        [Route("update/{id}")]
        [HttpPut]
        public IActionResult Editar(int id, UsuarioDTO c)
        {
            try
            {
                var usuario = usuarioDATA.Editar(id, c);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }


        [HttpDelete("{id}")]
public IActionResult EliminarUsuario(int id)
        {
            var result = usuarioDATA.Eliminar(id);
            if (result)
                return Ok(new { mensaje = "Usuario eliminado correctamente" });
            else
                return NotFound(new { mensaje = "Usuario no encontrado" });
        }



        [HttpPost("login")]
        public IActionResult Login([FromBody] UsuarioDTO usuario)
        {
            var user = usuarioDATA.VerificarLogin(usuario.Correo, usuario.Contrasena);

            if (user == null)
                return Unauthorized(new { mensaje = "Credenciales inválidas" });

            return Ok(user);
        }

        [HttpPut("update-credenciales/{id}")]
        public IActionResult ActualizarCredenciales(int id, [FromBody] UsuarioUpdateDTO data)
        {
            var usuario = usuarioDATA.ObtenerUsuarioPorId(id);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            string contrasenaActual = data.ContrasenaActual;
            string nuevaContrasena = data.NuevaContrasena;
            string correo = data.Correo;

            var validacion = usuarioDATA.VerificarLogin(usuario.Correo, contrasenaActual);
            if (validacion == null)
                return Unauthorized(new { mensaje = "La contraseña actual es incorrecta" });

            var usuarioDTO = new UsuarioDTO
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = correo,
                Telefono = usuario.Telefono,
                Direccion = usuario.Direccion,
                Contrasena = !string.IsNullOrEmpty(nuevaContrasena) ? nuevaContrasena : usuario.Contrasena,
                TipoUsuario = usuario.TipoUsuario
            };

            var actualizado = usuarioDATA.Editar(id, usuarioDTO);
            return Ok(actualizado);
        }











    }
}
