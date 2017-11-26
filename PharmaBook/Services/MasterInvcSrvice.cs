using Microsoft.EntityFrameworkCore;
using PharmaBook.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Services
{
    public interface Imaster
    {
        void Add(MasterInvoice mstinvc);      
        void Commit();
        MasterInvoice getlastproduct();
        IEnumerable<MasterInvoice> GetAll(string userName);
        MasterInvoice GetById(int Id);
    }
    public class MasterInvcSrvice : Imaster
    {
        private PharmaBookContext _context;
        public MasterInvcSrvice(PharmaBookContext context)
        {
            _context = context;
        }
        public void Add(MasterInvoice mstinvc)
        {
            _context.Add(mstinvc);
        }

        public void Commit()
        {
            _context.SaveChanges();
        }        

        public IEnumerable<MasterInvoice> GetAll(string userName)
        {
             return _context.InvMaster.Where(x=>x.UserName==userName).OrderByDescending(x => x.Id).ToList();
        }

        public MasterInvoice GetById(int Id)
        {
             return _context.InvMaster.FirstOrDefault(x => x.Id == Id);
        }        
       
        MasterInvoice Imaster.getlastproduct()
        {
            return _context.InvMaster.ToList().LastOrDefault();
        }
    }
}
