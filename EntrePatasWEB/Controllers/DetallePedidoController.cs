using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntrePatasWEB.Controllers
{
    public class DetallePedidoController : Controller
    {
        private readonly IConfiguration _config;

        public DetallePedidoController(IConfiguration config)
        {
            _config = config;
        }


        private async Task<List<DetallePedidoDTO>> ObtenerListadoDetallePedidoAsync()
        {
            var listado = new List<DetallePedidoDTO>();

            using (var clienteHttp = new HttpClient())
            {

                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var mensaje = await clienteHttp.GetAsync("DetallePedido");

                var data = await mensaje.Content.ReadAsStringAsync();


                listado = JsonConvert.DeserializeObject<List<DetallePedidoDTO>>(data);
            }
            return listado;
        }


        private async Task<DetallePedidoDTO> ObtenerDetallePedidoId(int id)
        {
            var detalle = new DetallePedidoDTO();
            using (var clienteHTTP = new HttpClient())
            {
                clienteHTTP.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHTTP.GetAsync($"DetallePedido/{id}");
                var data = await mensaje.Content.ReadAsStringAsync();
                detalle = JsonConvert.DeserializeObject<DetallePedidoDTO>(data);
            }
            return detalle;
        }



        private async Task<DetallePedidoDTO> RegistrarDetallePedido(DetallePedidoDTO detalle)

        {
            var nuevoDetalle = new DetallePedidoDTO();
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);
                    StringContent contenido = new StringContent(JsonConvert.SerializeObject(detalle), System.Text.Encoding.UTF8, "application/json");
                    var respuesta = await httpCliente.PostAsync("DetallePedido/registrar", contenido);
                    var data = await respuesta.Content.ReadAsStringAsync();
                    nuevoDetalle = JsonConvert.DeserializeObject<DetallePedidoDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return nuevoDetalle;
        }




        private async Task<DetallePedidoDTO> UpdateDetallePedido(int id, DetallePedidoDTO detalle)
        {
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);

                    var contenido = new StringContent(
                        JsonConvert.SerializeObject(detalle),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    var respuesta = await httpCliente.PutAsync($"DetallePedido/update/{id}", contenido);

                    if (!respuesta.IsSuccessStatusCode)
                        return null;

                    var data = await respuesta.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<DetallePedidoDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        private async Task<bool> EliminarDetallePedidoAsync(int id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var response = await clienteHttp.DeleteAsync($"DetallePedido/{id}");

                return response.IsSuccessStatusCode;
            }
        }






        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("TipoUsuario");

            if (rol != "Administrador")
                return RedirectToAction("Error", "Home");

            var listado = ObtenerListadoDetallePedidoAsync().Result;
            return View(listado);
        }

        public IActionResult Details(int id)
        {
            var rol = HttpContext.Session.GetString("TipoUsuario");

            if (rol != "Administrador")
                return RedirectToAction("Error", "Home");

            DetallePedidoDTO detalle = ObtenerDetallePedidoId(id).Result;
            return View(detalle);


        }

        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetString("TipoUsuario");

            if (rol != "Administrador")
                return RedirectToAction("Error", "Home");

            return View(new DetallePedidoDTO());
        }

        [HttpPost]
        public IActionResult Create(DetallePedidoDTO detalle)
        {
            DetallePedidoDTO nuevoDetalle = RegistrarDetallePedido(detalle).Result;


            return RedirectToAction("Details", new { id = nuevoDetalle.IdDetalle});
        }


        public IActionResult Edit(int id)
        {

            var Detalle = ObtenerDetallePedidoId(id).Result;

            if (Detalle == null)
                return NotFound();

            return View(Detalle);

        }

        [HttpPost]
        public IActionResult Edit(int id, DetallePedidoDTO detalle)
        {
            if (!ModelState.IsValid)
                return View(detalle);

            var detalleEditado = UpdateDetallePedido(id, detalle).Result;

            if (detalleEditado == null)
            {
                ModelState.AddModelError("", "No se pudo actualizar el usuario");
                return View(detalle);
            }

            return RedirectToAction("Index", new { id = detalleEditado.IdDetalle});
        }

        public IActionResult Delete(int id)
        {
            var rol = HttpContext.Session.GetString("TipoUsuario");

            if (rol != "Administrador")
                return RedirectToAction("Error", "Home");

            DetallePedidoDTO detalle = ObtenerDetallePedidoId(id).Result;
            return View(detalle);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            bool eliminado = EliminarDetallePedidoAsync(id).Result;

            if (eliminado)
            {
                TempData["Mensaje"] = "Detalle eliminado correctamente";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el detalle";
            }

            return RedirectToAction("Index");
        }




    }
}
