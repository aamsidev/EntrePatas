using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace EntrePatasWEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        private async Task<List<AnimalDTO>> ObtenerListadoAnimalAsync()
        {
            var listado = new List<AnimalDTO>();
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHttp.GetAsync("Animal");
                var data = await mensaje.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<AnimalDTO>>(data);
            }
            return listado;
        }

        private async Task<List<ProductoDTO>> ObtenerListadoProductoAsync()
        {
            var listado = new List<ProductoDTO>();
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHttp.GetAsync("Producto");
                var data = await mensaje.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<ProductoDTO>>(data);
            }
            return listado;
        }

        public IActionResult Index()
        {
            var nombre = HttpContext.Session.GetString("NombreUsuario");
            var listado = ObtenerListadoAnimalAsync().Result;
            var productos = ObtenerListadoProductoAsync().Result;
            ViewBag.NombreUsuario = nombre;
            ViewBag.Productos = productos;
            ViewBag.Estados = new List<string> { "Disponible", "Adoptado", "Reservado" };
            ViewBag.Edades = listado.Select(a => a.Edad).Distinct().OrderBy(e => e).ToList();
            ViewBag.Razas = listado.Select(a => a.Raza).Where(r => !string.IsNullOrEmpty(r)).Distinct().OrderBy(r => r).ToList();
            return View(listado);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Contacto()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contacto(string nombre, string correo, string mensaje)
        {
            TempData["ContactoExito"] = "Mensaje enviado correctamente. ¡Gracias por contactarnos!";

            return RedirectToAction(nameof(Contacto));
        }
    }
}
