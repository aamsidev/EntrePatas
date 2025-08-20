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

                var data = await mensaje.Content.ReadAsStringAsync();


                listado = JsonConvert.DeserializeObject<List<VacunaDTO>>(data);
            }
            return listado;
        }

        private async Task<VacunaDTO> ObtenerVacunaId(int id)
        {
            var usuario = new VacunaDTO();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_config["Services:Url_API"]);
                    var mensaje = await httpClient.GetAsync($"Vacuna/{id}");
                    var data = await mensaje.Content.ReadAsStringAsync();
                    usuario = JsonConvert.DeserializeObject<VacunaDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return usuario;
        }

        private async Task<VacunaDTO> createVacuna(VacunaDTO vacuna)
        {
            var nuevoVacuna = new VacunaDTO();
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);
                    StringContent contenido = new StringContent(JsonConvert.SerializeObject(vacuna), System.Text.Encoding.UTF8, "application/json");
                    var respuesta = await httpCliente.PostAsync("Vacuna/registrar", contenido);
                    var data = await respuesta.Content.ReadAsStringAsync();
                    nuevoVacuna = JsonConvert.DeserializeObject<VacunaDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return nuevoVacuna;
        }


        private async Task<VacunaDTO> UpdateVacuna(int id, VacunaDTO vacuna)
        {
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);

                    var contenido = new StringContent(
                        JsonConvert.SerializeObject(vacuna),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    // Consumimos el endpoint PUT
                    var respuesta = await httpCliente.PutAsync($"Vacuna/update/{id}", contenido);

                    if (!respuesta.IsSuccessStatusCode)
                        return null;

                    var data = await respuesta.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<VacunaDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }



        private async Task<bool> EliminarVacunaAsync(int id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var response = await clienteHttp.DeleteAsync($"Vacuna/{id}");

                return response.IsSuccessStatusCode;
            }
        }








        public IActionResult Index()
        {
            var listado = obtenerListadoVacunaAsync().Result;
            return View(listado);
        }


        public IActionResult Details(int id)
        {

            VacunaDTO vacuna = ObtenerVacunaId(id).Result;
            return View(vacuna);


        }

        public IActionResult Create()
        {

            return View(new VacunaDTO());
        }

        [HttpPost]
        public IActionResult Create(VacunaDTO create)
        {

            VacunaDTO nuevoVacuna = createVacuna(create).Result;
            return RedirectToAction("Details", new { id = nuevoVacuna.IdVacuna });
        }



        public IActionResult Edit(int id)
        {
            var vacuna = ObtenerVacunaId(id).Result;

            if (vacuna == null)
                return NotFound();

            return View(vacuna);
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, VacunaDTO vacuna)
        {
            if (!ModelState.IsValid)
                return View(vacuna);

            var vacunaEditado = UpdateVacuna(id, vacuna).Result;

            if (vacunaEditado == null)
            {
                ModelState.AddModelError("", "No se pudo actualizar el usuario");
                return View(vacuna);
            }

            return RedirectToAction("Index", new { id = vacunaEditado.IdVacuna });
        }




        public IActionResult Delete(int id)
        {
            VacunaDTO vacuna = ObtenerVacunaId(id).Result;
            return View(vacuna);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            bool eliminado = EliminarVacunaAsync(id).Result;

            if (eliminado)
            {
                TempData["Mensaje"] = "Vacuna eliminado correctamente";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el Vacuna";
            }

            return RedirectToAction("Index");
        }



    }
}
