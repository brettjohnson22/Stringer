using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Stringer.Controllers
{
    public class BusinessController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BusinessController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: Business
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var myBusiness = _context.Locations.SingleOrDefault(l => l.ApplicationUserId == user.Id);
            var myKnots = _context.Knots.Include(k => k.ApplicationUser).Where(k => k.LocationId == myBusiness.Id);
            ViewBag.BusinessId = myBusiness.Id;
            ViewBag.Name = myBusiness.Name;
            AnalyzeData(myKnots);
            return View(myKnots);
        }

        // GET: Business/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Business/Create
        //public async Task<IActionResult> CreateBusiness()
        //{
        //    var user = await _userManager.GetUserAsync(HttpContext.User);
        //    return View(user);
        //}

        //// POST: Business/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateBusiness(IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: Business/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Business/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Business/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Business/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> ClaimBusiness(string placeId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            string result;
            string name;
            using (var client = new HttpClient())
            {
                try
                {
                    result = await client.GetStringAsync("https://maps.googleapis.com/maps/api/place/details/json?placeid=" + placeId + "&fields=name&key=" + APIKey.SecretKey);
                    dynamic jObj = JsonConvert.DeserializeObject(result);
                    name = jObj.result.name.ToString();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            var existingBusiness = CheckExistingBuisiness(placeId);
            if (existingBusiness != null)
            {
                existingBusiness.ApplicationUserId = user.Id;
                existingBusiness.IsClaimed = true;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Location newLocation = new Location
                {
                    Id = placeId,
                    Name = name,
                    ApplicationUserId = user.Id,
                    IsClaimed = true
                };
                user.BusinessName = name;
                _context.Add(newLocation);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
        }

        public Location CheckExistingBuisiness(string placeId)
        {
            var existingBusiness = _context.Locations.SingleOrDefault(l => l.Id == placeId);
            if (existingBusiness != null)
            {
                return existingBusiness;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<string>> GetBusinessDetails(string placeId)
        {
            string result;
            using (var client = new HttpClient())
            {
                try
                {
                    result = await client.GetStringAsync("https://maps.googleapis.com/maps/api/place/details/json?placeid=" + placeId + "&fields=name,type&key=" + APIKey.SecretKey);
                    dynamic jObj = JsonConvert.DeserializeObject(result);
                    var name = jObj.result.name.ToString();
                    var types = jObj.result.types;
                    int count = types.Count;
                    var typeList = new List<string>();
                    typeList.Add(name);
                    for (int i = 0; i < count; i++)
                    {
                        var data = types[i].ToString();
                        typeList.Add(data);
                    }
                    return typeList;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        public ActionResult FindBusiness()
        {
            return View();
        }

        public void AnalyzeData(IQueryable<Knot> myKnots)
        {
            int maleCounter = 0;
            int femaleCounter = 0;
            int ageOneCounter = 0;
            int ageTwoCounter = 0;
            int ageThreeCounter = 0;
            int ageFourCounter = 0;
            int ageFiveCounter = 0;
            int foodCounter = 0;
            int nightlifeCounter = 0;
            int activitiesCounter = 0;
            int animalsCounter = 0;
            int outdoorsCounter = 0;
            int cultureCounter = 0;
            int fashionCounter = 0;
            int wellnessCounter = 0;

            foreach (Knot knot in myKnots)
            {
                if (knot.ApplicationUser.Gender == "male")
                {
                    maleCounter++;
                }
                if (knot.ApplicationUser.Gender == "female")
                {
                    femaleCounter++;
                }
                if (knot.ApplicationUser.Age <= 20)
                {
                    ageOneCounter++;
                }
                if (knot.ApplicationUser.Age > 20 && knot.ApplicationUser.Age <= 35)
                {
                    ageTwoCounter++;
                }
                if (knot.ApplicationUser.Age > 35 && knot.ApplicationUser.Age <= 49)
                {
                    ageThreeCounter++;
                }
                if (knot.ApplicationUser.Age > 49 && knot.ApplicationUser.Age <= 64)
                {
                    ageFourCounter++;
                }
                if (knot.ApplicationUser.Age > 64)
                {
                    ageFiveCounter++;
                }
                if (knot.ApplicationUser.TopInterest == 1 || knot.ApplicationUser.SecondInterest == 1)
                {
                    foodCounter++;
                }
                if (knot.ApplicationUser.TopInterest == 2 || knot.ApplicationUser.SecondInterest == 2)
                {
                    nightlifeCounter++;
                }
                if (knot.ApplicationUser.TopInterest == 3 || knot.ApplicationUser.SecondInterest == 3)
                {
                    activitiesCounter++;
                }
                if (knot.ApplicationUser.TopInterest == 4 || knot.ApplicationUser.SecondInterest == 4)
                {
                    animalsCounter++;
                }
                if (knot.ApplicationUser.TopInterest == 5 || knot.ApplicationUser.SecondInterest == 5)
                {
                    outdoorsCounter++;
                }
                if (knot.ApplicationUser.TopInterest == 6 || knot.ApplicationUser.SecondInterest == 6)
                {
                    cultureCounter++;
                }
                if (knot.ApplicationUser.TopInterest == 7 || knot.ApplicationUser.SecondInterest == 7)
                {
                    fashionCounter++;
                }
                if (knot.ApplicationUser.TopInterest == 8 || knot.ApplicationUser.SecondInterest == 8)
                {
                    wellnessCounter++;
                }
            }
            ViewBag.Male = maleCounter;
            ViewBag.Female = femaleCounter;
            ViewBag.AgeOne = ageOneCounter;
            ViewBag.AgeTwo = ageTwoCounter;
            ViewBag.AgeThree = ageThreeCounter;
            ViewBag.AgeFour = ageFourCounter;
            ViewBag.AgeFive = ageFiveCounter;

            //SortedDictionary<int, List<string>> sortedInterests = new SortedDictionary<int, List<string>>();
            //int[] visitorInterests = new int[8];

            //visitorInterests[0] = foodCounter;
            //visitorInterests[1] = nightlifeCounter;
            //visitorInterests[2] = activitiesCounter;
            //visitorInterests[4] = animalsCounter;
            //visitorInterests[4] = outdoorsCounter;
            //visitorInterests[5] = cultureCounter;
            //visitorInterests[6] = fashionCounter;
            //visitorInterests[7] = wellnessCounter;

            //return visitorInterests;
            ViewBag.Food = foodCounter;
            ViewBag.Nightlife = nightlifeCounter;
            ViewBag.Activities = activitiesCounter;
            ViewBag.Animals = animalsCounter;
            ViewBag.Outdoors = outdoorsCounter;
            ViewBag.Culture = cultureCounter;
            ViewBag.Fashion = fashionCounter;
            ViewBag.Wellness = wellnessCounter;
        }

        public void AnalyzeInterests(int[] customerInterests)
        {
            //customerInterests
        }
    }
}