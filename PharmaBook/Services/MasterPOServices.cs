using Microsoft.EntityFrameworkCore;
using PharmaBook.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Services
{
    public interface IMasterPOServices
    {
            void Add(MasterPO prd);           
            void Commit();
           Task<List<MasterPO>> GetAll(string userName);
           Task<MasterPO> GetById(int Id);        
    }
    public class MasterPOServices : IMasterPOServices
    {
        private PharmaBookContext _context;
        public MasterPOServices(PharmaBookContext context)
        {
            _context = context;
        }

        public async void Add(MasterPO prd)
        {
           await Task.FromResult(_context.MasterPO.Add(prd));
        }

        public async void Commit()
        {
           await Task.FromResult(_context.SaveChanges());
        }
        
        public async Task<List<MasterPO>> GetAll(string userName)
        {
            return await Task.FromResult(_context.MasterPO.Where(x => x.userName.Equals(userName)).ToList());
        }

        public async Task<MasterPO> GetById(int Id)
        {
            return await Task.FromResult(_context.MasterPO.Find(Id));
        }
    }
}
