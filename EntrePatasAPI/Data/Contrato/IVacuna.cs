using EntrePatasAPI.Models.DTO;
using EntrePatasAPI.Models;

namespace EntrePatasAPI.Data.Contrato
{
    public interface IVacuna
    {

       
        List<Vacuna> Listado();


        Vacuna ObtenerVacunaPorId(int id);


        Vacuna Registrar(VacunaDTO vacuna);

        Vacuna Actualizar(int id, VacunaDTO vacuna);

        bool Eliminar(int id);




    }
}
