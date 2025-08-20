using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntrePatasWEB.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IConfiguration _config;

        public ProductoController(IConfiguration config)
        {
            _config = config;
        }


        private async Task<List<ProductoDTO>> ObtenerListadoProductoAsync()
        {
            var listado = new List<ProductoDTO>();

            using (var clienteHttp = new HttpClient())
            {

                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var mensaje = await clienteHttp.GetAsync("Producto");

                var data = await mensaje.Content.ReadAsStringAsync();


                listado = JsonConvert.DeserializeObject<List<ProductoDTO>>(data);
            }
            return listado;
        }

        private async Task<ProductoDTO> ObtenerProductoId(int id)
        {
            var producto = new ProductoDTO();
            using (var clienteHTTP = new HttpClient())
            {
                clienteHTTP.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHTTP.GetAsync($"Producto/{id}");
                var data = await mensaje.Content.ReadAsStringAsync();
                producto = JsonConvert.DeserializeObject<ProductoDTO>(data);
            }
            return producto;
        }

        private async Task<ProductoDTO> RegistrarProducto(ProductoDTO producto)

        {
            var nuevoProducto = new ProductoDTO();
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);
                    StringContent contenido = new StringContent(JsonConvert.SerializeObject(producto), System.Text.Encoding.UTF8, "application/json");
                    var respuesta = await httpCliente.PostAsync("Producto/registrar", contenido);
                    var data = await respuesta.Content.ReadAsStringAsync();
                    nuevoProducto = JsonConvert.DeserializeObject<ProductoDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return nuevoProducto;
        }

        private async Task<ProductoDTO> UpdateProducto(int id, ProductoDTO producto)
        {
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);

                    var contenido = new StringContent(
                        JsonConvert.SerializeObject(producto),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    // Consumimos el endpoint PUT
                    var respuesta = await httpCliente.PutAsync($"Producto/update/{id}", contenido);

                    if (!respuesta.IsSuccessStatusCode)
                        return null;

                    var data = await respuesta.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ProductoDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        private async Task<bool> EliminarProductoAsync(int id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var response = await clienteHttp.DeleteAsync($"Producto/{id}");

                return response.IsSuccessStatusCode;
            }
        }








        public IActionResult Index()
        {
            var listado = ObtenerListadoProductoAsync().Result;
            return View(listado);
        }

        public IActionResult Details(int id)
        {

            ProductoDTO producto = ObtenerProductoId(id).Result;
            return View(producto);


        }

        public IActionResult Create()
        {
            return View(new ProductoDTO());
        }

        [HttpPost]
        public IActionResult Create(ProductoDTO producto)
        {
            ProductoDTO nuevoProducto = RegistrarProducto(producto).Result;


            return RedirectToAction("Details", new { id = nuevoProducto.IdProducto});
        }


        public IActionResult Edit(int id)
        {

            var Producto = ObtenerProductoId(id).Result;

            if (Producto == null)
                return NotFound();

            return View(Producto);

        }

        // POST: Usuario/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, ProductoDTO producto)
        {
            if (!ModelState.IsValid)
                return View(producto);

            var productoEditado = UpdateProducto(id, producto).Result;

            if (productoEditado == null)
            {
                ModelState.AddModelError("", "No se pudo actualizar el usuario");
                return View(producto);
            }

            return RedirectToAction("Index", new { id = productoEditado.IdProducto });
        }


        public IActionResult Delete(int id)
        {
            ProductoDTO producto = ObtenerProductoId(id).Result;
            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            bool eliminado = EliminarProductoAsync(id).Result;

            if (eliminado)
            {
                TempData["Mensaje"] = "Producto eliminado correctamente";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el producto";
            }

            return RedirectToAction("Index");
        }






    }
}
