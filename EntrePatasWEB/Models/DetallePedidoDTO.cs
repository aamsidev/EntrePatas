namespace EntrePatasWEB.Models
{
    public class DetallePedidoDTO
    {


        public int IdDetalle { get; set; }
        public int IdPedido { get; set; }

        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public ProductoDTO Producto { get; set; }






    }
}
