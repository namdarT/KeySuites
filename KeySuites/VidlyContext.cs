using System.Linq;
using System.Web;
using Vidly.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Vidly
{
    public class VidlyContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public VidlyContext() : base()
        {

        }
        public VidlyContext(DbContextOptions<VidlyContext> options)
            : base(options)
        {
            
        }
        //public Microsoft.EntityFrameworkCore.DbSet<Movie> Movies { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Customer> Customers { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<MembershipType> MembershipTypes { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<MembershipTypeGroup> MembershipTypeGroups { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Genre> Genres { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<VwAppUserAsset> VwAppUserAssets { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            modelBuilder.Entity<VwAppUserAsset>().ToTable("VwAppUserAsset");
            //modelBuilder.Entity<RuningDeviceData>(f =>
            //{
            //    f.HasKey(e => e.DeviceDataID);


            //});
            //modelBuilder.Entity<DeviceData>(f =>
            //{
            //    f.HasKey(e => e.DeviceDataID);


            //});
            //modelBuilder.Entity<UserTypes>(f =>
            //{
            //    f.HasKey(e => e.UserTypeID);


            //});
            ////modelBuilder.Entity<AspNetUsers1>().ToTable("AspNetUsers");
            ////modelBuilder.Entity<OrderRegistration>().Property(c=>c.PackageSize).HasColumnType("decimal(18,1)");
            ////modelBuilder.Entity<OrderRegistration>().Property(c => c.PackageWeight).HasColumnType("decimal(18,1)");
            //modelBuilder.Entity<OrderRegistration>().ToTable("OrderRegistration");
            ////modelBuilder.Entity<OrderRegistration>(f=> 
            ////{ 
            ////    f.Property(c => c.PackageSize).HasColumnType("decimal(18,1)");
            ////    f.Property(c => c.PackageWeight).HasColumnType("decimal(18,1)");
            ////});
            //modelBuilder.Entity<OrderList>().ToTable("OrderList");
            ////modelBuilder.Entity<OrderList>().Property(c => c.PackageSize).HasColumnType("decimal(18,1)");
            ////modelBuilder.Entity<OrderList>().Property(c => c.PackageWeight).HasColumnType("decimal(18,1)");

            ////modelBuilder.Entity<OrderList>(f =>
            ////{
            ////    f.Property(c => c.PackageSize).HasColumnType("decimal(18,1)");
            ////    f.Property(c => c.PackageWeight).HasColumnType("decimal(18,1)");
            ////});
            //modelBuilder.Entity<CustomerDef>().ToTable("CustomerDef");
            //modelBuilder.Entity<CategoryDef>().ToTable("CategoryDef");
            //modelBuilder.Entity<CategoryDef>(f =>
            //{
            //    f.HasKey(e => e.ProductCategoryID);


            //});

            //modelBuilder.Entity<DeliveryStatuses>(f =>
            //{
            //    f.HasKey(e => e.DeliveryStatus);

            //});

        }
    }
}