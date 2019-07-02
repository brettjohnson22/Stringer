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
            ViewBag.BusinessId = myBusiness.Id;
            return View(user);
        }

        // GET: Business/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Business/Create
        public async Task<IActionResult> CreateBusiness()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(user);
        }

        // POST: Business/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBusiness(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

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
}
}