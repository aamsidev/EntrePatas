using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntrePatasWEB.Controllers
{
    public class EnvioController : Controller
    {

        private readonly IConfiguration _config;

        public EnvioController(IConfiguration config)
        {
            _config = config;
        }


        private async Task<List<EnvioDTO>> ObtenerListadoEnvioAsync()
        {
            var listado = new List<EnvioDTO>();

            using (var clienteHttp = new HttpClient())
            {

                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var mensaje = await clienteHttp.GetAsync("Envio");

                var data = await mensaje.Content.ReadAsStringAsync();


                listado = JsonConvert.DeserializeObject<List<EnvioDTO>>(data);
            }
            return listado;
        }


        private async Task<EnvioDTO> ObtenerEnvioId(int id)
        {
            var animal = new EnvioDTO();
            using (var clienteHTTP = new HttpClient())
            {
                clienteHTTP.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHTTP.GetAsync($"Envio/{id}");
                var data = await mensaje.Content.ReadAsStringAsync();
                animal = JsonConvert.DeserializeObject<EnvioDTO>(data);
            }
            return animal;
        }

        private async Task<EnvioDTO> RegistrarEnvio(EnvioDTO envio)

        {
            var nuevoEnvio = new EnvioDTO();
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);
                    StringContent contenido = new StringContent(JsonConvert.SerializeObject(envio), System.Text.Encoding.UTF8, "application/json");
                    var respuesta = await httpCliente.PostAsync("Envio/registrar", contenido);
                    var data = await respuesta.Content.ReadAsStringAsync();
                    nuevoEnvio = JsonConvert.DeserializeObject<EnvioDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return nuevoEnvio;
        }


        private async Task<EnvioDTO> UpdateEnvio(int id, EnvioDTO envio)
        {
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);

                    var contenido = new StringContent(
                        JsonConvert.SerializeObject(envio),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    // Consumimos el endpoint PUT
                    var respuesta = await httpCliente.PutAsync($"Envio/update/{id}", contenido);

                    if (!respuesta.IsSuccessStatusCode)
                        return null;

                    var data = await respuesta.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<EnvioDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<bool> EliminarEnvioAsync(int id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var response = await clienteHttp.DeleteAsync($"Envio/{id}");

                return response.IsSuccessStatusCode;
            }
        }


        public IActionResult Index()
        {
            var listado = ObtenerListadoEnvioAsync().Result;
            return View(listado);
        }


        public IActionResult Details(int id)
        {

            EnvioDTO envio = ObtenerEnvioId(id).Result;
            return View(envio);


        }

        public IActionResult Create()
        {
            return View(new EnvioDTO());
        }

        [HttpPost]
        public IActionResult Create(EnvioDTO envio)
        {
            EnvioDTO nuevoEnvio = RegistrarEnvio(envio).Result;


            return RedirectToAction("Details", new { id = nuevoEnvio.IdEnvio });
        }


        public IActionResult Edit(int id)
        {

            var Envio = ObtenerEnvioId(id).Result;

            if (Envio == null)
                return NotFound();

            return View(Envio);

        }

        // POST: Usuario/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, EnvioDTO envio)
        {
            if (!ModelState.IsValid)
                return View(envio);

            var envioEditado = UpdateEnvio(id, envio).Result;

            if (envioEditado == null)
            {
                ModelState.AddModelError("", "No se pudo actualizar el envio");
                return View(envio);
            }

            return RedirectToAction("Index", new { id = envioEditado.IdEnvio});
        }



        public IActionResult Delete(int id)
        {
            EnvioDTO envio = ObtenerEnvioId(id).Result;
            return View(envio);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            bool eliminado = EliminarEnvioAsync(id).Result;

            if (eliminado)
            {
                TempData["Mensaje"] = "Envio eliminado correctamente";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el Envio";
            }

            return RedirectToAction("Index");
        }


    }
}
