using Microsoft.EntityFrameworkCore;
using PharmaBook.Entities;
using System.Collections.Generic;
using System.Linq;

namespace PharmaBook.Services
{
    public interface IchildPoServices
    {
        void Add(ChildPO prd);
        void Delete(ChildPO prd);
        void Update(ChildPO prd);
        void Commit();
        IEnumerable<ChildPO> GetAll();
        ChildPO GetById(int Id);
    }

    public class ChildPOServices : IchildPoServices
    {
        private PharmaBookContext _context;
        public ChildPOServices(PharmaBookContext context)
        {
            _context = context;
        }

        public void Add(ChildPO prd)
        {
            _context.Add(prd);
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Delete(ChildPO prd)
        {
            _context.Remove(prd);
        }

        public IEnumerable<ChildPO> GetAll()
        {
            return _context.ChildPO.ToList();
        }

        public ChildPO GetById(int Id)
        {
            return _context.ChildPO.Find(Id);
        }

        public void Update(ChildPO prd)
        {
            _context.Entry(prd).State = EntityState.Modified;
        }
    }
}
