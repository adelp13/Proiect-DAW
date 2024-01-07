using Cod.Data;
using Cod.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cod.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> um;

        public GroupsController(ApplicationDbContext context, UserManager<ApplicationUser> _um)
        {
            db = context;
            um = _um;
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
                TempData["message"] = "Grupul a fost adaugat cu succes!";
                TempData["messageType"] = "alert-success";
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
                TempData["message"] = "Grupul a fost modificat cu succes!";
                TempData["messageType"] = "alert-success";
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
            TempData["message"] = "Grupul a fost sters cu succes!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }

        private void SetAccessRights()
        {
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.UserCurent = um.GetUserId(User);
        }

        [HttpPost]
        public void JoinGroup(int id)
        {
            var group = db.Groups.Where(x => x.Id == id);

            if (group.Any())
            {
                // verific sa nu fiu deja adaugat in acel grup
            
            }
        }
    }
}

