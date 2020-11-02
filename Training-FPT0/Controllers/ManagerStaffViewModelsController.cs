
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Training_FPT0.Models;
using Training_FPT0.ViewModels;

namespace FTMS.Controllers
{
    public class ManagerStaffViewModelsController : Controller
    {
        private ApplicationDbContext _context;
        public ManagerStaffViewModelsController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Admin
        [Authorize(Roles = "TrainingStaff")]
        public ActionResult Index()
        {
            var userInfor = (from user in _context.Users
                             select new
                             {
                                 UserId = user.Id,
                                 Username = user.UserName,
                                 EmailAddress = user.Email,
                                 RoleName = (from userRole in user.Roles
                                             join role in _context.Roles
                                             on userRole.RoleId
                                             equals role.Id
                                             select role.Name)
                             }
                       ).ToList().Select(p => new Users_In_Role()
                       {
                           UserId = p.UserId,
                           Username = p.Username,
                           Email = p.EmailAddress,
                           Role = string.Join(",", p.RoleName)
                       }
                       );
            return View(userInfor);
        }
        //DELETE ACCOUNT
        [HttpGet]
        [Authorize(Roles = "TrainingStaff")]
        public ActionResult Delete(string id)
        {
            var AccountInDB = _context.Users.SingleOrDefault(p => p.Id == id);

            if (AccountInDB == null)
            {
                return HttpNotFound();
            }

            _context.Users.Remove(AccountInDB);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        //Edit 
        [HttpGet]
        [Authorize(Roles = "TrainingStaff")]
        public ActionResult Edit(string id)
        {
            var AccountInDB = _context.Users.SingleOrDefault(p => p.Id == id);
            if (AccountInDB == null)
            {
                return HttpNotFound();
            }
            return View(AccountInDB);
        }

        //EDIT
        [HttpPost]
        [Authorize(Roles = "TrainingStaff")]
        public ActionResult Edit(ApplicationUser user)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var AccountInDB = _context.Users.SingleOrDefault(p => p.Id == user.Id);

            if (AccountInDB == null)
            {
                return HttpNotFound();
            }

            AccountInDB.UserName = user.UserName;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        //RESET PASSWORD
        [HttpGet]
        [Authorize(Roles = "TrainingStaff")]
        public ActionResult ResetPass(string id)
        {
            var AccountInDB = _context.Users.SingleOrDefault(p => p.Id == id);

            if (AccountInDB == null)
            {
                return HttpNotFound();
            }
              if (AccountInDB.Id != null)
            {
                UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                userManager.RemovePassword(AccountInDB.Id);
                String newPassword = "A456456a@";
                userManager.AddPassword(AccountInDB.Id, newPassword);
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}