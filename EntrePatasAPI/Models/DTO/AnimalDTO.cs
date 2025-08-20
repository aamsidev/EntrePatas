namespace EntrePatasAPI.Models.DTO
{
    public class AnimalDTO
    {

        public int IdAnimal { get; set; }
        public int IdUsuario { get; set; }
        public String? Nombre { get; set; }
        public String? Especie { get; set; }
        public string? Raza { get; set; }
        public int Edad { get; set; }
        public String? Estado { get; set; }
        public DateTime FechaRegistro { get; set; }


    }
}
