using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain
{
    public class Location
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
