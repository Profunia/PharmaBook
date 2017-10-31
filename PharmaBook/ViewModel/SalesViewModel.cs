using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.ViewModel
{
    public class SalesViewModel
    {
        public InvcChildVmdl[] childinvc {get;set;}
        public InvcMstrVmdl masterinvc { get; set; }
        public IEnumerable<InvcChildVmdl> invcchld { get; set; }
    }
}
