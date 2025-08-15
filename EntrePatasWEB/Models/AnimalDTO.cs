namespace EntrePatasWEB.Models
{
    public class AnimalDTO
    {
        public int ID { get; set; }
        public String? Nombre { get; set; }
        public String? Especie { get; set; }
        public string? Raza { get; set; }
        public int Edad { get; set; }
        public string? EstadoSalud { get; set; }

        public string? Estado { get; set; }

        public DateTime FechaRegistro { get; set; }

    }
}
