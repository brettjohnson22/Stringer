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

namespace Stringer.Controllers
{
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["TimeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "time_asc" : "";
            ViewData["TypeSortParm"] = sortOrder == "type" ? "type_desc" : "type";
            ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "name";
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var myKnots = _context.Knots.Include(k => k.Location).Where(k => k.ApplicationUserId == userId);
            if (myKnots == null)
            {
                return RedirectToAction("CreateProfile");
            }
            else
            {
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
        public IActionResult CreateProfile(List<Interest> interests)
        {
            //This needs to take the selected buttons from the previous method and create a homepage with empty lists depending on what is selected.
            //If food, suggest restaurants and cafes
            //If nightlife, suggest bar
            //If Outdoor, suggest parks and campgrounds
            //If culture, suggest museums and 
            return RedirectToAction();
        }

        public ApplicationUser GetUser()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = _context.Users.SingleOrDefault(u => u.Id == currentUserId);
            return loggedInUser;
        }

        public IActionResult CreateKnot()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TieAKnot(string locationid)
        {
            Knot knot = new Knot();
            var types = await GetPlaceDetails(locationid);
            knot.ApplicationUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            knot.Time = DateTime.Now;
            var locationName = types[0];
            types.Remove(types[0]);
            AssignLocation(knot, locationid, locationName);
            var categories = CategorizeType(types);
            var categoryList = categories.ToList();
            knot.Type = categoryList[0];
            AssignCategories(categories);
            _context.Add(knot);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<List<string>> GetPlaceDetails(string placeId)
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
                    for(int i = 0; i < count; i++)
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

        public IEnumerable<string> CategorizeType(IEnumerable<string> types)
        {
            List<string> categorizedTypes = new List<string>();
            if (types.Contains("amusement_park") || types.Contains("aquarium") || types.Contains("bowling_alley") || types.Contains("casino") || types.Contains("movie_theater") || types.Contains("stadium") || types.Contains("zoo"))
            {
                categorizedTypes.Add("activities");
            }
            if (types.Contains("bar") || types.Contains("night_club"))
            {
                categorizedTypes.Add("nightlife");
            }
            if (types.Contains("bakery") || types.Contains("food") || types.Contains("restaurant"))
            {
                categorizedTypes.Add("food");
            }
            if (types.Contains("campground") || types.Contains("park") || types.Contains("natural_feature"))
            {
                categorizedTypes.Add("outdoors");
            }
            if(types.Contains("library") || types.Contains("book_store") || types.Contains("museum") || types.Contains("art_gallery"))
            {
                categorizedTypes.Add("culture");
            }
            if (types.Contains("pet_store") || types.Contains("zoo") || types.Contains("aquarium") || types.Contains("veterinary_care"))
            {
                categorizedTypes.Add("animals");
            }
            if(types.Contains("beauty_salon") || types.Contains("clothing_store") || types.Contains("shoe_store") || types.Contains("shopping_mall") || types.Contains("jewelry_store") || types.Contains("department_store"))
            {
                categorizedTypes.Add("fashion");
            }
            if(types.Contains("gym") || types.Contains("spa") || types.Contains("health") || types.Contains("beauty_salon"))
            {
                categorizedTypes.Add("wellness");
            }
            if(categorizedTypes.Count == 0)
            {
                categorizedTypes.Add("miscellaneous");
            }
            return categorizedTypes;
        }

        public void AssignLocation(Knot knot, string locationid, string locationName)
        {
            var existingPlace = _context.Locations.SingleOrDefault(l => l.Id == locationid);
            if (existingPlace == null)
            {
                Location newLocation = new Location();
                newLocation.Id = locationid;
                newLocation.Name = locationName;
                _context.Add(newLocation);
                _context.SaveChanges();
                existingPlace = newLocation;
            }
            knot.LocationId = existingPlace.Id;
        }

        public void AssignCategories(IEnumerable<string> categories)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            foreach (string category in categories)
            {
                if(category == "miscellaneous")
                {
                    continue;
                }
                var interest = _context.Interests.FirstOrDefault(i => i.Name == category);
                var userInterestInDb = _context.UserInterests.FirstOrDefault(ui => ui.ApplicationUserId == userId && ui.InterestId == interest.Id);
                if (userInterestInDb == null)
                {
                    _context.Add(new UserInterest { ApplicationUserId = userId, InterestId = interest.Id });
                }
            }
            _context.SaveChanges();
        }
    }
}
