using Microsoft.EntityFrameworkCore;
using PharmaBook.Entities;
using System.Collections.Generic;
using System.Linq;

namespace PharmaBook.Services
{
    public interface IMasterPOServices
    {
            void Add(MasterPO prd);           
            void Commit();
            IEnumerable<MasterPO> GetAll(string userName);
            MasterPO GetById(int Id);        
    }
    public class MasterPOServices : IMasterPOServices
    {
        private PharmaBookContext _context;
        public MasterPOServices(PharmaBookContext context)
        {
            _context = context;
        }

        public void Add(MasterPO prd)
        {
            _context.MasterPO.Add(prd);
        }

        public void Commit()
        {
            _context.SaveChanges();
        }
        
        public IEnumerable<MasterPO> GetAll(string userName)
        {
            return _context.MasterPO.Where(x => x.userName.Equals(userName)).ToList();
        }

        public MasterPO GetById(int Id)
        {
            return _context.MasterPO.Find(Id);
        }
    }
}
