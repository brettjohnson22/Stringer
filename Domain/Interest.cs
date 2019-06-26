using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain
{
    class Interest
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
