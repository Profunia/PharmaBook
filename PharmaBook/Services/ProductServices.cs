using PharmaBook.Entities;
using System.Collections.Generic;
using System.Linq;
namespace PharmaBook.Services
{
    public interface IProduct
    {
        void Add(Product prd);
        void Delete(Product prd);
        void Commit();
        IEnumerable<Product> GetAll(string userName);
        Product GetById(int Id);
    }
    public class ProductServices : IProduct
    {
        private PharmaBookContext _context;
        public ProductServices(PharmaBookContext context)
        {
            _context = context;
        }
        public void Add(Product prd)
        {
            _context.Add(prd);
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Delete(Product prd)
        {
            _context.Remove(prd);
        }

        public IEnumerable<Product> GetAll(string userName)
        {
            return _context.products.Where(x => x.cusUserName.Equals(userName)).OrderByDescending(x=>x.Id).ToList();
        }

        public Product GetById(int Id)
        {
            return _context.products.FirstOrDefault(x => x.Id == Id);
        }
    }
}
