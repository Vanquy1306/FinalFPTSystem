using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Training_FPT0.Models;

namespace Training_FPT0.Controllers
{
    public class CategoriesController : Controller
    {
		private ApplicationDbContext _context;
		public CategoriesController()
		{
			_context = new ApplicationDbContext();
		}
		// GET: Category
		[HttpGet]
		[Authorize(Roles = "TrainingStaff")]
		public ActionResult Index(string searchCategory)
		{
			var categories = _context.Categories.ToList();

			if (!String.IsNullOrEmpty(searchCategory))
			{
				categories = categories.FindAll(s => s.Name.Contains(searchCategory));
			}

			return View(categories);
		}


		[HttpGet]
		[Authorize(Roles = "TrainingStaff")]
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Authorize(Roles = "TrainingStaff")]
		public ActionResult Create(Category category)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			if (_context.Categories.Any(p => p.Name.Contains(category.Name)))
			{
				ModelState.AddModelError("Name", "Category Name Already Exists.");
				return View();
			}

			var newCategory = new Category
			{
				Name = category.Name,
				Description = category.Description,


			};

			_context.Categories.Add(newCategory);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(Roles = "TrainingStaff")]
		public ActionResult Delete(int id)
		{
			var categoryInDb = _context.Categories.SingleOrDefault(p => p.Id == id);

			if (categoryInDb == null)
			{
				return HttpNotFound();
			}

			_context.Categories.Remove(categoryInDb);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(Roles = "TrainingStaff")]
		public ActionResult Edit(int id)
		{
			var categoryInDb = _context.Categories.SingleOrDefault(p => p.Id == id);

			if (categoryInDb == null)
			{
				return HttpNotFound();
			}

			return View(categoryInDb);
		}

		[HttpPost]
		[Authorize(Roles = "TrainingStaff")]
		public ActionResult Edit(Category category)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			var categoryInDb = _context.Categories.SingleOrDefault(p => p.Id == category.Id);

			if (categoryInDb == null)
			{
				return HttpNotFound();
			}

			categoryInDb.Name = category.Name;
			categoryInDb.Description = category.Description;

			_context.SaveChanges();

			return RedirectToAction("Index");
		}
		[HttpGet]
		[Authorize(Roles = "TrainingStaff")]
		public ActionResult Details(int id)
		{
			var cateInDb = _context.Categories.SingleOrDefault(p => p.Id == id);

			if (cateInDb == null)
			{
				return HttpNotFound();
			}

			return View(cateInDb);
		}
	}
}