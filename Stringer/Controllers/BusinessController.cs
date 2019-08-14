using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using Infrastructure;
using Application;
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
        private readonly IBusinessService _businessService;

        public BusinessController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBusinessService businessService)
        {
            _context = context;
            _userManager = userManager;
            _businessService = businessService;
        }
        // GET: Business
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var myBusiness = _context.Locations.SingleOrDefault(l => l.ApplicationUserId == user.Id);
            if (myBusiness != null)
            {
                var myKnots = _context.Knots.Include(k => k.ApplicationUser).Where(k => k.LocationId == myBusiness.Id);
                ViewBag.BusinessId = myBusiness.Id;
                ViewBag.Name = myBusiness.Name;
                int[] data = _businessService.AnalyzeData(myKnots);
                ViewBag.Male = data[0];
                ViewBag.Female = data[1];
                ViewBag.AgeOne = data[2];
                ViewBag.AgeTwo = data[3];
                ViewBag.AgeThree = data[4];
                ViewBag.AgeFour = data[5];
                ViewBag.AgeFive = data[6];
                ViewBag.Food = data[7];
                ViewBag.Nightlife = data[8];
                ViewBag.Activities = data[9];
                ViewBag.Animals = data[10];
                ViewBag.Outdoors = data[11];
                ViewBag.Culture = data[12];
                ViewBag.Fashion = data[13];
                ViewBag.Wellness = data[14];
                return View(myKnots);
            }
            else return RedirectToAction(nameof(FindBusiness));
        }

        public ActionResult FindBusiness()
        {
            return View();
        }

        public IActionResult BusinessClaimed()
        {
            return View();
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
            var existingBusiness = _businessService.CheckExistingBusiness(placeId);
            if (existingBusiness != null && existingBusiness.IsClaimed == false)
            {
                existingBusiness.ApplicationUserId = user.Id;
                existingBusiness.IsClaimed = true;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else if (existingBusiness != null && existingBusiness.IsClaimed == true)
            {
                return RedirectToAction(nameof(BusinessClaimed));
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

    }
}