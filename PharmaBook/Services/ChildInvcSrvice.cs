using PharmaBook.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Services
{
    public interface IChild
    {
        void Add(ChildInvoice chldinvc);     
        ChildInvoice getlastproduct();
        void Commit();
        IEnumerable<ChildInvoice> GetAll();
        IEnumerable<ChildInvoice> GetById(int Id);
        ChildInvoice GetIdByPK(int Id);
    }
    public class ChildInvcSrvice : IChild
    {
        private PharmaBookContext _context;
        public ChildInvcSrvice(PharmaBookContext context)
        {
            _context = context;
        }
        public void Add(ChildInvoice chldinvc)
        {
            _context.Add(chldinvc);
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public IEnumerable<ChildInvoice> GetAll()
        {
            return _context.InvChild.OrderByDescending(x => x.Id).ToList();
        }
        public ChildInvoice getlastproduct()
        {
            return _context.InvChild.ToList().LastOrDefault();
        }

        IEnumerable<ChildInvoice> IChild.GetById(int id)
        {
            return _context.InvChild.Where(x => x.MasterInvID == id).ToList();
        }
      public ChildInvoice GetIdByPK(int Id)
        {
            return _context.InvChild.Find(Id);
        }
    }
}
