using EntrePatasAPI.Data.Contrato;

namespace EntrePatasAPI.Models
{
    public class Solicitud
    {
        public int IdSolicitud { get; set; }

        public int IdUsuario { get; set; }
        public int IdAnimal { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public  string Estado { get; set; }
       




    }
}
