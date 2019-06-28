using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{ 

    public class Knot
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("Location")]
        public string LocationId { get; set; }
        public Location Location { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }
        public string Comments { get; set; }
        public string Photo { get; set; }
    }
}
