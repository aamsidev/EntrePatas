namespace EntrePatasWEB.Models
{
    public class PedidoDetallesViewModel
    {
        public PedidoDTO Pedido { get; set; }
        public List<DetallePedidoConProductoVM> Detalles { get; set; }
    }

}
