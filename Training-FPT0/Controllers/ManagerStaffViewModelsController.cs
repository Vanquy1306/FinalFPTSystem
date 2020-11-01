using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Training_FPT0.Models;
using Training_FPT0.ViewModels;

namespace Training_FPT0.Controllers
{
    public class ManagerStaffViewModelsController : Controller
    {
        ApplicationDbContext _context;
        public ManagerStaffViewModelsController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: ManagerStaffViewModels
        [HttpGet]
        [Authorize(Roles = "TrainingStaff")]
        public ActionResult Index()
        {
        // declare role variable using the function (FROM-IN) to specify the query data source (Roles).
        // Look in the name column in the Roles table containing "Trainee" and assign it to the variable
        var role = (from r in _context.Roles where r.Name.Contains("Trainee") select r).FirstOrDefault();
            // declare variable users with function (FROM-IN) to specify the query data source (Users).
            //Find the user in the Users table whose "RoleId" was assigned when registering 
            //containing the "role.Id" selected in the above variable.
            var users = _context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(role.Id)).ToList();
            // declare variable userVM to assign the users selected on the variable "users"
            //to "ManagerStaffViewModel" to display
            var userVM = users.Select(user => new ManagerStaffViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                RoleName = "Trainee",
                UserId = user.Id
            }).ToList();

            // declare role2 variable using the function (FROM-IN) to specify the query data source (Roles).
            // Look in the name column in the Roles table containing "Trainer" and assign it to the variable
            var role2 = (from r in _context.Roles where r.Name.Contains("Trainer") select r).FirstOrDefault();
            // declare variable admins with function (FROM-IN) to specify the query data source (Users).
            //Find the user in the Users table whose "RoleId" was assigned when registering 
            //containing the "role2.Id" selected in the above variable.
            var admins = _context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(role2.Id)).ToList();
            // declare variable adminVM to assign the users selected on the variable "admins"
            //to "ManagerStaffViewModel" to display
            var adminVM = admins.Select(user => new ManagerStaffViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                RoleName = "Trainer",
                UserId = user.Id
            }).ToList();


            var model = new ManagerStaffViewModel { Trainee = userVM, Trainer = adminVM };
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "TrainingStaff")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var appUser = _context.Users.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        [HttpPost]
        [Authorize(Roles = "TrainingStaff")]
        public ActionResult Edit(ApplicationUser user)
        {
            var userInDb = _context.Users.Find(user.Id);

            if (userInDb == null)
            {
                return View(user);
            }

            if (ModelState.IsValid)
            {
                userInDb.Name = user.Name;
                userInDb.UserName = user.UserName;
                userInDb.Phone = user.Phone;
                userInDb.Email = user.Email;


                _context.Users.AddOrUpdate(userInDb);
                _context.SaveChanges();

                return RedirectToAction("Index", "ManagerStaffViewModels");
            }
            return View(user);

        }

        [Authorize(Roles = "TrainingStaff")]
        public ActionResult Delete(string id)
        {
            var userInDb = _context.Users.SingleOrDefault(p => p.Id == id);

            if (userInDb == null)
            {
                return HttpNotFound();
            }
            _context.Users.Remove(userInDb);
            _context.SaveChanges();

            return RedirectToAction("Index", "ManagerStaffViewModels");

        }
        [Authorize(Roles = "TrainingStaff")]
        public ActionResult ResetPass(ApplicationUser user)
        {
            // Declare the userId variable of Current.User.Identity and access the Id field through GetUserId
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            userId = user.Id;
            if (userId != null)
            {
                // userManager by managing new users, bringing new data
                UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                // Delete current password of userManager
                userManager.RemovePassword(userId);
                // Replace new password "A456456a @" for userManager
                String newPassword = "A456456a@";
                userManager.AddPassword(userId, newPassword);
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "ManagerStaffViewModels");
        }
    }
}