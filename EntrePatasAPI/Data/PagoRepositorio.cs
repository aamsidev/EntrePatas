using System.Data;
using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;
using Microsoft.Data.SqlClient;

namespace EntrePatasAPI.Data
{
    public class PagoRepositorio : IPago
    {

        private readonly string cadenaConexion;

        public PagoRepositorio(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"] ?? "valor_predeterminado";

        }


        public Pago Editar(int id, PagoDTO pago)
        {

            Pago editarPago = null;
            int newId = 0;
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_ActualizarEstadoPago", cnx))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdPago", id);
                        command.Parameters.AddWithValue("@EstadoPago", pago.EstadoPago);
                        command.Parameters.AddWithValue("@MetodoPago", pago.MetodoPago);
                        
                        var result = Convert.ToInt32(command.ExecuteScalar()); // 1 o 0
                        if (result == 1)
                        {
                            // ✅ Se actualizó, obtengo el usuario editado
                           editarPago = ObtenerPagoPorId(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return editarPago;



        }



        public bool Eliminar(int id)
        {

            bool eliminado = false;

            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_EliminarPago", cnx))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdPago", id);

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

        public List<Pago> Listado()
        {
            var listado = new List<Pago>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var command = new SqlCommand("sp_ListarPagos", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                listado.Add(new Pago()
                                {
                                    IdPago = reader.GetInt32(0),
                                    IdPedido = reader.GetInt32(1),
                                    FechaPago = reader.GetDateTime(2),  
                                    Monto = reader.GetDecimal(3),
                                    MetodoPago = reader.GetString(4),
                                    EstadoPago = reader.GetString(5),


                                });
                            }
                        }
                    }
                }
            }

            return listado;

        }


        public Pago ObtenerPagoPorId(int id)
        {
            Pago? pago = null;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_ObtenerPagoPorId", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdPago", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pago = new Pago()
                            {

                                IdPago = reader.GetInt32(0),
                                IdPedido = reader.GetInt32(1),
                                FechaPago = reader.GetDateTime(2),
                                Monto = reader.GetDecimal(3),
                                MetodoPago = reader.GetString(4),
                                EstadoPago = reader.GetString(5),
                            };
                        }
                    }
                }
            }

            if (pago == null)
            {
                throw new Exception("Pago no encontrada");
            }
            return pago;
        }



        public Pago Registrar(PagoDTO pago)
        {
            Pago? nuevoPago = null;

            int nuevoID = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_InsertarPago", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdPedido", pago.IdPedido);
                    command.Parameters.AddWithValue("@MetodoPago", pago.MetodoPago);
                    command.Parameters.AddWithValue("@Monto", pago.Monto);
                    command.Parameters.AddWithValue("@EstadoPago", pago.EstadoPago);



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
                            throw new Exception("Error: no se devolvió ningún ID del pago .");
                        }

                    }
                }

                // Solo buscamos el usuario si obtuvimos un ID válido
                if (nuevoID > 0)
                    nuevoPago = ObtenerPagoPorId(nuevoID);

                if (nuevoPago == null)
                {
                    throw new Exception("Pago no encontrada");
                }
                return nuevoPago;

            }










        }
        }
}
