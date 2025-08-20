using EntrePatasAPI.Models.DTO;
using EntrePatasAPI.Models;

namespace EntrePatasAPI.Data.Contrato
{
    public interface ISolicitudVacuna 
    {

        List<SolicitudVacuna> Listado();

        SolicitudVacuna ObtenerSolicitudVacunaPorId(int id);

        SolicitudVacuna Registrar(SolicitudVacunaDTO solicitudVacu);


        SolicitudVacuna Editar(int id, SolicitudVacunaDTO solicitudVacu);

        bool Eliminar(int id);




    }
}
