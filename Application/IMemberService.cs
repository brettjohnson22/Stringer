using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IMemberService
    {
        Task<List<string>> GetPlaceDetails(string placeId);
        IEnumerable<string> CategorizeType(IEnumerable<string> types);
        void AssignLocation(Knot knot, string locationid, string locationName);
        void AssignCategories(IEnumerable<string> categories, string userId);
        Task DetermineNewInterests(ApplicationUser user);

    }
}
