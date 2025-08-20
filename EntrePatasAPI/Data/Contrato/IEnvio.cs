using EntrePatasAPI.Models;
using EntrePatasAPI.Models.DTO;

namespace EntrePatasAPI.Data.Contrato
{
    public interface IEnvio
    {


        List<Envio> Listado();

        Envio ObtenerEnvioPorId(int id);

        Envio Registrar(EnvioDTO pago);

        Envio Editar(int id, EnvioDTO pago);

        bool Eliminar(int id);









    }
}
