using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace EntrePatasWEB.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _config;

        public AdminController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("TipoUsuario");

            if (rol != "Administrador")
                return RedirectToAction("Login", "PaginaPrincipal");

            var usuarios = ObtenerUsuariosAsync().Result;
            var animales = ObtenerAnimalesAsync().Result;
            var solicitudes = ObtenerSolicitudesAsync().Result;

            ViewBag.TotalUsuarios = usuarios.Count;
            ViewBag.TotalAnimales = animales.Count;
            ViewBag.SolicitudesAprobadas = solicitudes.Count(s => s.Estado == "Aprobada");
            ViewBag.SolicitudesPendientes = solicitudes.Count(s => s.Estado == "Pendiente");

            return View();
        }

        private async Task<List<UsuarioDTO>> ObtenerUsuariosAsync()
        {
            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri(_config["Services:Url_API"]);
                var respuesta = await cliente.GetAsync("Usuario");
                var data = await respuesta.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<UsuarioDTO>>(data);
            }
        }

        private async Task<List<AnimalDTO>> ObtenerAnimalesAsync()
        {
            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri(_config["Services:Url_API"]);
                var respuesta = await cliente.GetAsync("Animal");
                var data = await respuesta.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<AnimalDTO>>(data);
            }
        }

        private async Task<List<SolicitudDTO>> ObtenerSolicitudesAsync()
        {
            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri(_config["Services:Url_API"]);
                var respuesta = await cliente.GetAsync("Solicitud");
                var data = await respuesta.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<SolicitudDTO>>(data);
            }
        }
    }
}
