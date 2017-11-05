
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PharmaBook.Entities;
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
            int Dynaid = rnd.Next(100000, 999999);
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
            if (str != null)
            {
                DateTime dt = Convert.ToDateTime(str);
                string d = dt.Day + " - " + dt.Month + " - " + dt.Year;
                return d;
            }
            else
                return "N/A";
        }

        public static ProductViewModel MapProductToVM(Product x)
        {
            ProductViewModel prd = new ProductViewModel();
            prd.batchNo = x.batchNo;
            prd.companyName = x.companyName;
            prd.expDate = x.expDate.ToString("dd/MM/yyyy");
            prd.name = x.name;
            prd.batchNo = x.batchNo;
            prd.MRP = x.MRP;
            prd.openingStock = x.openingStock;
            prd.lastUpdated = x.lastUpdated;
            prd.cusUserName = x.cusUserName;

            prd.vendorID = x.vendorID != null ? x.vendorID : 0;

            return prd;
        }

        public static Product MapVMtoProduct(ProductViewModel x)
        {
            Product prd = new Product();
            prd.batchNo = x.batchNo;
            prd.companyName = x.companyName;
            prd.expDate = commonServices.ConvertToDate(x.expDate);
            prd.name = x.name;
            prd.batchNo = x.batchNo;
            prd.MRP = x.MRP;
            prd.openingStock = x.openingStock;
            prd.lastUpdated = x.lastUpdated;
            prd.cusUserName = x.cusUserName;
            prd.vendorID = x.vendorID != null ? x.vendorID : 0;

            return prd;
        }

        public static List<ProductViewModel> MapProductListToVM(List<Product> products)
        {
            List<ProductViewModel> pvm = new List<ProductViewModel>();
            ProductViewModel vm = null;
            Parallel.ForEach(products, x =>
            {
                vm = new ProductViewModel();
                vm.Id = x.Id;
                vm.batchNo = x.batchNo;
                vm.companyName = x.companyName;
                vm.expDate = x.expDate.ToString("dd/MM/yyy");
                vm.name = x.name;
                vm.batchNo = x.batchNo;
                vm.MRP = x.MRP;
                vm.openingStock = x.openingStock;
                vm.lastUpdated = x.lastUpdated;
                vm.cusUserName = x.cusUserName;
                vm.vendorID = x.vendorID;
                pvm.Add(vm);
            });
            return pvm;
        }

        public static IEnumerable<PurchasedHistoryVM> MapPurchasedHistoryToVM(IEnumerable<PurchasedHistory> obj, IEnumerable<Vendor> vendorList)
        {
            List<PurchasedHistoryVM> vmList = new List<PurchasedHistoryVM>();
            PurchasedHistoryVM vm = null;
            Parallel.ForEach(obj, x =>
            {
                vm = new PurchasedHistoryVM();
                vm.Id = x.Id;
                vm.ProductID = x.ProductID;
                vm.vendorID = x.vendorID;
                var vendorInfo = vendorList.Where(z => z.Id == x.vendorID).FirstOrDefault();
                if (vendorInfo != null)
                {
                    vm.vendorname = vendorInfo.vendorName;
                    vm.vendorcompany = vendorInfo.vendorCompnay;
                    vm.vendoradres = vendorInfo.vendorAddress;

                }
                vm.MRP = x.MRP;
                vm.qty = x.qty;
                vm.purchasedDated = x.purchasedDated.ToString("dd/MM/yyy");
                vm.cusUserName = x.cusUserName;
                vm.Name = x.Name;
                vm.Mfg = x.Mfg;
                vm.BatchNo = x.BatchNo;
                string [] s= x.ExpDate.Split(' ');
                vm.ExpDate = s[0];
                vm.Remark = x.Remark;
                vmList.Add(vm);
            });
            return vmList;
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
