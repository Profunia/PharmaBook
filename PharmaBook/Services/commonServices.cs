using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.Services
{
    public static class commonServices
    {

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
        public static int getIntValue(string str)
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
}
