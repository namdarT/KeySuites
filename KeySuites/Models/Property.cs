namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Property
    {
        [Column(TypeName = "numeric")]
        public Int64 PropertyId { get; set; }

        [StringLength(1000)]
        public string PropertyDescription { get; set; }

        public decimal? AdminFee { get; set; }

        public decimal? ApplicationFee { get; set; }

        public decimal? CleaningFee { get; set; }

        public decimal? PetFee { get; set; }

        [StringLength(500)]
        public string ValetTrash { get; set; }

        [StringLength(1000)]
        public string BreakLeasePolicy { get; set; }

        [StringLength(5000)]
        public string PetBreedRestrictions { get; set; }

        public decimal? WeightLimit { get; set; }

        public int? MaxNoOfPets { get; set; }

        public bool? Leased { get; set; }

        public DateTime? CreatedDatee { get; set; }

        public DateTime? CreatedBy { get; set; }

        [StringLength(500)]
        public string ElevatorFitnessImage { get; set; }

        [StringLength(500)]
        public string PoolImage { get; set; }

        [StringLength(500)]
        public string ParkingTypeImage { get; set; }

        [StringLength(500)]
        public string MailBoxImage { get; set; }

        [StringLength(500)]
        public string BusinessCenterImage { get; set; }

        [StringLength(500)]
        public string PropertyAddress { get; set; }

        [StringLength(500)]
        public string State { get; set; }

        [StringLength(500)]
        public string PropertyAddress2 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Zip { get; set; }

        [StringLength(500)]
        public string City { get; set; }

        [StringLength(500)]
        public string Area { get; set; }

        [StringLength(500)]
        public string Building { get; set; }

        [StringLength(500)]
        public string Floor { get; set; }

        [StringLength(500)]
        public string Status { get; set; }

        [StringLength(500)]
        public string Name { get; set; }
        [StringLength(500)]
        public string ParkingFee { get; set; }

        [StringLength(500)]
        public string PhoneNumber { get; set; }
        [StringLength(500)]
        public string EmergencyPhoneNumber { get; set; }
        [StringLength(500)]
        public string WebSite { get; set; }
        [StringLength(500)]
        public string UnitType { get; set; }
        [StringLength(500)]
        public string UnitSize { get; set; }
        [StringLength(500)]
        public string UnitSquareFootage { get; set; }
        [StringLength(500)]
        public string FloorPlanPic1

        { get; set; }
        [StringLength(500)]
        public string FloorPlanPic2
        { get; set; }
        [StringLength(500)]
        public string FloorPlanPic3
        { get; set; }
        [StringLength(500)]
        public string FloorPlanPic
        { get; set; }
        [StringLength(500)]
        public string Hours
        { get; set; }
        [StringLength(5000)]
        public string Features
        { get; set; }
        [StringLength(5000)]
        public string Amenities
        { get; set; }
        [StringLength(500)]
        public string OtherDeposit
        { get; set; }
        [StringLength(500)]
        public string OtherDepositAmount
        { get; set; }
        [StringLength(500)]
        public string NoticetoVacate
        { get; set; }
        [StringLength(500)]
        public string Elevator
        { get; set; }
        [StringLength(500)]
        public string Pool
        { get; set; }

        [StringLength(500)]
        public string Fitness { get; set; }

        [StringLength(500)]
        public string BusinessCenter
        { get; set; }
        [StringLength(500)]
        public string ParkingType
        { get; set; }
        [StringLength(500)]
        public string MailboxLocation
        { get; set; }

        [StringLength(500)]
        public string VendorName
        { get; set; }

        [StringLength(500)]
        public string LeaseEndDate { get; set; }
        public Int64 VendorId
        { get; set; }

        public bool IsActive
        { get; set; }

        [StringLength(5000)]
        public string CommunityFeatures
        { get; set; }

        [StringLength(5000)]
        public string UnitFeatures
        { get; set; }
    }
}
