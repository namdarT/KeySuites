using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Vidly.Models
{
    public partial class DataModel : DbContext
    {
        public DataModel()
            : base("name=DataModel")
        {
        }

        public virtual DbSet<ContactInfo> ContactInfoes { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Opportunity> Opportunities { get; set; }
        public virtual DbSet<OpportunityStatus> OpportunityStatuses { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<TaskStatus> TaskStatuses { get; set; }
        public virtual DbSet<TaskType> TaskTypes { get; set; }
        public virtual DbSet<Lead> Leads { get; set; }
        public virtual DbSet<Policy> Policies { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<Quote> Quotes { get; set; }
        public virtual DbSet<ReferalSource> ReferalSources { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactInfo>()
                .Property(e => e.ContactInfoId)
                .HasPrecision(18, 0);

            modelBuilder.Entity<ContactInfo>()
                .Property(e => e.ContactName)
                .IsUnicode(false);

            modelBuilder.Entity<ContactInfo>()
                .Property(e => e.CompanyName)
                .IsUnicode(false);

            modelBuilder.Entity<ContactInfo>()
                .Property(e => e.OccupantName)
                .IsUnicode(false);

            modelBuilder.Entity<ContactInfo>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<ContactInfo>()
                .Property(e => e.WebSite)
                .IsUnicode(false);

            modelBuilder.Entity<ContactInfo>()
                .Property(e => e.CompanyAddress)
                .IsUnicode(false);

            modelBuilder.Entity<ContactInfo>()
                .Property(e => e.CompanyPhone)
                .IsUnicode(false);

            modelBuilder.Entity<ContactInfo>()
                .Property(e => e.ContactAddress)
                .IsUnicode(false);

            modelBuilder.Entity<ContactInfo>()
                .Property(e => e.ContactPhone)
                .IsUnicode(false);

            modelBuilder.Entity<ContactInfo>()
                .Property(e => e.CreatedBy)
                .HasPrecision(18, 0);

            modelBuilder.Entity<ContactInfo>()
                .Property(e => e.CompanyType)
                .IsUnicode(false);

            
            modelBuilder.Entity<Country>()
                .Property(e => e.Iso)
                .IsUnicode(false);

            modelBuilder.Entity<Country>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Country>()
                .Property(e => e.Iso3)
                .IsUnicode(false);

            modelBuilder.Entity<Opportunity>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Opportunity>()
                .HasMany(e => e.Tasks)
                .WithRequired(e => e.Opportunity)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OpportunityStatus>()
                .HasMany(e => e.Opportunities)
                .WithRequired(e => e.OpportunityStatus)
                .HasForeignKey(e => e.StatusId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaskStatus>()
                .HasMany(e => e.Tasks)
                .WithOptional(e => e.TaskStatus)
                .HasForeignKey(e => e.StatusId);

            modelBuilder.Entity<TaskType>()
                .HasMany(e => e.Tasks)
                .WithRequired(e => e.TaskType)
                .HasForeignKey(e => e.TypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.LeadsId)
                ;

            modelBuilder.Entity<Lead>()
                .Property(e => e.LeadsName)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.ContactInfoId)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Lead>()
                .Property(e => e.Breed)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.Weight)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Lead>()
                .Property(e => e.PreferedArea)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.Address)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.State)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.Zip)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Lead>()
                .Property(e => e.LeaseTerm)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.FloorPreference)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.ReferelSource)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<Policy>()
                .Property(e => e.PolicyId)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Policy>()
                .Property(e => e.Policy1)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.PropertyId)
                ;

            modelBuilder.Entity<Property>()
                .Property(e => e.PropertyDescription)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.AdminFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Property>()
                .Property(e => e.ApplicationFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Property>()
                .Property(e => e.CleaningFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Property>()
                .Property(e => e.PetFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Property>()
                .Property(e => e.ValetTrash)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.BreakLeasePolicy)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.PetBreedRestrictions)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.WeightLimit)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Property>()
                .Property(e => e.ElevatorFitnessImage)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.PoolImage)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.ParkingTypeImage)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.MailBoxImage)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.BusinessCenterImage)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.PropertyAddress)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.State)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.Area)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.Building)
                .IsUnicode(false);

            modelBuilder.Entity<Property>()
                .Property(e => e.Floor)
                .IsUnicode(false);

            modelBuilder.Entity<Quote>()
                .Property(e => e.QuoteId)
                ;

            modelBuilder.Entity<Quote>()
                .Property(e => e.PropertyId)
                ;

            modelBuilder.Entity<Quote>()
                .Property(e => e.LeadsId)
                ;

            modelBuilder.Entity<Quote>()
                .Property(e => e.OneTimeAdminFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.OneTimeAmnityFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.OneTimeFurnitureDeliveryFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.OneTimeHouseWaversSetupFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyPropertyRent)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyPetRentFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyFurnitureUsageFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyHouseWaversFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyElectricFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyGasFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyWaterSewerTrashFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyValetTrashFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyCableFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyInternetFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyMicrowaveFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyFridgeFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyWasherDrayerFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyCourierFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyMarketingFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Quote>()
                .Property(e => e.ParkingType)
                .IsUnicode(false);

            modelBuilder.Entity<Quote>()
                .Property(e => e.CreditCard)
                .IsUnicode(false);

            modelBuilder.Entity<Quote>()
                .Property(e => e.MonthlyReferalFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<ReferalSource>()
                .Property(e => e.ReferalSourceId)
                ;

            modelBuilder.Entity<ReferalSource>()
                .Property(e => e.CompanyName)
                .IsUnicode(false);

            modelBuilder.Entity<ReferalSource>()
                .Property(e => e.Address)
                .IsUnicode(false);

            modelBuilder.Entity<ReferalSource>()
                .Property(e => e.ReferalType)
                .IsUnicode(false);

            modelBuilder.Entity<Reservation>()
                .Property(e => e.RId)
                ;

            modelBuilder.Entity<Reservation>()
                .Property(e => e.QouteId)
                ;

            modelBuilder.Entity<Reservation>()
                .Property(e => e.PropertyId)
                ;

            modelBuilder.Entity<Reservation>()
                .Property(e => e.ArrivalInstructions)
                .IsUnicode(false);

            modelBuilder.Entity<Reservation>()
                .Property(e => e.DepartureInstructions)
                .IsUnicode(false);

            modelBuilder.Entity<Reservation>()
                .Property(e => e.TotalOneTime)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Reservation>()
                .Property(e => e.TotalMonthly)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Vendor>()
                .Property(e => e.VendorId)
                ;

            modelBuilder.Entity<Vendor>()
                .Property(e => e.CompanyName)
                .IsUnicode(false);

            modelBuilder.Entity<Vendor>()
                .Property(e => e.VendorType)
                .IsUnicode(false);

            modelBuilder.Entity<Vendor>()
                .Property(e => e.Address)
                .IsUnicode(false);

            modelBuilder.Entity<Vendor>()
                .Property(e => e.PhoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Vendor>()
                .Property(e => e.Website)
                .IsUnicode(false);

            modelBuilder.Entity<Vendor>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Vendor>()
                .Property(e => e.Notes)
                .IsUnicode(false);
        }
    }
}
