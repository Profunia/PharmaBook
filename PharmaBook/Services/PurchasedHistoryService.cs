using PharmaBook.Entities;
using System.Collections.Generic;
using System.Linq;

namespace PharmaBook.Services
{
    public interface IPurchasedHistory
    {
        void Add(PurchasedHistory prd);      
        void Commit();
        IEnumerable<PurchasedHistory> GetAll(string userName);
        PurchasedHistory GetById(int Id);
    }
    public class PurchasedHistoryService : IPurchasedHistory
    {
        private PharmaBookContext _context;
        public PurchasedHistoryService(PharmaBookContext context)
        {
            _context = context;
        }
        public void Add(PurchasedHistory prd)
        {
            _context.Add(prd);
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public IEnumerable<PurchasedHistory> GetAll(string userName)
        {
            return _context.purchasedHistory.Where(x => x.cusUserName.Equals(userName)).OrderByDescending(x => x.Id).ToList();
        }

        public PurchasedHistory GetById(int Id)
        {
            return _context.purchasedHistory.FirstOrDefault(x => x.Id == Id);
        }
    }
}
