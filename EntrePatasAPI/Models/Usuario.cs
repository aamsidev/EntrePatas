namespace EntrePatasAPI.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public String? Nombre { get; set; }

        public String? Apellido { get; set; }
        public String? Correo { get; set; }

        public String? Telefono { get; set; }
        public String? Direccion { get; set; }
        public String?  Contrasena { get; set; }

        public String? TipoUsuario { get; set; }
        public DateTime FechaRegistro { get; set; }


    }
}
