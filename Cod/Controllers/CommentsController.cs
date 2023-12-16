using Cod.Data;
using Cod.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cod.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> um;

        public CommentsController(ApplicationDbContext _db, UserManager<ApplicationUser> _um)
        {
            db = _db;
            um = _um;
        }

        // TODO debug method

        public IActionResult Index()
        {
            var comments = db.Comments.Include("Profile").OrderBy(x => x.CreationDate);
            ViewBag.Comments = comments;

            return View();
        }

        [HttpGet]
        public IActionResult Show(int id)
        {
            Comment comment = db.Comments.Include("Profile").Where(x => x.Id == id).First();
            return View(comment);
        }

        // TODO link comment with post

        [HttpPost]
        public IActionResult Show([FromForm] Comment comment)
        {
            return View(comment);
        }

        [HttpGet]
        public IActionResult New(int id)
        {
            Comment comment = new Comment();
            return View(comment);
        }

        [HttpPost]
        public IActionResult New(Comment comment)
        {
            // TODO validate ModelState.IsValid
            // brute force values
            comment.ProfileId = um.GetUserId(User);
            comment.CreationDate = DateTime.Now;
            // TODO hardcodat
            comment.PostId = 2;
            db.SaveChanges();

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            } else
            {
                return View(comment);
            }
        }

        // TODO check if user that made the post is the user trying to modify the post

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Comment comment = db.Comments.Where(x => x.Id == id).First();

            return View(comment);
        }

        [HttpPost]
        public IActionResult Edit(int id, Comment reqComment)
        {
            Comment comment = db.Comments.Find(id);
            comment.Content = reqComment.Content;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Comment comment = db.Comments.Where(x => x.Id == id).First();
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
