using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web;




namespace NMS.Tools
{
    public class Tools
    {
        public string getProgectPath()
        {
            string path = HttpContext.Current.Server.MapPath("~/Reports/").ToString();
            //    path = path.Replace("\\Reports\\", ".Data\\Reports\\");
            return path;
        }

    }
}

 
 
 
   
 
