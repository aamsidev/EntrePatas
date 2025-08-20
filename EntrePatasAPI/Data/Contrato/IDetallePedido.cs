using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;

namespace EntrePatasAPI.Data.Contrato
{
    public interface IDetallePedido
    {


        List<DetallePedido> Listado();

        DetallePedido ObtenerDetallePedidoPorId(int id);

        DetallePedido Registrar(DetallePedidoDTO animal);

        DetallePedido Editar(int id, DetallePedidoDTO animal);

        bool Eliminar(int id);






    }
}
