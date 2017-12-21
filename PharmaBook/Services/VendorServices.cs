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
        Task<List<Vendor>> GetAll(string userName);
        Task<Vendor> GetById(int id);      
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

        public async void Add(Vendor obj)
        {
           await Task.FromResult( _context.Add(obj));            
        }
       
        public async void Commit()
        {
          await Task.FromResult(_context.SaveChanges());
        }

        public async Task<List<Vendor>> GetAll(string userName)
        {
            return  await Task.FromResult( _context.vendors.Where(x=>x.cusUserName.Equals(userName) 
            && x.cusUserName!=null).OrderByDescending(x => x.Id).ToList());
        }

        public async Task<Vendor> GetById(int id)
        {
            return await Task.FromResult(_context.vendors.FirstOrDefault(x => x.Id == id));
        }
    }
}
