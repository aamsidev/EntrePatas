using Microsoft.AspNetCore.Mvc;

namespace EntrePatasWEB.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("TipoUsuario");

            if (rol != "Administrador")
                return RedirectToAction("Login", "PaginaPrincipal");

            return View();
        }
    }
}
