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
        //void Update(UserProfile profile);       
        void Commit();
        IEnumerable<UserProfile> GetAll(string userName);
        UserProfile GetById(int Id);
        UserProfile GetByUserName(string userName);
        IEnumerable<UserProfile> GetAllforAdmin();
    }


    public class ProfileServices : IProfileServices
    {
        private PharmaBookContext _context;
        public ProfileServices(PharmaBookContext context)
        {
            _context = context;
        }
        public void Add(UserProfile profile)
        {
            _context.Add(profile);
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public IEnumerable<UserProfile> GetAll(string userName)
        {
            return _context.UserProfile.Where(x => x.userName.Equals(userName)).ToList();
        }

        public UserProfile GetById(int Id)
        {
            return _context.UserProfile.Find(Id);
        }
        public IEnumerable<UserProfile> GetAllforAdmin()
        {
            return _context.UserProfile.OrderByDescending(x => x.Id).ToList();
        }

        //public void Update(UserProfile profile)
        //{
        //    _context.Entry(profile).State = EntityState.Modified;
        //}
        public UserProfile GetByUserName(string userName)
        {
            return _context.UserProfile.FirstOrDefault(x => x.userName.Equals(userName));
        }
    }
}
