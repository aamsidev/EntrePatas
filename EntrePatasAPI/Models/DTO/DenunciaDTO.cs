namespace EntrePatasAPI.Models.DTO
{
    public class DenunciaDTO
    {
        public int IdDenuncia { get; set; }
        public DateTime FechaDenuncia { get; set; }
        public String? Descripcion { get; set; }
        public String? Evidencia { get; set; }
        public String? Ubicacion { get; set; }
        public int IdUsuario { get; set; }




    }
}
