﻿using System.Reflection.Metadata.Ecma335;

namespace EntrePatasAPI.Models
{
    public class DetallePedido
    {

        public int IdDetalle { get; set; }
        public int IdPedido { get; set; }

        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
