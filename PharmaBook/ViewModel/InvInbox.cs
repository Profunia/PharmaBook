using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.ViewModel
{
   
    public class MasterInv
    {        
        public int Id { get; set; }
        public string InvId { get; set; }
        public string Patient { get; set; }
        public string Paddress { get; set; }
        public string DocName { get; set; }
        public string DocRegi { get; set; }
        public string CreatedDate { get; set; }
        public int NoOfMedicine { get; set; }
        public string BillingAmount { get; set; }
        public ChildInv childInv { get; set; }
    }
    public class ChildInv
    {

        public int ProdID { get; set; }
        public string Name { get; set; }
        public string Mfg { get; set; }
        public string ExpDate { get; set; }
        public string BatchNo { get; set; }
        public int Qty { get; set; }
    }
}
