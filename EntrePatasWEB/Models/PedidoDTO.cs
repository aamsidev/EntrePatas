namespace EntrePatasWEB.Models
{
    public class PedidoDTO
    {

        public int IdPedido { get; set; }
        public int IdUsuario { get; set; }

        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; }
        public decimal Total { get; set; }



    }
}
