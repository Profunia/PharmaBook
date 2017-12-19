using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.ViewModel
{
    public class InvcChildVmdl
    {
        public int Id { get; set; }
        public int PrdId { get; set; }
        public int Qty { get; set; }
        public string Description { get; set; }
        public string Mfg { get; set; }
        public string unitprice { get; set; }
        public string BatchNo { get; set; }
        public string ExpDt { get; set; }
        public double Amount { get; set; }
    }
}
