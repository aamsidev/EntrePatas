using EntrePatasAPI.Data.Contrato;
using System;

namespace EntrePatasAPI.Models
{
    public class Pedido
    {

        public int IdPedido { get; set; }
        public int IdUsuario { get; set; }

        public DateTime FechaPedido { get; set; }
        public  string Estado { get; set; }
        public decimal Total { get; set; }
      





    }
}
