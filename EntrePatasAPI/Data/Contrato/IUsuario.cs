using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EntrePatasAPI.Data.Contrato
{
    public interface IUsuario
    {

        List<Usuario> Listado();

        Usuario ObtenerUsuarioPorId(int id);

        Usuario Registrar(UsuarioDTO usuario);

        Usuario Editar(int id, UsuarioDTO usuario);

        bool Eliminar(int id);

        Usuario VerificarLogin(string correo, string contrasena);
    }
}
