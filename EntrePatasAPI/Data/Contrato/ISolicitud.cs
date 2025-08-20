using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;

namespace EntrePatasAPI.Data.Contrato
{
    public interface ISolicitud
    {

        List<Solicitud> Listado();

        Solicitud ObtenerSolicitudPorId(int id);

        Solicitud Registrar(SolicitudDTO solicitud);

        Solicitud Editar(int id, SolicitudDTO solicitud);

        bool Eliminar(int id);



    }
}
