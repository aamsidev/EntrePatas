using System.Data;
using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;
using Microsoft.Data.SqlClient;

namespace EntrePatasAPI.Data
{
    public class PedidoRepositorio  : IPedido
    {

        private readonly string cadenaConexion; 

        public PedidoRepositorio(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"] ?? "valor_predeterminado";

        }

        public Pedido Editar(int id, PedidoDTO pedido)
        {

            Pedido editarPedido = null;
            int newId = 0;
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_ActualizarPedido", cnx))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdPedido", id);
                        command.Parameters.AddWithValue("@Estado", pedido.Estado);
                        command.Parameters.AddWithValue("@Total", pedido.Total);
                   
                        var result = Convert.ToInt32(command.ExecuteScalar()); // 1 o 0
                        if (result == 1)
                        {
                            // ✅ Se actualizó, obtengo el usuario editado
                            editarPedido = ObtenerPedidoPorId(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return editarPedido;



        }

        public bool Eliminar(int id)
        {

            bool eliminado = false;

            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_EliminarPedido", cnx))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdPedido", id);

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




        public List<Pedido> Listado()
        {
            var listado = new List<Pedido>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var command = new SqlCommand("sp_ListarPedidos", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                listado.Add(new Pedido()
                                {



                                    IdPedido = reader.GetInt32(0),
                                    IdUsuario = reader.GetInt32(1),
                                    FechaPedido = reader.GetDateTime(2),
                                    Estado = reader.GetString(3),
                                    Total = reader.GetDecimal(4)
                                   
                             

                                });
                            }
                        }
                    }
                }
            }

            return listado;

        }

        public Pedido ObtenerPedidoPorId(int id)
        {
            Pedido? pedido = null;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_ObtenerPedidoPorId", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdPedido", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pedido = new Pedido()
                            {
                                IdPedido = reader.GetInt32(0),
                                IdUsuario = reader.GetInt32(1),
                                FechaPedido = reader.GetDateTime(2),
                                Estado = reader.GetString(3),
                                Total = reader.GetDecimal(4)
                            };
                        }
                    }
                }
            }

            if (pedido == null)
            {
                throw new Exception("Animal no encontrada");
            }
            return pedido;
        }


        public Pedido Registrar(PedidoDTO pedido)
        {
            Pedido? nuevoPedido = null;

            int nuevoID = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_InsertarPedido", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdUsuario", pedido.IdUsuario);
                    command.Parameters.AddWithValue("@Estado", pedido.Estado);
                    command.Parameters.AddWithValue("@Total", pedido.Total);


                    // Capturamos el resultado del SP
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
                            throw new Exception("Error: no se devolvió ningún ID del animal .");
                        }

                    }
                }

                // Solo buscamos el usuario si obtuvimos un ID válido
                if (nuevoID > 0)
                    nuevoPedido = ObtenerPedidoPorId(nuevoID);

                if (nuevoPedido == null)
                {
                    throw new Exception("Animal no encontrada");
                }
                return nuevoPedido;

            }






        }
        }
}
