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

        public Animal Editar(int id, AnimalDTO animal)
        {

            Animal editarAnimal = null;
            int newId = 0;
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_ActualizarAnimal", cnx))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdAnimal", id);
                        command.Parameters.AddWithValue("@IdUsuario", animal.IdUsuario);
                        command.Parameters.AddWithValue("@Nombre", animal.Nombre);
                        command.Parameters.AddWithValue("@Especie", animal.Especie);
                        command.Parameters.AddWithValue("@Raza", animal.Raza);
                        command.Parameters.AddWithValue("@Edad", animal.Edad);
                        command.Parameters.AddWithValue("@Estado", animal.Estado);
                        command.Parameters.AddWithValue("@Foto", animal.Foto);
                        command.Parameters.AddWithValue("@Descripcion", animal.Descripcion);
                        var result = Convert.ToInt32(command.ExecuteScalar()); // 1 o 0
                        if (result == 1)
                        {
                            // ✅ Se actualizó, obtengo el usuario editado
                            editarAnimal = ObtenerAnimalPorId(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return editarAnimal;



        }

        public bool Eliminar(int id)
        {

            bool eliminado = false;

            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_EliminarAnimal", cnx))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdAnimal", id);

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

        public List<Animal> Listado()
        {
            var listado = new List<Animal>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var command = new SqlCommand("sp_ListarAnimal", conexion))
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
                                    IdUsuario = reader.GetInt32(1),
                                    Nombre = reader.GetString(2),
                                    Especie = reader.GetString(3),
                                    Raza = reader.GetString(4),
                                    Edad = reader.GetInt32(5),
                                    Estado = reader.GetString(6),
                                    FechaRegistro = reader.GetDateTime(7),
                                    Foto = reader.GetString(8),
                                    Descripcion = reader.GetString(9)

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
                using (var command = new SqlCommand("sp_ObtenerAnimalPorId", conexion))
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
                                IdUsuario = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Especie = reader.GetString(3),
                                Raza = reader.GetString(4),
                                Edad = reader.GetInt32(5),
                                Estado = reader.GetString(6),
                                FechaRegistro = reader.GetDateTime(7),
                                Foto = reader.GetString(8),
                                Descripcion = reader.GetString(9)
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
                    command.Parameters.AddWithValue("@IdUsuario", animal.IdUsuario);
                    command.Parameters.AddWithValue("@Nombre", animal.Nombre);
                    command.Parameters.AddWithValue("@Especie", animal.Especie);
                    command.Parameters.AddWithValue("@Raza", animal.Raza);
                    command.Parameters.AddWithValue("@Edad", animal.Edad);
                    command.Parameters.AddWithValue("@Estado", animal.Estado);
                    command.Parameters.AddWithValue("@Foto", animal.Foto);
                    command.Parameters.AddWithValue("@Descripcion", animal.Descripcion);


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
