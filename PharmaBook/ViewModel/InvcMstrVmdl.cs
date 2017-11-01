using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.ViewModel
{
    public class InvcMstrVmdl
    {
        public int Id { get; set; }
        public string InvId { get; set; }
        public string PatientName { get; set; }
        public string PatientAdres { get; set; }
        public string DrName { get; set; }
        public string RegNo { get; set; }
        public DateTime InvCrtdate { get; set; }
        public string UserName { get; set; }
    }
}
