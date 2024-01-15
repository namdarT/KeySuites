namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Quote
    {
        [Column(TypeName = "numeric")]
        public Int64 QuoteId { get; set; }

        [Column(TypeName = "numeric")]
        public Int64? PropertyId { get; set; }

        public DateTime? LeaseStartDate { get; set; }

        public DateTime? LeaseEndDate { get; set; }

        [Column(TypeName = "numeric")]
        public Int64? LeadsId { get; set; }

        public decimal? OneTimeAdminFee { get; set; }

        public decimal? OneTimeAmnityFee { get; set; }

        public decimal? OneTimeFurnitureDeliveryFee { get; set; }

        public decimal? OneTimeHouseWaversSetupFee { get; set; }

        public decimal? MonthlyPropertyRent { get; set; }

        public decimal? MonthlyPetRentFee { get; set; }

        public decimal? MonthlyFurnitureUsageFee { get; set; }

        public string MonthlyFurniture { get; set; }
        public decimal? MonthlyHouseWaversFee { get; set; }

        public string MonthlyHouseWavers { get; set; }

        public decimal? MonthlyElectricFee { get; set; }

        public string MonthlyElectric { get; set; }
        public decimal? MonthlyGasFee { get; set; }

        public string MonthlyGas { get; set; }
        public decimal? MonthlyWaterSewerTrashFee { get; set; }

        public string MonthlyWaterSewerTrash { get; set; }
        public decimal? MonthlyValetTrashFee { get; set; }

        public string MonthlyValetTrash { get; set; }
        public decimal? MonthlyCableFee { get; set; }

        public string MonthlyCable { get; set; }
        public decimal? MonthlyInternetFee { get; set; }

        public string MonthlyInternet { get; set; }
        public decimal? MonthlyMicrowaveFee { get; set; }
        public string MonthlyMicrowave { get; set; }
        public decimal? MonthlyFridgeFee { get; set; }
        public string MonthlyFridge { get; set; }
        public decimal? MonthlyWasherDrayerFee { get; set; }

        public string MonthlyWasherDrayer { get; set; }

        public string MonthlyWasherDrayerType { get; set; }
        public decimal? MonthlyCourierFee { get; set; }

        public decimal? MonthlyMarketingFee { get; set; }

        [StringLength(50)]
        public string ParkingType { get; set; }

        public int? ParkingPlaces { get; set; }

        public int? VacancyDays { get; set; }

        [StringLength(50)]
        public string CreditCard { get; set; }

        public decimal? MonthlyReferalFee { get; set; }

        public Lead lead { get; set; }

        public ReferalSource referalSource { get; set; }
        public Property property { get; set; }
        public decimal? MonthlyBreakLeaseFee { get; set; }
        public decimal? MonthlyKSProfitFee { get; set; }
        public decimal? MonthlyInsuranceBlanketFee { get; set; }
        public decimal? MonthlyParcelServicePropertyFee { get; set; }
        public decimal? MonthlyParkingPlacesFee { get; set; }
        public string Vacancy { get; set; }
        public Int64 TotalStay { get; set; }
        public DateTime PropertyStartDate { get; set; }
        public DateTime PropertyEndDate { get; set; }
        public DateTime ClientStartDate { get; set; }
        public DateTime ClientEndDate { get; set; }
        public decimal? OneTimeOccupantBackgroundcheck { get; set; }
        public decimal? OneTimeCable { get; set; }
        public decimal? OneTimeInternet { get; set; }
        public decimal? OneTimeElectric { get; set; }
        public decimal? OneTimeGas { get; set; }
        public decimal? OneTimeWater { get; set; }
        public decimal? OneTimeTrash { get; set; }
        public decimal? OneTimeInspection { get; set; }

        public string OneTimeInspectionName { get; set; }

        public decimal? OneTimeCleaning { get; set; }

        public string OneTimeCleaningName { get; set; }
        public decimal? OneTimeGiftBasket { get; set; }
        public decimal? OneTimeRemoteFOBKeyCard { get; set; }
        public decimal? OneTimeRefundablePropFees { get; set; }
        public decimal? OneTimeRefKSDep { get; set; }
        public decimal? OneTimePropSecDep { get; set; }
        public decimal? OneTimeKSSecDep { get; set; }
        public decimal? OneTimePropPetDep { get; set; }
        public decimal? OneTimeKSPetDep { get; set; }
        public decimal? OneTimeNonRefFees { get; set; }
        public decimal? OneTimeKSAdminfee { get; set; }
        public decimal? OneTimeSureDeposit { get; set; }
        public decimal? OneTimeKSAppFee { get; set; }
        public decimal? OneTimePropPetFee { get; set; }
        public decimal? OneTimeKSPetFee { get; set; }
        public decimal? OneTimePropHoldFees { get; set; }

        public Int64 KeyID { get; set; }
        public decimal? OneTimePropertyCorporateApplicationFee { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }

        public decimal? TotalOneTime { get; set; }
        public decimal? TotalMonthly { get; set; }
        public decimal? TotalMonthlyCCost { get; set; }
        public decimal? DailyCash { get; set; }
        public decimal? DailyCredit { get; set; }
        public decimal? Charges { get; set; }
        public decimal? RefFinalAmount { get; set; }

        public decimal? MonthlyOther { get; set; }
        public decimal? OneTimeOther { get; set; }

        public string MonthlyOtherName { get; set; }
        public string OneTimeOtherName { get; set; }


    }
}
