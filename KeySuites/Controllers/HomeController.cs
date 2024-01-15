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

namespace Vidly.Controllers
{
    public class HomeController : Controller
    {

        #region Class Global Members
        public decimal graphAssetID = 0;

        public string connect = Injector.SConnection;
        #endregion

        #region Class Action Methods
        [HttpPost]
        public ActionResult Index(string latitude = "")
        {

            if (Session["LoginUserRole"] != null)
            {
                //vwAppUserAsset_Save.last_update_user_id = Session["LoginUserEmail"].ToString();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag["id"] = 0;



            string[] monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            ///Lease
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("DashboardGraphLeases", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader DrLeases;
            con.Open();
            DrLeases = cmd.ExecuteReader();
            DataTable dtLeases = new DataTable("Vw");
            dtLeases.Load(DrLeases);

            con.Close();

            ///Leads

            cmd = new SqlCommand("DashboardGraphLeads", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader DrLeads;
            con.Open();
            DrLeads = cmd.ExecuteReader();
            DataTable dtLeads = new DataTable("Vw");
            dtLeads.Load(DrLeads);

            con.Close();

            ///Lease

            cmd = new SqlCommand("DashboardGraphQuotes", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader DrQuotes;
            con.Open();
            DrQuotes = cmd.ExecuteReader();
            DataTable dtQuotes = new DataTable("Vw");
            dtQuotes.Load(DrQuotes);

            con.Close();

            DashboardGraphReport DashboardGraphReport_Single = new DashboardGraphReport();
            DashboardGraphReport DashboardGraphReport_Detail = new DashboardGraphReport();

            List<DashboardGraphReport> DashboardGraphReport_List = new List<DashboardGraphReport>();

            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dtLeads.Rows.Count > 0)
            {
                int k = 0;
                for (int i = 0; i <= 11; i++)
                {
                    if (Int64.Parse(dtLeads.Rows[i]["Month"].ToString()) == k + 1)
                    {
                        DashboardGraphReport_Single.MonthName = monthNames[k];
                        DashboardGraphReport_Single.Month = Int64.Parse(dtLeads.Rows[i]["Month"].ToString());
                        DashboardGraphReport_Single.Count = Int64.Parse(dtLeads.Rows[i]["Count"].ToString());

                        DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                        DashboardGraphReport_Single = new DashboardGraphReport();

                    }
                    else
                    {
                        for (int j = 0; j <= Int64.Parse(dtLeads.Rows[i]["Month"].ToString()); j++)
                        {
                            DashboardGraphReport_Single.MonthName = monthNames[j];
                            DashboardGraphReport_Single.Month = j + 1;
                            DashboardGraphReport_Single.Count = 0;

                            DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                            DashboardGraphReport_Single = new DashboardGraphReport();

                        }
                        k = Int16.Parse(dtLeads.Rows[i]["Month"].ToString());
                    }

                }

            }

            ViewBag.Leads = DashboardGraphReport_List;

            DashboardGraphReport_Single = new DashboardGraphReport();
            DashboardGraphReport_Detail = new DashboardGraphReport();

            DashboardGraphReport_List = new List<DashboardGraphReport>();

            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dtQuotes.Rows.Count > 0)
            {
                int k = 0;
                for (int i = 0; i <= 11; i++)
                {
                    if (Int64.Parse(dtQuotes.Rows[i]["Month"].ToString()) == k + 1)
                    {
                        DashboardGraphReport_Single.MonthName = monthNames[k];
                        DashboardGraphReport_Single.Month = Int64.Parse(dtQuotes.Rows[i]["Month"].ToString());
                        DashboardGraphReport_Single.Count = Int64.Parse(dtQuotes.Rows[i]["Count"].ToString());

                        DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                        DashboardGraphReport_Single = new DashboardGraphReport();

                    }
                    else
                    {
                        for (int j = 0; j <= Int64.Parse(dtQuotes.Rows[i]["Month"].ToString()); j++)
                        {
                            DashboardGraphReport_Single.MonthName = monthNames[j];
                            DashboardGraphReport_Single.Month = j + 1;
                            DashboardGraphReport_Single.Count = 0;

                            DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                            DashboardGraphReport_Single = new DashboardGraphReport();

                        }
                        k = Int16.Parse(dtQuotes.Rows[i]["Month"].ToString());
                    }

                }

            }

            ViewBag.Quotes = DashboardGraphReport_List;

            DashboardGraphReport_Single = new DashboardGraphReport();
            DashboardGraphReport_Detail = new DashboardGraphReport();

            DashboardGraphReport_List = new List<DashboardGraphReport>();

            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dtLeases.Rows.Count > 0)
            {
                int k = 0;
                for (int i = 0; i <= 11; i++)
                {
                    if (Int64.Parse(dtLeases.Rows[i]["Month"].ToString()) == k + 1)
                    {
                        DashboardGraphReport_Single.MonthName = monthNames[k];
                        DashboardGraphReport_Single.Month = Int64.Parse(dtLeases.Rows[i]["Month"].ToString());
                        DashboardGraphReport_Single.Count = Int64.Parse(dtLeases.Rows[i]["Count"].ToString());

                        DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                        DashboardGraphReport_Single = new DashboardGraphReport();

                    }
                    else
                    {
                        for (int j = 0; j <= Int64.Parse(dtLeases.Rows[i]["Month"].ToString()); j++)
                        {
                            DashboardGraphReport_Single.MonthName = monthNames[j];
                            DashboardGraphReport_Single.Month = j + 1;
                            DashboardGraphReport_Single.Count = 0;

                            DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                            DashboardGraphReport_Single = new DashboardGraphReport();

                        }
                        k = Int16.Parse(dtLeases.Rows[i]["Month"].ToString());
                    }

                }

            }

            ViewBag.Leases = DashboardGraphReport_List;

            return View();
        }

        public ActionResult SensorsList()
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public ActionResult Index(decimal id = 0)
        {

            if (Session["LoginUserRole"] != null)
            {
                //vwAppUserAsset_Save.last_update_user_id = Session["LoginUserEmail"].ToString();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }


            try
            {


                ViewBag.Leads = LeadBinding();

                ViewBag.Quotes = QuoteBinding();

                ViewBag.Leases = LeaseBinding();

                DashboardFigures l_DFigures = new DashboardFigures();
                l_DFigures = DashboardFiguresBinding();

                ViewBag.Difference = l_DFigures.Difference;
                ViewBag.LeadCount = l_DFigures.LeadCount;
                ViewBag.LeadCountCurrentYear = l_DFigures.LeadCountCurrentYear;
                ViewBag.LeaseCountCurrentYear = l_DFigures.LeaseCountCurrentYear;
                ViewBag.Leased = l_DFigures.Leased;
                ViewBag.LeasedCurrentYear = l_DFigures.LeasedCurrentYear;
                ViewBag.LeasedPrevMonth = l_DFigures.LeasedPrevMonth;
                ViewBag.QuoteCountCurrentYear = l_DFigures.QuoteCountCurrentYear;
                ViewBag.QuoteCount = l_DFigures.QuoteCount;

                ViewBag.PendingLease = PendingLeaseBinding();

                ViewBag.PendingQuote = PendingQuoteBinding();

                ViewBag.LeaseHistory = LeaseHistoryBinding();

                ViewBag.RecentContact = RecentContactsBinding();


            }
            catch (Exception ex)
            {
                
            }

            return View();
        }


        public ActionResult HomePage(decimal id = 0)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            if (TempData["Latitude"] != null)
            {
                string lat = TempData["Latitude"].ToString();
            }
            string connect = System.Configuration.ConfigurationManager.ConnectionStrings["simplicityCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name, temperature, deviceSN, folder_Id, latitude, longitude, logical_termination_dt, username from dbo.vw_app_user_assets where logical_termination_dt is null and username = 'admin@simplicityintegration.com'", con);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);
            //var itemss = this.context.Database.As("select * from device_attribute_data");
            //this.OnVwAppUserAssetsRead(ref items);

            //var file = System.IO.File.Create("/Vwlog.txt");
            //BinaryFormatter b = new BinaryFormatter();

            //b.Serialize(file, dt.Rows[0][1].ToString());

            //file.Close();

            con.Close();

            VwAppUserAsset vwAppUserAsset_Single = new VwAppUserAsset();
            VwAppUserAsset vwAppUserAsset_Detail = new VwAppUserAsset();
            JsonResult jR = new JsonResult();
            List<VwAppUserAsset> vwAppUserAsset_List = new List<VwAppUserAsset>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            for (int i = 0; i < 8; i++)
            {

                vwAppUserAsset_Single.manufacturer_id = (decimal)dt.Rows[i]["manufacturer_id"];
                vwAppUserAsset_Single.asset_id = (decimal)dt.Rows[i]["asset_id"];
                vwAppUserAsset_Single.asset_name = dt.Rows[i]["asset_name"].ToString();
                vwAppUserAsset_Single.folder_Id = dt.Rows[i]["folder_Id"].ToString();
                vwAppUserAsset_Single.deviceSN = dt.Rows[i]["deviceSN"].ToString();
                vwAppUserAsset_Single.latitude = dt.Rows[i]["latitude"].ToString();
                vwAppUserAsset_Single.longitude = dt.Rows[i]["longitude"].ToString();
                vwAppUserAsset_Single.temperature = dt.Rows[i]["temperature"].ToString();
                vwAppUserAsset_List.Add(vwAppUserAsset_Single);
                vwAppUserAsset_Single = new VwAppUserAsset();

            }

            if (id == 0)
            {
                vwAppUserAsset_Detail = vwAppUserAsset_List[0];
            }
            else
            {
                vwAppUserAsset_Detail = vwAppUserAsset_List.Where(a => a.asset_id == id).FirstOrDefault();
            }

            IEnumerable<VwAppUserAsset> ienum = vwAppUserAsset_List;
            jR.Data = ienum;
            object str = jR.Data;
            //string json = str.ToString();


            DataTable dt1 = new DataTable();
            dt1.Columns.AddRange(new DataColumn[2] { new DataColumn("Lattiude"), new DataColumn("Longitude") });
            dt1.Rows.Add("32.81109", "-96.57952");
            dt1.Rows.Add("17.266700", "78.530200");


            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt1.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt1.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }

