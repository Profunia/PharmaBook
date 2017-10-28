﻿using PharmaBook.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Services
{
    public interface IChild
    {
        void Add(ChildInvoice chldinvc);
        void Delete(ChildInvoice chldinvc);
        void Update(ChildInvoice chldinvc);
        ChildInvoice getlastproduct();
        void Commit();
        IEnumerable<ChildInvoice> GetAll(string userName);
        ChildInvoice GetById(int Id);
    }
    public class ChildInvcSrvice : IChild
    {
        private PharmaBookContext _context;
        public ChildInvcSrvice(PharmaBookContext context)
        {
            _context = context;
        }
        public void Add(ChildInvoice chldinvc)
        {
            _context.Add(chldinvc);
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Delete(ChildInvoice chldinvc)
        {
            _context.Remove(chldinvc);
        }

        public IEnumerable<ChildInvoice> GetAll(string userName)
        {
            return _context.InvChild.OrderByDescending(x => x.Id).ToList();
        }

        public ChildInvoice GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public ChildInvoice getlastproduct()
        {
            return _context.InvChild.ToList().LastOrDefault();
        }

        public void Update(ChildInvoice chldinvc)
        {
            throw new NotImplementedException();
        }
    }
}