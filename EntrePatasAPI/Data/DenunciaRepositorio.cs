using System.Data;
using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;
using Microsoft.Data.SqlClient;

namespace EntrePatasAPI.Data
{
    public class DenunciaRepositorio : IDenuncia
    {

        private readonly string cadenaConexion;

        public DenunciaRepositorio(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"] ?? "valor_predeterminado";

        }

        public List<Denuncia> Listado()
        {
            var listado = new List<Denuncia>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var command = new SqlCommand("sp_ObtenerDenuncias", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                listado.Add(new Denuncia()
                                {
                                    IdDenuncia = reader.GetInt32(0),
                                    FechaDenuncia = reader.GetDateTime(1),
                                    Descripcion = reader.GetString(2),
                                    Evidencia  = reader.GetString(3),
                                    Ubicacion = reader.GetString(4),
                                    IdUsuario = reader.GetInt32(5)


                                });
                            }
                        }
                    }
                }
            }

            return listado;
        }

        public Denuncia ObtenerDenunciaPorId(int id)
        {

            Denuncia? denuncia = null;
            
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_ObtenerDenunciasPorId", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdDenuncia", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            denuncia = new Denuncia()
                            {

                                IdDenuncia = reader.GetInt32(0),
                                FechaDenuncia = reader.GetDateTime(1),
                                Descripcion = reader.GetString(2),
                                Evidencia = reader.GetString(3),
                                Ubicacion = reader.GetString(4),
                                IdUsuario = reader.GetInt32(5)
                               
                            };
                        }
                    }
                }
            }

            if (denuncia == null)
            {
                throw new Exception("Denuncia no encontrada");
            }

            return denuncia;




        }

        public Denuncia Registrar(DenunciaDTO denuncia)
        {
            Denuncia? nuevaDenuncia = null;
            
            int nuevoID = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_InsertarDenuncia", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Descripcion", denuncia.Descripcion);
                    command.Parameters.AddWithValue("@Evidencia", denuncia.Evidencia);
                    command.Parameters.AddWithValue("@Ubicacion", denuncia.Ubicacion);
                    command.Parameters.AddWithValue("@IdUsuario", denuncia.IdUsuario);


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
                            throw new Exception("Error: no se devolvió ningún ID de la denuncia.");
                        }

                    }
                }

                // Solo buscamos el usuario si obtuvimos un ID válido
                if (nuevoID > 0)
                    nuevaDenuncia = ObtenerDenunciaPorId(nuevoID);

                if (nuevaDenuncia == null)
                {
                    throw new Exception("Denuncia no encontrada");
                }
                return nuevaDenuncia;

            }
        }
    }
}
