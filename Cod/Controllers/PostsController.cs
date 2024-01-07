using Cod.Data;
using Cod.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.ObjectModelRemoting;
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

        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
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

        // TODO link comment with post

        [HttpPost]
        public IActionResult Show([FromForm] Post post)
        {
            return View(post);
        }

        // TODO inca pot posta ca user neintregistrat

        [HttpGet]
        public IActionResult New(int id)
        {
            Post post = new Post();
            post.Groups = GetAllGroups();
            return View(post);
        }

        [HttpPost]
        public IActionResult New(Post post)
        {
            post.ProfileId = um.GetUserId(User);
            post.CreationDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            } else
            {
                post.Groups = GetAllGroups();
                return View(post);
            }
            // TODO validate ModelState.IsValid
            // brute force values
            // TODO hardcodat
        }

        // TODO check if user that made the post is the user trying to modify the post

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Post post = db.Posts.Include("Profile").Include("Group").Where(x => x.Id == id).First();
            post.Groups = GetAllGroups();
            return View(post);
        }

        // TODO when I press edit post, I still get a warning message

        [HttpPost]
        public IActionResult Edit(int id, Post reqPost)
        {
            Post post = db.Posts.Find(id);
            if (ModelState.IsValid)
            {
                post.Content = reqPost.Content;
                post.GroupId = reqPost.GroupId;
                db.SaveChanges();
                return RedirectToAction("Index");
            } else
            {
                post.Groups = GetAllGroups();
                return View(post);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Post post = db.Posts.Where(x => x.Id == id).First();
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllGroups()
        {
            var selectList = new List<SelectListItem>();
            var groups = from gr in db.Groups select gr;

            foreach (var group in groups)
            {
                selectList.Add(new SelectListItem
                {
                    Value = group.Id.ToString(),
                    Text = group.Name
                });
            }
            return selectList;
        }
    }
}
