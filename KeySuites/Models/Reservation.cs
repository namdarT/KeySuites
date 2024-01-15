namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Reservation
    {
        [Key]
        [Column(TypeName = "numeric")]
        public Int64 RId { get; set; }

        [Column(TypeName = "numeric")]
        public Int64? QouteId { get; set; }

        [Column(TypeName = "numeric")]
        public Int64? PropertyId { get; set; }

        public DateTime? LeaseStartDate { get; set; }

        public DateTime? LeaseEndDate { get; set; }

        public DateTime? CheckInTime { get; set; }

        [StringLength(1000)]
        public string ArrivalInstructions { get; set; }

        public DateTime? CheckOutTime { get; set; }

        [StringLength(1000)]
        public string DepartureInstructions { get; set; }

        public decimal? TotalOneTime { get; set; }

        public decimal? TotalMonthly { get; set; }

        public Property property { get; set; }

        public Quote quote { get; set; }
        public bool IsActive
        { get; set; }

        public string CheckinKeyArrangements { get; set; }

        public string EntryGateCode { get; set; }

        public string MailboxNumber { get; set; }
        public string MailboxLocation { get; set; }
        public string TrashDisposal { get; set; }
        public string WifiNetworkName { get; set; }

        public string WifiPassword { get; set; }
        public string Housekeeping { get; set; }
        public string ParkingNumberofspaces { get; set; }
        public string ParkingAssignedSpace { get; set; }
        public string ParkingBusinessCenterHours { get; set; }
        public string ParkingFitnessCenterHours { get; set; }

        public string ParkingPoolHours { get; set; }

        public string CustomerServiceNumber { get; set; }

        public string Emergencynumber { get; set; }

        public string GuestName { get; set; }
        public string Status { get; set; }
    }
}
