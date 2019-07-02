using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base() { }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BusinessName { get; set; }
        //public string Street { get; set; }
        //public string City { get; set; }
        //public string State { get; set; }   
        //public string ZipCode { get; set; }
        //public string Country { get; set; }
        public ICollection<UserInterest> UserInterests { get; set; }
        public int TopInterest { get; set; }
        public int SecondInterest { get; set; }
    }
}
