using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntrePatasWEB.Controllers
{
    public class VacunaController : Controller
    {
        private readonly IConfiguration _config;

        public VacunaController(IConfiguration config)
        {
            _config = config;
        }

        private async Task<List<VacunaDTO>> obtenerListadoVacunaAsync()
        {
            var listado = new List<VacunaDTO>();

            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var mensaje = await clienteHttp.GetAsync("Vacuna");

                if (mensaje.IsSuccessStatusCode)
                {
                    var data = await mensaje.Content.ReadAsStringAsync();
                    listado = JsonConvert.DeserializeObject<List<VacunaDTO>>(data);
                }
            }

            return listado;
        }

        public IActionResult Index()
        {
            var listado = obtenerListadoVacunaAsync().Result;
            return View(listado);
        }

        public async Task<IActionResult> Details(int id)
        {
            VacunaDTO vacuna = null;

            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var mensaje = await clienteHttp.GetAsync($"Vacuna/{id}");

                if (mensaje.IsSuccessStatusCode)
                {
                    var data = await mensaje.Content.ReadAsStringAsync();
                    vacuna = JsonConvert.DeserializeObject<VacunaDTO>(data);
                }
            }

            if (vacuna == null)
                return NotFound();

            return View(vacuna);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VacunaDTO vacuna)
        {
            if (!ModelState.IsValid)
                return View(vacuna);

            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var contenido = new StringContent(JsonConvert.SerializeObject(vacuna), System.Text.Encoding.UTF8, "application/json");
                var respuesta = await clienteHttp.PostAsync("Vacuna", contenido);

                if (respuesta.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Error al registrar la vacuna.");
            return View(vacuna);
        }

        public async Task<IActionResult> Edit(int id)
        {
            VacunaDTO vacuna = null;
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHttp.GetAsync($"Vacuna/{id}");
                if (mensaje.IsSuccessStatusCode)
                {
                    var data = await mensaje.Content.ReadAsStringAsync();
                    vacuna = JsonConvert.DeserializeObject<VacunaDTO>(data);
                }
            }
            if (vacuna == null)
                return NotFound();
            return View(vacuna);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VacunaDTO vacuna)
        {
            if (id != vacuna.IdVacuna)
                return NotFound();
            if (!ModelState.IsValid)
                return View(vacuna);
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var contenido = new StringContent(JsonConvert.SerializeObject(vacuna), System.Text.Encoding.UTF8, "application/json");
                var respuesta = await clienteHttp.PutAsync($"Vacuna/{id}", contenido);
                if (respuesta.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al actualizar la vacuna.");
            return View(vacuna);
        }

        public async Task<IActionResult> Delete(int id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHttp.GetAsync($"Vacuna/{id}");
                if (mensaje.IsSuccessStatusCode)
                {
                    var data = await mensaje.Content.ReadAsStringAsync();
                    var vacuna = JsonConvert.DeserializeObject<VacunaDTO>(data);
                    return View(vacuna);
                }
            }
            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHttp.DeleteAsync($"Vacuna/{id}");
                if (mensaje.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al eliminar la vacuna.");
            return View();
        }
    }
}
