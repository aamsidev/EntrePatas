namespace EntrePatasWEB.Models
{
    public class PagoDTO
    {

        public int IdPago { get; set; }
        public int IdPedido { get; set; }
        public DateTime FechaPago { get; set; }

        public Decimal Monto { get; set; }

        public string MetodoPago { get; set; }

        public string EstadoPago { get; set; }











    }
}
