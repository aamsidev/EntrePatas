using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntrePatasWEB.Controllers
{
    public class CarritoController : Controller
    {
        private const string SessionKey = "Carrito";

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

    }
}
