using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntrePatasWEB.Controllers
{
    public class PagoController : Controller
    {
        private readonly IConfiguration _config;

        public PagoController(IConfiguration config)
        {
            _config = config;
        }


        private async Task<List<PagoDTO>> ObtenerListadoPagoAsync()
        {
            var listado = new List<PagoDTO>();

            using (var clienteHttp = new HttpClient())
            {

                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var mensaje = await clienteHttp.GetAsync("Pago");

                var data = await mensaje.Content.ReadAsStringAsync();


                listado = JsonConvert.DeserializeObject<List<PagoDTO>>(data);
            }
            return listado;
        }



        private async Task<PagoDTO> ObtenerPagoId(int id)
        {
            var pago = new PagoDTO();
            using (var clienteHTTP = new HttpClient())
            {
                clienteHTTP.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHTTP.GetAsync($"Pago/{id}");
                var data = await mensaje.Content.ReadAsStringAsync();
                pago = JsonConvert.DeserializeObject<PagoDTO>(data);
            }
            return pago;
        }

        private async Task<PagoDTO> RegistrarPago(PagoDTO pago)

        {
            var nuevoPago = new PagoDTO();
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);
                    StringContent contenido = new StringContent(JsonConvert.SerializeObject(pago), System.Text.Encoding.UTF8, "application/json");
                    var respuesta = await httpCliente.PostAsync("Pago/registrar", contenido);
                    var data = await respuesta.Content.ReadAsStringAsync();
                    nuevoPago = JsonConvert.DeserializeObject<PagoDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return nuevoPago;
        }


        private async Task<PagoDTO> UpdatePago(int id, PagoDTO pago)
        {
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);

                    var contenido = new StringContent(
                        JsonConvert.SerializeObject(pago),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    var respuesta = await httpCliente.PutAsync($"Pago/update/{id}", contenido);

                    if (!respuesta.IsSuccessStatusCode)
                        return null;

                    var data = await respuesta.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<PagoDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        private async Task<bool> EliminarPagoAsync(int id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var response = await clienteHttp.DeleteAsync($"pAGO/{id}");

                return response.IsSuccessStatusCode;
            }
        }



        public IActionResult Index()
        {
            var listado = ObtenerListadoPagoAsync().Result;
            return View(listado);
        }

        public IActionResult Details(int id)
        {

            PagoDTO pago = ObtenerPagoId(id).Result;
            return View(pago);


        }


        public IActionResult Create()
        {
            return View(new PagoDTO());
        }

        [HttpPost]
        public IActionResult Create(PagoDTO pago)
        {
            PagoDTO nuevoPago = RegistrarPago(pago).Result;


            return RedirectToAction("Details", new { id = nuevoPago.IdPago });
        }

        public IActionResult Edit(int id)
        {

            var Pago = ObtenerPagoId(id).Result;

            if (Pago == null)
                return NotFound();

            return View(Pago);

        }

        [HttpPost]
        public IActionResult Edit(int id, PagoDTO pago)
        {
            if (!ModelState.IsValid)
                return View(pago);

            var pagoEditado = UpdatePago(id, pago).Result;

            if (pagoEditado == null)
            {
                ModelState.AddModelError("", "No se pudo actualizar el usuario");
                return View(pago);
            }

            return RedirectToAction("Index", new { id = pagoEditado.IdPago });
        }

        public IActionResult Delete(int id)
        {
            PagoDTO pago = ObtenerPagoId(id).Result;
            return View(pago);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            bool eliminado = EliminarPagoAsync(id).Result;

            if (eliminado)
            {
                TempData["Mensaje"] = "Pago eliminado correctamente";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el pago";
            }

            return RedirectToAction("Index");
        }






    }
}
