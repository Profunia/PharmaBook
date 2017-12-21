using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PharmaBook.Entities;
namespace PharmaBook.Services
{
    public interface IErrorLogger
    {
        Task<List<ErrorLogger>> GetAllError();
    }
    public class ErrorLoggerServices
    {
    }
}
