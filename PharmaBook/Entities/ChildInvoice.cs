using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Entities
{
    public class ChildInvoice
    {
        public int Id { get; set; }
        public int PrdId { get; set; }
        public int Qty { get; set; }
        public string Description { get; set; }
        public string Mrg { get; set; }
        public string BatchNo { get; set; }
        public string ExpDt { get; set; }
        public double Amount { get; set; }
        public int MasterInvID { get; set; }
        
    }
}
