using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Data;

namespace EntrePatasAPI.Data
{
    public class VacunaRepositorio : IVacuna
    {

        private readonly string cadenaConexion;

        public VacunaRepositorio(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"] ?? "valor_predeterminado";

        }


        public List<Vacuna> Listado()
        {
            var listado = new List<Vacuna>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var command = new SqlCommand("sp_ListarVacunas", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                listado.Add(new Vacuna()
                                {
                                    IdVacuna = reader.GetInt32(0),
                                    Nombre = reader.GetString(1),
                                    Descripcion = reader.GetString(2),
                                    Precio = reader.GetDecimal(3)
                               

                                });
                            }
                        }
                    }
                }
            }

            return listado;
        }

        public Vacuna ObtenerVacunaPorId(int id)
        {
            Vacuna? vacuna = null;


            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_ObtenerVacunaPorId", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdVacuna", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            vacuna = new Vacuna()
                            {

                                IdVacuna = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                Precio = reader.GetDecimal(3)

                            };
                        }
                    }
                }
            }

            if (vacuna == null)
            {
                throw new Exception("Vacuna no encontrada");
            }

            return vacuna;
        }

        public Vacuna Registrar(VacunaDTO vacuna)
        {


            Vacuna? nuevaVacuna = null;

            int nuevoID = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_InsertarVacuna", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Nombre", vacuna.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", vacuna.Descripcion);
                    command.Parameters.AddWithValue("@Precio", vacuna.Precio);
                   

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
                            throw new Exception("Error: no se devolvió ningún ID de la vacuna.");
                        }

                    }
                }
            }

            if (nuevoID > 0)
                nuevaVacuna = ObtenerVacunaPorId(nuevoID);

            if (nuevaVacuna == null)
            {
                throw new Exception("Vacuna no encontrada");
            }

            return nuevaVacuna;



        }

        public Vacuna Actualizar(int id, VacunaDTO vacuna)
        {
            Vacuna editarVacuna = null;
            int newId = 0;
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_ActualizarVacuna", cnx))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdVacuna", id);
                        command.Parameters.AddWithValue("@Nombre", vacuna.Nombre);
                        command.Parameters.AddWithValue("@Descripcion", vacuna.Descripcion);
                        command.Parameters.AddWithValue("@Precio", vacuna.Precio);
                        var result = Convert.ToInt32(command.ExecuteScalar());                        if (result == 1)
                        {
                                                       editarVacuna = ObtenerVacunaPorId(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return editarVacuna;



        }

        public bool Eliminar(int id)
            {
                using (var conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    using (var command = new SqlCommand("sp_EliminarVacuna", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdVacuna", id);
                        int filasAfectadas = command.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
        }
    }

}
