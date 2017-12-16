using Microsoft.EntityFrameworkCore;
using PharmaBook.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace PharmaBook.Services
{
    public interface IProduct
    {
        void Add(Product prd);       
        void Commit();
        Task<List<Product>> GetAll(string userName);
        List<Product> GetAllSync(string userName);
        Task<Product> GetById(int Id);
    }
    public class ProductServices : IProduct
    {
        private PharmaBookContext _context;
        public ProductServices(PharmaBookContext context)
        {
            _context = context;
        }
        public async void Add(Product prd)
        {
           await Task.FromResult(_context.Add(prd));
        }

        public async void Commit()
        {
           await Task.FromResult(_context.SaveChanges());
        }

        public async Task<List<Product>> GetAll(string userName)
        {
            return await Task.FromResult(_context.products.Where(x => x.cusUserName.Equals(userName)).OrderByDescending(x => x.Id).ToList());
        }

        public List<Product> GetAllSync(string userName)
        {
           return  _context.products.Where(x => x.cusUserName.Equals(userName)).OrderByDescending(x => x.Id).ToList();
        }

        public async Task<Product> GetById(int Id)
        {
            return await Task.FromResult(_context.products.FirstOrDefault(x => x.Id == Id));
        }
    }
}
