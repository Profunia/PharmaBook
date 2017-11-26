using Microsoft.EntityFrameworkCore;
using PharmaBook.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Services
{
    public interface IVendorServices
    {
        IEnumerable<Vendor> GetAll(string userName);
        Vendor GetById(int id);      
        void Add(Vendor obj);
        void Commit();
    }
    public class VendorServices : IVendorServices
    {
        private PharmaBookContext _context;
        public VendorServices(PharmaBookContext obj)
        {
            _context = obj;
        }

        public void Add(Vendor obj)
        {
            _context.Add(obj);            
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public IEnumerable<Vendor> GetAll(string userName)
        {
            return _context.vendors.Where(x=>x.cusUserName.Equals(userName) 
            && x.cusUserName!=null).OrderBy(x => x.vendorName);
        }

        public Vendor GetById(int id)
        {
            return _context.vendors.FirstOrDefault(x => x.Id == id);
        }
    }
}
