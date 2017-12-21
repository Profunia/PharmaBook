using Microsoft.EntityFrameworkCore;
using PharmaBook.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Services
{
    public interface IProfileServices
    {
        void Add(UserProfile profile);
        void Commit();       
       Task<UserProfile> GetById(int Id);
        Task<UserProfile> GetByUserName(string userName);
        Task<List<UserProfile>> GetAllforAdmin();
    }


    public class ProfileServices : IProfileServices
    {
        private PharmaBookContext _context;
        public ProfileServices(PharmaBookContext context)
        {
            _context = context;
        }
        public async void Add(UserProfile profile)
        {
           await Task.FromResult(_context.Add(profile));
        }

        public async void Commit()
        {
           await Task.FromResult(_context.SaveChanges());
        }       

        public async Task<UserProfile> GetById(int Id)
        {
            return await Task.FromResult(_context.UserProfile.Find(Id));
        }
        public async Task<List<UserProfile>> GetAllforAdmin()
        {
            return await Task.FromResult(_context.UserProfile.OrderByDescending(x => x.Id).ToList());
        }

        public async Task<UserProfile> GetByUserName(string userName)
        {
            return await Task.FromResult(_context.UserProfile.FirstOrDefault(x => x.userName.Equals(userName)));
        }
    }
}
