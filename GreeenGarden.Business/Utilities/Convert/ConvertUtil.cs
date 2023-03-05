using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Utilities.Convert
{
    public  class ConvertUtil
    {
        public static DateTime convertStringToDateTime(string data)
        {
            return DateTime.ParseExact(data, "dd/MM/yyyy", null);
        }
    }
}
