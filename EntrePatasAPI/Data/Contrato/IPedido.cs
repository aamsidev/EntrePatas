using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;

namespace EntrePatasAPI.Data.Contrato
{
    public interface IPedido
    {

        List<Pedido> Listado();

        Pedido ObtenerPedidoPorId(int id);

        Pedido Registrar(PedidoDTO pedido);

        Pedido Editar(int id, PedidoDTO pedido);

        bool Eliminar(int id);





    }
}
