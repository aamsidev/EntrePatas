namespace EntrePatasAPI.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public String? Nombre { get; set; }

        public String? Apellido { get; set; }
        public String? Correo { get; set; }
        public String?  Contrasena { get; set; }

        public DateTime FechaRegistro { get; set; }

        public String? TipoUsuario { get; set; }
    }
}
