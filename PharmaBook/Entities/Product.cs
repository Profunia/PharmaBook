using System;

namespace PharmaBook.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string batchNo { get; set; }
        public DateTime expDate { get; set; }
        public string companyName { get; set; }
        public string MRP { get; set; }
        public int? vendorID { get; set; }
        public int openingStock { get; set; }
        public int? stef { get; set; }
        public int? tabletsCapsule { get; set; }
        public double? eachStefPrice { get; set; }
        public string lastUpdated { get; set; }
        public bool isActive { get; set; }
        public string cusUserName { get; set; }
        
    }
}
