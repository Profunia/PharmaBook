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
        public List<ChildInv> childInv { get; set; }
    }
    public class ChildInv
    {
        public int Id { get; set; }
        public int ProdID { get; set; }
        public string Name { get; set; }
        public string Mfg { get; set; }
        public string ExpDate { get; set; }
        public string BatchNo { get; set; }
        public int Qty { get; set; }
        public double MRP { get; set; }
    }

    public class ReturnInv
    {
        public int mastInv { get; set; }
        public int id { get; set; }
        public int prodID { get; set; }
        public int qty { get; set; }
        public string remarks { get; set; }
        public string parentIndex { get; set; }
        public string batchNo { get; set; }
        public string expDate { get; set; }
        public string mfg { get; set; }
        public string mrp { get; set; }
        public string soldQty { get; set; }
        public string name { get; set; }
    }
}
