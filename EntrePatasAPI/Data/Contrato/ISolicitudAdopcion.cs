using EntrePatasAPI.Models.DTO;
using EntrePatasAPI.Models;

namespace EntrePatasAPI.Data.Contrato
{
    public interface ISolicitudAdopcion 
    {


        List<SolicitudAdopcion> Listado();

        SolicitudAdopcion ObtenerSolicitudAdopcionPorId(int id);

        SolicitudAdopcion Registrar(SolicitudAdopcionDTO solicitud);






    }
}
