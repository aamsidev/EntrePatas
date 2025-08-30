using System.ComponentModel.DataAnnotations.Schema;

namespace EntrePatasWEB.Models
{
    public class ProductoDTO
    {


        public int IdProducto { get; set; }

        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public decimal Precio { get; set; }

        public string FotoUrl { get; set; }

        public int Stock { get; set; }


        [NotMapped] public IFormFile? FotoFile { get; set; }


    }


}
