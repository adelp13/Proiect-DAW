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
                TempData["message"] = "Ati incercat sa adaugati un comentariu fara sa precizati o postare!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Show","Posts", new {id = id});
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                TempData["message"] = "Ati incercat sa adaugati un comentariu la o postare inexistenta!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Show", "Posts", new { id = id });
            }
            Comment comment = new Comment();
            comment.PostId = id;
            return View(comment);
        }

        [HttpPost]
        public IActionResult New([FromForm] Comment comment)
        {
            comment.ProfileId = um.GetUserId(User);
            comment.CreationDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                TempData["message"] = "Comentariul a fost adaugat cu succes!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", "Posts", new { id = comment.PostId });
            } else
            {
                return View(comment);
            }
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            var comment = db.Comments.Where(x => x.Id == id);

            if (comment.Any())
            {
                if (comment.First().ProfileId == um.GetUserId(User) || User.IsInRole("Admin"))
                {
                    return View(comment.First());
                }
                else
                {
                    TempData["message"] = "Nu puteti modifica acest comentariu!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Show", "Posts", new { id = comment.First().PostId });
                }
            } else
            {
                TempData["message"] = "Ati incercat modificarea unui comentariu inexistent!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Show", "Posts");
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
                    TempData["message"] = "Comentariul a fost modificat cu succes!";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Show", "Posts", new { id = comment.PostId });
                }
                else
                {
                    TempData["message"] = "Nu puteti modifica acest comentariu!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Show", "Posts", new { id = comment.PostId });
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
            var comment = db.Comments.Where(x => x.Id == id);
            if (comment.Any())
            {
                if (comment.First().ProfileId == um.GetUserId(User) || User.IsInRole("Admin"))
                {
                    int? idpostare = comment.First().PostId;
                    db.Comments.Remove(comment.First());
                    db.SaveChanges();
                    TempData["message"] = "Comentariul a fost sters cu succes!";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Show", "Posts", new { id = idpostare });
                } else
                {
                    TempData["message"] = "Nu puteti sterge acest comentariu!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Show", "Posts", new { id = comment.First().PostId });
                }
            } else
            {
                TempData["message"] = "Ati incercat stergerea unui comentariu care nu exista!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Posts");
            }
        }
    }
}
