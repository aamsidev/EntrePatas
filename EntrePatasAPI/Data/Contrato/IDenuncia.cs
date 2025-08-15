using EntrePatasAPI.Models.DTO;
using EntrePatasAPI.Models;

namespace EntrePatasAPI.Data.Contrato
{
    public interface IDenuncia
    {

        List<Denuncia> Listado();

        Denuncia ObtenerDenunciaPorId(int id);

        Denuncia Registrar(DenunciaDTO usuario);

       



    }
}
