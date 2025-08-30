using System.Text;
using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntrePatasWEB.Controllers
{
    public class PaginaPrincipalController : Controller
    {
        private readonly IConfiguration _config;

        public PaginaPrincipalController(IConfiguration config)
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


        [HttpGet]
        public IActionResult Login()
        {
            return View();        }

        [HttpPost]
        public IActionResult Login(UsuarioDTO usuario)
        {
            var resultado = VerificarLoginAsync(usuario).Result;

            if (resultado == null)
            {
                ViewBag.Error = "Email o contraseña incorrectos";
                return View(usuario);
            }

            HttpContext.Session.SetString("NombreUsuario", $"{resultado.Nombre} {resultado.Apellido}");
            HttpContext.Session.SetInt32("IdUsuario", resultado.IdUsuario);
            HttpContext.Session.SetString("TipoUsuario", resultado.TipoUsuario);

            return resultado.TipoUsuario switch
            {
                "Administrador" => RedirectToAction("Index", "Admin"),
                "Cliente" => RedirectToAction("Index", "Home"),
                "Veterinario" => RedirectToAction("Index", "Home"),
                _ => RedirectToAction("Index", "Usuario")
            };
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "PaginaPrincipal");
        }




        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(UsuarioDTO usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);
                    var contenidoJson = JsonConvert.SerializeObject(usuario);
                    var contenido = new StringContent(contenidoJson, Encoding.UTF8, "application/json");

                    var respuesta = httpCliente.PostAsync("Usuario/Registrar", contenido).Result;

                    if (!respuesta.IsSuccessStatusCode)
                    {
                        ViewBag.Error = "No se pudo registrar el usuario. Intenta nuevamente.";
                        return View(usuario);
                    }

                    // Registro exitoso, redirigimos al login
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(usuario);
            }
        }








        public IActionResult Index()
        {
            return View();
        }
    }
}
