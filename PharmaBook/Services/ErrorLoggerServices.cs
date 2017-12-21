using PharmaBook.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Services
{
    public interface IErrorLogger
    {
        Task<List<ErrorLogger>> GetAll();
        Task<ErrorLogger> GetErrorById(int id);
        void Add(ErrorLogger obj);
    }
    public class ErrorLoggerServices : IErrorLogger
    {
        private PharmaBookContext _context;
        public ErrorLoggerServices(PharmaBookContext context)
        {
            _context = context;
        }

        public async void Add(ErrorLogger obj)
        {
            await Task.FromResult(_context.Add(obj));
            await Task.FromResult(_context.SaveChanges());
        }

        public async Task<List<ErrorLogger>> GetAll()
        {
            return await Task.FromResult(_context.ErrorLogger.OrderByDescending(x => x.id).ToList());
        }

        public  async Task<ErrorLogger> GetErrorById(int id)
        {
            return await Task.FromResult(_context.ErrorLogger.Find(id));

        }
    }
}
