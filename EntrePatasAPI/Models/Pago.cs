using EntrePatasAPI.Data.Contrato;
using System;
using System.Reflection.Metadata.Ecma335;

namespace EntrePatasAPI.Models
{
    public class Pago
    {
        public int IdPago { get; set; }
        public int IdPedido { get; set; }
        public DateTime FechaPago { get; set; }

        public Decimal Monto { get; set; }

        public string MetodoPago { get; set; }  

        public string EstadoPago { get; set; }

      








    }
}
