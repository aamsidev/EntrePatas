namespace EntrePatasWEB.Models
{
    public class SolicitudDTO
    {
        public int IdSolicitud { get; set; }
        public int IdUsuario { get; set; }
        public int IdAnimal { get; set; }
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente";

        public AnimalDTO? Animal { get; set; }


        // Relaciones
        public UsuarioDTO Usu{ get; set; }
        public AnimalDTO Ani{ get; set; }




    }
}
