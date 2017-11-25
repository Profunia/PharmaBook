using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.ViewModel
{
    public class PurchasedHistoryVM
    {
        public int Id { get; set; }
        public int ProductID { get; set; }
        public int? vendorID { get; set; }
        public string vendorname { get; set; }
        public string vendorcompany { get; set; }
        public string vendoradres { get; set; }
        public string MRP { get; set; }
        public string qty { get; set; }
        public int? stef { get; set; }
        public int? tabletsCapsule { get; set; }
        public double? eachStefPrice { get; set; }
        public string purchasedDated { get; set; }
        public string cusUserName { get; set; }
        public string Name { get; set; }
        public string Mfg { get; set; }
        public string BatchNo { get; set; }
        public string ExpDate { get; set; }
        public string Remark { get; set; }
    }
}
