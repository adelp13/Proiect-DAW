using Cod.Data;
using Cod.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cod.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext db;

        public GroupsController(ApplicationDbContext context)
        {
            db = context;
        }
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var groups = db.Groups.Include("Posts").Include("Posts.Comments");
            ViewBag.Groups = groups;
            return View();
        }

        public ActionResult Show(int id)
        {
            Group group = db.Groups.Include("Posts").Include("Posts.Comments")
                              .Where(gr => gr.Id == id)
                              .First();
            return View(group);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Group gr)
        {
            gr.CreationDate = DateTime.Now;

            try
            {
                db.Groups.Add(gr);
                db.SaveChanges();
                TempData["message"] = "Gr";
                return RedirectToAction("Index");
            }

            catch (Exception e)
            {
                return View(gr);
            }
        }

        public ActionResult Edit(int id)
        {
            Group group = db.Groups.Find(id);
            return View(group);
        }

        [HttpPost]
        public ActionResult Edit(int id, Group groupToEdit)
        {
            Group group = db.Groups.Find(id);

            try
            {
                group.Name = groupToEdit.Name;
                group.Description = groupToEdit.Description;
                group.CreationDate = groupToEdit.CreationDate;
                db.SaveChanges();
                TempData["message"] = "Group modified!";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View(groupToEdit);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
            db.SaveChanges();
            TempData["message"] = "Group has been deleted";
            return RedirectToAction("Index");
        }
    }
}

