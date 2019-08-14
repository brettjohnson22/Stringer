using System.Text;
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
using Stringer;

namespace Application
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public MemberService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            if (types.Contains("library") || types.Contains("book_store") || types.Contains("museum") || types.Contains("art_gallery"))
            {
                categorizedTypes.Add("culture");
            }
            if (types.Contains("pet_store") || types.Contains("zoo") || types.Contains("aquarium") || types.Contains("veterinary_care"))
            {
                categorizedTypes.Add("animals");
            }
            if (types.Contains("beauty_salon") || types.Contains("clothing_store") || types.Contains("shoe_store") || types.Contains("shopping_mall") || types.Contains("jewelry_store") || types.Contains("department_store"))
            {
                categorizedTypes.Add("fashion");
            }
            if (types.Contains("gym") || types.Contains("spa") || types.Contains("health") || types.Contains("beauty_salon"))
            {
                categorizedTypes.Add("wellness");
            }
            if (categorizedTypes.Count == 0)
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
                Location newLocation = new Location
                {
                    Id = locationid,
                    Name = locationName
                };
                _context.Add(newLocation);
                _context.SaveChanges();
                existingPlace = newLocation;
            }
            knot.LocationId = existingPlace.Id;
        }

        public void AssignCategories(IEnumerable<string> categories, string userId)
        {
            //var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            foreach (string category in categories)
            {
                if (category == "miscellaneous")
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

        public async Task DetermineNewInterests(ApplicationUser user)
        {
            
            //var user = await _userManager.GetUserAsync(HttpContext.User);
            var myInterests = _context.UserInterests.Where(ui => ui.ApplicationUserId == user.Id);
            var myKnots = _context.Knots.Where(k => k.ApplicationUserId == user.Id);
            if (myKnots.Count() > 3)
            {
                var topInterest = myKnots.GroupBy(k => k.Type).OrderByDescending(g => g.Count()).First().Key;
                var topInterestInDB = _context.Interests.Where(i => i.Name == topInterest).FirstOrDefault();
                if (topInterestInDB.Id != user.TopInterest)
                {
                    user.SecondInterest = user.TopInterest;
                    user.TopInterest = topInterestInDB.Id;
                }
            }
        }

    }
}
