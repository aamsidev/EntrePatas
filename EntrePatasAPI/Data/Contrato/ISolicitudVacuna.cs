using EntrePatasAPI.Models.DTO;
using EntrePatasAPI.Models;

namespace EntrePatasAPI.Data.Contrato
{
    public interface ISolicitudVacuna 
    {

        List<SolicitudVacuna> Listado();

        SolicitudVacuna ObtenerSolicitudVacunaPorId(int id);

        SolicitudVacuna Registrar(SolicitudVacunaDTO solicitud);






    }
}
