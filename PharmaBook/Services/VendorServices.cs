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

        Task<IEnumerable<Vendor>> asyncGetAll(string userName);
        Task<bool> asyncCommit();
        Task<Vendor> asyncGetById(int id);
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

        public Task<bool> asyncCommit()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Vendor>> asyncGetAll(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<Vendor> asyncGetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public IEnumerable<Vendor> GetAll(string userName)
        {
            return _context.vendors.Where(x=>x.cusUserName.Equals(userName) 
            && x.cusUserName!=null).OrderByDescending(x => x.Id);
        }

        public Vendor GetById(int id)
        {
            return _context.vendors.FirstOrDefault(x => x.Id == id);
        }
    }
}
