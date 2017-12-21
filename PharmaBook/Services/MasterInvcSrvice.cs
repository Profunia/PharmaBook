using Microsoft.EntityFrameworkCore;
using PharmaBook.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Services
{
    public interface Imaster
    {
        void Add(MasterInvoice mstinvc);
        void Commit();        
        Task<List<MasterInvoice>> GetAll(string userName);
        Task<MasterInvoice> GetById(int Id);
    }
    public class MasterInvcSrvice : Imaster
    {
        private PharmaBookContext _context;
        public MasterInvcSrvice(PharmaBookContext context)
        {
            _context = context;
        }
        public async void Add(MasterInvoice mstinvc)
        {
           await Task.FromResult(_context.Add(mstinvc));
        }

        public async void Commit()
        {
          await Task.FromResult(_context.SaveChanges());
        }

        public async Task<List<MasterInvoice>> GetAll(string userName)
        {
            return await Task.FromResult(_context.InvMaster.Where(x => x.UserName == userName).OrderByDescending(x => x.Id).ToList());
        }

        public async Task<MasterInvoice> GetById(int Id)
        {
            return await Task.FromResult(_context.InvMaster.FirstOrDefault(x => x.Id == Id));
        }
       
    }
}
