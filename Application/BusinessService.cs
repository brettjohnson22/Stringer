using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stringer;

namespace Application
{
    public class BusinessService : IBusinessService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;


        public Location CheckExistingBusiness(string placeId)
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

        public int[] AnalyzeData(IQueryable<Knot> myKnots)
        {
            int[] data = new int[15];
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
                if (knot.ApplicationUser.Gender == "Male")
                {
                    maleCounter++;
                }
                if (knot.ApplicationUser.Gender == "Female")
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
            data[0] = maleCounter;
            data[1] = femaleCounter;
            data[2] = ageOneCounter;
            data[3] = ageTwoCounter;
            data[4] = ageThreeCounter;
            data[5] = ageFourCounter;
            data[6] = ageFiveCounter;
            data[7] = foodCounter;
            data[8] = nightlifeCounter;
            data[9] = activitiesCounter;
            data[10] = animalsCounter;
            data[11] = outdoorsCounter;
            data[12] = cultureCounter;
            data[13] = fashionCounter;
            data[14] = wellnessCounter;

            return data;
        }

    }
}
