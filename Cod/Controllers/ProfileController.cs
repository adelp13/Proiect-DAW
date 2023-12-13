using Microsoft.AspNetCore.Mvc;

namespace Cod.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
