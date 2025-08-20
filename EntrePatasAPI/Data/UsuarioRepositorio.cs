using System.Data;
using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;
using Microsoft.Data.SqlClient;

namespace EntrePatasAPI.Data
{
    public class UsuarioRepositorio : IUsuario
    {

        private readonly string cadenaConexion;

        public UsuarioRepositorio(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"] ?? "valor_predeterminado";
        
        }

        public List<Usuario> Listado()
        {
            var listado = new List<Usuario>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var command = new SqlCommand("sp_ListarUsuarios", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                listado.Add(new Usuario()
                                {
                                    IdUsuario = reader.GetInt32(0),
                                    Nombre = reader.GetString(1),
                                    Apellido = reader.GetString(2), 
                                    Correo = reader.GetString(3),
                                    Telefono = reader.GetString(4),
                                    Direccion = reader.GetString(5),
                                    Contrasena = reader.GetString(6),
                                    TipoUsuario = reader.GetString(7),
                                    FechaRegistro = reader.GetDateTime(8),


                                }); 
                            }
                        }
                    }
                }
            }

            return listado;
        }

        public Usuario ObtenerUsuarioPorId(int id)
        {
            Usuario? usuario = null;
           

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_ObtenerUsuarioPorId", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdUsuario", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario()
                            {

                                IdUsuario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Apellido = reader.GetString(2),
                                Correo = reader.GetString(3),
                                Telefono = reader.GetString(4),
                                Direccion = reader.GetString(5),
                                Contrasena = reader.GetString(6),
                                TipoUsuario = reader.GetString(7),
                                FechaRegistro = reader.GetDateTime(8),
                            };
                        }
                    }
                }
            }

            if (usuario == null)
            {
                throw new Exception("Usuario no encontrada");
            }

            return usuario;
        }

        public Usuario Registrar(UsuarioDTO usuario)
        {
            Usuario? nuevoUsuario = null;
           
            int nuevoID = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_InsertarUsuario", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    command.Parameters.AddWithValue("@Email", usuario.Correo);
                    command.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                    command.Parameters.AddWithValue("@Direccion", usuario.Direccion);
                    command.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);
                    command.Parameters.AddWithValue("@TipoUsuario", usuario.TipoUsuario);

                    // Capturamos el resultado del SP
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        int codigo = Convert.ToInt32(result);

                        if (codigo == -1)
                            throw new Exception("Faltan datos obligatorios para registrar el usuario.");
                        else if (codigo == -2)
                            throw new Exception("El correo ya está registrado.");
                        else
                            nuevoID = codigo; // Guardamos el ID válido
                    }
                    else
                    {
                        throw new Exception("Error desconocido: no se devolvió ningún valor.");
                    }
                }
            }

            // Solo buscamos el usuario si obtuvimos un ID válido
            if (nuevoID > 0)
                nuevoUsuario = ObtenerUsuarioPorId(nuevoID);

            if (nuevoUsuario == null)
            {
                throw new Exception("Usuario no encontrada");
            }

            return nuevoUsuario;
        }

        public Usuario Editar(int id, UsuarioDTO usuario)
        {

            Usuario editarUsuario = null;
            int newId = 0;
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_ActualizarUsuario", cnx))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdUsuario", id);
                        command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                        command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                        command.Parameters.AddWithValue("@Email", usuario.Correo);
                        command.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                        command.Parameters.AddWithValue("@Direccion", usuario.Direccion);
                        command.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);
                        command.Parameters.AddWithValue("@TipoUsuario", usuario.TipoUsuario);
                        var result = Convert.ToInt32(command.ExecuteScalar()); // 1 o 0
                        if (result == 1)
                        {
                            // ✅ Se actualizó, obtengo el usuario editado
                            editarUsuario = ObtenerUsuarioPorId(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return editarUsuario;

        }

        public bool Eliminar(int id)
        {
            bool eliminado = false;

            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_EliminarUsuario", cnx))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdUsuario", id);

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
    }
}

