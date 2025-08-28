using System.Data;
using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;
using Microsoft.Data.SqlClient;

namespace EntrePatasAPI.Data
{
    public class DetallePedidoRepositorio : IDetallePedido
    {



        private readonly string cadenaConexion;

        public DetallePedidoRepositorio(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"] ?? "valor_predeterminado";

        }

        public DetallePedido Editar(int id, DetallePedidoDTO detalle)
        {

            DetallePedido editarDetalle = null;
            int newId = 0;
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_ActualizarDetallePedido", cnx))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdDetalle", id);
                        command.Parameters.AddWithValue("@IdPedido", detalle.IdPedido);
                        command.Parameters.AddWithValue("@IdProducto", detalle.IdProducto);
                        command.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                        command.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);

                        var result = Convert.ToInt32(command.ExecuteScalar()); if (result == 1)
                        {
                            editarDetalle = ObtenerDetallePedidoPorId(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return editarDetalle;



        }

        public bool Eliminar(int id)
        {

            bool eliminado = false;

            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_EliminarDetallePedido", cnx))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdDetalle", id);

                        var result = Convert.ToInt32(command.ExecuteScalar());
                        eliminado = result == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return eliminado;



        }


        public List<DetallePedido> Listado()
        {
            var listado = new List<DetallePedido>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var command = new SqlCommand("sp_ListarDetallesPedido", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                listado.Add(new DetallePedido()
                                {
                                    IdDetalle = reader.GetInt32(0),
                                    IdPedido = reader.GetInt32(1),
                                    IdProducto = reader.GetInt32(2),
                                    Cantidad = reader.GetInt32(3),
                                    PrecioUnitario = reader.GetDecimal(4),

                                });
                            }
                        }
                    }
                }
            }

            return listado;

        }



        public DetallePedido ObtenerDetallePedidoPorId(int id)
        {
            DetallePedido? detalle = null;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_ObtenerDetallePedidoPorId", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdDetalle", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            detalle = new DetallePedido()
                            {

                                IdDetalle = reader.GetInt32(0),
                                IdPedido = reader.GetInt32(1),
                                IdProducto = reader.GetInt32(2),
                                Cantidad = reader.GetInt32(3),
                                PrecioUnitario = reader.GetDecimal(4),
                            };
                        }
                    }
                }
            }

            if (detalle == null)
            {
                throw new Exception("Animal no encontrada");
            }
            return detalle;
        }


        public DetallePedido Registrar(DetallePedidoDTO detalle)
        {
            DetallePedido? nuevoDetalle = null;

            int nuevoID = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_InsertarDetallePedido", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdPedido", detalle.IdPedido);
                    command.Parameters.AddWithValue("@IdProducto", detalle.IdProducto);
                    command.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                    command.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);


                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        int codigo = Convert.ToInt32(result);

                        if (result != null && result != DBNull.Value)
                        {
                            nuevoID = Convert.ToInt32(result);
                        }
                        else
                        {
                            throw new Exception("Error: no se devolvió ningún ID del detalle .");
                        }

                    }
                }

                if (nuevoID > 0)
                    nuevoDetalle = ObtenerDetallePedidoPorId(nuevoID);

                if (nuevoDetalle == null)
                {
                    throw new Exception("Detalle no encontrada");
                }
                return nuevoDetalle;

            }


        }
    }
}
