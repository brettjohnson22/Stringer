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
        public string LocationName { get; set; }
        [NotMapped]
        public IEnumerable<string> Types { get; set; }
        public DateTime Time { get; set; }
        public string Comments { get; set; }
        public string Photo { get; set; }
    }
}
