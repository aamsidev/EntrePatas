using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EntrePatasAPI.Data
{
    public class SolicitudVacunaRepositorio :ISolicitudVacuna
    {

        private readonly string cadenaConexion;

        public SolicitudVacunaRepositorio(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"] ?? "valor_predeterminado";

        }

        public List<SolicitudVacuna> Listado()
        {
            var listado = new List<SolicitudVacuna>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var command = new SqlCommand("sp_ObtenerSolicitudVacunas", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                listado.Add(new SolicitudVacuna()
                                {
                                    IdSolicitudVacuna = reader.GetInt32(0),
                                    IdSolicitud = reader.GetInt32(1),
                                    IdVacuna = reader.GetInt32(2),
                                    Cantidad = reader.GetInt32(3)

            

                                });
                            }
                        }
                    }
                }
            }

            return listado;
        }





      
        public SolicitudVacuna ObtenerSolicitudVacunaPorId(int id)
        {

            SolicitudVacuna? solicitud = null;


            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_ObtenerSolicitudVacunaPorId", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdSolicitudVacuna", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            solicitud = new SolicitudVacuna()
                            {

                                IdSolicitudVacuna = reader.GetInt32(0),
                                IdSolicitud = reader.GetInt32(1),
                                IdVacuna = reader.GetInt32(2),
                                Cantidad = reader.GetInt32(3)

                            };

                        }
                    }
                }
            }

            if (solicitud == null)
            {
                throw new Exception("SolicitudVacuna no encontrada");
            }

            return solicitud;




        }

        public SolicitudVacuna Registrar(SolicitudVacunaDTO solicitud)
        {
            SolicitudVacuna? nuevaSolicitud = null;

            int nuevoID = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_InsertarSolicitudVacuna", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                  
                    command.Parameters.AddWithValue("@IdSolicitud", solicitud.IdSolicitud);
                    command.Parameters.AddWithValue("@IdVacuna", solicitud.IdVacuna);
                    command.Parameters.AddWithValue("@Cantidad", solicitud.Cantidad);

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
            }

            // Solo buscamos el solicitud si obtuvimos un ID válido
            if (nuevoID > 0)
                nuevaSolicitud = ObtenerSolicitudVacunaPorId(nuevoID);

            if (nuevaSolicitud == null)
            {
                throw new Exception("Solicitud no encontrada");
            }

            return nuevaSolicitud;
        }









    }









}
