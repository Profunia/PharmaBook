using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Entities
{
    public class ErrorLogger
    {
        public int id { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public string ExpectionMsg { get; set; }
        public string InnnerExceptionMsg { get; set; }
        public string stackDetails { get; set; }
        public DateTime LoggedDate { get; set; }
        public bool isActive { get; set; }
    }
}
