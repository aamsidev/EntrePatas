using System.Data;
using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;
using Microsoft.Data.SqlClient;

namespace EntrePatasAPI.Data
{
    public class EnvioRepositorio : IEnvio
    {
        private readonly string cadenaConexion;

        public EnvioRepositorio(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"] ?? "valor_predeterminado";

        }

        public Envio Editar(int id, EnvioDTO envio)
        {

            Envio editarEnvio = null;
            int newId = 0;
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_ActualizarEnvio", cnx))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdEnvio", id);
                        command.Parameters.AddWithValue("@DireccionEnvio", envio.DireccionEnvio);
                        command.Parameters.AddWithValue("@EstadoEnvio", envio.EstadoEnvio);
                        
                        var result = Convert.ToInt32(command.ExecuteScalar());                        if (result == 1)
                        {
                                                       editarEnvio = ObtenerEnvioPorId(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return editarEnvio;



        }


        public bool Eliminar(int id)
        {

            bool eliminado = false;

            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_EliminarEnvio", cnx))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdEnvio", id);

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

        public List<Envio> Listado()
        {
            var listado = new List<Envio>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var command = new SqlCommand("sp_ListarEnvios", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                listado.Add(new Envio()
                                {
                                    IdEnvio = reader.GetInt32(0),
                                    IdPedido = reader.GetInt32(1),
                                    DireccionEnvio = reader.GetString(2),
                                    EstadoEnvio = reader.GetString(3),
                                    FechaEnvio = reader.GetDateTime(4)


                                });
                            }
                        }
                    }
                }
            }

            return listado;

        }



        public Envio ObtenerEnvioPorId(int id)
        {
            Envio? envio = null;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_ObtenerEnvioPorId", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdEnvio", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            envio = new Envio()
                            {

                                IdEnvio = reader.GetInt32(0),
                                IdPedido = reader.GetInt32(1),
                                DireccionEnvio = reader.GetString(2),
                                EstadoEnvio = reader.GetString(3),
                                FechaEnvio = reader.GetDateTime(4)
                            };
                        }
                    }
                }
            }

            if (envio == null)
            {
                throw new Exception("Envio no encontrada");
            }
            return envio;
        }

        public Envio Registrar(EnvioDTO envio)
        {
            Envio? nuevoEnvio = null;

            int nuevoID = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_InsertarEnvio", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdPedido", envio.IdPedido);
                    command.Parameters.AddWithValue("@DireccionEnvio", envio.DireccionEnvio);
                    command.Parameters.AddWithValue("@EstadoEnvio", envio.EstadoEnvio);




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
                            throw new Exception("Error: no se devolvió ningún ID del envio .");
                        }

                    }
                }

                               if (nuevoID > 0)
                    nuevoEnvio = ObtenerEnvioPorId(nuevoID);

                if (nuevoEnvio == null)
                {
                    throw new Exception("Envio no encontrada");
                }
                return nuevoEnvio;

            }




        }


        }
}
