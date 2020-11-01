using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Training_FPT0.Models;
using System.Data.Entity;
using Training_FPT0.ViewModels;

namespace Training_FPT0.Controllers
{
    public class TraineeCoursesController : Controller
    {
        private ApplicationDbContext _context;

        public TraineeCoursesController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            if (User.IsInRole("TrainingStaff"))
            {
                var traineecourses = _context.TraineeCourses.Include(t => t.Course).Include(t => t.Trainee).ToList();
                return View(traineecourses);
            }
            if (User.IsInRole("Trainee"))
            {
                var traineeId = User.Identity.GetUserId();
                var Res = _context.TraineeCourses.Where(e => e.TraineeId == traineeId).Include(t => t.Course).ToList();
                return View(Res);
            }
            return View("Login");
        }
        [Authorize(Roles = "TrainingStaff")]

        public ActionResult Create()
        {
            //get trainee
            var role = (from r in _context.Roles where r.Name.Contains("Trainee") select r).FirstOrDefault();
            var users = _context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(role.Id)).ToList();

            //get Course

            var courses = _context.Courses.ToList();

            var TraineeCourseVM = new TraineeCourseViewModel()
            {
                Courses = courses,
                Trainees = users,
                TraineeCourse = new TraineeCourse()
            };

            return View(TraineeCourseVM);
        }
        [Authorize(Roles = "TrainingStaff")]
        [HttpPost]
        public ActionResult Create(TraineeCourse traineeCourse)
        {
         
            if (!ModelState.IsValid)
            {
                return View();
            }
            var checkTraineeCourses = _context.TraineeCourses.Any(c => c.TraineeId == traineeCourse.TraineeId &&
                                                                      c.CourseId == traineeCourse.CourseId);
               //Check if Trainer Name or Topic Name existed or not
            if (checkTraineeCourses == true)
            {
                return View("~/Views/TraineeCourses/AssignExistTraineeCourse.cshtml");
            }
            var newTraineeCourse = new TraineeCourse
            {
                TraineeId = traineeCourse.TraineeId,
                CourseId = traineeCourse.CourseId
            };

            _context.TraineeCourses.Add(newTraineeCourse);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize(Roles = "TrainingStaff")]
        public ActionResult Edit(int id)
        {
            var tcInDb = _context.TraineeCourses.SingleOrDefault(p => p.Id == id);
            if (tcInDb == null)
            {
                return HttpNotFound();
            }
            var viewModel = new TraineeCourseViewModel
            {
                TraineeCourse = tcInDb,
                Courses = _context.Courses.ToList(),

            };

            return View(viewModel);
        }
        [HttpPost]
        [Authorize(Roles = "TrainingStaff")]
        public ActionResult Edit(TraineeCourse traineeCourse)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            var tcInDb = _context.TraineeCourses.SingleOrDefault(p => p.Id == traineeCourse.Id);
            if (tcInDb == null)
            {
                return HttpNotFound();
            }
            tcInDb.CourseId = traineeCourse.CourseId;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "TrainingStaff")]
        public ActionResult Delete(int id)
        {
            var traineecourseInDb = _context.TraineeCourses.SingleOrDefault(p => p.Id == id);

            if (traineecourseInDb == null)
            {
                return HttpNotFound();
            }
            _context.TraineeCourses.Remove(traineecourseInDb);
            _context.SaveChanges();

            return RedirectToAction("Index");

        }
    }
}