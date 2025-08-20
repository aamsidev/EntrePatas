using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntrePatasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {

        private readonly IPedido pedidoDATA;

        public PedidoController(IPedido pedido)
        {

            pedidoDATA = pedido;


        }



        [HttpGet]
        public IActionResult ListarPedido()
        {

            return Ok(pedidoDATA.Listado());

        }


        [HttpGet]
        [Route("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var pedido = pedidoDATA.ObtenerPedidoPorId(id);
            if (pedido == null)
                return NotFound();

            return Ok(pedido);
        }

        [HttpPost("Registrar")]
        public IActionResult Registrar(PedidoDTO pedido)
        {
            var nuevoPedido = pedidoDATA.Registrar(pedido);
            if (nuevoPedido == null)
                return NotFound();
            return Ok(nuevoPedido);
        }


        [Route("update/{id}")]
        [HttpPut]
        public IActionResult Editar(int id, PedidoDTO c)
        {
            try
            {
                var pedido = pedidoDATA.Editar(id, c);
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }


        [HttpDelete("{id}")]
        public IActionResult EliminarPedido(int id)
        {
            var result = pedidoDATA.Eliminar(id);
            if (result)
                return Ok(new { mensaje = "Pedido eliminado correctamente" });
            else
                return NotFound(new { mensaje = "Pedido no encontrado" });
        }






    }
}
