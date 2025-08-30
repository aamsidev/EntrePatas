namespace EntrePatasWEB.Models
{
    public class CheckoutViewModel
    {


        public PedidoDTO Pedido { get; set; }
        public EnvioDTO Envio { get; set; }
        public PagoDTO Pago { get; set; }
        public List<DetallePedidoDTO> Detalles { get; set; } = new List<DetallePedidoDTO>();







    }
}
