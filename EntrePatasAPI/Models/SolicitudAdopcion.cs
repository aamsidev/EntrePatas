using System.Security.Policy;

namespace EntrePatasAPI.Models
{
    public class SolicitudAdopcion
    {

        public int IdSolicitud { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public String? Evidencia { get; set; }
        public String? Ubicacion { get; set; }
        public int IdUsuario { get; set; }
        public int IdAnimal { get; set; }

    }
}
