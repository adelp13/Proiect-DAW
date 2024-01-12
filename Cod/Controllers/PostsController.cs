using Cod.Data;
using Cod.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Text.RegularExpressions;

namespace Cod.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> um;

        public PostsController(ApplicationDbContext _db, UserManager<ApplicationUser> _um)
        {
            db = _db;
            um = _um;
        }

        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.messageType = TempData["messageType"].ToString();
            }

            var posts = db.Posts.Include("Profile").Include("Comments").Include("Group").Include("Comments.Profile").OrderBy(x => x.CreationDate);
            ViewBag.Posts = posts;
            ViewBag.UserCurent = um.GetUserId(User);

            return View();
        }

        [HttpGet]
        public ActionResult Show(int id)
        {
            var post = db.Posts.Include("Profile").Include("Comments").Include("Group").Include("Comments.Profile").Where(x => x.Id == id);
            if (post.Any())
            {   
                ViewBag.UserCurent = um.GetUserId(User);
                return View(post.First());
            } else
            {
                TempData["message"] = "Ati incercat accesarea unei postari inexistente!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        // TODO de ce am nevoie de asta?
        [HttpPost]
        public ActionResult Show([FromForm] Post post)
        {
            return View(post);
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(int? id)
        {
            if (id == null)
            {
                TempData["message"] = "Ati incercat sa adaugati o postare fara sa precizati un grup!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Groups");
            }
            var group = db.Groups.Find(id);
            if (group == null)
            {
                TempData["message"] = "Ati incercat sa adaugati o postare la un grup inexistent!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Groups");
            }
            if (db.UserGroup.Where(x => x.ProfileId == um.GetUserId(User) && x.GroupId == id).Any() || User.IsInRole("Admin"))
            {
                Post post = new Post();
                post.GroupId = id;
                return View(post);
            } else
            {
                TempData["message"] = "Nu puteti posta in acest grup! Alaturati-va lui mai intai!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Show", "Groups", new { id = id });
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New([FromForm] Post post)
        {
            post.ProfileId = um.GetUserId(User);
            post.CreationDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                TempData["message"] = "Postarea a fost adaugata cu succes!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", "Groups", new {id = post.GroupId});
            } else
            {
                return View(post);
            }
        }

        [HttpGet]
        // TODO check if data exists!
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id)
        {
            var post = db.Posts.Include("Profile").Include("Group").Where(x => x.Id == id);

            if (post.Any())
            {
                if (post.First().ProfileId == um.GetUserId(User) || User.IsInRole("Admin"))
                    return View(post.First());
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa modificati postarea!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }

            } else
            {
                TempData["message"] = "Ati incercat modificarea unei postari inexistente!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id, Post reqPost)
        {
            Post post = db.Posts.Find(id);
            if (ModelState.IsValid)
            {
                if (post.ProfileId == um.GetUserId(User) || User.IsInRole("Admin"))
                {
                    post.Content = reqPost.Content;
                    post.GroupId = reqPost.GroupId;
                    db.SaveChanges();
                    TempData["message"] = "Postarea a fost modificata cu succes!";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Show", "Posts", new { id = id });
                } else
                {
                    TempData["message"] = "Nu aveti dreptul sa modificati postarea!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Show", "Posts", new { id = id });
                }
            } else
            {
                return View(reqPost);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            var post = db.Posts.Include("Comments").Where(x => x.Id == id);
            if (post.Any())
            {
                if (post.First().ProfileId == um.GetUserId(User) || User.IsInRole("Admin"))
                {
                    db.Posts.Remove(post.First());
                    db.SaveChanges();
                    TempData["message"] = "Postarea a fost stearsa cu succes!";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa stergeti postarea!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            } else
            {
                TempData["message"] = "Ati incercat stergerea unei postari care nu exista!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }
    }
}
