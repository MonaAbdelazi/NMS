using System.Web;
using System.Net.Http;
using System.Web.Mvc;

using System.Web.Http.Controllers;

namespace NMS.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuthorization
    {
        string ProjectName { get; set; }
        string Url { get; set; }
        string UserName { get; set; }
        string Area { get; set; }
        string Controller { get; set; }
        string Action { get; set; }
        AuthorizationContext FilterContext { get; set; }
        HttpActionContext HttpFilterContext { get; set; }
        bool CheckPermission();

    }
}
