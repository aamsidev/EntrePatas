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
        private async Task<List<AnimalDTO>> obtenerListadoAnimalAsync()
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
        private async Task<AnimalDTO> obtenerAnimalPorID(int id)
        {
            var animal = new AnimalDTO();
            using (var clienteHTTP = new HttpClient())
            {
                clienteHTTP.BaseAddress = new Uri(_config["Services:URL_API"]);
                var mensaje = await clienteHTTP.GetAsync($"animal/{id}");
                var data = await mensaje.Content.ReadAsStringAsync();
                animal = JsonConvert.DeserializeObject<AnimalDTO>(data);
            }
            return animal;
        }
        private async Task<AnimalDTO> registrarAnimal(AnimalDTO animal)

        {
            AnimalDTO nuevoAnimal;
            using (var clienteHTTP = new HttpClient())
            {
                clienteHTTP.BaseAddress = new Uri(_config["Services:URL_API"]);
                StringContent contenido = new StringContent(JsonConvert.SerializeObject(animal),
                    System.Text.Encoding.UTF8, "application/json");
                var mensaje = await clienteHTTP.PostAsync("animal", contenido);
                var data = await mensaje.Content.ReadAsStringAsync();
                nuevoAnimal = JsonConvert.DeserializeObject<AnimalDTO>(data);
            }
            return nuevoAnimal;
        }

        public IActionResult Index()
        {
            var listado = obtenerListadoAnimalAsync().Result;
            return View(listado);
        }

        public IActionResult Create()
        {
            return View(new AnimalDTO());
        }

        [HttpPost]
        public IActionResult Create(AnimalDTO animal)
        {
            AnimalDTO nuevoAnimal = registrarAnimal(animal).Result;

           
            return RedirectToAction("Details", new { id = nuevoAnimal.ID });
        }
    }
}
