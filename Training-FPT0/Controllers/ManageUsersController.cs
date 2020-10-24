using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Training_FPT0.Models;
using Training_FPT0.ViewModels;

namespace Training_FPT0.Controllers
{
    public class ManageUsersController : Controller
    {
        private ApplicationDbContext _context;
        public ManageUsersController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: ManageUsers
        public ActionResult UsersWithRoles()
        {
            //declare variable usersWithRoles using the (FROM-IN) function
            //to define the query data source (Users).
            var usersWithRoles = (from user in _context.Users
                                  select new // SELECT: indicates the data exported from the source
                                  {
                                      UserId = user.Id,
                                      Name = user.Name,
                                      Username = user.UserName,
                                      Emailaddress = user.Email,
                                      RoleNames = (from userRole in user.Roles //from-in function selects external data source
                                                   join role in _context.Roles //join-in function selects the data source inside
                                                   on userRole.RoleId //Data columns are connected between 2 tables
                                                   equals role.Id //fetching the data using on condition (role.Id)
                                                   select role.Name).ToList() //Select the value you want to display
                                  }).ToList().Select(p => new Users_In_Role() //Attaches the selected values to the viewmodel

                                  {
                                      UserId = p.UserId,
                                      Name = p.Name,
                                      Username = p.Username,
                                      Email = p.Emailaddress,
                                      Role = string.Join(",", p.RoleNames) //(join) Combine the selected value above to display
                                  });


            return View(usersWithRoles);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
                userInDb.WorkingPlace = user.WorkingPlace;
                userInDb.Email = user.Email;


                _context.Users.AddOrUpdate(userInDb);
                _context.SaveChanges();

                return RedirectToAction("UsersWithRoles");
            }
            return View(user);
        }

        [Authorize(Roles = "Admin")]
              public ActionResult Delete(string id)
        {
            var userInDb = _context.Users.SingleOrDefault(p => p.Id == id);

            if (userInDb == null)
            {
                return HttpNotFound();
            }
                _context.Users.Remove(userInDb);
                _context.SaveChanges();

                return RedirectToAction("UsersWithRoles");
            
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Details(string id)
        {
            var usersInDb = _context.Users.SingleOrDefault(p => p.Id == id);

            if (usersInDb == null)
            {
                return HttpNotFound();
            }

            return View(usersInDb);
        }
        [Authorize(Roles = "Admin")]
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
            return RedirectToAction("UsersWithRoles", "ManageUsers");
        }
    }
}