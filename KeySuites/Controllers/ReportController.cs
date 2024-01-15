using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
//using System.Data.Entity;
using Vidly.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.ModelConfiguration.Conventions;
using ElasticsearchCRUD.Model.GeoModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using Aspose.Html;
using Aspose.Html.Converters;
using Aspose.Html.Saving;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf.draw;
using iText.Html2pdf;
using iText.Html2pdf.Html;

using iTextSharp.text.pdf;

namespace Vidly.Controllers
{
    public class ReportController : Controller
    {

        #region Class Global Members
        public decimal graphAssetID = 0;

        public string connect = Injector.SConnection;
        #endregion

        #region Class Action Methods
        [HttpPost]
        public ActionResult Index(string latitude = "")
        {
            ViewBag["id"] = 0;
            return View();
        }

       
        /// <summary>
        /// ///Unit Status Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        public ActionResult UnitStatus()
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            string connect = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("UnitStatus", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ValetTrash", DateTime.Now.AddDays(-60));
            cmd.Parameters.AddWithValue("@BreakLeasePolicy", DateTime.Now.AddDays(-1));
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Property Property_Single = new Property();
            Property Property_Detail = new Property();
            JsonResult jR = new JsonResult();
            List<Property> Property_List = new List<Property>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    
                    //Property_Single.Address = dt.Rows[i]["Address"].ToString();
                    Property_Single.AdminFee = decimal.Parse(dt.Rows[i]["AdminFee"].ToString());
                    Property_Single.PropertyId = Int64.Parse(dt.Rows[i]["PropertyId"].ToString());
                    Property_Single.ApplicationFee = decimal.Parse(dt.Rows[i]["ApplicationFee"].ToString());
                    Property_Single.City = dt.Rows[i]["City"].ToString();
                    Property_Single.Area = dt.Rows[i]["Area"].ToString();
                    if(dt.Rows[i]["RId"] == null || dt.Rows[i]["RId"].ToString() == "")
                    {
                        Property_Single.Status = dt.Rows[i]["Status"].ToString();
                    }
                    else
                    {
                        Property_Single.Status = dt.Rows[i]["ReservStatus"].ToString();
                    }
                    
                    if (dt.Rows[i]["BusinessCenterImage"].ToString() == "")
                    {
                        Property_Single.BusinessCenterImage = "No Image";
                    }
                    else
                    {
                        Property_Single.BusinessCenterImage = dt.Rows[i]["BusinessCenterImage"].ToString();
                    }
                    //Property_Single.BusinessCenterImage = dt.Rows[i]["BusinessCenterImage"].ToString();
                    Property_Single.WeightLimit = decimal.Parse(dt.Rows[i]["WeightLimit"].ToString());
                    Property_Single.BreakLeasePolicy = dt.Rows[i]["BreakLeasePolicy"].ToString();
                    Property_Single.Building = dt.Rows[i]["Building"].ToString();
                    Property_Single.CleaningFee = decimal.Parse(dt.Rows[i]["CleaningFee"].ToString());
                    Property_Single.CreatedBy = DateTime.Now;
                    Property_Single.CreatedDatee = DateTime.Now;
                    if (dt.Rows[i]["ElevatorFitnessImage"].ToString() == "")
                    {
                        Property_Single.ElevatorFitnessImage = "No Image";
                    }
                    else
                    {
                        Property_Single.ElevatorFitnessImage = dt.Rows[i]["ElevatorFitnessImage"].ToString();
                    }
                    Property_Single.PetFee = decimal.Parse(dt.Rows[i]["PetFee"].ToString());
                    Property_Single.MaxNoOfPets = Int16.Parse(dt.Rows[i]["MaxNoOfPets"].ToString());
                    Property_Single.Leased = false;
                    if (dt.Rows[i]["MailBoxImage"].ToString() == "")
                    {
                        Property_Single.MailBoxImage = "No Image";
                    }
                    else
                    {
                        Property_Single.MailBoxImage = dt.Rows[i]["MailBoxImage"].ToString();
                    }

                    Property_Single.Floor = dt.Rows[i]["Floor"].ToString();
                    if (dt.Rows[i]["ParkingTypeImage"].ToString() == "")
                    {
                        Property_Single.ParkingTypeImage = "No Image";
                    }
                    else
                    {
                        Property_Single.ParkingTypeImage = dt.Rows[i]["ParkingTypeImage"].ToString();
                    }

                    Property_Single.PetBreedRestrictions = dt.Rows[i]["PetBreedRestrictions"].ToString();
                    if (dt.Rows[i]["PoolImage"].ToString() == "")
                    {
                        Property_Single.PoolImage = "No Image";
                    }
                    else
                    {
                        Property_Single.PoolImage = dt.Rows[i]["PoolImage"].ToString();
                    }
                    //Property_Single.Floor = dt.Rows[i]["Floor"].ToString();
                    //Property_Single.ParkingTypeImage = dt.Rows[i]["ParkingTypeImage"].ToString();
                    //Property_Single.PetBreedRestrictions = dt.Rows[i]["PetBreedRestrictions"].ToString();
                    //Property_Single.PoolImage = dt.Rows[i]["PoolImage"].ToString();
                    Property_Single.PropertyAddress = dt.Rows[i]["PropertyAddress"].ToString();
                    Property_Single.State = dt.Rows[i]["State"].ToString();
                    Property_Single.PropertyDescription = dt.Rows[i]["PropertyDescription"].ToString();
                    //Property_Single.ValetTrash = dt.Rows[i]["ValetTrash"].ToString();

                    ///
                    Property_Single.CreatedBy = DateTime.Now.AddDays(-60);
                    Property_Single.CreatedDatee = DateTime.Now.AddDays(-1);

                    Property_List.Add(Property_Single);
                    Property_Single = new Property();

                }

