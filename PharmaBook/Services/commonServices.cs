
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PharmaBook.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Services
{
    public static class commonServices
    {
        public static string getDynamicId()
        {
            Random rnd = new Random();
            int Dynaid =rnd.Next(100000, 999999);
            return Convert.ToString(Dynaid);
        }

        public static double getDoubleValue(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                double temp = Convert.ToDouble(str);
                return temp;
            }
            else
                return 0;
        }
        public static int ConvertToInt(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                int temp = Convert.ToInt32(str);
                return temp;
            }
            else
                return 0;
        }

        public static string getStringValue(string str)
        {
            if (!string.IsNullOrEmpty(str))
                return str;
            else
                return "N/A";
        }
        public static DateTime ConvertToDate(string str)
        {
            var provider = CultureInfo.InvariantCulture;
            provider = new CultureInfo("en-US");
            DateTime dt = DateTime.ParseExact(str, "dd/MM/yyyy", provider);
            return dt;
        }
        public static string getDateValue(DateTime? str)
        {
            if (str!=null)
            {
                DateTime dt = Convert.ToDateTime(str);
                string d = dt.Day + " - " + dt.Month + " - " + dt.Year;
                return d;                
            }
            else
                return "N/A";
        }
    }

    public class BulkFileUpload
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public BulkFileUpload(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<string> fileUpload(IFormFile file)
        {
            SalesViewModel obj = new SalesViewModel();
            var uploads = _hostingEnvironment.WebRootPath;
            string uploadFileName = string.Empty;
            if (file.Length > 0)
            {
                string UnicFileName = Guid.NewGuid() + Path.GetExtension(file.FileName.Trim('"'));
                using (var fileStream = new FileStream(Path.Combine(uploads, UnicFileName), FileMode.Create))
                {
                    uploadFileName = UnicFileName;
                    await file.CopyToAsync(fileStream);
                }
            }
            return uploadFileName;
        }

    }
}
