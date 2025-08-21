using Microsoft.AspNetCore.Mvc;

namespace EntrePatasWEB.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
