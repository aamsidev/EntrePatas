using System.Text;
using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntrePatasWEB.Controllers
{
    public class ClienteController : Controller
    {


        private readonly IConfiguration _config;

        public ClienteController(IConfiguration config)
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


        public IActionResult _AnimalFiltro(string estado, int? edad, string raza)
        {
            var listado = ObtenerListadoAnimalAsync().Result;

            // Llenar combos
            ViewBag.Estados = new List<string> { "Disponible", "Adoptado", "Reservado" };
            ViewBag.Edades = listado.Select(a => a.Edad).Distinct().OrderBy(e => e).ToList();
            ViewBag.Razas = listado.Select(a => a.Raza).Where(r => !string.IsNullOrEmpty(r)).Distinct().OrderBy(r => r).ToList();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(estado))
                listado = listado.Where(a => a.Estado == estado).ToList();
            if (edad.HasValue)
                listado = listado.Where(a => a.Edad == edad.Value).ToList();
            if (!string.IsNullOrEmpty(raza))
                listado = listado.Where(a => a.Raza == raza).ToList();

            ViewBag.EstadoSeleccionado = estado;
            ViewBag.EdadSeleccionada = edad;
            ViewBag.RazaSeleccionada = raza;

            return PartialView("_AnimalFiltro", listado); // <- IMPORTANTE
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






    }
}
