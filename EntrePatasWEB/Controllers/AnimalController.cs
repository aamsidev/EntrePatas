using EntrePatasWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntrePatasWEB.Controllers
{
    public class AnimalController : Controller
    {
        private readonly IConfiguration _config;

        public AnimalController(IConfiguration config)
        {
            _config = config;
        }
        private async Task<List<AnimalDTO>> ObtenerListadoAnimalAsync()
        {
            var listado = new List<AnimalDTO>();
    
            using (var clienteHttp = new HttpClient())
            {
       
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var mensaje = await clienteHttp.GetAsync("Animal");

                var data = await mensaje.Content.ReadAsStringAsync();

        
                listado = JsonConvert.DeserializeObject<List<AnimalDTO>>(data);
            }
            return listado;
        }
        private async Task<AnimalDTO> ObtenerAnimalId(int id)
        {
            var animal = new AnimalDTO();
            using (var clienteHTTP = new HttpClient())
            {
                clienteHTTP.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHTTP.GetAsync($"Animal/{id}");
                var data = await mensaje.Content.ReadAsStringAsync();
                animal = JsonConvert.DeserializeObject<AnimalDTO>(data);
            }
            return animal;
        }
        private async Task<AnimalDTO> RegistrarAnimal(AnimalDTO animal)

        {
            var nuevoAnimal = new AnimalDTO();
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);
                    StringContent contenido = new StringContent(JsonConvert.SerializeObject(animal), System.Text.Encoding.UTF8, "application/json");
                    var respuesta = await httpCliente.PostAsync("Animal/registrar", contenido);
                    var data = await respuesta.Content.ReadAsStringAsync();
                    nuevoAnimal = JsonConvert.DeserializeObject<AnimalDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return nuevoAnimal;
        }


        private async Task<AnimalDTO> UpdateAnimal(int id, AnimalDTO animal)
        {
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_config["Services:Url_API"]);

                    var contenido = new StringContent(
                        JsonConvert.SerializeObject(animal),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    var respuesta = await httpCliente.PutAsync($"Animal/update/{id}", contenido);

                    if (!respuesta.IsSuccessStatusCode)
                        return null;

                    var data = await respuesta.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<AnimalDTO>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        private async Task<bool> EliminarAnimalAsync(int id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                var response = await clienteHttp.DeleteAsync($"Animal/{id}");

                return response.IsSuccessStatusCode;
            }
        }

        public async Task<IActionResult> Reporte(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var rol = HttpContext.Session.GetString("TipoUsuario");

            if (rol != "Administrador")
                return RedirectToAction("Error", "Home");

            var animales = await ObtenerListadoAnimalAsync();

            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                animales = animales
                    .Where(a => a.FechaRegistro.Date >= fechaInicio.Value.Date
                             && a.FechaRegistro.Date <= fechaFin.Value.Date)
                    .ToList();
            }

            return View(animales);
        }


        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("TipoUsuario");

            if (rol != "Administrador")
                return RedirectToAction("Error", "Home");

            var listado = ObtenerListadoAnimalAsync().Result;
            return View(listado);
        }

        public async Task<IActionResult> Adopcion()
        {
            var nombre = HttpContext.Session.GetString("NombreUsuario");
            var listado = await ObtenerListadoAnimalAsync();

            ViewBag.NombreUsuario = nombre;
            ViewBag.Estados = new List<string> { "Disponible", "Adoptado", "Reservado" };
            ViewBag.Edades = listado.Select(a => a.Edad).Distinct().OrderBy(e => e).ToList();
            ViewBag.Razas = listado.Select(a => a.Raza).Where(r => !string.IsNullOrEmpty(r)).Distinct().OrderBy(r => r).ToList();

            return View(listado);
        }

        public IActionResult Details(int id)
        {

            AnimalDTO animal = ObtenerAnimalId(id).Result;
            return View(animal);


        }

        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetString("TipoUsuario");

            if (rol != "Administrador")
                return RedirectToAction("Error", "Home");

            return View(new AnimalDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(AnimalDTO animal)
        {
            // 👉 Validar sesión de usuario
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
                return RedirectToAction("Login", "Usuario");

            animal.IdUsuario = idUsuario.Value;

            // 👉 Procesar la foto
            if (animal.FotoFile != null && animal.FotoFile.Length > 0)
            {
                // Tomar el nombre original del archivo
                var fileName = Path.GetFileName(animal.FotoFile.FileName);

                // Ruta absoluta de la carpeta donde quieres guardar
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes", "animal");

                // 👉 Crear la carpeta si no existe
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Ruta completa donde se guardará la foto
                var filePath = Path.Combine(folderPath, fileName);

                // ❗ Si ya existe un archivo con ese nombre, lo sobrescribe
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await animal.FotoFile.CopyToAsync(stream);
                }

                // ✅ Guardar solo el nombre en la BD
                animal.Foto = fileName;
            }

            // 👉 Llamar a tu API/servicio para registrar
            var nuevo = await RegistrarAnimal(animal);

            if (nuevo != null)
            {
                TempData["Mensaje"] = "Animal registrado correctamente";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "No se pudo registrar el animal.");
            return View(animal);
        }










        public IActionResult Edit(int id)
        {
            var rol = HttpContext.Session.GetString("TipoUsuario");

            if (rol != "Administrador")
                return RedirectToAction("Error", "Home");

            var Animal = ObtenerAnimalId(id).Result;

            if (Animal == null)
                return NotFound();

            return View(Animal);

        }

        [HttpPost]
        public IActionResult Edit(int id, AnimalDTO animal)
        {
            if (!ModelState.IsValid)
                return View(animal);

            var animalEditado = UpdateAnimal(id, animal).Result;

            if (animalEditado == null)
            {
                ModelState.AddModelError("", "No se pudo actualizar el usuario");
                return View(animal);
            }

            return RedirectToAction("Index", new { id = animalEditado.IdAnimal });
        }


        public IActionResult Delete(int id)
        {
            var rol = HttpContext.Session.GetString("TipoUsuario");

            if (rol != "Administrador")
                return RedirectToAction("Error", "Home");

            AnimalDTO animal = ObtenerAnimalId(id).Result;
            return View(animal);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            bool eliminado = EliminarAnimalAsync(id).Result;

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

    }
}
