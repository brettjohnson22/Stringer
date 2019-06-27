using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain
{
    class Location
    {
        [Key]
        string Id { get; set; }
        string Name { get; set; }
    }
}
