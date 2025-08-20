using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntrePatasWEB.Controllers
{
    public class SolicitudVacunaController : Controller
    {


        private readonly IConfiguration _config;

        public SolicitudVacunaController(IConfiguration config)
        {
            _config = config;
        }


        private async Task<List<SolicitudVacunaDTO>> ObtenerListadoSolicitudVacunaAsync()
        {
            var listado = new List<SolicitudVacunaDTO>();

            using (var clienteHttp = new HttpClient())
            {

                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var mensaje = await clienteHttp.GetAsync("SolicitudVacuna");

                var data = await mensaje.Content.ReadAsStringAsync();


                listado = JsonConvert.DeserializeObject<List<SolicitudVacunaDTO>>(data);
            }
            return listado;
        }


        private async Task<SolicitudVacunaDTO> ObtenerSolicitudId(int id)
        {
            var solicitud = new SolicitudVacunaDTO();
            using (var clienteHTTP = new HttpClient())
            {
                clienteHTTP.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHTTP.GetAsync($"SolicitudVacuna/{id}");
                var data = await mensaje.Content.ReadAsStringAsync();
                solicitud = JsonConvert.DeserializeObject<SolicitudVacunaDTO>(data);
            }
            return solicitud;
        }

        private async Task<SolicitudVacunaDTO> RegistrarSolicitudVacuna(SolicitudVacunaDTO solicitudVacu)

        {
            var nuevoSolicitudVacu = new SolicitudVacunaDTO();
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);
                    StringContent contenido = new StringContent(JsonConvert.SerializeObject(solicitudVacu), System.Text.Encoding.UTF8, "application/json");
                    var respuesta = await httpCliente.PostAsync("SolicitudVacuna/registrar", contenido);
                    var data = await respuesta.Content.ReadAsStringAsync();
                    nuevoSolicitudVacu = JsonConvert.DeserializeObject<SolicitudVacunaDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return nuevoSolicitudVacu;
        }



        private async Task<SolicitudVacunaDTO> UpdateSolicitudVacuna(int id, SolicitudVacunaDTO solicitudVacu)
        {
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);

                    var contenido = new StringContent(
                        JsonConvert.SerializeObject(solicitudVacu),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    // Consumimos el endpoint PUT
                    var respuesta = await httpCliente.PutAsync($"SolicitudVacuna/update/{id}", contenido);

                    if (!respuesta.IsSuccessStatusCode)
                        return null;

                    var data = await respuesta.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<SolicitudVacunaDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        private async Task<bool> EliminarSolicitudVacunaAsync(int id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var response = await clienteHttp.DeleteAsync($"SolicitudVacuna/{id}");

                return response.IsSuccessStatusCode;
            }
        }



        public IActionResult Index()
        {
            var listado = ObtenerListadoSolicitudVacunaAsync().Result;
            return View(listado);
        }

        public IActionResult Details(int id)
        {

            SolicitudVacunaDTO solicitud = ObtenerSolicitudId(id).Result;
            return View(solicitud);


        }



        public IActionResult Create()
        {
            return View(new SolicitudVacunaDTO());
        }

        [HttpPost]
        public IActionResult Create(SolicitudVacunaDTO solicitud)
        {
            SolicitudVacunaDTO nuevoSolicitud = RegistrarSolicitudVacuna(solicitud).Result;


            return RedirectToAction("Details", new { id = nuevoSolicitud.IdSolicitudVacuna });
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
        public IActionResult Edit(int id, SolicitudVacunaDTO solicitud)
        {
            if (!ModelState.IsValid)
                return View(solicitud);

            var solicitudEditado = UpdateSolicitudVacuna(id, solicitud).Result;

            if (solicitudEditado == null)
            {
                ModelState.AddModelError("", "No se pudo actualizar la solucitudVacuna");
                return View(solicitud);
            }

            return RedirectToAction("Index", new { id = solicitudEditado.IdSolicitudVacuna });
        }


        public IActionResult Delete(int id)
        {
            SolicitudVacunaDTO solicitud = ObtenerSolicitudId(id).Result;
            return View(solicitud);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            bool eliminado = EliminarSolicitudVacunaAsync(id).Result;

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
