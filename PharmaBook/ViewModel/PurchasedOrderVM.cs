using System.Collections;
using System.Collections.Generic;

namespace PharmaBook.ViewModel
{
    public class PurchasedOrderVM
    {
        public string VendorID { get; set; }
        public string ProdID { get; set; }
        public string Qty { get; set; }
        public string Remarks { get; set; }
    }

    public class createPoVM
    {
        public IEnumerable<PurchasedOrderVM> purchasedOrderVM { get; set; }
    }
}
