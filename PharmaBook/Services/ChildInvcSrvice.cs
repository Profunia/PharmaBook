using PharmaBook.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Services
{
    public interface IChild
    {
        void Add(ChildInvoice chldinvc);
        Task<ChildInvoice> getlastproduct();
        void Commit();
        Task<List<ChildInvoice>> GetAll();
        Task<List<ChildInvoice>> GetById(int Id);
        Task<ChildInvoice> GetIdByPK(int Id);
    }
    public class ChildInvcSrvice : IChild
    {
        private PharmaBookContext _context;
        public ChildInvcSrvice(PharmaBookContext context)
        {
            _context = context;
        }
        public async void Add(ChildInvoice chldinvc)
        {
            await Task.FromResult(_context.Add(chldinvc));
        }

        public async void Commit()
        {
            await Task.FromResult(_context.SaveChanges());
        }

        public async Task<List<ChildInvoice>> GetAll()
        {
            return await Task.FromResult(_context.InvChild.OrderByDescending(x => x.Id).ToList());
        }
        public async Task<ChildInvoice> getlastproduct()
        {
            return await Task.FromResult(_context.InvChild.ToList().LastOrDefault());
        }

        public async Task<List<ChildInvoice>> GetById(int id)
        {
            return await Task.FromResult(_context.InvChild.Where(x => x.MasterInvID == id).ToList());
        }
        public async Task<ChildInvoice> GetIdByPK(int Id)
        {
            return await Task.FromResult(_context.InvChild.Find(Id));
        }
    }
}
