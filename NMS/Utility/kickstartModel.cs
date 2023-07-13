using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RtecERP
{
    public class kickstartModel
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }

        public DateTime LastUpdated { get; set; }

    }
}