                //if (id == 0)
                //{
                //    ViewData["ProcessQuote"] = null;
                //    Property_Detail = Property_List[0];
                //}
                //else
                //{
                //    ViewData["ProcessQuote"] = "p";
                //    ViewData["LeadId"] = id;
                //    //Property_Detail = Property_List.Where(a => a.PropertyId == id).FirstOrDefault();
                //}
            }

            if (Session["SuccessMessage"] != null)
            {
                if (Session["SuccessMessage"].ToString() != "")
                {
                    ViewData["SuccessMessage"] = Session["SuccessMessage"].ToString();
                    Session["SuccessMessage"] = null;
                }
                else
                {
                    Session["SuccessMessage"] = null;
                }

            }
            else
            {
                ViewData["SuccessMessage"] = "";
            }

            //Add View
            if (ViewData["error"] == null || ViewData["error"].ToString() == "")
            {
                if (Session["error"] != null)
                {

                    ViewData["error"] = Session["error"].ToString();
                }
                else
                {

                    ViewData["error"] = "";
                }
            }

            return View(Property_List);
        }

        [HttpPost]
        public ActionResult UnitStatus(FormCollection model)
        {

            string connect = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("UnitStatus", con);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                DateTime FromDate = DateTime.Parse(model["CreatedBy"]);
                //DateTime ToDate = DateTime.Parse(model["BreakLeasePolicy"]);
                cmd.Parameters.AddWithValue("@ValetTrash", FromDate.Month + "/" + FromDate.Day + "/" + FromDate.Year);
                //cmd.Parameters.AddWithValue("@BreakLeasePolicy", ToDate.Month + "/" + ToDate.Day + "/" + ToDate.Year);
            }
            catch (Exception ex)
            {
                //DateTime FromDate = DateTime.Parse(model["FromDate"]);
                //DateTime ToDate = DateTime.Parse(model["ToDate"]);
                cmd.Parameters.AddWithValue("@ValetTrash", model["CreatedBy"]);
                //cmd.Parameters.AddWithValue("@BreakLeasePolicy", model["BreakLeasePolicy"]);
            }
            try
            {
                //DateTime FromDate = DateTime.Parse(model["ValetTrash"]);
                DateTime ToDate = DateTime.Parse(model["CreatedDatee"]);
                //cmd.Parameters.AddWithValue("@ValetTrash", FromDate.Month + "/" + FromDate.Day + "/" + FromDate.Year);
                cmd.Parameters.AddWithValue("@BreakLeasePolicy", ToDate.Month + "/" + ToDate.Day + "/" + ToDate.Year);
            }
            catch (Exception ex)
            {
                //DateTime FromDate = DateTime.Parse(model["FromDate"]);
                //DateTime ToDate = DateTime.Parse(model["ToDate"]);
                //cmd.Parameters.AddWithValue("@ValetTrash", model["ValetTrash"]);
                cmd.Parameters.AddWithValue("@BreakLeasePolicy", model["CreatedDatee"]);
            }
            //cmd.Parameters.AddWithValue("@ValetTrash",  model["ValetTrash"].ToString());
            //cmd.Parameters.AddWithValue("@BreakLeasePolicy", model["BreakLeasePolicy"].ToString());
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Property Property_Single = new Property();
            Property Property_Detail = new Property();
            JsonResult jR = new JsonResult();
            List<Property> Property_List = new List<Property>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    //Property_Single.Address = dt.Rows[i]["Address"].ToString();
                    Property_Single.AdminFee = decimal.Parse(dt.Rows[i]["AdminFee"].ToString());
                    Property_Single.PropertyId = Int64.Parse(dt.Rows[i]["PropertyId"].ToString());
                    Property_Single.ApplicationFee = decimal.Parse(dt.Rows[i]["ApplicationFee"].ToString());
                    Property_Single.City = dt.Rows[i]["City"].ToString();
                    Property_Single.Area = dt.Rows[i]["Area"].ToString();
                    Property_Single.Status = dt.Rows[i]["ReservStatus"].ToString();
                    if (dt.Rows[i]["BusinessCenterImage"].ToString() == "")
                    {
                        Property_Single.BusinessCenterImage = "No Image";
                    }
                    else
                    {
                        Property_Single.BusinessCenterImage = dt.Rows[i]["BusinessCenterImage"].ToString();
                    }
                    //Property_Single.BusinessCenterImage = dt.Rows[i]["BusinessCenterImage"].ToString();
                    Property_Single.WeightLimit = decimal.Parse(dt.Rows[i]["WeightLimit"].ToString());
                    Property_Single.BreakLeasePolicy = dt.Rows[i]["BreakLeasePolicy"].ToString();
                    Property_Single.Building = dt.Rows[i]["Building"].ToString();
                    Property_Single.CleaningFee = decimal.Parse(dt.Rows[i]["CleaningFee"].ToString());
                    Property_Single.CreatedBy = DateTime.Parse(model["CreatedBy"]);
                    Property_Single.CreatedDatee = DateTime.Parse(model["CreatedDatee"]);
                    if (dt.Rows[i]["ElevatorFitnessImage"].ToString() == "")
                    {
                        Property_Single.ElevatorFitnessImage = "No Image";
                    }
                    else
                    {
                        Property_Single.ElevatorFitnessImage = dt.Rows[i]["ElevatorFitnessImage"].ToString();
                    }
                    Property_Single.PetFee = decimal.Parse(dt.Rows[i]["PetFee"].ToString());
                    Property_Single.MaxNoOfPets = Int16.Parse(dt.Rows[i]["MaxNoOfPets"].ToString());
                    Property_Single.Leased = false;
                    if (dt.Rows[i]["MailBoxImage"].ToString() == "")
                    {
                        Property_Single.MailBoxImage = "No Image";
                    }
                    else
                    {
                        Property_Single.MailBoxImage = dt.Rows[i]["MailBoxImage"].ToString();
                    }

                    Property_Single.Floor = dt.Rows[i]["Floor"].ToString();
                    if (dt.Rows[i]["ParkingTypeImage"].ToString() == "")
                    {
                        Property_Single.ParkingTypeImage = "No Image";
                    }
                    else
                    {
                        Property_Single.ParkingTypeImage = dt.Rows[i]["ParkingTypeImage"].ToString();
                    }

                    Property_Single.PetBreedRestrictions = dt.Rows[i]["PetBreedRestrictions"].ToString();
                    if (dt.Rows[i]["PoolImage"].ToString() == "")
                    {
                        Property_Single.PoolImage = "No Image";
                    }
                    else
                    {
                        Property_Single.PoolImage = dt.Rows[i]["PoolImage"].ToString();
                    }
                    //Property_Single.Floor = dt.Rows[i]["Floor"].ToString();
                    //Property_Single.ParkingTypeImage = dt.Rows[i]["ParkingTypeImage"].ToString();
                    //Property_Single.PetBreedRestrictions = dt.Rows[i]["PetBreedRestrictions"].ToString();
                    //Property_Single.PoolImage = dt.Rows[i]["PoolImage"].ToString();
                    Property_Single.PropertyAddress = dt.Rows[i]["PropertyAddress"].ToString();
                    Property_Single.State = dt.Rows[i]["State"].ToString();
                    Property_Single.PropertyDescription = dt.Rows[i]["PropertyDescription"].ToString();
                    //Property_Single.ValetTrash = dt.Rows[i]["ValetTrash"].ToString();

                    Property_Single.ValetTrash = model["CreatedBy"].ToString();
                    Property_Single.BreakLeasePolicy = model["CreatedDatee"].ToString();

                    Property_List.Add(Property_Single);
                    Property_Single = new Property();

                }
                

                //if (id == 0)
                //{
                //    ViewData["ProcessQuote"] = null;
                //    Property_Detail = Property_List[0];
                //}
                //else
                //{
                //    ViewData["ProcessQuote"] = "p";
                //    ViewData["LeadId"] = id;
                //    //Property_Detail = Property_List.Where(a => a.PropertyId == id).FirstOrDefault();
                //}
            }
            else
            {
                Property_Single = new Property()
                {
                AdminFee = 0,
                PropertyId = 0,
                ApplicationFee = 0,
                City = "",
                Area = "",
                Status = "",
                
                BusinessCenterImage = "No Image",
                
                WeightLimit = 0,
                BreakLeasePolicy = model["BreakLeasePolicy"],
                Building = "",
                CleaningFee = 0,
                CreatedBy = DateTime.Now,
                CreatedDatee = DateTime.Now,
                
                ElevatorFitnessImage = "No Image",
                
                PetFee = 0,
                MaxNoOfPets = 0,
                Leased = false,
                
                MailBoxImage = "No Image",
                
                Floor = "",
                
                ParkingTypeImage = "No Image",
                

                PetBreedRestrictions = "",
                
                PoolImage = "No Image",
                
                PropertyAddress = "",
                State = "",
                PropertyDescription = "",
                

                ValetTrash = model["ValetTrash"]

                };
                Property_List.Add(Property_Single);
                Property_Single = new Property();
            }
            if (Session["SuccessMessage"] != null)
            {
                if (Session["SuccessMessage"].ToString() != "")
                {
                    ViewData["SuccessMessage"] = Session["SuccessMessage"].ToString();
                    Session["SuccessMessage"] = null;
                }
                else
                {
                    Session["SuccessMessage"] = null;
                }

            }
            else
            {
                ViewData["SuccessMessage"] = "";
            }

            //Add View
            if (ViewData["error"] == null || ViewData["error"].ToString() == "")
            {
                if (Session["error"] != null)
                {

                    ViewData["error"] = Session["error"].ToString();
                }
                else
                {

                    ViewData["error"] = "";
                }
            }

            return View(Property_List);
        }

        public FileResult DownloadKeysuitesNTV()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "Keysuites NTV.docx", "text/plain", "Keysuites NTV.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");
                
            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.pdf", "text/plain", "R-" + 5 + "KLS Reservation Agreement.pdf");
        }

        public FileResult DownloadCreditCardAuthorization()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "Credit Card Authorization.docx", "text/plain", "Credit Card Authorization.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }

        public FileResult DownloadKeyluxeSuitesPDFreservationquote()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/");
                return File(path + "keyluxe Suite Quote template.docx", "text/plain", "keyluxe Suite Quote template.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }

        public FileResult DownloadMiFi()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "MiFi.docx", "text/plain", "MiFi.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }

        public FileResult DownloadRentalAgreement()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "Rental Agreement.docx", "text/plain", "Rental Agreement.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }

        public FileResult DownloadSmokingAddendum()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "Smoking Addendum.docx", "text/plain", "Smoking Addendum.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }

        public FileResult DownloadAMICS()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "AMICS.docx", "text/plain", "AMICS.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }

        public FileResult DownloadFurnitureandHousewaresCheckList()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "Furniture and Housewares Check List.docx", "text/plain", "Furniture and Housewares Check List.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }

        public FileResult DownloadFurniturePackage()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "Furniture Package.docx", "text/plain", "Furniture Package.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }

        public FileResult DownloadIndividualRentalApplication()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "Individual Rental Application.docx", "text/plain", "Individual Rental Application.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }

        public FileResult DownloadInsuranceChecklist()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "Insurance Checklist.docx", "text/plain", "Insurance Checklist.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }

        public FileResult DownloadKeyluxeAuthorizedSigners()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "Keyluxe Authorized Signers.docx", "text/plain", "Keyluxe Authorized Signers.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }

        public FileResult Downloadleasecancellation()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "lease cancellation.docx", "text/plain", "lease cancellation.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }

        public FileResult DownloadLetterofFinancialResponsibility()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "Letter of Financial Responsibility.docx", "text/plain", "Letter of Financial Responsibility.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }

        public FileResult DownloadTradeReferenceSheet()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "Trade Reference Sheet.docx", "text/plain", "Trade Reference Sheet.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }

        public FileResult DownloadVendorApplication()
        {
            if (Session["LoginUserRole"] != null)
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "Vendor Application.docx", "text/plain", "Vendor Application.docx");
            }
            else
            {
                string path = Server.MapPath("~/Uploads/DownloadableDocs/");
                return File(path + "", "text/plain", "");

            }
            //string path = Server.MapPath("~/Uploads/");
            //return File(path + "R-" + 5 + "KLS Reservation Agreement.docx", "text/plain", "R-" + 5 + "KLS Reservation Agreement.docx");
        }


        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string GridHtml)
        {
            return File(Encoding.ASCII.GetBytes(GridHtml), "application/vnd.ms-excel", "UnitsStatus-" + DateTime.Now + ".xls");
        }


        /// <summary>
        /// ///Leased List Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        public ActionResult LeasedList()
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            //string connect = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("LeasedList", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Year", DateTime.Now.Year.ToString());
            
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            LeasedList LeasedList_Single = new LeasedList();
            LeasedList LeasedList_Detail = new LeasedList();
            
            List<LeasedList> LeasedList_List = new List<LeasedList>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    //LeasedList_Single.Address = dt.Rows[i]["Address"].ToString();
                    LeasedList_Single.AdminFee = decimal.Parse(dt.Rows[i]["OneTimeKSAdminFee"].ToString());
                    LeasedList_Single.PetFee = decimal.Parse(dt.Rows[i]["OneTimeKSPetFee"].ToString());
                    LeasedList_Single.ApplicationFee = decimal.Parse(dt.Rows[i]["OneTimeKSAppFee"].ToString());
                    //LeasedList_Single.ArrivalInstructions = dt.Rows[i]["ArrivalInstructions"].ToString();
                    //LeasedList_Single.CheckInTime = DateTime.Parse(dt.Rows[i]["CheckInTime"].ToString());
                    //LeasedList_Single.CheckOutTime = DateTime.Parse(dt.Rows[i]["CheckOutTime"].ToString());
                    
                    //LeasedList_Single.BusinessCenterImage = dt.Rows[i]["BusinessCenterImage"].ToString();
                    LeasedList_Single.ContactEmail = dt.Rows[i]["ContactEmail"].ToString();
                    LeasedList_Single.ContactNumber = dt.Rows[i]["ContactNumber"].ToString();
                    //LeasedList_Single.DepartureInstructions = dt.Rows[i]["DepartureInstructions"].ToString();
                    //LeasedList_Single.CleaningFee = decimal.Parse(dt.Rows[i]["CleaningFee"].ToString());
                    
                    
                    LeasedList_Single.OcupantName =  dt.Rows[i]["OcupantName"].ToString();
                    LeasedList_Single.LeaseStartDate = DateTime.Parse(dt.Rows[i]["LeaseStartDate"].ToString());
                    LeasedList_Single.LeaseEndDate = DateTime.Parse(dt.Rows[i]["LeaseEndDate"].ToString());
                    LeasedList_Single.PropertyDescription = dt.Rows[i]["PropertyDescription"].ToString();
                    LeasedList_Single.PropertyId = Int64.Parse(dt.Rows[i]["PropertyId"].ToString());

                    LeasedList_Single.QouteId = Int64.Parse(dt.Rows[i]["QuoteId"].ToString());
                    LeasedList_Single.RId = Int64.Parse(dt.Rows[i]["RId"].ToString());
                    LeasedList_Single.TotalMonthly = decimal.Parse(dt.Rows[i]["Charges"].ToString());
                    LeasedList_Single.TotalOneTime = decimal.Parse(dt.Rows[i]["TotalStay"].ToString());
                    //LeasedList_Single.TotalOneTime = decimal.Parse(dt.Rows[i]["TotalOneTime"].ToString());

                    LeasedList_List.Add(LeasedList_Single);
                    LeasedList_Single = new LeasedList();

                }

                //if (id == 0)
                //{
                //    ViewData["ProcessQuote"] = null;
                //    LeasedList_Detail = LeasedList_List[0];
                //}
                //else
                //{
                //    ViewData["ProcessQuote"] = "p";
                //    ViewData["LeadId"] = id;
                //    //LeasedList_Detail = LeasedList_List.Where(a => a.LeasedListId == id).FirstOrDefault();
                //}
            }

            if (Session["SuccessMessage"] != null)
            {
                if (Session["SuccessMessage"].ToString() != "")
                {
                    ViewData["SuccessMessage"] = Session["SuccessMessage"].ToString();
                    Session["SuccessMessage"] = null;
                }
                else
                {
                    Session["SuccessMessage"] = null;
                }

            }
            else
            {
                ViewData["SuccessMessage"] = "";
            }

            //Add View
            if (ViewData["error"] == null || ViewData["error"].ToString() == "")
            {
                if (Session["error"] != null)
                {

                    ViewData["error"] = Session["error"].ToString();
                }
                else
                {

                    ViewData["error"] = "";
                }
            }
            ViewBag.Year = DropDownListYear(DateTime.Now.Year.ToString());
            return View(LeasedList_List);
        }

        [HttpPost]
        public ActionResult LeasedList(FormCollection model)
        {
            string connect = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("LeasedList", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Year", model["Year"].ToString());

            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            LeasedList LeasedList_Single = new LeasedList();
            LeasedList LeasedList_Detail = new LeasedList();

            List<LeasedList> LeasedList_List = new List<LeasedList>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    //LeasedList_Single.Address = dt.Rows[i]["Address"].ToString();
                    LeasedList_Single.AdminFee = decimal.Parse(dt.Rows[i]["OneTimeKSAdminFee"].ToString());
                    LeasedList_Single.PetFee = decimal.Parse(dt.Rows[i]["OneTimeKSPetFee"].ToString());
                    LeasedList_Single.ApplicationFee = decimal.Parse(dt.Rows[i]["OneTimeKSAppFee"].ToString());
                    //LeasedList_Single.ArrivalInstructions = dt.Rows[i]["ArrivalInstructions"].ToString();
                    //LeasedList_Single.CheckInTime = DateTime.Parse(dt.Rows[i]["CheckInTime"].ToString());
                    //LeasedList_Single.CheckOutTime = DateTime.Parse(dt.Rows[i]["CheckOutTime"].ToString());

                    //LeasedList_Single.BusinessCenterImage = dt.Rows[i]["BusinessCenterImage"].ToString();
                    LeasedList_Single.ContactEmail = dt.Rows[i]["ContactEmail"].ToString();
                    LeasedList_Single.ContactNumber = dt.Rows[i]["ContactNumber"].ToString();
                    //LeasedList_Single.DepartureInstructions = dt.Rows[i]["DepartureInstructions"].ToString();
                    //LeasedList_Single.CleaningFee = decimal.Parse(dt.Rows[i]["CleaningFee"].ToString());


                    LeasedList_Single.OcupantName = dt.Rows[i]["OcupantName"].ToString();
                    LeasedList_Single.LeaseStartDate = DateTime.Parse(dt.Rows[i]["LeaseStartDate"].ToString());
                    LeasedList_Single.LeaseEndDate = DateTime.Parse(dt.Rows[i]["LeaseEndDate"].ToString());
                    LeasedList_Single.PropertyDescription = dt.Rows[i]["PropertyDescription"].ToString();
                    LeasedList_Single.PropertyId = Int64.Parse(dt.Rows[i]["PropertyId"].ToString());

                    LeasedList_Single.QouteId = Int64.Parse(dt.Rows[i]["QuoteId"].ToString());
                    LeasedList_Single.RId = Int64.Parse(dt.Rows[i]["RId"].ToString());
                    LeasedList_Single.TotalMonthly = decimal.Parse(dt.Rows[i]["Charges"].ToString());
                    LeasedList_Single.TotalOneTime = decimal.Parse(dt.Rows[i]["TotalStay"].ToString());
                    //LeasedList_Single.TotalOneTime = decimal.Parse(dt.Rows[i]["TotalOneTime"].ToString());

                    LeasedList_List.Add(LeasedList_Single);
                    LeasedList_Single = new LeasedList();

                }

                //if (id == 0)
                //{
                //    ViewData["ProcessQuote"] = null;
                //    LeasedList_Detail = LeasedList_List[0];
                //}
                //else
                //{
                //    ViewData["ProcessQuote"] = "p";
                //    ViewData["LeadId"] = id;
                //    //LeasedList_Detail = LeasedList_List.Where(a => a.LeasedListId == id).FirstOrDefault();
                //}
            }

            if (Session["SuccessMessage"] != null)
            {
                if (Session["SuccessMessage"].ToString() != "")
                {
                    ViewData["SuccessMessage"] = Session["SuccessMessage"].ToString();
                    Session["SuccessMessage"] = null;
                }
                else
                {
                    Session["SuccessMessage"] = null;
                }

            }
            else
            {
                ViewData["SuccessMessage"] = "";
            }

            //Add View
            if (ViewData["error"] == null || ViewData["error"].ToString() == "")
            {
                if (Session["error"] != null)
                {

                    ViewData["error"] = Session["error"].ToString();
                }
                else
                {

                    ViewData["error"] = "";
                }
            }
            ViewBag.Year = DropDownListYear(model["Year"].ToString());
            return View(LeasedList_List);
        }

        [HttpPost]
        [ValidateInput(false)]
        public FileResult ExportLeasedList(string GridHtml)
        {
            return File(Encoding.ASCII.GetBytes(GridHtml), "application/vnd.ms-excel", "LeasedList-" + DateTime.Now + ".xls");
        }

        public List<SelectListItem> DropDownListYear(string Year)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (Year != "")
            {
                item = new SelectListItem()
                {
                    Value = Year,
                    Text = Year
                };
                ProductList1.Add(item);
            }
            
            for (Int64 i = 2022; i <= DateTime.Now.Year; i++)
            {
                if (Year == i.ToString())
                {

                }
                else
                {

                    item = new SelectListItem()
                    {
                        Value = i.ToString(),

                        Text = i.ToString()
                    };
                    ProductList1.Add(item);
                }
            }

            return ProductList1;
        }



        /// <summary>
        /// ///Referal Source Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        public ActionResult ReferalSourceReport()
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            //string connect = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("ReferelSourcesReport", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", DateTime.Now.AddDays(-60));
            cmd.Parameters.AddWithValue("@ToDate", DateTime.Now.AddDays(-1));
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            ReferalSourceReport ReferalSourceReport_Single = new ReferalSourceReport();
            ReferalSourceReport ReferalSourceReport_Detail = new ReferalSourceReport();
            JsonResult jR = new JsonResult();
            List<ReferalSourceReport> ReferalSourceReport_List = new List<ReferalSourceReport>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    ReferalSourceReport_Single.FromDate = DateTime.Now.AddDays(-60);
                    ReferalSourceReport_Single.ToDate = DateTime.Now.AddDays(-1);

                    ReferalSourceReport_Single.Address = dt.Rows[i]["Address"].ToString();
                    ReferalSourceReport_Single.CompanyName = dt.Rows[i]["CompanyName"].ToString();
                    ReferalSourceReport_Single.ReferalSourceId = Int64.Parse(dt.Rows[i]["ReferalSourceId"].ToString());
                    if(dt.Rows[i]["NoOfReservation"].ToString() == "")
                    {
                        ReferalSourceReport_Single.NoOfReservation = 0;
                    }
                    else
                    {
                        ReferalSourceReport_Single.NoOfReservation = decimal.Parse(dt.Rows[i]["NoOfReservation"].ToString());
                    }
                    
                    ReferalSourceReport_Single.Number = dt.Rows[i]["Number"].ToString();
                    if (dt.Rows[i]["PropertiesRent"].ToString() == "")
                    {
                        ReferalSourceReport_Single.PropertiesRent = 0;
                    }
                    else
                    {
                        ReferalSourceReport_Single.PropertiesRent = decimal.Parse(dt.Rows[i]["PropertiesRent"].ToString());
                    }
                    
                    ReferalSourceReport_Single.ReferalType = dt.Rows[i]["ReferalType"].ToString();
                    if (dt.Rows[i]["ShareAmount"].ToString() == "")
                    {
                        ReferalSourceReport_Single.ShareAmount = 0;
                    }
                    else
                    {
                        ReferalSourceReport_Single.ShareAmount = decimal.Parse(dt.Rows[i]["ShareAmount"].ToString());
                    }

                    
                    if (dt.Rows[i]["TotalFinalAmount"].ToString() == "")
                    {
                        ReferalSourceReport_Single.TotalFinalAmount = 0;
                    }
                    else
                    {
                        ReferalSourceReport_Single.TotalFinalAmount = decimal.Parse(dt.Rows[i]["TotalFinalAmount"].ToString());
                    }

                    ReferalSourceReport_List.Add(ReferalSourceReport_Single);
                    ReferalSourceReport_Single = new ReferalSourceReport();


                }

                //if (id == 0)
                //{
                //    ViewData["ProcessQuote"] = null;
                //    ReferalSourceReport_Detail = ReferalSourceReport_List[0];
                //}
                //else
                //{
                //    ViewData["ProcessQuote"] = "p";
                //    ViewData["LeadId"] = id;
                //    //ReferalSourceReport_Detail = ReferalSourceReport_List.Where(a => a.ReferalSourceReportId == id).FirstOrDefault();
                //}
            }

            if (Session["SuccessMessage"] != null)
            {
                if (Session["SuccessMessage"].ToString() != "")
                {
                    ViewData["SuccessMessage"] = Session["SuccessMessage"].ToString();
                    Session["SuccessMessage"] = null;
                }
                else
                {
                    Session["SuccessMessage"] = null;
                }

            }
            else
            {
                ViewData["SuccessMessage"] = "";
            }

            //Add View
            if (ViewData["error"] == null || ViewData["error"].ToString() == "")
            {
                if (Session["error"] != null)
                {

                    ViewData["error"] = Session["error"].ToString();
                }
                else
                {

                    ViewData["error"] = "";
                }
            }

            return View(ReferalSourceReport_List);
        }

        [HttpPost]
        public ActionResult ReferalSourceReport(FormCollection model)
        {

            //string connect = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("ReferelSourcesReport", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@FromDate", DateTime.Parse(model["FromDate"].ToString()).Month + "/" + DateTime.Parse(model["FromDate"].ToString()).Day + "/" + DateTime.Parse(model["FromDate"].ToString()).Year);
            //cmd.Parameters.AddWithValue("@ToDate", DateTime.Parse(model["ToDate"].ToString()).Month + "/" + DateTime.Parse(model["ToDate"].ToString()).Day + "/" + DateTime.Parse(model["ToDate"].ToString()).Year);
            
            try 
            {
                DateTime FromDate = DateTime.Parse(model["FromDate"]);
                //DateTime ToDate = DateTime.Parse(model["ToDate"]);
                cmd.Parameters.AddWithValue("@FromDate", FromDate.Month + "/" + FromDate.Day + "/" + FromDate.Year);
                //cmd.Parameters.AddWithValue("@ToDate", ToDate.Month + "/" + ToDate.Day + "/" + ToDate.Year);
            }
            catch(Exception ex)
            {
                //DateTime FromDate = DateTime.Parse(model["FromDate"]);
                //DateTime ToDate = DateTime.Parse(model["ToDate"]);
                cmd.Parameters.AddWithValue("@FromDate", model["FromDate"]);
                //cmd.Parameters.AddWithValue("@ToDate", model["ToDate"]);
            }

            try
            {
                //DateTime FromDate = DateTime.Parse(model["FromDate"]);
                DateTime ToDate = DateTime.Parse(model["ToDate"]);
                //cmd.Parameters.AddWithValue("@FromDate", FromDate.Month + "/" + FromDate.Day + "/" + FromDate.Year);
                cmd.Parameters.AddWithValue("@ToDate", ToDate.Month + "/" + ToDate.Day + "/" + ToDate.Year);
            }
            catch (Exception ex)
            {
                //DateTime FromDate = DateTime.Parse(model["FromDate"]);
                //DateTime ToDate = DateTime.Parse(model["ToDate"]);
                //cmd.Parameters.AddWithValue("@FromDate", model["FromDate"]);
                cmd.Parameters.AddWithValue("@ToDate", model["ToDate"]);
            }

            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            ReferalSourceReport ReferalSourceReport_Single = new ReferalSourceReport();
            ReferalSourceReport ReferalSourceReport_Detail = new ReferalSourceReport();
            JsonResult jR = new JsonResult();
            List<ReferalSourceReport> ReferalSourceReport_List = new List<ReferalSourceReport>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ReferalSourceReport_Single.FromDate = DateTime.Parse( model["FromDate"].ToString());
                    ReferalSourceReport_Single.ToDate = DateTime.Parse(model["ToDate"].ToString());
                    ReferalSourceReport_Single.Address = dt.Rows[i]["Address"].ToString();
                    ReferalSourceReport_Single.CompanyName = dt.Rows[i]["CompanyName"].ToString();
                    ReferalSourceReport_Single.ReferalSourceId = Int64.Parse(dt.Rows[i]["ReferalSourceId"].ToString());
                    if (dt.Rows[i]["NoOfReservation"].ToString() == "")
                    {
                        ReferalSourceReport_Single.NoOfReservation = 0;
                    }
                    else
                    {
                        ReferalSourceReport_Single.NoOfReservation = decimal.Parse(dt.Rows[i]["NoOfReservation"].ToString());
                    }

                    ReferalSourceReport_Single.Number = dt.Rows[i]["Number"].ToString();
                    if (dt.Rows[i]["PropertiesRent"].ToString() == "")
                    {
                        ReferalSourceReport_Single.PropertiesRent = 0;
                    }
                    else
                    {
                        ReferalSourceReport_Single.PropertiesRent = decimal.Parse(dt.Rows[i]["PropertiesRent"].ToString());
                    }

                    ReferalSourceReport_Single.ReferalType = dt.Rows[i]["ReferalType"].ToString();
                    if (dt.Rows[i]["ShareAmount"].ToString() == "")
                    {
                        ReferalSourceReport_Single.ShareAmount = 0;
                    }
                    else
                    {
                        ReferalSourceReport_Single.ShareAmount = decimal.Parse(dt.Rows[i]["ShareAmount"].ToString());
                    }

                    if (dt.Rows[i]["TotalFinalAmount"].ToString() == "")
                    {
                        ReferalSourceReport_Single.TotalFinalAmount = 0;
                    }
                    else
                    {
                        ReferalSourceReport_Single.TotalFinalAmount = decimal.Parse(dt.Rows[i]["TotalFinalAmount"].ToString());
                    }


                    ReferalSourceReport_List.Add(ReferalSourceReport_Single);
                    ReferalSourceReport_Single = new ReferalSourceReport();

                }

                //if (id == 0)
                //{
                //    ViewData["ProcessQuote"] = null;
                //    ReferalSourceReport_Detail = ReferalSourceReport_List[0];
                //}
                //else
                //{
                //    ViewData["ProcessQuote"] = "p";
                //    ViewData["LeadId"] = id;
                //    //ReferalSourceReport_Detail = ReferalSourceReport_List.Where(a => a.ReferalSourceReportId == id).FirstOrDefault();
                //}
            }

            if (Session["SuccessMessage"] != null)
            {
                if (Session["SuccessMessage"].ToString() != "")
                {
                    ViewData["SuccessMessage"] = Session["SuccessMessage"].ToString();
                    Session["SuccessMessage"] = null;
                }
                else
                {
                    Session["SuccessMessage"] = null;
                }

            }
            else
            {
                ViewData["SuccessMessage"] = "";
            }

            //Add View
            if (ViewData["error"] == null || ViewData["error"].ToString() == "")
            {
                if (Session["error"] != null)
                {

                    ViewData["error"] = Session["error"].ToString();
                }
                else
                {

                    ViewData["error"] = "";
                }
            }

            return View(ReferalSourceReport_List);
        }

        [HttpPost]
        [ValidateInput(false)]
        public FileResult ExportReferalSource(string GridHtml)
        {
            return File(Encoding.ASCII.GetBytes(GridHtml), "application/vnd.ms-excel", "ReferalSource-"+ DateTime.Now +".xls");
        }






        //////////////////
        #endregion
    }
}