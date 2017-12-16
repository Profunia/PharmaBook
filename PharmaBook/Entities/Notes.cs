using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Entities
{
    public class Notes
    {
        public int id { get; set; }
        public string remarks { get; set; }
        public string userName { get; set; }
        public bool isActive { get; set; }
        public string createdDate { get; set; }
    }
}
