using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntrePatasWEB.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IConfiguration _config;

        public PedidoController(IConfiguration config)
        {
            _config = config;
        }

        private async Task<List<PedidoDTO>> ObtenerListadoPedidoAsync()
        {
            var listado = new List<PedidoDTO>();

            using (var clienteHttp = new HttpClient())
            {

                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var mensaje = await clienteHttp.GetAsync("Pedido");

                var data = await mensaje.Content.ReadAsStringAsync();


                listado = JsonConvert.DeserializeObject<List<PedidoDTO>>(data);
            }
            return listado;
        }



        private async Task<PedidoDTO> ObtenerPedidoId(int id)
        {
            var pedido = new PedidoDTO();
            using (var clienteHTTP = new HttpClient())
            {
                clienteHTTP.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHTTP.GetAsync($"Pedido/{id}");
                var data = await mensaje.Content.ReadAsStringAsync();
                pedido = JsonConvert.DeserializeObject<PedidoDTO>(data);
            }
            return pedido;
        }



        private async Task<PedidoDTO> RegistrarPedido(PedidoDTO pedido)

        {
            var nuevoPedido = new PedidoDTO();
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);
                    StringContent contenido = new StringContent(JsonConvert.SerializeObject(pedido), System.Text.Encoding.UTF8, "application/json");
                    var respuesta = await httpCliente.PostAsync("Pedido/registrar", contenido);
                    var data = await respuesta.Content.ReadAsStringAsync();
                    nuevoPedido = JsonConvert.DeserializeObject<PedidoDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return nuevoPedido;
        }

        private async Task<PedidoDTO> UpdatePedido(int id, PedidoDTO pedido)
        {
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);

                    var contenido = new StringContent(
                        JsonConvert.SerializeObject(pedido),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    var respuesta = await httpCliente.PutAsync($"Pedido/update/{id}", contenido);

                    if (!respuesta.IsSuccessStatusCode)
                        return null;

                    var data = await respuesta.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<PedidoDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        private async Task<bool> EliminarPedidoAsync(int id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var response = await clienteHttp.DeleteAsync($"Pedido/{id}");

                return response.IsSuccessStatusCode;
            }
        }



        public IActionResult Index()
        {
            var listado = ObtenerListadoPedidoAsync().Result;
            return View(listado);
        }

        public IActionResult Details(int id)
        {

            PedidoDTO pedido = ObtenerPedidoId(id).Result;
            return View(pedido);


        }


        public IActionResult Create()
        {
            return View(new PedidoDTO());
        }

        [HttpPost]
        public IActionResult Create(PedidoDTO pedido)
        {
            PedidoDTO nuevoPedido = RegistrarPedido(pedido).Result;


            return RedirectToAction("Details", new { id = nuevoPedido.IdPedido });
        }



        public IActionResult Edit(int id)
        {

            var Pedido = ObtenerPedidoId(id).Result;

            if (Pedido == null)
                return NotFound();

            return View(Pedido);

        }

        [HttpPost]
        public IActionResult Edit(int id, PedidoDTO pedido)
        {
            if (!ModelState.IsValid)
                return View(pedido);

            var pedidoEditado = UpdatePedido(id, pedido).Result;

            if (pedidoEditado == null)
            {
                ModelState.AddModelError("", "No se pudo actualizar el usuario");
                return View(pedido);
            }

            return RedirectToAction("Index", new { id = pedidoEditado.IdPedido });
        }


        public IActionResult Delete(int id)
        {
            PedidoDTO pedido = ObtenerPedidoId(id).Result;
            return View(pedido);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            bool eliminado = EliminarPedidoAsync(id).Result;

            if (eliminado)
            {
                TempData["Mensaje"] = "Animal eliminado correctamente";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el animal";
            }

            return RedirectToAction("Index");
        }

        private async Task<UsuarioDTO> ObtenerUsuarioPorId(int id)
        {
            var usuario = new UsuarioDTO();
            using (var clienteHTTP = new HttpClient())
            {
                clienteHTTP.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHTTP.GetAsync($"Usuario/{id}");
                var data = await mensaje.Content.ReadAsStringAsync();
                usuario = JsonConvert.DeserializeObject<UsuarioDTO>(data);
            }
            return usuario;
        }
        private async Task<List<DetallePedidoDTO>> ObtenerDetallesDePedido(int idPedido)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                try
                {
                    var resp = await clienteHttp.GetAsync($"DetallePedido/porPedido/{idPedido}");
                    if (!resp.IsSuccessStatusCode)
                        return new List<DetallePedidoDTO>(); 

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json) || json == "null")
                        return new List<DetallePedidoDTO>();

                    var lista = JsonConvert.DeserializeObject<List<DetallePedidoDTO>>(json);
                    return lista ?? new List<DetallePedidoDTO>();
                }
                catch
                {
                    return new List<DetallePedidoDTO>();
                }
            }
        }

        private async Task<ProductoDTO> ObtenerProductoPorId(int idProducto)
        {
            var producto = new ProductoDTO();
            using (var clienteHTTP = new HttpClient())
            {
                clienteHTTP.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHTTP.GetAsync($"Producto/{idProducto}");
                var data = await mensaje.Content.ReadAsStringAsync();
                producto = JsonConvert.DeserializeObject<ProductoDTO>(data);
            }
            return producto;
        }


        public async Task<IActionResult> HistorialCliente()
        {
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
                return RedirectToAction("Login", "PaginaPrincipal"); // tu login está aquí

            var cliente = await ObtenerUsuarioPorId(idUsuario.Value);

            var pedidos = await ObtenerListadoPedidoAsync() ?? new List<PedidoDTO>();
            var pedidosCliente = pedidos.Where(p => p != null && p.IdUsuario == idUsuario.Value).ToList();

            var historial = new HistorialPedidosClienteVM
            {
                Cliente = cliente,
                Pedidos = new List<PedidoDetallesViewModel>()
            };

            foreach (var pedido in pedidosCliente)
            {
                var detalles = await ObtenerDetallesDePedido(pedido.IdPedido); // ya nunca es null

                // Si no hay detalles, igual agregamos el pedido con lista vacía
                var detallesConProducto = new List<DetallePedidoConProductoVM>();
                foreach (var det in detalles)
                {
                    var producto = await ObtenerProductoPorId(det.IdProducto) ?? new ProductoDTO();
                    detallesConProducto.Add(new DetallePedidoConProductoVM
                    {
                        Detalle = det,
                        Producto = producto
                    });
                }

                historial.Pedidos.Add(new PedidoDetallesViewModel
                {
                    Pedido = pedido,
                    Detalles = detallesConProducto
                });
            }

            return View(historial);
        }






    }
}
