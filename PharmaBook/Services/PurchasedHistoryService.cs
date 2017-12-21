using PharmaBook.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Services
{
    public interface IPurchasedHistory
    {
        void Add(PurchasedHistory prd);      
        void Commit();
       Task<List<PurchasedHistory>> GetAll(string userName);
        Task<PurchasedHistory> GetById(int Id);
    }
    public class PurchasedHistoryService : IPurchasedHistory
    {
        private PharmaBookContext _context;
        public PurchasedHistoryService(PharmaBookContext context)
        {
            _context = context;
        }
        public async void Add(PurchasedHistory prd)
        {
          await Task.FromResult(_context.Add(prd));
        }

        public async void Commit()
        {
           await Task.FromResult(_context.SaveChanges());
        }

        public async Task<List<PurchasedHistory>> GetAll(string userName)
        {
            return await Task.FromResult(_context.purchasedHistory.Where(x => x.cusUserName.Equals(userName)).OrderByDescending(x => x.Id).ToList());
        }

        public async Task<PurchasedHistory> GetById(int Id)
        {
            return await  Task.FromResult(_context.purchasedHistory.FirstOrDefault(x => x.Id == Id));
        }
    }
}
