using System.Text;
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
                    httpCliente.BaseAddress = new Uri(_config["Services:URL_API"]);
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
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_config["Services:URL_API"]);
                var response = await client.PutAsJsonAsync($"Producto/update/{id}", producto);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ProductoDTO>();
                }
            }
            return null;
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

        public IActionResult Catalogo()
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
        public async Task<IActionResult> Create(ProductoDTO producto)
        {
            try
            {
                // 👉 Procesar la foto si se sube
                if (producto.FotoFile != null && producto.FotoFile.Length > 0)
                {
                    // Tomar el nombre original del archivo
                    var originalFileName = Path.GetFileName(producto.FotoFile.FileName);

                    // Crear un nombre único (evita duplicados y bloqueos)
                    var uniqueFileName = $"{Path.GetFileNameWithoutExtension(originalFileName)}_{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";

                    // Ruta absoluta de la carpeta donde quieres guardar
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes", "producto");

                    // Crear la carpeta si no existe
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Ruta completa del archivo
                    var filePath = Path.Combine(folderPath, uniqueFileName);

                    // Guardar la foto en disco
                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await producto.FotoFile.CopyToAsync(stream);
                    }

                    // Guardar el nombre único en la BD
                    producto.FotoUrl = uniqueFileName;
                }

                // 👉 Llamar a tu API/servicio para registrar el producto
                var nuevoProducto = await RegistrarProducto(producto);

                if (nuevoProducto != null)
                {
                    TempData["Mensaje"] = "Producto registrado correctamente";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "No se pudo registrar el producto.");
                return View(producto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al registrar el producto.");
                Console.WriteLine("❌ ERROR EN CREATE: " + ex.ToString()); // log completo en consola
                return View(producto);
            }
        }





        public IActionResult Edit(int id)
        {

            var rol = HttpContext.Session.GetString("TipoUsuario");

            if (rol != "Administrador")
                return RedirectToAction("Error", "Home");

            var Producto = ObtenerProductoId(id).Result;

            if (Producto == null)
                return NotFound();

            return View(Producto);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductoDTO producto)
        {
            if (!ModelState.IsValid)
                return View(producto);

            // Obtener producto existente
            var productoExistente = await ObtenerProductoId(id);
            if (productoExistente == null)
            {
                ModelState.AddModelError("", "Producto no encontrado");
                return View(producto);
            }

            // --- Manejo de la foto ---
            if (producto.FotoFile != null && producto.FotoFile.Length > 0)
            {
                var extension = Path.GetExtension(producto.FotoFile.FileName).ToLower();
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("", "Formato de imagen no válido.");
                    return View(producto);
                }

                // Eliminar foto anterior si existía
                if (!string.IsNullOrEmpty(productoExistente.FotoUrl))
                {
                    var rutaAnterior = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/imagenes/producto",
                        productoExistente.FotoUrl
                    );

                    if (System.IO.File.Exists(rutaAnterior))
                        System.IO.File.Delete(rutaAnterior);
                }

                // Guardar nueva foto (se genera nombre único con GUID)
                var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                var ruta = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/imagenes/producto",
                    nombreArchivo
                );

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await producto.FotoFile.CopyToAsync(stream);
                }

                producto.FotoUrl = nombreArchivo;
            }
            else
            {
                // Mantener la foto existente si no se sube nueva
                producto.FotoUrl = productoExistente.FotoUrl;
            }

            // --- Preparar datos para actualizar ---
            var productoParaActualizar = new ProductoDTO
            {
                IdProducto = productoExistente.IdProducto,
                Nombre = !string.IsNullOrWhiteSpace(producto.Nombre)
                            ? producto.Nombre
                            : productoExistente.Nombre,
                Descripcion = !string.IsNullOrWhiteSpace(producto.Descripcion)
                            ? producto.Descripcion
                            : productoExistente.Descripcion,
                Precio = producto.Precio != 0
                            ? producto.Precio
                            : productoExistente.Precio,
                Stock = producto.Stock,
                FotoUrl = producto.FotoUrl
            };

            // --- Llamar al API ---
            var resultado = await UpdateProducto(id, productoParaActualizar);
            if (resultado == null)
            {
                ModelState.AddModelError("", "No se pudo actualizar el producto.");
                return View(producto);
            }

            TempData["Mensaje"] = "Producto actualizado correctamente";
            return RedirectToAction("Index");
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


        public IActionResult _ProductoFiltro(string nombre, decimal? precioMin, decimal? precioMax)
        {
            var productos = ObtenerListadoProductoAsync().Result;

            if (!string.IsNullOrEmpty(nombre))
                productos = productos
                    .Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (precioMin.HasValue)
                productos = productos.Where(p => p.Precio >= precioMin.Value).ToList();

            if (precioMax.HasValue)
                productos = productos.Where(p => p.Precio <= precioMax.Value).ToList();

            return PartialView("_ProductoFiltro", productos);
        }



    }
}
