using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntrePatasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProducto productoDATA;

        public ProductoController(IProducto producto)
        {

            productoDATA = producto;


        }

        [HttpGet]
        public IActionResult ListarProductos()
        {

            return Ok(productoDATA.Listado());

        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var producto = productoDATA.ObtenerProductoPorId(id);
            if (producto == null)
                return NotFound();

            return Ok(producto);
        }

        [HttpPost("Registrar")]
        public IActionResult Registrar(ProductoDTO producto)
        {
            var nuevoProducto = productoDATA.Registrar(producto);
            if (nuevoProducto == null)
                return NotFound();
            return Ok(nuevoProducto);
        }


        [Route("update/{id}")]
        [HttpPut]
        public IActionResult Editar(int id, ProductoDTO c)
        {
            try
            {
                var producto = productoDATA.Editar(id, c);
                return Ok(producto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }



        [HttpDelete("{id}")]
        public IActionResult EliminarProducto(int id)
        {
            var result = productoDATA.Eliminar(id);
            if (result)
                return Ok(new { mensaje = "Producto eliminado correctamente" });
            else
                return NotFound(new { mensaje = "Producto no encontrado" });
        }







    }
}
