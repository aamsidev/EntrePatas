using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntrePatasWEB.Controllers
{
    public class SolicitudController : Controller
    {

        private readonly IConfiguration _config;

        public SolicitudController(IConfiguration config)
        {
            _config = config;
        }


        private async Task<List<SolicitudDTO>> ObtenerListadoSolicitudAsync()
        {
            var listado = new List<SolicitudDTO>();

            using (var clienteHttp = new HttpClient())
            {

                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var mensaje = await clienteHttp.GetAsync("Solicitud");

                var data = await mensaje.Content.ReadAsStringAsync();


                listado = JsonConvert.DeserializeObject<List<SolicitudDTO>>(data);
            }
            return listado;
        }


        private async Task<SolicitudDTO> ObtenerSolicitudId(int id)
        {
            var solicitud = new SolicitudDTO();
            using (var clienteHTTP = new HttpClient())
            {
                clienteHTTP.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHTTP.GetAsync($"Solicitud/{id}");
                var data = await mensaje.Content.ReadAsStringAsync();
                solicitud = JsonConvert.DeserializeObject<SolicitudDTO>(data);
            }
            return solicitud;
        }


        private async Task<SolicitudDTO> RegistrarSolicitud(SolicitudDTO solicitud)

        {
            var nuevoSolicitud = new SolicitudDTO();
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);
                    StringContent contenido = new StringContent(JsonConvert.SerializeObject(solicitud), System.Text.Encoding.UTF8, "application/json");
                    var respuesta = await httpCliente.PostAsync("Solicitud/registrar", contenido);
                    var data = await respuesta.Content.ReadAsStringAsync();
                    nuevoSolicitud = JsonConvert.DeserializeObject<SolicitudDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return nuevoSolicitud;
        }


        private async Task<SolicitudDTO> UpdateSolicitud(int id, SolicitudDTO solicitud)
        {
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);

                    var contenido = new StringContent(
                        JsonConvert.SerializeObject(solicitud),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    // Consumimos el endpoint PUT
                    var respuesta = await httpCliente.PutAsync($"Solicitud/update/{id}", contenido);

                    if (!respuesta.IsSuccessStatusCode)
                        return null;

                    var data = await respuesta.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<SolicitudDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        private async Task<bool> EliminarSolicitudAsync(int id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var response = await clienteHttp.DeleteAsync($"Solicitud/{id}");

                return response.IsSuccessStatusCode;
            }
        }


        public IActionResult Index()
        {
            var listado = ObtenerListadoSolicitudAsync().Result;
            return View(listado);
        }



        public IActionResult Details(int id)
        {

            SolicitudDTO solicitud = ObtenerSolicitudId(id).Result;
            return View(solicitud);


        }

        public IActionResult Create()
        {
            return View(new SolicitudDTO());
        }

        [HttpPost]
        public IActionResult Create(SolicitudDTO solicitud)
        {
            SolicitudDTO nuevoSolicitud = RegistrarSolicitud(solicitud).Result;


            return RedirectToAction("Details", new { id = nuevoSolicitud.IdSolicitud });
        }



        public IActionResult Edit(int id)
        {

            var Solicitud = ObtenerSolicitudId(id).Result;

            if (Solicitud == null)
                return NotFound();

            return View(Solicitud);

        }

        // POST: Usuario/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, SolicitudDTO solicitud)
        {
            if (!ModelState.IsValid)
                return View(solicitud);

            var solicitudEditado = UpdateSolicitud(id, solicitud).Result;

            if (solicitudEditado == null)
            {
                ModelState.AddModelError("", "No se pudo actualizar la solucitud");
                return View(solicitud);
            }

            return RedirectToAction("Index", new { id = solicitudEditado.IdSolicitud});
        }

        public IActionResult Delete(int id)
        {
            SolicitudDTO solicitud = ObtenerSolicitudId(id).Result;
            return View(solicitud);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            bool eliminado = EliminarSolicitudAsync(id).Result;

            if (eliminado)
            {
                TempData["Mensaje"] = "Solicitud eliminado correctamente";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el solicitud";
            }

            return RedirectToAction("Index");
        }
    }
}
