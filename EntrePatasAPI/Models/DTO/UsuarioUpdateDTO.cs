namespace EntrePatasAPI.Models.DTO
{
    public class UsuarioUpdateDTO
    {
        public string Correo { get; set; }
        public string ContrasenaActual { get; set; }
        public string NuevaContrasena { get; set; }
    }
}
