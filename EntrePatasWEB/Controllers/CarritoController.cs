using System.Text;
using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntrePatasWEB.Controllers
{
    public class CarritoController : Controller
    {
        private const string SessionKey = "Carrito";
        private readonly IConfiguration _config;

        public CarritoController(IConfiguration config)
        {
            _config = config;
        }

        // ================== MÉTODOS DE SESIÓN ==================
        private List<CarritoItem> GetCarrito()
        {
            var carrito = HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(carrito))
                return new List<CarritoItem>();
            return JsonConvert.DeserializeObject<List<CarritoItem>>(carrito);
        }

        private void SaveCarrito(List<CarritoItem> carrito)
        {
            HttpContext.Session.SetString(SessionKey, JsonConvert.SerializeObject(carrito));
        }

        // ================== ACCIONES ==================
        public IActionResult Index()
        {
            var carrito = GetCarrito();
            return View(carrito);
        }

        public IActionResult Agregar(int id, string nombre, decimal precio, string fotoUrl)
        {
            var carrito = GetCarrito();
            var item = carrito.FirstOrDefault(c => c.IdProducto == id);

            if (item != null)
                item.Cantidad++;
            else
                carrito.Add(new CarritoItem
                {
                    IdProducto = id,
                    Nombre = nombre,
                    Precio = precio,
                    FotoUrl = fotoUrl,
                    Cantidad = 1
                });

            SaveCarrito(carrito);
            return RedirectToAction("Index");
        }

        public IActionResult Eliminar(int id)
        {
            var carrito = GetCarrito();
            var item = carrito.FirstOrDefault(c => c.IdProducto == id);
            if (item != null)
                carrito.Remove(item);

            SaveCarrito(carrito);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Actualizar(int id, int cantidad)
        {
            var carrito = GetCarrito();
            var item = carrito.FirstOrDefault(c => c.IdProducto == id);
            if (item != null)
                item.Cantidad = cantidad;

            SaveCarrito(carrito);
            return RedirectToAction("Index");
        }

        public IActionResult MiniCarrito()
        {
            var carrito = GetCarrito();
            return PartialView("_MiniCarrito", carrito);
        }

        // ✅ NUEVA ACCIÓN VACIAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Vaciar()
        {
            HttpContext.Session.Remove(SessionKey); // limpia todo el carrito
            TempData["Mensaje"] = "El carrito fue vaciado correctamente.";
            return RedirectToAction("Index");
        }

        // ================== CHECKOUT ==================
        public IActionResult Checkout()
        {
            var usuarioId = HttpContext.Session.GetInt32("IdUsuario");
            if (usuarioId == null)
                return RedirectToAction("Login", "PaginaPrincipal");

            var carrito = GetCarrito();
            if (carrito == null || !carrito.Any())
                return RedirectToAction("Index", "Carrito");

            var total = carrito.Sum(x => (decimal)x.Precio * x.Cantidad);

            var viewModel = new CheckoutViewModel
            {
                Pedido = new PedidoDTO
                {
                    IdUsuario = usuarioId.Value,
                    FechaPedido = DateTime.Now,
                    Estado = "Pendiente",
                    Total = total
                },
                Envio = new EnvioDTO(),
                Pago = new PagoDTO
                {
                    Monto = total,
                    EstadoPago = "Pendiente"
                },
                Detalles = carrito.Select(x => new DetallePedidoDTO
                {
                    IdProducto = x.IdProducto,
                    Producto = new ProductoDTO
                    {
                        IdProducto = x.IdProducto,
                        Nombre = x.Nombre,
                        Precio = x.Precio,
                        FotoUrl = x.FotoUrl
                    },
                    Cantidad = x.Cantidad,
                    PrecioUnitario = (decimal)x.Precio
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel viewModel)
        {
            var usuarioId = HttpContext.Session.GetInt32("IdUsuario");
            if (usuarioId == null)
                return RedirectToAction("Login", "PaginaPrincipal");

            var carrito = GetCarrito();
            if (carrito == null || !carrito.Any())
                return RedirectToAction("Index", "Carrito");

            var total = carrito.Sum(x => (decimal)x.Precio * x.Cantidad);

            viewModel.Pedido.IdUsuario = usuarioId.Value;
            viewModel.Pedido.FechaPedido = DateTime.Now;
            viewModel.Pedido.Estado = "Pendiente";
            viewModel.Pedido.Total = total;

            viewModel.Pago.Monto = total;
            viewModel.Pago.EstadoPago = "Pendiente";

            if (string.IsNullOrEmpty(viewModel.Envio.DireccionEnvio))
            {
                ModelState.AddModelError("Envio.DireccionEnvio", "La dirección de envío es obligatoria.");
                return View(viewModel);
            }

            viewModel.Envio.EstadoEnvio = "Pendiente";

            using (var httpCliente = new HttpClient())
            {
                httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);

                // Registrar Pedido
                var pedidoJson = JsonConvert.SerializeObject(viewModel.Pedido);
                var pedidoResp = await httpCliente.PostAsync("Pedido/Registrar",
                    new StringContent(pedidoJson, Encoding.UTF8, "application/json"));

                if (!pedidoResp.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Error registrando el pedido";
                    return View(viewModel);
                }

                var pedidoData = await pedidoResp.Content.ReadAsStringAsync();
                var pedidoGuardado = JsonConvert.DeserializeObject<PedidoDTO>(pedidoData);
                var idPedido = pedidoGuardado.IdPedido;

                // Registrar Detalles
                foreach (var detalle in carrito)
                {
                    var detalleDTO = new DetallePedidoDTO
                    {
                        IdPedido = idPedido,
                        IdProducto = detalle.IdProducto,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = (decimal)detalle.Precio
                    };
                    await httpCliente.PostAsync("DetallePedido/Registrar",
                        new StringContent(JsonConvert.SerializeObject(detalleDTO), Encoding.UTF8, "application/json"));
                }

                // Registrar Envío
                viewModel.Envio.IdPedido = idPedido;
                var envioJson = JsonConvert.SerializeObject(viewModel.Envio);
                var envioResp = await httpCliente.PostAsync("Envio/Registrar",
                    new StringContent(envioJson, Encoding.UTF8, "application/json"));

                if (!envioResp.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Error registrando el envío";
                    return View(viewModel);
                }

                // Registrar Pago
                viewModel.Pago.IdPedido = idPedido;
                var pagoJson = JsonConvert.SerializeObject(viewModel.Pago);
                await httpCliente.PostAsync("Pago/Registrar",
                    new StringContent(pagoJson, Encoding.UTF8, "application/json"));
            }

            // ✅ Vaciar carrito después de comprar
            HttpContext.Session.Remove(SessionKey);

            return RedirectToAction("Index", "Home");
        }
    }
}
