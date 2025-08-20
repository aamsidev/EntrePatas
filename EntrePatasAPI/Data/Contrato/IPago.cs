using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;

namespace EntrePatasAPI.Data.Contrato
{
    public interface IPago
    {


        List<Pago> Listado();

        Pago ObtenerPagoPorId(int id);

        Pago Registrar(PagoDTO pago);

        Pago Editar(int id, PagoDTO pago);

        bool Eliminar(int id);











    }
}
