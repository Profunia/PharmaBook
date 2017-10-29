using System;

namespace PharmaBook.Entities
{
    public class UserProfile
    {
        public int Id { get; set; }
        public DateTime lastLogin { get; set; }
        public string Name { get; set; }
        public string subTitle { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string DLNo { get; set; }
        public string userName { get; set; }
        public DateTime CreatedDt { get; set; }
        public DateTime AccountExpDt { get; set; }
        public string Pwd { get; set; }
        public bool IsActive { get; set; }
    }
}
