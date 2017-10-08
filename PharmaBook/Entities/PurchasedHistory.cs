using System;

namespace PharmaBook.Entities
{
    public class PurchasedHistory
    {
        public int Id { get; set; }
        public int ProductID { get; set; }
        public int vendorID { get; set; }
        public string MRP { get; set; }
        public string qty { get; set; }
        public DateTime purchasedDated { get; set; }
        public string cusUserName { get; set; }

    }
}
