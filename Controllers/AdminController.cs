using Microsoft.AspNetCore.Mvc;

namespace mi_pham_kem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
