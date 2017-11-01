﻿using System;
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
        public List<ProductViewModel> successlst { get; set; }
        public List<Duplicatelist> duplictlst { get; set; }
        public List<ProductError> producterrlst { get; set; }
    }
}
