﻿using Cod.Data;
using Cod.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        // TODO sa las asta doar pentru admin?
        [Authorize(Roles = "User,Admin")]
        public IActionResult Index() {
            var users = db.Users;
            ViewBag.Users = users;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult AddFollow(string id)
        {
            // verificam daca userul catre care vrem sa facem request are profilul privat

            if (id == um.GetUserId(User))
            {
                TempData["message"] = "Nu aveti dreptul sa va urmariti pe dumeanvoastra!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            var user = db.Users.Where(us => us.Id == id);

            if (user.Any())
            {
                if (user.First().isPrivate == false)
                {
                    // verificam daca exista deja relatia de follow
                    var follow = db.Follows.Where(us => us.FollowedProfileId == user.First().Id && us.FollowingProfileId == um.GetUserId(User));
                    if (!follow.Any())
                    {
                        ProfileFollowsProfile newfollow = new ProfileFollowsProfile { FollowedProfileId = user.First().Id, FollowingProfileId = um.GetUserId(User) };
                        db.Follows.Add(newfollow);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Show", new { id = id });
                }
                else
                {
                    // vom trimite un follow request
                    // verificam daca nu apare deja in lista de followeri acceptati sau in lista de requests
                    var request = db.FollowRequests.Where(us => us.RequestedProfileId == user.First().Id && us.RequestingProfileId == um.GetUserId(User));
                    var follow = db.Follows.Where(us => us.FollowedProfileId == user.First().Id && us.FollowingProfileId == um.GetUserId(User));
                    if (!request.Any() && !follow.Any())
                    {
                        ProfileRequestsProfile newreq = new ProfileRequestsProfile { RequestedProfileId = user.First().Id, RequestingProfileId = um.GetUserId(User) };
                        db.FollowRequests.Add(newreq);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Show", new { id = id });
                }
            }
            else
            {
                TempData["message"] = "Ati incercat sa urmariti un profil inexistent!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public ActionResult ViewRequests()
        {
            var user = db.Profiles.Where(x => x.Id == um.GetUserId(User));

            if (user.First().isPrivate == true)
            {
                var cereri = db.FollowRequests.Where(x=> x.RequestedProfileId == um.GetUserId(User));

                if (cereri.Any())
                {
                    List<ApplicationUser> conturi = new List<ApplicationUser>();
                    foreach (var req in cereri)
                    {
                        conturi.Add(db.Profiles.Where(x => x.Id == req.RequestingProfileId).First());
                    }
                    ViewBag.profiles = conturi;
                } else
                {
                    ViewBag.profiles = null;
                }
                return View();
            } else
            {
                TempData["message"] = "Profilele publice nu primesc cereri de follow!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        // TODO verifica integritatea acestor metode (nu dau crash daca vreau sa accept aiurea de exemplu)

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult AcceptRequest(string id)
        {
            // pentru userul din um, ii voi da voie userului al carui id il am ca parametru sa ma urmareasca
            // practic, mut din tabelul de requests in tabelul de follow
            
            var request = db.FollowRequests.Where(us => us.RequestedProfileId == um.GetUserId(User) && us.RequestingProfileId == id);
            if (request.Any())
            {
                ProfileFollowsProfile newreq = new ProfileFollowsProfile { FollowedProfileId = um.GetUserId(User), FollowingProfileId = id };
                db.FollowRequests.Remove(request.First());
                db.Follows.Add(newreq);
                db.SaveChanges();
                return RedirectToAction("ViewRequests");
            } else
            {
                TempData["message"] = "Nu exista profilul caruia doriti sa ii acceptati cererea de urmarire!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
}

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult DenyRequest(string id)
        {
            // practic, scot din tabelul de requests cererea
            var request = db.FollowRequests.Where(us => us.RequestedProfileId == um.GetUserId(User) && us.RequestingProfileId == id);
            if (request.Any())
            {
                db.FollowRequests.Remove(request.First());
                db.SaveChanges();
                return RedirectToAction("ViewRequests");
            } else
            {
                TempData["message"] = "Nu exista profilul caruia doriti sa ii respingeti cererea de urmarire!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult UnFollow(string id)
        {
            // practic, scot din tabelul de follows cererea
            var follow = db.Follows.Where(us => us.FollowedProfileId == id && us.FollowingProfileId == um.GetUserId(User));
            if (follow.Any())
            {
                db.Follows.Remove(follow.First());
                db.SaveChanges();
                return RedirectToAction("Show", new {id = id});
            } else
            {
                TempData["message"] = "Nu exista profilul pe care doriti sa nu il mai urmariti!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(string id)
        {
            var user = db.Users.Where(us => us.Id == id);
            if (user.Any()){
                int followedCount = db.Follows.Where(us => us.FollowedProfileId == id).Count();
                int followingCount = db.Follows.Where(us => us.FollowingProfileId == id).Count();
                var follow = db.Follows.Where(us => us.FollowedProfileId == user.First().Id && us.FollowingProfileId == um.GetUserId(User));
                var request = db.FollowRequests.Where(us => us.RequestedProfileId == user.First().Id && us.RequestingProfileId == um.GetUserId(User));
                if ((user.First().isPrivate && (!User.IsInRole("Admin") && um.GetUserId(User) != id && follow.Count() == 0))) {
                    // privat
                    ViewBag.canSeeProfile = false;
                }
                else {
                    // public
                    ViewBag.canSeeProfile = true;
                    ViewBag.followedCount = followedCount == 0 ? "0" : followedCount.ToString();
                    ViewBag.followingCount = followingCount == 0 ? "0" : followingCount.ToString();
                }
                ViewBag.UserCurent = um.GetUserId(User);

                if (follow.Count() == 1)
                {
                    ViewBag.StatusCerere = "Urmarind";
                } else if (request.Count() == 1)
                {
                    ViewBag.StatusCerere = "Cerere";
                } else
                {
                    ViewBag.StatusCerere = "Fara";
                }
                return View(user.First());
            } else
            {
                TempData["message"] = "Ati incercat accesarea unui profil inexistent!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(string id)
        {
            var profile = db.Profiles.Include("Posts").Include("Posts.Comments").Include("Comments").Where(x => x.Id == id);
            if (profile.Any())
            {
                if (profile.First().Id == um.GetUserId(User) || User.IsInRole("Admin"))
                {
                    db.Profiles.Remove(profile.First());
                    db.SaveChanges();
                    TempData["message"] = "Profilul a fost sters cu succes!";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa stergeti profilul!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["message"] = "Ati incercat stergerea unui profil care nu exista!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }
    }
}
