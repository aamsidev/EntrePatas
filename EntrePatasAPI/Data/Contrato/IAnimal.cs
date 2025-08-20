using EntrePatasAPI.Models.DTO;
using EntrePatasAPI.Models;

namespace EntrePatasAPI.Data.Contrato
{
    public interface IAnimal
    {

        List<Animal> Listado();

        Animal ObtenerAnimalPorId(int id);

        Animal Registrar(AnimalDTO animal);

        Animal Editar(int id, AnimalDTO animal);

        bool Eliminar(int id);


    }
}
