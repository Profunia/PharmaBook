using PharmaBook.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PharmaBook.ViewModel
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please provide Medicine Name")]
        public string name { get; set; }
        [Required(ErrorMessage = "Please provide Batch Number")]
        public string batchNo { get; set; }
        [Required(ErrorMessage ="Please provide Exp Date")]
        public string expDate { get; set; }
        public string companyName { get; set; }
        public string MRP { get; set; }     
        [Required(ErrorMessage = "Please provide opening Stock")]
        public int openingStock { get; set; }
        public int? stef { get; set; }
        public int? tabletsCapsule { get; set; }
        public double? eachStefPrice { get; set; }
        public string lastUpdated { get; set; }
        public string cusUserName { get; set; }
        public int? vendorID { get; set; }
        public string Remarks { get; set; }


        public IEnumerable<Product> products { get; set; }
    }
}
