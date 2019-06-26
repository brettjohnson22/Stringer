using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{ 

    public class Knot
    {
        public string LocationName { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }
        public string Comments { get; set; }
        public string Photo { get; set; }
    }
}
