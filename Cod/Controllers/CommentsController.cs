using Cod.Data;
using Cod.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cod.Controllers
{
    public class CommentsController : Controller
    {
        // manager de useri si roluri
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> um;
        private readonly RoleManager<IdentityRole> rm;
        public CommentsController(ApplicationDbContext _db, UserManager<ApplicationUser> _um, RoleManager<IdentityRole> _rm)
        {
            db = _db;
            um = _um;
            rm = _rm;
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index","Posts");
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return RedirectToAction("Index","Posts");
            }
            Comment comment = new Comment();
            comment.PostId = id;
            return View(comment);
        }

        [HttpPost]
        public IActionResult New([FromForm] Comment comment)
        {
            // TODO validate ModelState.IsValid
            comment.ProfileId = um.GetUserId(User);
            comment.CreationDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index", "Posts");
            } else
            {
                return View(comment);
            }
        }

        // TODO check if user that made the post is the user trying to modify the post

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            Comment comment = db.Comments.Where(x => x.Id == id).First();

            if (comment.ProfileId == um.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(comment);
            }
            else
            {
                TempData["message"] = "Nu puteti modifica acest comentariu!";
                TempData["messageType"] = "alert-danger";
                // TODO where redirect?
                return RedirectToAction("/Posts/Show/" + comment.PostId);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Comment reqComment)
        {
            Comment comment = db.Comments.Find(id);

            if (ModelState.IsValid)
            {
                if (comment.ProfileId == um.GetUserId(User) || User.IsInRole("Admin"))
                {

                    comment.Content = reqComment.Content;
                    db.SaveChanges();
                    TempData["message"] = "Comentariul a fost modificat!";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Index", "Posts");
                }
                else
                {
                    TempData["message"] = "Nu puteti modifica acest comentariu!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Posts");
                }
            }
            else
            {
                return View(reqComment);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int id)
        {
            Comment comment = db.Comments.Where(x => x.Id == id).First();
            if (comment.ProfileId == um.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comment);
                db.SaveChanges();
                TempData["message"] = "Comentariul a fost sters!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index", "Posts");
            } else
            {
                TempData["message"] = "Nu puteti sterge acest comentariu!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Posts");
            }
        }
    }
}
