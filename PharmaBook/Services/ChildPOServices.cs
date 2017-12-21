using Microsoft.EntityFrameworkCore;
using PharmaBook.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Services
{
    public interface IchildPoServices
    {
        void Add(ChildPO prd);
        void Delete(ChildPO prd);
        void Update(ChildPO prd);
        void Commit();
        Task<List<ChildPO>> GetAll();
        Task<ChildPO> GetById(int Id);
    }

    public class ChildPOServices : IchildPoServices
    {
        private PharmaBookContext _context;
        public ChildPOServices(PharmaBookContext context)
        {
            _context = context;
        }

        public async void Add(ChildPO prd)
        {
           await Task.FromResult(_context.Add(prd));
        }

        public async void Commit()
        {
            await Task.FromResult(_context.SaveChanges());
        }

        public async void Delete(ChildPO prd)
        {
           await Task.FromResult(_context.Remove(prd));
        }

        public async Task<List<ChildPO>> GetAll()
        {
            return await Task.FromResult(_context.ChildPO.ToList());
        }

        public async Task<ChildPO> GetById(int Id)
        {
            return await Task.FromResult(_context.ChildPO.Find(Id));
        }

        public async void Update(ChildPO prd)
        {
           await Task.FromResult(_context.Entry(prd).State = EntityState.Modified);
        }
    }
}
