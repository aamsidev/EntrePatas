using System.Data;
using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;
using Microsoft.Data.SqlClient;

namespace EntrePatasAPI.Data
{
    public class SolicitudRepositorio : ISolicitud
    {

        private readonly string cadenaConexion;

        public SolicitudRepositorio(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"] ?? "valor_predeterminado";

        }

        public Solicitud Editar(int id, SolicitudDTO solicitud)
        {

            Solicitud editarSolicitud = null;
            int newId = 0;
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_ActualizarSolicitud", cnx))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdSolicitud", id);
                        command.Parameters.AddWithValue("@Estado", solicitud.Estado);
                       
                        var result = Convert.ToInt32(command.ExecuteScalar());                        if (result == 1)
                        {
                                                       editarSolicitud = ObtenerSolicitudPorId(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return editarSolicitud;



        }

       

        public bool Eliminar(int id)
        {
            bool eliminado = false;

            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_EliminarSolicitud", cnx))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdSolicitud", id);

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

        public List<Solicitud> Listado()
        {
            var listado = new List<Solicitud>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var command = new SqlCommand("sp_ListarSolicitudes", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                listado.Add(new Solicitud()
                                {
                                    IdSolicitud = reader.GetInt32(0),
                                    IdUsuario = reader.GetInt32(1),
                                    IdAnimal = reader.GetInt32(2),
                                    FechaSolicitud = reader.GetDateTime(3),

                                    Estado = reader.GetString(4),   
                                    

                                });
                            }
                        }
                    }
                }
            }

            return listado;






        }

        public Solicitud ObtenerSolicitudPorId(int id)
        {
            Solicitud? solicitud = null;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_ObtenerSolicitudPorId", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdSolicitud", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            solicitud = new Solicitud()
                            {
                                IdSolicitud = reader.GetInt32(0),
                                IdUsuario = reader.GetInt32(1),
                                IdAnimal = reader.GetInt32(2),
                                FechaSolicitud = reader.GetDateTime(3),
                                Estado = reader.GetString(4),

                            };
                        }
                    }
                }
            }

            if (solicitud == null)
            {
                throw new Exception("Solicitud no encontrada");
            }
            return solicitud;






        }

        public Solicitud Registrar(SolicitudDTO solicitud)
        {
            Solicitud? nuevoSolicitud = null;

            int nuevoID = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_InsertarSolicitud", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdUsuario", solicitud.IdUsuario);
                    command.Parameters.AddWithValue("@IdAnimal", solicitud.IdAnimal);
                    command.Parameters.AddWithValue("@Estado", solicitud.Estado);
                   


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
                            throw new Exception("Error: no se devolvió ningún ID de solicitud .");
                        }

                    }
                }

                               if (nuevoID > 0)
                    nuevoSolicitud = ObtenerSolicitudPorId(nuevoID);

                if (nuevoSolicitud == null)
                {
                    throw new Exception("Animal no encontrada");
                }
                return nuevoSolicitud;

            }



        }
    }
}
