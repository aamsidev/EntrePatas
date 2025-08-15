using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntrePatasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly IAnimal animalDATA;

        public AnimalController(IAnimal animal)
        {

            animalDATA = animal;


        }

        [HttpGet]
        public IActionResult ListarAnimales()
        {

            return Ok(animalDATA.Listado());

        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var animal = animalDATA.ObtenerAnimalPorId(id);
            if (animal == null)
                return NotFound();

            return Ok(animal);
        }

        [HttpPost("Registrar")]
        public IActionResult Registrar(AnimalDTO animal)
        {
            var nuevoAnimal = animalDATA.Registrar(animal);
            if (nuevoAnimal == null)
                return NotFound();
            return Ok(nuevoAnimal);
        }


    }
}