            var searchresult = new MapDetail
            {
                DetailsCoordinates = new GeoPoint(-95.4061, 29.1992),
                Id = 1,
                Name = ".NET User Group Bern",
                Details = "http://www.dnug-bern.ch/",
                DetailsType = "Work",

            };


            var mapModel = new MapModel
            {
                MapData = serializer.Serialize(searchresult),
                // Bern	Lat 46.94792, Long 7.44461
                CenterLatitude = 29.1992,
                CenterLongitude = -95.4061,
                MaxDistanceInMeter = 0
            };

            //return View(mapModel);
            Tuple<IEnumerable<Vidly.Models.VwAppUserAsset>, Vidly.Models.VwAppUserAsset> tuple1 = new Tuple<IEnumerable<VwAppUserAsset>, VwAppUserAsset>(ienum, vwAppUserAsset_Detail);


            ViewBag.Markers = serializer.Serialize(rows);
            return View(tuple1);
        }
        public ActionResult User()
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult Table()
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            string connect = System.Configuration.ConfigurationManager.ConnectionStrings["simplicityCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name, temperature, deviceSN, folder_Id, latitude, longitude, logical_termination_dt, username from dbo.vw_app_user_assets where logical_termination_dt is null and username = 'admin@simplicityintegration.com'", con);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);
            con.Close();

            VwAppUserAsset vwAppUserAsset_Single = new VwAppUserAsset();
            VwAppUserAsset vwAppUserAsset_Detail = new VwAppUserAsset();
            JsonResult jR = new JsonResult();
            List<VwAppUserAsset> vwAppUserAsset_List = new List<VwAppUserAsset>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            for (int i = 0; i < 8; i++)
            {

                vwAppUserAsset_Single.manufacturer_id = (decimal)dt.Rows[i]["manufacturer_id"];
                vwAppUserAsset_Single.asset_id = (decimal)dt.Rows[i]["asset_id"];
                vwAppUserAsset_Single.asset_name = dt.Rows[i]["asset_name"].ToString();
                vwAppUserAsset_Single.folder_Id = dt.Rows[i]["folder_Id"].ToString();
                vwAppUserAsset_Single.deviceSN = dt.Rows[i]["deviceSN"].ToString();
                vwAppUserAsset_Single.latitude = dt.Rows[i]["latitude"].ToString();
                vwAppUserAsset_Single.longitude = dt.Rows[i]["longitude"].ToString();
                vwAppUserAsset_Single.temperature = dt.Rows[i]["temperature"].ToString();
                vwAppUserAsset_List.Add(vwAppUserAsset_Single);
                vwAppUserAsset_Single = new VwAppUserAsset();

            }


            vwAppUserAsset_Detail = vwAppUserAsset_List[0];
            //vwAppUserAsset_Detail.VwAppUserAssetList = vwAppUserAsset_List;
            IEnumerable<VwAppUserAsset> ienum = vwAppUserAsset_List;
            jR.Data = ienum;
            object str = jR.Data;
            //Tuple<IEnumerable<Vidly.Models.VwAppUserAsset>, Vidly.Models.VwAppUserAsset> tuple1 = new Tuple<IEnumerable<VwAppUserAsset>, VwAppUserAsset>(ienum, vwAppUserAsset_Detail);

            return View(vwAppUserAsset_Detail);
        }

        public ActionResult Typography()
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult Icons()
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult Notifications()
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult Upgrade()
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        #region Class Call Pipe Line Methods
        public ActionResult Status(decimal id = 0)
        {
            graphAssetID = id;
            Session["TabID"] = "2";
            Session["Tab3AssetID"] = id;
            return RedirectToAction("Index");
        }
        public ActionResult Graph(decimal id = 0)
        {
            graphAssetID = id;
            Session["TabID"] = "3";
            Session["Tab3AssetID"] = id;
            return RedirectToAction("Index");
        }

        public ActionResult Map(decimal id = 0)
        {
            graphAssetID = id;
            Session["TabID"] = "1";
            Session["Tab3AssetID"] = id;
            return RedirectToAction("Index");
        }

        #endregion

        #endregion

        #region Class Dependent Methods

        public DashboardFigures DashboardFiguresBinding()
        {
            SqlConnection con = new SqlConnection(connect);

            SqlCommand cmd = new SqlCommand("DashboardFigures", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader DrLease;
            con.Open();
            DrLease = cmd.ExecuteReader();
            DataTable dtLease = new DataTable("Vw");
            dtLease.Load(DrLease);

            con.Close();

            DashboardFigures DashboardFigures_Single = new DashboardFigures();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dtLease.Rows.Count > 0)
            {

                
                    DashboardFigures_Single.QuoteCountCurrentYear = Int16.Parse(dtLease.Rows[0]["QuoteCountCurrentYear"].ToString());
                    DashboardFigures_Single.QuoteCount = Int16.Parse(dtLease.Rows[0]["QuoteCount"].ToString());
                    DashboardFigures_Single.LeasedPrevMonth = decimal.Parse( dtLease.Rows[0]["LeasedPrevMonth"].ToString());
                    DashboardFigures_Single.LeasedCurrentYear = decimal.Parse(dtLease.Rows[0]["LeasedCurrentYear"].ToString());
                    DashboardFigures_Single.Leased = decimal.Parse(dtLease.Rows[0]["Leased"].ToString());
                    DashboardFigures_Single.LeaseCountCurrentYear = short.Parse(dtLease.Rows[0]["LeaseCountCurrentYear"].ToString());
                    DashboardFigures_Single.LeadCountCurrentYear = short.Parse(dtLease.Rows[0]["LeadCountCurrentYear"].ToString());
                    DashboardFigures_Single.LeadCount = short.Parse(dtLease.Rows[0]["LeadCount"].ToString());
                    DashboardFigures_Single.Difference = decimal.Parse( dtLease.Rows[0]["Difference"].ToString());


            }

            return DashboardFigures_Single;


        }
        public IEnumerable<Contact> RecentContactsBinding()
        {
            SqlConnection con = new SqlConnection(connect);

            SqlCommand cmd = new SqlCommand("DashboardRecentContacts", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader DrLease;
            con.Open();
            DrLease = cmd.ExecuteReader();
            DataTable dtLease = new DataTable("Vw");
            dtLease.Load(DrLease);

            con.Close();

            Contact Contact_Single = new Contact();

            List<Contact> Contact_List = new List<Contact>();
            
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dtLease.Rows.Count > 0)
            {

                for (int i = 0; i < dtLease.Rows.Count; i++)
                {
                    Contact_Single.Company = dtLease.Rows[i]["CompanyName"].ToString();
                    Contact_Single.FirstName = dtLease.Rows[i]["FirstName"].ToString();
                    Contact_Single.LastName = dtLease.Rows[i]["LastName"].ToString();
                    Contact_Single.Phone = dtLease.Rows[i]["Phone"].ToString();
                    Contact_Single.Email = dtLease.Rows[i]["Email"].ToString();
                    Contact_Single.Id = Int64.Parse(dtLease.Rows[i]["Id"].ToString());

                    Contact_List.Add(Contact_Single);
                    Contact_Single = new Contact();
                }
            }
            else
            {
                Contact_Single = new Contact()
                {
                    Address = "",
                    Company = "",
                    Email = "",
                    CompanyId = 0,
                    FirstName = "",
                    Id = 0,
                    IsActive = true,
                    LastName = "",
                    Phone = ""
                };
                
                Contact_List.Add(Contact_Single);
            }

            return Contact_List;


        }
        public IEnumerable<DashboardLeaseHistoryReport> LeaseHistoryBinding()
        {
            SqlConnection con = new SqlConnection(connect);

            SqlCommand cmd = new SqlCommand("DashboardLeaseHistory", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader DrLease;
            con.Open();
            DrLease = cmd.ExecuteReader();
            DataTable dtLease = new DataTable("Vw");
            dtLease.Load(DrLease);

            con.Close();

            DashboardLeaseHistoryReport DashboardLeaseHistoryReport_Single = new DashboardLeaseHistoryReport();

            List<DashboardLeaseHistoryReport> DashboardLeaseHistoryReport_List = new List<DashboardLeaseHistoryReport>();

            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dtLease.Rows.Count > 0)
            {

                for (int i = 0; i < dtLease.Rows.Count; i++)
                {
                    DashboardLeaseHistoryReport_Single.Charges = decimal.Parse(dtLease.Rows[i]["Charges"].ToString());
                    DashboardLeaseHistoryReport_Single.Name = dtLease.Rows[i]["Name"].ToString();
                    DashboardLeaseHistoryReport_Single.OcupantName = dtLease.Rows[i]["OcupantName"].ToString();

                    DashboardLeaseHistoryReport_List.Add(DashboardLeaseHistoryReport_Single);
                    DashboardLeaseHistoryReport_Single = new DashboardLeaseHistoryReport();
                }
            }
            else
            {
                DashboardLeaseHistoryReport_Single  = new DashboardLeaseHistoryReport() { Charges = 0,
                Name = "",
                OcupantName = "No Data"};
                
                DashboardLeaseHistoryReport_List.Add(DashboardLeaseHistoryReport_Single);
                
            }

            return DashboardLeaseHistoryReport_List;


        }

        public IEnumerable<DashboardPendingLeasesReport> PendingLeaseBinding()
        {
            SqlConnection con = new SqlConnection(connect);

            SqlCommand cmd = new SqlCommand("DashboardPendingLease", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader DrLease;
            con.Open();
            DrLease = cmd.ExecuteReader();
            DataTable dtLease = new DataTable("Vw");
            dtLease.Load(DrLease);

            con.Close();

            DashboardPendingLeasesReport DashboardPendingLeasesReport_Single = new DashboardPendingLeasesReport();

            List<DashboardPendingLeasesReport> DashboardPendingLeasesReport_List = new List<DashboardPendingLeasesReport>();

            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dtLease.Rows.Count > 0)
            {

                for (int i = 0; i < dtLease.Rows.Count; i++)
                {
                    DashboardPendingLeasesReport_Single.QuoteId = Int64.Parse(dtLease.Rows[i]["QuoteId"].ToString());
                    DashboardPendingLeasesReport_Single.Name = dtLease.Rows[i]["Name"].ToString();
                    DashboardPendingLeasesReport_Single.ReferelSource = dtLease.Rows[i]["ReferelSource"].ToString();
                    DashboardPendingLeasesReport_Single.SpentDays = dtLease.Rows[i]["SpentDays"].ToString();
                    DashboardPendingLeasesReport_Single.OcupantName = dtLease.Rows[i]["OcupantName"].ToString();

                    DashboardPendingLeasesReport_List.Add(DashboardPendingLeasesReport_Single);
                    DashboardPendingLeasesReport_Single = new DashboardPendingLeasesReport();
                }
            }
            else
            {
                DashboardPendingLeasesReport_Single  = new DashboardPendingLeasesReport(){ QuoteId = 0,
                OcupantName = "No Data",
                 Name = "",
                CreatedDate = DateTime.Now,
                ReferelSource = "",
                SpentDays = ""};
                DashboardPendingLeasesReport_List.Add(DashboardPendingLeasesReport_Single);
                
            }

            return DashboardPendingLeasesReport_List;

            
        }

        public IEnumerable<DashboardPendingQuoteReport> PendingQuoteBinding()
        {
            SqlConnection con = new SqlConnection(connect);

            SqlCommand cmd = new SqlCommand("DashboardPendingQuote", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader DrQuote;
            con.Open();
            DrQuote = cmd.ExecuteReader();
            DataTable dtQuote = new DataTable("Vw");
            dtQuote.Load(DrQuote);

            con.Close();

            DashboardPendingQuoteReport DashboardPendingQuoteReport_Single = new DashboardPendingQuoteReport();

            List<DashboardPendingQuoteReport> DashboardPendingQuoteReport_List = new List<DashboardPendingQuoteReport>();

            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dtQuote.Rows.Count > 0)
            {

                for (int i = 0; i < dtQuote.Rows.Count; i++)
                {
                    DashboardPendingQuoteReport_Single.LeadsId = Int64.Parse(dtQuote.Rows[i]["LeadsId"].ToString());
                    //DashboardPendingQuoteReport_Single.Name = dtQuote.Rows[i]["Name"].ToString();
                    DashboardPendingQuoteReport_Single.ReferelSource = dtQuote.Rows[i]["ReferelSource"].ToString();
                    DashboardPendingQuoteReport_Single.SpentDays = dtQuote.Rows[i]["SpentDays"].ToString();
                    DashboardPendingQuoteReport_Single.OcupantName = dtQuote.Rows[i]["OcupantName"].ToString();

                    DashboardPendingQuoteReport_List.Add(DashboardPendingQuoteReport_Single);
                    DashboardPendingQuoteReport_Single = new DashboardPendingQuoteReport();
                }
            }
            else
            {
                DashboardPendingQuoteReport_Single = new DashboardPendingQuoteReport()
                {
                    LeadsId = 0,
                    OcupantName = "No Data",
                    Name = "",
                    CreatedDate = DateTime.Now,
                    ReferelSource = "",
                    SpentDays = ""
                };
                DashboardPendingQuoteReport_List.Add(DashboardPendingQuoteReport_Single);

            }

            return DashboardPendingQuoteReport_List;


        }
        public IEnumerable< DashboardGraphReport> LeadBinding()
        {
            string[] monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            ///Lease
            SqlConnection con = new SqlConnection(connect);
            ///Leads

            SqlCommand cmd = new SqlCommand("DashboardGraphLeads", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader DrLeads;
            con.Open();
            DrLeads = cmd.ExecuteReader();
            DataTable dtLeads = new DataTable("Vw");
            dtLeads.Load(DrLeads);

            con.Close();

            DashboardGraphReport DashboardGraphReport_Single = new DashboardGraphReport();
            DashboardGraphReport DashboardGraphReport_Detail = new DashboardGraphReport();

            List<DashboardGraphReport> DashboardGraphReport_List = new List<DashboardGraphReport>();

            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dtLeads.Rows.Count > 0)
            {
                int k = 0;
                for (int i = 0; i <= 11; i++)
                {
                    if (i < dtLeads.Rows.Count)
                    {
                        if (Int64.Parse(dtLeads.Rows[i]["Month"].ToString()) == k + 1)
                        {
                            DashboardGraphReport_Single.MonthName = monthNames[k];
                            DashboardGraphReport_Single.Month = Int64.Parse(dtLeads.Rows[i]["Month"].ToString());
                            DashboardGraphReport_Single.Count = Int64.Parse(dtLeads.Rows[i]["Count"].ToString());

                            DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                            DashboardGraphReport_Single = new DashboardGraphReport();
                            k = k + 1;
                        }
                        else
                        {
                            for (int j = 0; j < Int64.Parse(dtLeads.Rows[i]["Month"].ToString()) - 1; j++)
                            {
                                DashboardGraphReport_Single.MonthName = monthNames[j];
                                DashboardGraphReport_Single.Month = j + 1;
                                DashboardGraphReport_Single.Count = 0;

                                DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                                DashboardGraphReport_Single = new DashboardGraphReport();

                            }
                            DashboardGraphReport_Single.MonthName = monthNames[Int16.Parse(dtLeads.Rows[i]["Month"].ToString()) - 1];
                            DashboardGraphReport_Single.Month = Int64.Parse(dtLeads.Rows[i]["Month"].ToString());
                            DashboardGraphReport_Single.Count = Int64.Parse(dtLeads.Rows[i]["Count"].ToString());

                            DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                            DashboardGraphReport_Single = new DashboardGraphReport();
                            k = Int16.Parse(dtLeads.Rows[i]["Month"].ToString());
                        }
                    }
                    else
                    {
                        DashboardGraphReport_Single.MonthName = monthNames[k];
                        DashboardGraphReport_Single.Month = k + 1;
                        DashboardGraphReport_Single.Count = 0;

                        DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                        DashboardGraphReport_Single = new DashboardGraphReport();
                        k = k + 1;
                        i = k - 1;
                    }
                }

            }

            return DashboardGraphReport_List;


        }

        public IEnumerable<DashboardGraphReport> QuoteBinding()
        {
            string[] monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            ///Lease
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("DashboardGraphQuotes", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader DrQuotes;
            con.Open();
            DrQuotes = cmd.ExecuteReader();
            DataTable dtQuotes = new DataTable("Vw");
            dtQuotes.Load(DrQuotes);

            con.Close();

            DashboardGraphReport DashboardGraphReport_Single = new DashboardGraphReport();
            DashboardGraphReport DashboardGraphReport_Detail = new DashboardGraphReport();

            List<DashboardGraphReport> DashboardGraphReport_List = new List<DashboardGraphReport>();

            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dtQuotes.Rows.Count > 0)
            {
                int k = 0;
                for (int i = 0; i <= 11; i++)
                {
                    if (i < dtQuotes.Rows.Count)
                    {
                        if (Int64.Parse(dtQuotes.Rows[i]["Month"].ToString()) == k + 1)
                        {
                            DashboardGraphReport_Single.MonthName = monthNames[k];
                            DashboardGraphReport_Single.Month = Int64.Parse(dtQuotes.Rows[i]["Month"].ToString());
                            DashboardGraphReport_Single.Count = Int64.Parse(dtQuotes.Rows[i]["Count"].ToString());

                            DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                            DashboardGraphReport_Single = new DashboardGraphReport();
                            k = k + 1;
                        }
                        else
                        {
                            for (int j = 0; j < Int64.Parse(dtQuotes.Rows[i]["Month"].ToString()) - 1; j++)
                            {
                                DashboardGraphReport_Single.MonthName = monthNames[j];
                                DashboardGraphReport_Single.Month = j + 1;
                                DashboardGraphReport_Single.Count = 0;

                                DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                                DashboardGraphReport_Single = new DashboardGraphReport();

                            }
                            DashboardGraphReport_Single.MonthName = monthNames[Int16.Parse(dtQuotes.Rows[i]["Month"].ToString()) - 1];
                            DashboardGraphReport_Single.Month = Int64.Parse(dtQuotes.Rows[i]["Month"].ToString());
                            DashboardGraphReport_Single.Count = Int64.Parse(dtQuotes.Rows[i]["Count"].ToString());

                            DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                            DashboardGraphReport_Single = new DashboardGraphReport();
                            k = Int16.Parse(dtQuotes.Rows[i]["Month"].ToString());
                        }
                    }
                    else
                    {
                        DashboardGraphReport_Single.MonthName = monthNames[k];
                        DashboardGraphReport_Single.Month = k + 1;
                        DashboardGraphReport_Single.Count = 0;

                        DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                        DashboardGraphReport_Single = new DashboardGraphReport();
                        k = k + 1;
                        i = k - 1;
                    }
                }

            }

            

            return DashboardGraphReport_List;


        }

        public IEnumerable<DashboardGraphReport> LeaseBinding()
        {
            string[] monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            ///Lease
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("DashboardGraphLeases", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader DrLeases;
            con.Open();
            DrLeases = cmd.ExecuteReader();
            DataTable dtLeases = new DataTable("Vw");
            dtLeases.Load(DrLeases);

            con.Close();

            DashboardGraphReport DashboardGraphReport_Single = new DashboardGraphReport();
            DashboardGraphReport DashboardGraphReport_Detail = new DashboardGraphReport();

            List<DashboardGraphReport> DashboardGraphReport_List = new List<DashboardGraphReport>();

            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dtLeases.Rows.Count > 0)
            {
                int k = 0;
                for (int i = 0; i <= 11; i++)
                {
                    if (i < dtLeases.Rows.Count)
                    {
                        if (Int64.Parse(dtLeases.Rows[i]["Month"].ToString()) == k + 1)
                        {
                            DashboardGraphReport_Single.MonthName = monthNames[k];
                            DashboardGraphReport_Single.Month = Int64.Parse(dtLeases.Rows[i]["Month"].ToString());
                            DashboardGraphReport_Single.Count = Int64.Parse(dtLeases.Rows[i]["Count"].ToString());

                            DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                            DashboardGraphReport_Single = new DashboardGraphReport();
                            k = k + 1;
                        }
                        else
                        {
                            for (int j = 0; j < Int64.Parse(dtLeases.Rows[i]["Month"].ToString()) - 1; j++)
                            {
                                DashboardGraphReport_Single.MonthName = monthNames[j];
                                DashboardGraphReport_Single.Month = j + 1;
                                DashboardGraphReport_Single.Count = 0;

                                DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                                DashboardGraphReport_Single = new DashboardGraphReport();

                            }
                            DashboardGraphReport_Single.MonthName = monthNames[Int16.Parse(dtLeases.Rows[i]["Month"].ToString()) - 1];
                            DashboardGraphReport_Single.Month = Int64.Parse(dtLeases.Rows[i]["Month"].ToString());
                            DashboardGraphReport_Single.Count = Int64.Parse(dtLeases.Rows[i]["Count"].ToString());

                            DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                            DashboardGraphReport_Single = new DashboardGraphReport();
                            k = Int16.Parse(dtLeases.Rows[i]["Month"].ToString());
                        }
                    }
                    else
                    {
                        DashboardGraphReport_Single.MonthName = monthNames[k];
                        DashboardGraphReport_Single.Month = k + 1;
                        DashboardGraphReport_Single.Count = 0;

                        DashboardGraphReport_List.Add(DashboardGraphReport_Single);
                        DashboardGraphReport_Single = new DashboardGraphReport();
                        k = k + 1;
                        i = k - 1;
                    }
                }

            }

            return DashboardGraphReport_List;


        }
        public bool ReceivingSensorData(decimal asset_id)
        {
            string connect = System.Configuration.ConfigurationManager.ConnectionStrings["simplicityCon"].ConnectionString;
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = @"SELECT asset_id
                                FROM [dbo].[asset_attribute_data] where asset_id=" + asset_id;
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);
            con.Close();
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        #endregion
    }
}