using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    class UserInterest
    {
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int InterestId { get; set; }
        public Interest Interest { get; set; }
    }
}
