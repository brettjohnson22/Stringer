using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Domain;
using System.Security.Claims;
using Infrastructure;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Application;

namespace Stringer.Controllers
{
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemberService _memberService;

        public MembersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMemberService memberService)
        {
            _context = context;
            _userManager = userManager;
            _memberService = memberService;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["TimeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "time_asc" : "";
            ViewData["TypeSortParm"] = sortOrder == "type" ? "type_desc" : "type";
            ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "name";
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var myKnots = _context.Knots.Include(k => k.Location).Where(k => k.ApplicationUserId == userId);
            if (myKnots == null)
            {
                return RedirectToAction("CreateProfile");
            }
            else
            {
                var topInterest = _context.Interests.Single(i => i.Id == user.TopInterest);
                var secondInterest = _context.Interests.Single(i => i.Id == user.SecondInterest);
                ViewBag.TopInterest = topInterest.Name;
                ViewBag.SecondInterest = secondInterest.Name;
                switch (sortOrder)
                {
                    case "name":
                        myKnots = myKnots.OrderBy(k => k.Location.Name);
                        break;
                    case "name_desc":
                        myKnots = myKnots.OrderByDescending(k => k.Location.Name);
                        break;
                    case "type":
                        myKnots = myKnots.OrderBy(k => k.Type);
                        break;
                    case "type_desc":
                        myKnots = myKnots.OrderByDescending(k => k.Type);
                        break;
                    case "time_asc":
                        myKnots = myKnots.OrderBy(k => k.Time);
                        break;
                    default:
                        myKnots = myKnots.OrderByDescending(k => k.Time);
                        break;
                }
                return View(await myKnots.AsNoTracking().ToListAsync());
            }
        }

        public IActionResult CreateProfile()
        {
            List<Interest> myInterests = new List<Interest>();
            return View(myInterests);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfile(int[] interests)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            foreach (int input in interests)
            {
                var interest = _context.Interests.FirstOrDefault(i => i.Id == input);
                var userInterestInDb = _context.UserInterests.FirstOrDefault(ui => ui.ApplicationUserId == user.Id && ui.InterestId == interest.Id);
                if (userInterestInDb == null)
                {
                    _context.Add(new UserInterest { ApplicationUserId = user.Id, InterestId = interest.Id });
                }
            }
            user.TopInterest = interests[0];
            user.SecondInterest = interests[1];
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CreateKnot()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CheckIn(string placeid)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            Knot knot = new Knot();
            var types = await _memberService.GetPlaceDetails(placeid);
            knot.ApplicationUserId = user.Id;
            knot.Time = DateTime.Now;
            var locationName = types[0];
            types.Remove(types[0]);
            _memberService.AssignLocation(knot, placeid, locationName);
            var categories = _memberService.CategorizeType(types);
            var categoryList = categories.ToList();
            knot.Type = categoryList[0];
            _memberService.AssignCategories(categories, user.Id);
            _context.Add(knot);
            _context.SaveChanges();
            await _memberService.DetermineNewInterests(user);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditKnot (string id)
        {
            var knot = _context.Knots.Include(k => k.Location).Single(k => k.Id == id);
            return View(knot);
        }

        [HttpPost]
        public async Task<IActionResult> EditKnot(Knot knot)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var knotInDb = _context.Knots.Single(k => k.Id == knot.Id);
            knotInDb.Type = knot.Type;
            knotInDb.Photo = knot.Photo;
            knotInDb.Comments = knot.Comments;
            _context.SaveChanges();
            await _memberService.DetermineNewInterests(user);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteKnot (string id)
        {
            var knot = _context.Knots.Include(k => k.Location).Single(k => k.Id == id);
            return View(knot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteKnot(Knot knot)
        {
            try
            {
                _context.Remove(knot);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public IActionResult AddPhoto(string id)
        {
            var knot = _context.Knots.Include(k => k.Location).Single(k => k.Id == id);
            return View(knot);
        }

        [HttpPost]
        public IActionResult AddPhoto(Knot knot)
        {
            var knotInDb = _context.Knots.Single(k => k.Id == knot.Id);
            knotInDb.Photo = knot.Photo;
            knotInDb.Comments = knot.Comments;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
