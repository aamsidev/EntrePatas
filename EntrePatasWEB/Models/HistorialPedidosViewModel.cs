namespace EntrePatasWEB.Models
{
    public class HistorialPedidosClienteVM
    {
        public UsuarioDTO Cliente { get; set; } 
        public List<PedidoDetallesViewModel> Pedidos { get; set; }
    }
}
