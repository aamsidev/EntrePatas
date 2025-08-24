using System.Text;
using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntrePatasWEB.Controllers
{
    public class UsuarioController : Controller
    {


        private readonly IConfiguration _config;

        public UsuarioController(IConfiguration config)
        {
            _config = config;
        }




        private async Task<UsuarioDTO?> VerificarLoginAsync(UsuarioDTO usuario)
        {
            UsuarioDTO? loginResponse = null;

            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);

                    var contenidoJson = JsonConvert.SerializeObject(usuario);
                    var contenido = new StringContent(contenidoJson, Encoding.UTF8, "application/json");

                    var respuesta = await httpCliente.PostAsync("Usuario/login", contenido);

                    if (!respuesta.IsSuccessStatusCode)
                        return null;

                    var data = await respuesta.Content.ReadAsStringAsync();
                    loginResponse = JsonConvert.DeserializeObject<UsuarioDTO>(data);

                    if (loginResponse == null)
                        return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción: {ex.Message}");
            }

            return loginResponse;
        }









        private async Task<List<UsuarioDTO>> obtenerListadoUsuarioAsync()
        {
            var listado = new List<UsuarioDTO>();

            using (var clienteHttp = new HttpClient())
            {

                clienteHttp.BaseAddress = new Uri(_config["Services:Url_API"]);

                var mensaje = await clienteHttp.GetAsync("Usuario");

                var data = await mensaje.Content.ReadAsStringAsync();


                listado = JsonConvert.DeserializeObject<List<UsuarioDTO>>(data);
            }
            return listado;
        }

        private async Task<UsuarioDTO> ObtenerUsuarioId(int id)
        {
            var usuario = new UsuarioDTO();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_config["Services:Url_API"]);
                    var mensaje = await httpClient.GetAsync($"Usuario/{id}");
                    var data = await mensaje.Content.ReadAsStringAsync();
                    usuario = JsonConvert.DeserializeObject<UsuarioDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return usuario;
        }

        private async Task<UsuarioDTO> createUsuario(UsuarioDTO cliente)
        {
            var nuevoUsuario = new UsuarioDTO();
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);
                    StringContent contenido = new StringContent(JsonConvert.SerializeObject(cliente), System.Text.Encoding.UTF8, "application/json");
                    var respuesta = await httpCliente.PostAsync("Usuario/registrar", contenido);
                    var data = await respuesta.Content.ReadAsStringAsync();
                    nuevoUsuario = JsonConvert.DeserializeObject<UsuarioDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return nuevoUsuario;
        }


        private async Task<UsuarioDTO> UpdateUsuario(int id, UsuarioDTO usuario)
        {
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);

                    var contenido = new StringContent(
                        JsonConvert.SerializeObject(usuario),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    var respuesta = await httpCliente.PutAsync($"Usuario/update/{id}", contenido);

                    if (!respuesta.IsSuccessStatusCode)
                        return null;

                    var data = await respuesta.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<UsuarioDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        private async Task<bool> EliminarUsuarioAsync(int id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:Url_API"]);
                var response = await clienteHttp.DeleteAsync($"Usuario/{id}");

                return response.IsSuccessStatusCode;
            }
        }







        public IActionResult Index()
        {
            var listado = obtenerListadoUsuarioAsync().Result;
            return View(listado);
        }


        public IActionResult Details(int id )
        {

            UsuarioDTO usuario = ObtenerUsuarioId(id).Result;
            return View(usuario);
       
        
        }

        public IActionResult Create() {

            return View(new UsuarioDTO());
        }

        [HttpPost]
        public IActionResult Create(UsuarioDTO create) {

            UsuarioDTO nuevoUsuario = createUsuario(create).Result;
            return RedirectToAction("Details", new { id = nuevoUsuario.IdUsuario});
        }

        public IActionResult Edit(int id)
        {
            var usuario = ObtenerUsuarioId(id).Result;

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost]
        public IActionResult Edit(int id, UsuarioDTO usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);
            
            var usuarioEditado = UpdateUsuario(id, usuario).Result;

            if (usuarioEditado == null)
            {
                ModelState.AddModelError("", "No se pudo actualizar el usuario");
                return View(usuario);
            }

            return RedirectToAction("Index", new { id = usuarioEditado.IdUsuario });
        }


        public IActionResult Delete(int id)
        {
            UsuarioDTO usuario = ObtenerUsuarioId(id).Result;
            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            bool eliminado = EliminarUsuarioAsync(id).Result;

            if (eliminado)
            {
                TempData["Mensaje"] = "Usuario eliminado correctamente";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el usuario";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Perfil()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (!idUsuario.HasValue)
            {
                TempData["Error"] = "No se encontró información de usuario en sesión.";
                return RedirectToAction("Login", "PaginaPrincipal");
            }

            var usuario = ObtenerUsuarioId(idUsuario.Value).Result;

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }


        [HttpPost]
        [HttpPost]
        public IActionResult Perfil(UsuarioDTO usuario, string contrasenaActual, string nuevaContrasena)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (!idUsuario.HasValue)
            {
                TempData["Error"] = "No se encontró información de usuario en sesión.";
                return RedirectToAction("Login", "PaginaPrincipal");
            }

            var actualizado = UpdateCredencialesUsuario(
                idUsuario.Value,
                usuario.Correo.Trim(),
                contrasenaActual.Trim(),
                nuevaContrasena?.Trim()
            ).Result;

            if (actualizado == null)
            {
                TempData["Error"] = "No se pudo actualizar. Verifica tu contraseña actual.";
                return View(usuario);
            }

            TempData["Mensaje"] = "Perfil actualizado correctamente.";
            return RedirectToAction("Perfil");
        }


        private async Task<UsuarioDTO?> UpdateCredencialesUsuario(int id, string correo, string contrasenaActual, string nuevaContrasena)
        {
            using (var httpCliente = new HttpClient())
            {
                httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);

                var payload = new
                {
                    Correo = correo,
                    ContrasenaActual = contrasenaActual,
                    NuevaContrasena = nuevaContrasena
                };

                var contenido = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                var respuesta = await httpCliente.PutAsync($"Usuario/update-credenciales/{id}", contenido);

                if (!respuesta.IsSuccessStatusCode)
                    return null;

                var data = await respuesta.Content.ReadAsStringAsync();

                var usuarioActualizado = JsonConvert.DeserializeObject<UsuarioDTO>(data);

                return usuarioActualizado;
            }
        }











    }
}
