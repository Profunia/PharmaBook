using System.Collections;
using System.Collections.Generic;

namespace PharmaBook.ViewModel
{
    public class PurchasedOrderVM
    {
        public string VendorID { get; set; }
        public string ProdID { get; set; }
        public string stef { get; set; }
        public string Remarks { get; set; }
        public List<MPO> mpoList { get; set; }
        public List<CPO> cpoList { get; set; }
    }

    public class MPO
    {
        public int MasterPOid { get; set; }
        public int vendorID { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string VendorContact { get; set; }
        public string VendorCompany { get; set; }
        public string PlacedOrder { get; set; }
        public int NoOfItems { get; set; }
        public string Status { get; set; }
        public List<CPO> cpoList { get; set; }
    }

    public class CPO
    {
        public int ChildPoId { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int masterPOid { get; set; }
        public string Mfg { get; set; }
        public int stef { get; set; }
        public int tabletsCapsule { get; set; }
        public double eachStefPrice { get; set; }
        public string Remarks { get; set; }
        public int Qty { get; set; }
        public string BatchNo { get; set; }
        public string MRP { get; set; }
        public string ExpDate { get; set; }

    }

    public class CreatePurchased
    {
        public int ProductID { get; set; }
        public int vendorID { get; set; }
        public int MasterPOid { get; set; }
        public string Name { get; set; }
        public string Mfg { get; set; }
        public int Qty { get; set; }
        public int stef { get; set; }
        public int tabletsCapsule { get; set; }
        public double eachStefPrice { get; set; }
        public string BatchNo { get; set; }
        public string MRP { get; set; }
        public string ExpDate { get; set; }
        public string Remark { get; set; }
    }

}



