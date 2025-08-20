using System.Reflection.Metadata.Ecma335;

namespace EntrePatasAPI.Models
{
    public class Envio
    {

        public int IdEnvio { get; set; }
        public int IdPedido { get; set; }
        public string DireccionEnvio { get; set; }

        public string EstadoEnvio { get; set; }
        public DateTime FechaEnvio { get; set; }
        
    }
}
