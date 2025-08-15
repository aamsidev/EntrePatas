using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EntrePatasAPI.Data
{
    public class SolicitudAdopcionRepositorio : ISolicitudAdopcion
    {

        private readonly string cadenaConexion;

        public SolicitudAdopcionRepositorio(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"] ?? "valor_predeterminado";

        }


        public List<SolicitudAdopcion> Listado()
        {
            var listado = new List<SolicitudAdopcion>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var command = new SqlCommand("sp_ObtenerSolicitudAdopcion", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                listado.Add(new SolicitudAdopcion()
                                {
                                    IdSolicitud = reader.GetInt32(0),
                                    FechaSolicitud = reader.GetDateTime(1),
                                    Evidencia = reader.GetString(2),
                                    Ubicacion = reader.GetString(3),
                                    IdUsuario = reader.GetInt32(4),
                                    IdAnimal = reader.GetInt32(5)


                                });
                            }
                        }
                    }
                }
            }

            return listado;
        }

       
       

        public SolicitudAdopcion ObtenerSolicitudAdopcionPorId(int id)
        {

            SolicitudAdopcion? solicitud = null;

           

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
                            solicitud = new SolicitudAdopcion()
                            {

                                IdSolicitud = reader.GetInt32(0),
                                FechaSolicitud = reader.GetDateTime(1),
                                Evidencia = reader.GetString(2),
                                Ubicacion = reader.GetString(3),
                                IdUsuario = reader.GetInt32(4),
                                IdAnimal = reader.GetInt32(5)

                            };
                        }
                    }
                }
            }
            if (solicitud == null)
            {
                throw new Exception("solicitud no encontrada");
            }
            return solicitud;




        }


        public SolicitudAdopcion Registrar(SolicitudAdopcionDTO solicitud)
        {
            SolicitudAdopcion? nuevaSolicitud = null;
           
            int nuevoID = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_InsertarSolicitudAdopcion", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Evidencia", solicitud.Evidencia);
                    command.Parameters.AddWithValue("@Ubicacion", solicitud.Ubicacion);
                    command.Parameters.AddWithValue("@IdUsuario", solicitud.IdUsuario);
                    command.Parameters.AddWithValue("@IdAnimal", solicitud.IdAnimal);


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
                            throw new Exception("Error: no se devolvió ningún ID de la solicitud.");
                        }

                    }
                }

                // Solo buscamos el usuario si obtuvimos un ID válido
                if (nuevoID > 0)
                    nuevaSolicitud = ObtenerSolicitudAdopcionPorId(nuevoID);

                if (nuevaSolicitud == null)
                {
                    throw new Exception("Solicitud no encontrada");
                }
                return nuevaSolicitud;

            }
        }







    }
}
