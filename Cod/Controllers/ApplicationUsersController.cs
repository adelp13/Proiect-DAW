using Cod.Data;
using Cod.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cod.Controllers
{
    public class ApplicationUsersController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> um;

        public ApplicationUsersController(ApplicationDbContext _db)
        {
            db = _db;
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(string id)
        {
            ApplicationUser user = db.Users.Include("Follows").Where(us => us.Id == id).First();
            ApplicationUser reqUser = db.Users.Include("Follows").Where(us => us.Id == um.GetUserId(User)).First();
            // if the user we are looking for has a private profile we have 2 cases:
            // the user sending the request is not in the follow list, then we must ask for a follo request
            // otherwise, we can see their profile
            if (user.isPrivate == true)
            {
                bool bHasAccess = false;
                foreach (Cod.Models.ApplicationUser otherUser in reqUser.Follows)
                {
                    if (otherUser.Id == user.Id)
                    {
                        bHasAccess = true;
                        break;
                    }
                }
                if (bHasAccess)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else // the user is public, so we can see their profile
            {
                return View();
            }
            return View();
        }
    }
}
