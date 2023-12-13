using Cod.Data;
using Cod.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cod.Controllers
{
    public class ApplicationUserController : Controller
    {

        private readonly ApplicationDbContext db;

        public ApplicationUserController(ApplicationDbContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult New()
        {
            ApplicationUser user = new ApplicationUser();
            return View(user);
        }

        [HttpPost]
        public IActionResult New(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                db.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(user);
            }
        }
    }
}
