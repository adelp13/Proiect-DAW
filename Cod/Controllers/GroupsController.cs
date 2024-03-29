﻿using Cod.Data;
using Cod.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
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

            var groups = db.Groups;
            ViewBag.Groups = groups;
            return View();
        }

        public ActionResult Show(int id)
        {
            var group = db.Groups.Include("Posts").Include("Posts.Profile")
                              .Where(gr => gr.Id == id).OrderBy(x => x.CreationDate);
            if (group.Any())
            {
                if (TempData["message"] != null)
                {
                    ViewBag.message = TempData["message"];
                    ViewBag.messageType = TempData["messageType"];
                }
                ViewBag.isInGroup = db.UserGroup.Where(x => x.ProfileId == um.GetUserId(User) && x.GroupId == id).Any();
                return View(group.First());
            } else
            {
                TempData["message"] = "Ati incercat accesarea unui grup inexistent!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
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
            if (group != null)
            {
                if (User.IsInRole("Admin")){
                    return View(group);
                } else
                {
                    TempData["message"] = "Nu aveti dreptul sa modificati acest grup!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Groups");
                }
            } else
            {
                TempData["message"] = "Ati incercat modificarea unui grup inexistent!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
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
            var group = db.Groups.Include("Posts").Include("Posts.Comments").Where(x => x.Id == id);
            if (group.Any())
            {
                if (User.IsInRole("Admin"))
                {
                    db.Groups.Remove(group.First());
                    ImmutableList<ProfileGroup> assoc = db.UserGroup.Where(x => x.GroupId == id).ToImmutableList();
                    foreach (ProfileGroup elem in assoc)
                    {
                        db.UserGroup.Remove(elem);
                    }
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
            } else
            {
                TempData["message"] = "Ati incercat stergerea unui grup inexistent!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
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
                    return RedirectToAction("Show", new {id = id});
                }
                else // intru pe grup
                {
                    // il adaug
                    ProfileGroup newItem = new ProfileGroup { ProfileId = um.GetUserId(User), GroupId = id };
                    db.UserGroup.Add(newItem);
                    db.SaveChanges();
                    return RedirectToAction("Show", new {id = id});
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