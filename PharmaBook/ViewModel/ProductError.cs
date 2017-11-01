using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace PharmaBook.ViewModel
{
    public class ProductError
    {
             
        public string name { get; set; }       
        public string batchNo { get; set; }        
        public string expDate { get; set; }
        public string companyName { get; set; }
        public string MRP { get; set; }       
        public string openingStock { get; set; }
        public string lastUpdated { get; set; }
        public string cusUserName { get; set; }       
    }
}
