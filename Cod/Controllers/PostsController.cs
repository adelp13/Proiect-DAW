using Cod.Data;
using Cod.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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

        public IActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.messageType = TempData["messageType"].ToString();
            }

            var posts = db.Posts.Include("Profile").Include("Comments").Include("Group").Include("Comments.Profile").OrderBy(x => x.CreationDate);
            ViewBag.Posts = posts;

            return View();
        }

        [HttpGet]
        public IActionResult Show(int id)
        {
            Post post = db.Posts.Include("Profile").Include("Comments").Include("Group").Include("Comments.Profile").Where(x => x.Id == id).First();
            return View(post);
        }

        [HttpPost]
        public IActionResult Show([FromForm] Post post)
        {
            return View(post);
        }


        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Groups");
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return RedirectToAction("Index", "Groups");
            }
            // TODO inca pot posta ca user care nu a intrat in grup
            // TODO daca ies din grup ce se intampla?
            Post post = new Post();
            post.GroupId = id;
            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New([FromForm] Post post)
        {
            post.ProfileId = um.GetUserId(User);
            post.CreationDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                TempData["message"] = "Postarea a fost adaugata cu succes!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index", "Groups");
            } else
            {
                return View(post);
            }
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            Post post = db.Posts.Include("Profile").Include("Group").Where(x => x.Id == id).First();

            if (post.ProfileId == um.GetUserId(User) || User.IsInRole("Admin"))
                return View(post);
            else
            {
                TempData["message"] = "Nu aveti dreptul sa modificati postarea!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Post reqPost)
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
                    return RedirectToAction("Index");
                } else
                {
                    TempData["message"] = "Nu aveti dreptul sa modificati postarea!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            } else
            {
                return View(reqPost);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int id)
        {
            Post post = db.Posts.Where(x => x.Id == id).First();
            if (post.ProfileId == um.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Posts.Remove(post);
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
        }
    }
}
