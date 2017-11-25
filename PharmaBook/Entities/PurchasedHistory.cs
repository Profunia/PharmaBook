using System;

namespace PharmaBook.Entities
{
    public class PurchasedHistory
    {
        public int Id { get; set; }
        public int ProductID { get; set; }
        public int? vendorID { get; set; }
        public string MRP { get; set; }
        public string qty { get; set; }
        public string stef { get; set; }
        public string tabletsCapsule { get; set; }
        public string eachStefPrice { get; set; }
        public DateTime purchasedDated { get; set; }
        public string cusUserName { get; set; }
        public string Name { get; set; }
        public string Mfg { get; set; }
        public string BatchNo { get; set; }
        public string ExpDate { get; set; }
        public string Remark { get; set; }
    }
}
