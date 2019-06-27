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

namespace Stringer.Controllers
{
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
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
        public IActionResult TieAKnot(string locationid)
        {
            GetPlaceDetails(locationid);
            Knot knot = new Knot();
            knot.ApplicationUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            knot.Time = DateTime.Now;
            //knot.LocationName = locationname;
            //Method to prompt member to enter location
            //Method to find type
            return RedirectToAction(nameof(Index));
        }

        public async void GetPlaceDetails(string placeId)
        {
            string result;
            using (var client = new HttpClient())
            {
                try
                {
                    result = await client.GetStringAsync("https://maps.googleapis.com/maps/api/place/details/json?placeid=" + placeId + "&fields=name,type&key=" + APIKey.SecretKey);
                    dynamic jsonData = JObject.Parse(result);
                    var types = jsonData.result.types.ToList();

                }
                catch (Exception ex)
                {

                }

            }
        }

        public IEnumerable<string> CategorizeType(List<string> types)
        {
            List<string> categorizedTypes = new List<string>();
            if (types.Contains("bakery") || types.Contains("food") || types.Contains("restaurant"))
            {
                categorizedTypes.Add("food");
            }
            if (types.Contains("bar") || types.Contains("night_club"))
            {
                categorizedTypes.Add("nightlift");
            }
            if (types.Contains("amusement_park") || types.Contains("aquarium") || types.Contains("bowling_alley") || types.Contains("casino") || types.Contains("movie_theater") || types.Contains("stadium") || types.Contains("zoo"))
            {
                categorizedTypes.Add("activities");
            }
            if (types.Contains("campground") || types.Contains("park") || types.Contains("natural_feature"))
            {
                categorizedTypes.Add("outdoors");
            }
            if(types.Contains("library") || types.Contains("book_store") || types.Contains("museum") || types.Contains("art_gallery"))
            {
                categorizedTypes.Add("culture");
            }
            if (types.Contains("pet_store") || types.Contains("zoo") || types.Contains("aquarium") || types.Contains("veterinary_Care"))
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
            return categorizedTypes;
        }

        public void CheckType(string type)
        {
            switch (type)
            {

            }
        }
    }
}
