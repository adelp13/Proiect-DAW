using Cod.Data;
using Cod.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var posts = db.Posts.OrderBy(x => x.CreationDate);
            ViewBag.Posts = posts;

            return View();
        }

        [HttpGet]
        public IActionResult Show(int id)
        {
            Post post = db.Posts.Where(x => x.Id == id).First();
            return View(post);
        }

        // TODO link comment with post

        [HttpPost]
        public IActionResult Show([FromForm] Post post)
        {
            return View(post);
        }

        [HttpGet]
        public IActionResult New(int id)
        {
            Post post = new Post();
            return View(post);
        }

        [HttpPost]
        public IActionResult New(Post post)
        {
            // TODO validate ModelState.IsValid
            // brute force values
            post.ProfileId = um.GetUserId(User);
            post.CreationDate = DateTime.Now;
            // TODO hardcodat
            post.GroupId = 1;
            db.Posts.Add(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // TODO check if user that made the post is the user trying to modify the post

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Post post = db.Posts.Where(x => x.Id == id).First();

            return View(post);
        }

        [HttpPost]
        public IActionResult Edit(int id, Post reqPost)
        {
            Post post = db.Posts.Find(id);
            post.Content = reqPost.Content;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Post post = db.Posts.Where(x => x.Id == id).First();
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
