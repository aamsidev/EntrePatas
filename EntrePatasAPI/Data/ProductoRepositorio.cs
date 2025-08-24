using System.Data;
using EntrePatasAPI.Data.Contrato;
using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;
using Microsoft.Data.SqlClient;

namespace EntrePatasAPI.Data
{
    public class ProductoRepositorio : IProducto
    {
        private readonly string cadenaConexion;

        public ProductoRepositorio(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"] ?? "valor_predeterminado";

        }



        public List<Producto> Listado()
        {
            var listado = new List<Producto>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var command = new SqlCommand("sp_ListarProductos", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                listado.Add(new Producto()
                                {
                                    IdProducto = reader.GetInt32(0),
                                    Nombre = reader.GetString(1),
                                    Descripcion = reader.GetString(2),
                                    Precio = reader.GetDecimal(3),
                                    FotoUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    Stock = reader.GetInt32(5)
                                    


                                });
                            }
                        }
                    }
                }
            }

            return listado;

        }


        public Producto ObtenerProductoPorId(int id)
        {
            Producto? producto = null;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_ObtenerProductoPorId", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdProducto", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new Producto()
                            {

                                IdProducto = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                Precio = reader.GetDecimal(3),
                                FotoUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Stock = reader.GetInt32(5)
                            };
                        }
                    }
                }
            }

            if (producto
                == null)
            {
                throw new Exception("Animal no encontrada");
            }
            return producto;
        }



        public Producto Registrar(ProductoDTO producto)
        {
            Producto? nuevoProducto = null;

            int nuevoID = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("sp_InsertarProducto", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@Precio", producto.Precio);
                    command.Parameters.AddWithValue("@FotoUrl", (object?)producto.FotoUrl ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Stock", producto.Stock);


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
                            throw new Exception("Error: no se devolvió ningún ID del producto .");
                        }

                    }
                }

                               if (nuevoID > 0)
                    nuevoProducto = ObtenerProductoPorId(nuevoID);

                if (nuevoProducto == null)
                {
                    throw new Exception("Animal no encontrada");
                }
                return nuevoProducto;

            }
        }

        public bool Eliminar(int id)
        {

            bool eliminado = false;

            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_EliminarProducto", cnx))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdProducto", id);

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







        public Producto Editar(int id, ProductoDTO producto)
        {

            Producto editarProducto = null;
            int newId = 0;
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var command = new SqlCommand("sp_ActualizarProducto", cnx))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdProducto", id);
                        command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                        command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                        command.Parameters.AddWithValue("@Precio", producto.Precio);
                        command.Parameters.AddWithValue("@FotoUrl", (object?)producto.FotoUrl ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Stock", producto.Stock);
                       
                        var result = Convert.ToInt32(command.ExecuteScalar());                        if (result == 1)
                        {
                                                       editarProducto = ObtenerProductoPorId(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return editarProducto;



        }



    }
}
