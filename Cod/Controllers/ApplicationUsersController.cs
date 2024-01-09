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
        private readonly RoleManager<IdentityRole> rm;

        public ApplicationUsersController(ApplicationDbContext _db, UserManager<ApplicationUser> _um, RoleManager<IdentityRole> _rm)
        {
            db = _db;
            um = _um;
            rm = _rm;
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Index() {
            var users = db.Users.Include("Follows");
            ViewBag.Users = users;
            return View();
        }

        [HttpPost]
        public void AddFollow(string id)
        {
            // verificam daca userul catre care vrem sa facem request are profilul privat

            var user = db.Users.Where(us => us.Id == id).FirstOrDefault();

            if (user.isPrivate == false) {
                // verificam daca exista deja relatia de follow
                var follow = db.Follows.Where(us => us.FollowedProfileId == user.Id && us.FollowingProfileId == um.GetUserId(User)).First();
                if (follow == null)
                {
                    ProfileFollowsProfile newfollow = new ProfileFollowsProfile { FollowedProfileId = user.Id, FollowingProfileId = um.GetUserId(User)};
                    db.Follows.Add(newfollow);
                    db.SaveChanges();
                }
                else return;
            } else
            {
                // vom trimite un follow request
                // verificam daca nu apare deja in lista de followeri acceptati sau in lista de requests
                var request = db.FollowRequests.Where(us => us.RequestedProfileId == user.Id && us.RequestingProfileId == um.GetUserId(User)).First();
                var follow = db.Follows.Where(us => us.FollowedProfileId == user.Id && us.FollowingProfileId == um.GetUserId(User)).First();
                if (request == null && follow == null)
                {
                    ProfileRequestsProfile newreq = new ProfileRequestsProfile { RequestedProfileId = user.Id, RequestingProfileId = um.GetUserId(User) };
                    db.FollowRequests.Add(newreq);
                    db.SaveChanges();
                }
                else return;
            }
        }

        // TODO verifica integritatea acestor metode (nu dau crash daca vreau sa accept aiurea de exemplu)

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public void AcceptRequest(string id)
        {
            // pentru userul din um, ii voi da voie userului al carui id il am ca parametru sa ma urmareasca
            // practic, mut din tabelul de requests in tabelul de follow
            
            var request = db.FollowRequests.Where(us => us.RequestedProfileId == um.GetUserId(User) && us.RequestingProfileId == id).First();
            ProfileFollowsProfile newreq = new ProfileFollowsProfile { FollowedProfileId = um.GetUserId(User), FollowingProfileId = id };
            db.FollowRequests.Remove(request);
            db.Follows.Add(newreq);
            db.SaveChanges();
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public void DenyRequest(string id)
        {
            // practic, scot din tabelul de requests cererea
            var request = db.FollowRequests.Where(us => us.RequestedProfileId == um.GetUserId(User) && us.RequestingProfileId == id).First();
            db.FollowRequests.Remove(request);
            db.SaveChanges();
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public void UnFollow(string id)
        {
            // practic, scot din tabelul de follows cererea
            var follow = db.Follows.Where(us => us.FollowedProfileId == um.GetUserId(User) && us.FollowingProfileId == id).First();
            db.Follows.Remove(follow);
            db.SaveChanges();
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(string id)
        {
            // TODO do I really need to include Follows?

            ApplicationUser user = db.Users.Include("Follows").Where(us => us.Id == id).First();
            // daca vreau sa ma vad pe mine pot sa ma vad indiferent de conditii
            if (user.Id == um.GetUserId(User))
            {
                return View(user);
            }
            //ApplicationUser reqUser = db.Users.Include("Follows").Where(us => us.Id == um.GetUserId(User)).First();
            // if the user we are looking for has a private profile we have 2 cases:
            // the user sending the request is not in the follow list, then we must ask for a follow request
            // otherwise, we can see their profile
            if (user.isPrivate == true)
            {
                var follow = db.Follows.Where(us => us.FollowedProfileId == user.Id && us.FollowingProfileId == um.GetUserId(User));
                if (follow.Any())
                {
                    return View(user);
                }
                else
                {
                    // TODO add viewbag message for not following
                    // or think about what to do
                    return RedirectToAction("Index");
                }
            }
            else // the user is public, so we can see their profile
            {
                return View(user);
            }
        }
    }
}
