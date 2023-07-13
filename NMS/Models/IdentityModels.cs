using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NMS.Data;

namespace NMS.Models
{
    
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public int Branch_ID { get; set; }
        public int WareHouse_ID { get; set; }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // MUST go first.

            //  modelBuilder.HasDefaultSchema("dbo"); // Use uppercase!

            modelBuilder.Entity<ApplicationUser>().ToTable("dbo.AspNetUsers");
            modelBuilder.Entity<IdentityRole>().ToTable("dbo.AspNetRoles");
            modelBuilder.Entity<IdentityUserRole>().ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("AspNetUserClaims");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("AspNetUserLogins");
           
           


        }
        //public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        //public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        ////public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        ////public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        //public virtual DbSet<AspNetRolePermission> AspNetRolePermissions { get; set; }
        //public virtual DbSet<AspNetUserPermission> AspNetUserPermissions { get; set; }
    }

}