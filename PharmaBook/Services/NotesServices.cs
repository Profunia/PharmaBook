using PharmaBook.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Services
{
    public interface INoteServices
    {
        void Add(Notes notes);
        void Commit();
        Task<List<Notes>> GetAll(string userName);
        Task<Notes> GetById(int Id);        
    }
    public class NotesServices : INoteServices
    {
        private PharmaBookContext _context;
        public NotesServices(PharmaBookContext context)
        {
            _context = context;
        }
        public async void Add(Notes notes)
        {
           await Task.FromResult(_context.Add(notes));
        }

        public async void Commit()
        {
            await Task.FromResult(_context.SaveChanges());
        }

        public async Task<List<Notes>> GetAll(string userName)
        {
            return await Task.FromResult(_context.Notes.Where(x=>x.userName.Equals(userName)).ToList());
        }

        public async Task<Notes> GetById(int Id)
        {
            return await Task.FromResult(_context.Notes.Find(Id));
        }
       
    }
}
