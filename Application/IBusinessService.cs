using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IBusinessService
    {
        Location CheckExistingBusiness(string placeId);
        Task<List<string>> GetBusinessDetails(string placeId);
        int[] AnalyzeData(IQueryable<Knot> myKnots);
    }
}
