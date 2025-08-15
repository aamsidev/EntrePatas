using System.Data;
using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;
using Microsoft.Data.SqlClient;

namespace EntrePatasAPI.Data
{
    public class AnimalRepositorio : IAnimal
    {
        private readonly string cadenaConexion;

        public AnimalRepositorio(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"] ?? "valor_predeterminado";

        }


        public List<Animal> Listado()
        {
            var listado = new List<Animal>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var command = new SqlCommand("sp_ObtenerAnimales", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                listado.Add(new Animal()
                                {
                                    IdAnimal = reader.GetInt32(0),
                                    Nombre = reader.GetString(1),
                                    Especie = reader.GetString(2),
                                    Raza = reader.GetString(3),
                                    Edad = reader.GetInt32(4),
                                    EstadoSalud = reader.GetString(5),
                                    Estado = reader.GetString(6),
                                    FechaRegistro = reader.GetDateTime(7)
                                    



                                });
                            }
                        }
                    }
                }
            }

            return listado;
        
        }

        public Animal ObtenerAnimalPorId(int id)
        {
            Animal? animal = null;
            
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_ObtenerAnimalesPorId", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdAnimal", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            animal = new Animal()
                            {

                                IdAnimal = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Especie = reader.GetString(2),
                                Raza = reader.GetString(3),
                                Edad = reader.GetInt32(4),
                                EstadoSalud = reader.GetString(5),
                                Estado = reader.GetString(6),
                                FechaRegistro = reader.GetDateTime(7)
                            };
                        }
                    }
                }
            }

            if (animal == null)
            {
                throw new Exception("Animal no encontrada");
            }
            return animal;
        }

        public Animal Registrar(AnimalDTO animal)
        {
            Animal? nuevoAnimal = null;

            int nuevoID = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_InsertarAnimal", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Nombre", animal.Nombre);
                    command.Parameters.AddWithValue("@Especie", animal.Especie);
                    command.Parameters.AddWithValue("@Raza", animal.Raza);
                    command.Parameters.AddWithValue("@Edad", animal.Edad);
                    command.Parameters.AddWithValue("@EstadoSalud", animal.EstadoSalud);
                    command.Parameters.AddWithValue("@Estado", animal.Estado);


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
                    nuevoAnimal = ObtenerAnimalPorId(nuevoID);

                if (nuevoAnimal == null)
                {
                    throw new Exception("Animal no encontrada");
                }
                return nuevoAnimal;

            }
        }
    }
}
