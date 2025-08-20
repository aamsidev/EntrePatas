using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;

namespace EntrePatasAPI.Data.Contrato
{
    public interface IProducto
    {

        List<Producto> Listado();

        Producto ObtenerProductoPorId(int id);

        Producto Registrar(ProductoDTO animal);

        Producto Editar(int id, ProductoDTO animal);

        bool Eliminar(int id);




    }
}
