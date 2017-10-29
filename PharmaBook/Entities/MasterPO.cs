using System;

namespace PharmaBook.Entities
{
    public class MasterPO
    {
        public int Id { get; set; }
        public DateTime placedOrderDt { get; set; }
        public string userName { get; set; }
        public int VendorID { get; set; }
        public bool isActive { get; set; }
        
    }
}
