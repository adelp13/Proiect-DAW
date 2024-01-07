using Cod.Data;
using Cod.Models;
using Microsoft.AspNetCore.Authorization;
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
                ViewBag.messageType = TempData["messageType"].ToString();
            }

            var groups = db.Groups.Include("Posts").Include("Posts.Profile");
            ViewBag.Groups = groups;
            return View();
        }

        public ActionResult Show(int id)
        {
            Group group = db.Groups.Include("Posts").Include("Posts.Profile")
                              .Where(gr => gr.Id == id)
                              .First();
            return View(group);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            if (User.IsInRole("Admin"))
            {
                return View();
            } else
            {
                TempData["message"] = "Nu aveti dreptul sa adaugati grupuri!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Group gr)
        {
            gr.CreationDate = DateTime.Now;

            if(ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    db.Groups.Add(gr);
                    db.SaveChanges();
                    TempData["message"] = "Grupul a fost adaugat cu succes!";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Index");
                } else
                {
                    TempData["message"] = "Nu aveti dreptul sa adaugati grupuri!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(gr);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Group group = db.Groups.Find(id);
            if (User.IsInRole("Admin")){
                return View(group);
            } else
            {
                TempData["message"] = "Nu puteti modifica acest grup!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Groups");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, [FromForm] Group groupToEdit)
        {
            Group group = db.Groups.Find(id);

            if(ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    group.Name = groupToEdit.Name;
                    group.Description = groupToEdit.Description;
                    db.SaveChanges();
                    TempData["message"] = "Grupul a fost modificat cu succes!";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu puteti modifica acest grup!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Groups");
                }
            }
            else
            {
                return View(groupToEdit);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            if (User.IsInRole("Admin"))
            {
                Group group = db.Groups.Find(id);
                db.Groups.Remove(group);
                db.SaveChanges();
                TempData["message"] = "Grupul a fost sters cu succes!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            } else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti grupul!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Groups");
            }
        }


        [NonAction]
        public bool isInGroup(int groupId)
        {
            return db.UserGroup.Where(x => x.ProfileId == um.GetUserId(User) && x.GroupId == groupId).Any();
        }

        [HttpPost]
        [Authorize(Roles ="User,Admin")]
        public ActionResult GroupJoin(int id)
        {
            Group group = db.Groups.Find(id);

            if (group != null)
            {
                // verific sa nu fiu deja adaugat in acel grup
                var userGroup = db.UserGroup.Where(x => x.ProfileId == um.GetUserId(User) && x.GroupId == id);

                // sunt deja in grup, inseamna ca ies
                if (userGroup.Any())
                {
                    // TODO cand ies din grup, toate postarile mele si comentariile aferente vor fi sterse
                    ProfileGroup remove = userGroup.First();
                    db.UserGroup.Remove(remove);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else // intru pe grup
                {
                    // il adaug
                    ProfileGroup newItem = new ProfileGroup { ProfileId = um.GetUserId(User), GroupId = id };
                    db.UserGroup.Add(newItem);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // bad group request
                TempData["message"] = "Nu exista grupul pe care ati dorit sa il accesati!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }
    }
}