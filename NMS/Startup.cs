using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using NMS.Models;

[assembly: OwinStartupAttribute(typeof(NMS.Startup))]
namespace NMS
{
    public partial class Startup
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateDefaultRolesAndUsers();
        }
        public void CreateDefaultRolesAndUsers()
        {
            var rolemanger = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
             var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
          
            IdentityRole role = new IdentityRole();
            if (!rolemanger.RoleExists("Admins"))
            {

                role.Name = "Admins";
                rolemanger.Create(role);
                ApplicationUser user = new ApplicationUser();
                user.UserName = "Ahmed";
                var check = usermanager.Create(user, "inedu0000");
                if (check.Succeeded)
                {
                    usermanager.AddToRole(user.Id, "Admins");
                }
            }
        }
    }
}
