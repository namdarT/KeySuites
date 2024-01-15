using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
//using System.Data.Entity;
using Vidly.Models;
using ElasticsearchCRUD.Model.GeoModel;
using System.Text.RegularExpressions;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

using System.Text;
using System.Runtime.InteropServices;
//using GroupDocs.Conversion.Options.Convert;

//using DocuSign.eSign;
//using DocuSign.eSign.Api;
//using DocuSign.eSign.Client;
//using DocuSign.eSign.Model;

using DocumentFormat.OpenXml;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;

using System.IO;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
namespace Vidly.Controllers
{
    public class SetupController : Controller
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
        /// ///////Referel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 

        public ActionResult MapTest()
        {
            return View();
        }
        public string OpenModelPopup()
        {
            //can send some data also.  
            return "<h1>This is Modal Popup Window</h1>";
        }

        public ActionResult ReferelSources(decimal id = 0)
        {

            //string connect = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
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
            /////List<ReferalSource> referalSources = new List<ReferalSource>();
            /////return View(referalSources);
            return View(BindDataReferalAll(id));
        }

        public ActionResult AddReferel(Int64 Id = 0)
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
            SqlCommand cmd = new SqlCommand("ReferalSourcesSelect", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ReferalSourceId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            ReferalSource ReferalSource_Single = new ReferalSource();
            ReferalSource ReferalSource_Detail = new ReferalSource();
            JsonResult jR = new JsonResult();
            List<ReferalSource> ReferalSource_List = new List<ReferalSource>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dt.Rows.Count > 0)
            {


                ReferalSource_Single.Address = dt.Rows[0]["Address"].ToString();

                ReferalSource_Single.ReferalSourceId = Int64.Parse(dt.Rows[0]["ReferalSourceId"].ToString());

                ReferalSource_Single.CompanyName = dt.Rows[0]["CompanyName"].ToString();
                ReferalSource_Single.ReferalType = dt.Rows[0]["ReferalType"].ToString();
                ReferalSource_Single.Number = decimal.Parse(dt.Rows[0]["Number"].ToString());
                ReferalSource_Single.CostPerDay = decimal.Parse(dt.Rows[0]["CostPerDay"].ToString());

                ReferalSource_Single.Address = dt.Rows[0]["Address"].ToString();
                ReferalSource_Single.Address2 = dt.Rows[0]["Address2"].ToString();
                ReferalSource_Single.City = dt.Rows[0]["City"].ToString();
                ReferalSource_Single.State = dt.Rows[0]["State"].ToString();
                //ReferalSource_Single.Zip = Int64.Parse(dt.Rows[0]["zip"].ToString());

                if (dt.Rows[0]["Zip"] == null || dt.Rows[0]["Zip"].ToString() == "" || Int64.Parse(dt.Rows[0]["Zip"].ToString()) == 0)
                {
                    ReferalSource_Single.Zip = null;
                }
                else
                {
                    ReferalSource_Single.Zip = Int64.Parse(dt.Rows[0]["Zip"].ToString());
                }

                ViewBag.ReferalType = DropDownListReferelType(dt.Rows[0]["ReferalType"].ToString());
                ViewBag.States = BindDataStatesAll(0);
            }
            else
            {
                ReferalSource_Single = new ReferalSource() { Zip = 75881, State = "TX", City = "Houston", Address2 = "", CostPerDay = 0, ReferalSourceId = 0, ReferalType = "", CompanyName = "", Address = "", Number = 0 };
                ViewBag.ReferalType = DropDownListReferelType("");
                ViewBag.States = BindDataStatesAll(0);
            }



            return View(ReferalSource_Single);
        }

        [HttpPost]
        public ActionResult AddReferel(FormCollection model)
        {
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["error"] = null;
            ReferalSource ReferalSource_Save = new ReferalSource();

            ReferalSource_Save.ReferalSourceId = Int64.Parse(model["ReferalSourceId"]);

            ReferalSource_Save.ReferalType = model["ReferalType"].ToString();


            ReferalSource_Save.Address = model["Address"];
            if (ReferalSource_Save.Address == null)
                ReferalSource_Save.Address = "";

            ReferalSource_Save.CompanyName = model["CompanyName"].ToString();
            if (ReferalSource_Save.CompanyName == null)
                ReferalSource_Save.CompanyName = "";

            ReferalSource_Save.Number = decimal.Parse(model["Number"].ToString());
            if (ReferalSource_Save.Number == null)
                ReferalSource_Save.Number = 0;

            ReferalSource_Save.CostPerDay = decimal.Parse(model["CostPerDay"].ToString());


            ReferalSource_Save.Address = model["Address"].ToString();
            ReferalSource_Save.Address2 = model["Address2"].ToString();
            ReferalSource_Save.City = model["City"].ToString();
            ReferalSource_Save.State = model["State"].ToString();
            //ReferalSource_Save.Zip = Int64.Parse(model["zip"].ToString());
            if (model["Zip"] == null || model["Zip"].ToString() == "")
            {
                ReferalSource_Save.Zip = 0;
            }
            else
            {
                ReferalSource_Save.Zip = Int64.Parse(model["Zip"].ToString());
            }

            //if (Session["LoginUserRole"] != null)
            //{
            //    //vwAppUserAsset_Save.last_update_user_id = Session["LoginUserEmail"].ToString();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                if (ReferalSource_Save.ReferalSourceId == 0)
                {


                    if (!DuplicateReferel(ReferalSource_Save.CompanyName))
                    {
                        cmd.CommandText = "ReferalSourcesInsert";
                        cmd.CommandType = CommandType.StoredProcedure;


                        cmd.Parameters.AddWithValue("@CompanyName", ReferalSource_Save.CompanyName);
                        cmd.Parameters.AddWithValue("@ReferalType", ReferalSource_Save.ReferalType);
                        cmd.Parameters.AddWithValue("@Address", ReferalSource_Save.Address);
                        cmd.Parameters.AddWithValue("@Number", ReferalSource_Save.Number);
                        cmd.Parameters.AddWithValue("@CostPerDay", ReferalSource_Save.CostPerDay);


                        cmd.Parameters.AddWithValue("@Address2", ReferalSource_Save.Address2);
                        cmd.Parameters.AddWithValue("@City", ReferalSource_Save.City);
                        cmd.Parameters.AddWithValue("@State", ReferalSource_Save.State);
                        cmd.Parameters.AddWithValue("@Zip", ReferalSource_Save.Zip);


                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            Session["error"] = null;
                            Session["SuccessMessage"] = "Success: Referel Successfully Added";
                        }
                        catch (SqlException e)
                        {

                            ViewBag.error = "Transaction Failure";
                            Session["error"] = ViewBag.error;
                            Session["Message"] = e.Message;
                        }

                    }
                    else
                    {
                        ViewBag.Message = "Company Name is already exists!";
                        Session["Message"] = ViewBag.Message;
                        Session["error"] = ViewBag.Message;
                    }




                }
                else
                {

                    cmd.CommandText = "ReferalSourcesUpdate";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ReferalSourceId", ReferalSource_Save.ReferalSourceId);
                    cmd.Parameters.AddWithValue("@CompanyName", ReferalSource_Save.CompanyName);
                    cmd.Parameters.AddWithValue("@ReferalType", ReferalSource_Save.ReferalType);
                    cmd.Parameters.AddWithValue("@Address", ReferalSource_Save.Address);
                    cmd.Parameters.AddWithValue("@Number", ReferalSource_Save.Number);
                    cmd.Parameters.AddWithValue("@CostPerDay", ReferalSource_Save.CostPerDay);


                    cmd.Parameters.AddWithValue("@Address2", ReferalSource_Save.Address2);
                    cmd.Parameters.AddWithValue("@City", ReferalSource_Save.City);
                    cmd.Parameters.AddWithValue("@State", ReferalSource_Save.State);
                    cmd.Parameters.AddWithValue("@Zip", ReferalSource_Save.Zip);


                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        Session["SuccessMessage"] = "Success: Referel Successfully Updated";
                    }
                    catch (SqlException e)
                    {

                        ViewBag.error = "Transaction Failure";
                        Session["Message"] = e.Message;
                    }

                }

            }
            con.Close();

            if (Session["error"] != null)
            {
                ViewData["Message"] = Session["error"];
                ViewBag.ReferalType = DropDownListReferelType("");
                return View(ReferalSource_Save);
            }
            else
            {
                ViewData["SuccessMessage"] = Session["SuccessMessage"];
                return RedirectToAction("ReferelSources");
            }




        }

        public ActionResult DeleteReferel(decimal id = 0)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"ReferalSourcesDelete";

                cmd.Parameters.AddWithValue("@ReferalSourceId", id);

                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Referel Deleted Completely Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("ReferelSources");
        }

        public ActionResult ActiveInactiveReferal(decimal id = 0, int IsActive = 1)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"ReferalSourcesEnableDisable";

                cmd.Parameters.AddWithValue("@Id", id);
                if (IsActive == 1)
                {
                    cmd.Parameters.AddWithValue("@IsActive", true);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsActive", false);
                }
                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Referal Source Updated Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("ReferelSources");
        }


        /// <summary>
        /// ///////////Vendor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public ActionResult Vendors(decimal Id = 0)
        //{

        //    //string connect = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
        //    //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
        //    if (Session["LoginUserRole"] != null)
        //    {

        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }

        //    if (Session["SuccessMessage"] != null)
        //    {
        //        if (Session["SuccessMessage"].ToString() != "")
        //        {
        //            ViewData["SuccessMessage"] = Session["SuccessMessage"].ToString();
        //            Session["SuccessMessage"] = null;
        //        }
        //        else
        //        {
        //            Session["SuccessMessage"] = null;
        //        }

        //    }
        //    else
        //    {
        //        ViewData["SuccessMessage"] = "";
        //    }

        //    //Add View
        //    if (ViewData["error"] == null || ViewData["error"].ToString() == "")
        //    {
        //        if (Session["error"] != null)
        //        {

        //            ViewData["error"] = Session["error"].ToString();
        //        }
        //        else
        //        {

        //            ViewData["error"] = "";
        //        }
        //    }
        //    ViewBag.VendorType = DropDownListVendorType("All");

        //    return View(BindDataVendorAll("All"));
        //}

        //[HttpPost]
        public ActionResult Vendors(Int64 Id = 0)
        {


            //string connect = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
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
            List<SelectListItem> vendorList = new List<SelectListItem>();
            if (Id == 0)
            {
                ViewBag.VendorType = DropDownListVendorType_Change("All", "0");
                vendorList = DropDownListVendorType_Change("All", "0");
            }
            else
            {
                ViewBag.VendorType = DropDownListVendorType_Change("", Id.ToString());
                vendorList = DropDownListVendorType_Change("", Id.ToString());
            }

            return View(BindDataVendorAll(vendorList[0].Text));
        }

        public ActionResult VendorsChange(Int64 Id = 0)
        {

            //string connect = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
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
            if (Id == 1)
            {
                ViewBag.VendorType = DropDownListVendorType("All");
            }
            else
            {
                ViewBag.VendorType = DropDownListVendorType("");
            }

            return View(BindDataVendorAll(""));
        }

        public ActionResult AddVendor(Int64 Id = 0)
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
            SqlCommand cmd = new SqlCommand("VendorsSelect", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@VendorId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Vendor Vendor_Single = new Vendor();
            Vendor Vendor_Detail = new Vendor();
            JsonResult jR = new JsonResult();
            List<Vendor> Vendor_List = new List<Vendor>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {


                Vendor_Single.Address = dt.Rows[0]["Address"].ToString();
                Vendor_Single.Address2 = dt.Rows[0]["Address2"].ToString();
                Vendor_Single.Email = dt.Rows[0]["Email"].ToString();
                Vendor_Single.VendorId = Int64.Parse(dt.Rows[0]["VendorId"].ToString());
                Vendor_Single.Website = dt.Rows[0]["Website"].ToString();
                Vendor_Single.Notes = dt.Rows[0]["Notes"].ToString();
                Vendor_Single.CompanyName = dt.Rows[0]["CompanyName"].ToString();
                Vendor_Single.VendorType = dt.Rows[0]["VendorType"].ToString();
                Vendor_Single.PhoneNumber = dt.Rows[0]["PhoneNumber"].ToString();

                Vendor_Single.DollarAmount = decimal.Parse(dt.Rows[0]["DollarAmount"].ToString());
                Vendor_Single.PercentageAmount = decimal.Parse(dt.Rows[0]["PercentageAmount"].ToString());

                Vendor_Single.Zip = dt.Rows[0]["Zip"].ToString();
                Vendor_Single.State = dt.Rows[0]["State"].ToString();
                Vendor_Single.City = dt.Rows[0]["City"].ToString();
                Vendor_Single.Street = dt.Rows[0]["Street"].ToString();



                ViewBag.VendorType = DropDownListVendorType(dt.Rows[0]["VendorType"].ToString());
                ViewBag.VendContactList = BindDataVendContactAll(Int64.Parse(dt.Rows[0]["VendorId"].ToString()));
                ViewBag.States = BindDataStatesAll(0);
                ViewBag.VendorTypeGroup = CheckBoxListVendorType(Vendor_Single.Street);
                //    var model = new HomeModel
                //    {
                //        AvailableVendorTypes = CheckBoxListVendorType(Vendor_Single.Street)
                //};
            }
            else
            {
                Vendor_Single = new Vendor() { City = "Houston", State = "TX", Street = "", Zip = "75581", VendorId = 0, VendorType = "", CompanyName = "", Website = "", Email = "", PhoneNumber = "", Notes = "", Address = "", DollarAmount = 0, PercentageAmount = 0 };
                ViewBag.VendorType = DropDownListVendorType("");
                ViewBag.VendContactList = BindDataVendContactAll(0);
                ViewBag.States = BindDataStatesAll(0);
                ViewBag.VendorTypeGroup = CheckBoxListVendorType(Vendor_Single.Street);
            }



            return View(Vendor_Single);
        }

        [HttpPost]
        public ActionResult AddVendor(FormCollection model)
        {
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["error"] = null;


            Vendor Vendor_Save = new Vendor();

            Vendor_Save.VendorId = Int64.Parse(model["VendorId"]);
            Vendor_Save.CompanyName = model["CompanyName"].ToString();
            Vendor_Save.VendorType = model["VendorType"].ToString();

            Vendor_Save.Notes = model["Notes"];
            if (Vendor_Save.Notes == null)
                Vendor_Save.Notes = "";

            Vendor_Save.PhoneNumber = model["PhoneNumber"];

            Vendor_Save.Website = model["Website"];
            if (Vendor_Save.Website == null)
                Vendor_Save.Website = "";

            Vendor_Save.Email = model["Email"];
            if (Vendor_Save.Email == null)
                Vendor_Save.Email = "";

            Vendor_Save.Address = model["Address"];
            if (Vendor_Save.Address == null)
                Vendor_Save.Address = "";

            Vendor_Save.DollarAmount = Int64.Parse(model["DollarAmount"].ToString());
            if (model["DollarAmount"] == null)
                Vendor_Save.DollarAmount = 0;

            Vendor_Save.PercentageAmount = decimal.Parse(model["PercentageAmount"].ToString());
            if (model["PercentageAmount"] == null)
                Vendor_Save.PercentageAmount = 0;

            Vendor_Save.Address2 = model["Address2"];
            Vendor_Save.Zip = model["Zip"].ToString();
            Vendor_Save.State = model["State"].ToString();
            Vendor_Save.City = model["City"].ToString();
            Vendor_Save.Street = string.Join(",", model["SelectedTypes"]);


            List<VendContact> vendContactList = new List<VendContact>();
            VendContact vendContact = new VendContact();
            string[] keys = model.AllKeys;
            for (int i = 0; i <= 50; i++)
            {
                if (keys.Contains<string>("VendContactId" + i))
                {
                    vendContact.VendContactId = Int64.Parse(model["VendContactId" + i].ToString());
                    if (Int64.Parse(model["VendorId" + i].ToString()) > 0)
                    {
                        vendContact.VendorId = Int64.Parse(model["VendorId" + i].ToString());
                    }
                    else
                    {
                        vendContact.VendorId = Int64.Parse(model["VendorId" + i].ToString());
                    }

                    vendContact.VendContactEmail = model["VendContactEmail" + i].ToString();
                    vendContact.VendContactFirstName = model["VendContactFirstName" + i].ToString();
                    vendContact.VendContactLastName = model["VendContactLastName" + i].ToString();
                    vendContact.VendContactNumber = model["VendContactNumber" + i].ToString();
                    vendContactList.Add(vendContact);
                    vendContact = new VendContact();
                }

            }

            //if (Session["LoginUserRole"] != null)
            //{
            //    //vwAppUserAsset_Save.last_update_user_id = Session["LoginUserEmail"].ToString();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                if (Vendor_Save.VendorId == 0)
                {
                    //if (WebSiteValidator(Vendor_Save.Website))
                    //{
                    if (USNumberValidator(Vendor_Save.PhoneNumber))
                    {
                        //if (EmailValidator(Vendor_Save.Email))
                        //{
                        if (!DuplicateAsset(Vendor_Save.CompanyName, Vendor_Save.VendorType))
                        {
                            cmd.CommandText = "VendorsInsert";
                            cmd.CommandType = CommandType.StoredProcedure;


                            cmd.Parameters.AddWithValue("@CompanyName", Vendor_Save.CompanyName);
                            cmd.Parameters.AddWithValue("@VendorType", Vendor_Save.VendorType);
                            cmd.Parameters.AddWithValue("@Address", Vendor_Save.Address);
                            cmd.Parameters.AddWithValue("@PhoneNumber", Vendor_Save.PhoneNumber);
                            cmd.Parameters.AddWithValue("@DollarAmount", Vendor_Save.DollarAmount);
                            cmd.Parameters.AddWithValue("@PercentageAmount", Vendor_Save.PercentageAmount);
                            cmd.Parameters.AddWithValue("@Website", Vendor_Save.Website);

                            cmd.Parameters.AddWithValue("@Address2", Vendor_Save.Address2);
                            cmd.Parameters.AddWithValue("@Street", Vendor_Save.Street);
                            cmd.Parameters.AddWithValue("@Zip", Vendor_Save.Zip);
                            cmd.Parameters.AddWithValue("@State", Vendor_Save.State);
                            cmd.Parameters.AddWithValue("@City", Vendor_Save.City);

                            cmd.Parameters.AddWithValue("@Email", Vendor_Save.Email);
                            cmd.Parameters.AddWithValue("@Notes", Vendor_Save.Notes);
                            con.Open();
                        }
                        else
                        {
                            ViewBag.Message = "Company Name with same Vendor Type is already exists!";
                            Session["Message"] = ViewBag.Message;
                            Session["error"] = ViewBag.Message;
                        }

                        //}
                        //else
                        //{
                        //    ViewBag.Message = "Email is Incorrect!";
                        //    Session["Message"] = ViewBag.Message;
                        //    Session["error"] = ViewBag.Message;
                        //}
                    }
                    else
                    {
                        ViewBag.Message = "Phone Number is not in a correct Format! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                        Session["Message"] = ViewBag.Message;
                        Session["error"] = ViewBag.Message;
                    }
                    //}
                    //else
                    //{
                    //    ViewBag.Message = "WebSite is Incorrect!";
                    //    Session["Message"] = ViewBag.Message;
                    //    Session["error"] = ViewBag.Message;
                    //}
                    try
                    {
                        //con.Open();
                        //cmd.ExecuteNonQuery();
                        //Session["error"] = null;
                        //Session["SuccessMessage"] = "Success: Vendor Successfully Added";

                        if (vendContactList.Count > 0)
                        {
                            foreach (var contact_Save in vendContactList)
                            {

                                if (USNumberValidator(contact_Save.VendContactNumber))
                                {
                                    if (EmailValidator(contact_Save.VendContactEmail))
                                    {
                                        if (!DuplicateCommon("VendContact", "VendContactId", "VendContactFirstName", "VendorId", contact_Save.VendContactFirstName, contact_Save.VendorId.ToString()))
                                        {
                                            //SavePropContact(contact_Save);
                                        }
                                        else
                                        {
                                            ViewBag.Message = "Duplicate Contact Name of this Vendor is already exists!";
                                            Session["Message"] = ViewBag.Message;
                                            Session["error"] = ViewBag.Message;
                                        }

                                    }
                                    else
                                    {
                                        ViewBag.Message = "Email is Incorrect!";
                                        Session["Message"] = ViewBag.Message;
                                        Session["error"] = ViewBag.Message;
                                    }

                                }
                                else
                                {
                                    ViewBag.Message = "Phone Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                                    Session["Message"] = ViewBag.Message;
                                    Session["error"] = ViewBag.Message;
                                }
                            }
                        }
                        if (Session["error"] != null)
                        {
                            //ViewBag.VendContactList = vendContactList;
                        }
                        else
                        {
                            object id = cmd.ExecuteScalar();
                            //transaction.Commit();
                            Int64 VendorId = Int64.Parse(id.ToString());
                            SaveVendContact(vendContactList, VendorId);
                            Session["error"] = null;
                            Session["SuccessMessage"] = "Success: Vendor Successfully Added";
                        }



                    }
                    catch (SqlException e)
                    {

                        ViewBag.error = "Transaction Failure";
                        Session["error"] = ViewBag.error;
                        Session["Message"] = e.Message;
                    }


                }
                else
                {
                    //if (WebSiteValidator(Vendor_Save.Website))
                    //{
                    if (USNumberValidator(Vendor_Save.PhoneNumber))
                    {
                        //if (EmailValidator(Vendor_Save.Email))
                        //{
                        cmd.CommandText = "VendorsUpdate";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@VendorId", Vendor_Save.VendorId);
                        cmd.Parameters.AddWithValue("@CompanyName", Vendor_Save.CompanyName);
                        cmd.Parameters.AddWithValue("@VendorType", Vendor_Save.VendorType);
                        cmd.Parameters.AddWithValue("@Address", Vendor_Save.Address);
                        cmd.Parameters.AddWithValue("@Address2", Vendor_Save.Address2);
                        cmd.Parameters.AddWithValue("@PhoneNumber", Vendor_Save.PhoneNumber);
                        cmd.Parameters.AddWithValue("@DollarAmount", Vendor_Save.DollarAmount);
                        cmd.Parameters.AddWithValue("@PercentageAmount", Vendor_Save.PercentageAmount);
                        cmd.Parameters.AddWithValue("@Website", Vendor_Save.Website);
                        cmd.Parameters.AddWithValue("@Street", Vendor_Save.Street);
                        cmd.Parameters.AddWithValue("@Zip", Vendor_Save.Zip);
                        cmd.Parameters.AddWithValue("@State", Vendor_Save.State);
                        cmd.Parameters.AddWithValue("@City", Vendor_Save.City);

                        cmd.Parameters.AddWithValue("@Email", Vendor_Save.Email);
                        cmd.Parameters.AddWithValue("@Notes", Vendor_Save.Notes);
                        con.Open();
                        //}
                        //else
                        //{
                        //    ViewBag.Message = "Email is Incorrect!";
                        //    Session["Message"] = ViewBag.Message;
                        //    Session["error"] = ViewBag.Message;
                        //}
                    }
                    else
                    {
                        ViewBag.Message = "Phone Number is not in a correct Format! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                        Session["Message"] = ViewBag.Message;
                        Session["error"] = ViewBag.Message;
                    }
                    //}
                    //else
                    //{
                    //    ViewBag.Message = "WebSite is Incorrect!";
                    //    Session["Message"] = ViewBag.Message;
                    //    Session["error"] = ViewBag.Message;
                    //}
                    try
                    {
                        //con.Open();
                        //cmd.ExecuteNonQuery();
                        //Session["SuccessMessage"] = "Success: Vendor Successfully Updated";

                        if (vendContactList.Count > 0)
                        {
                            foreach (var contact_Save in vendContactList)
                            {
                                if (USNumberValidator(contact_Save.VendContactNumber))
                                {
                                    if (EmailValidator(contact_Save.VendContactEmail))
                                    {
                                        if (!DuplicateCommon("VendContact", "VendContactId", "VendContactFirstName", "VendorId", contact_Save.VendContactFirstName, contact_Save.VendorId.ToString()))
                                        {
                                            //SavePropContact(contact_Save);  
                                        }
                                        else
                                        {
                                            ViewBag.Message = "Duplicate Contact Name of This Vendor is already exists!";
                                            Session["Message"] = ViewBag.Message;
                                            Session["error"] = ViewBag.Message;
                                        }

                                    }
                                    else
                                    {
                                        ViewBag.Message = "Email is Incorrect!";
                                        Session["Message"] = ViewBag.Message;
                                        Session["error"] = ViewBag.Message;
                                    }

                                }
                                else
                                {
                                    ViewBag.Message = "Phone Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                                    Session["Message"] = ViewBag.Message;
                                    Session["error"] = ViewBag.Message;
                                }
                            }
                        }
                        if (Session["error"] != null)
                        {

                        }
                        else
                        {
                            cmd.ExecuteNonQuery();
                            //transaction.Commit();
                            Int64 VendorId = Vendor_Save.VendorId;
                            SaveVendContact(vendContactList, VendorId);
                            Session["SuccessMessage"] = "Success: Vendor Successfully Updated";
                        }

                    }
                    catch (SqlException e)
                    {

                        ViewBag.error = "Transaction Failure";
                        Session["Message"] = e.Message;
                    }
                }

            }
            con.Close();

            if (Session["error"] != null)
            {
                ViewData["Message"] = Session["error"];
                ViewBag.VendorType = DropDownListVendorType("");
                ViewBag.VendorTypeGroup = CheckBoxListVendorType(model["SelectedTypes"]);
                ViewBag.States = BindDataStatesAll(0);
                if (Int64.Parse(model["VendorId"].ToString()) == 0)
                {
                    ViewBag.VendContactList = vendContactList;
                }
                else
                {
                    if (vendContactList.Count > 0)
                    {
                        List<VendContact> vendContacts = BindDataVendContactAll(Int64.Parse(model["VendorId"].ToString()));
                        foreach (var contact in vendContactList)
                        {
                            vendContacts.Add(contact);
                        }

                        ViewBag.VendContactList = vendContacts;
                    }
                    else
                    {
                        ViewBag.VendContactList = BindDataVendContactAll(Int64.Parse(model["VendorId"].ToString()));
                    }

                }
                //ViewBag.VendContactList = BindDataVendContactAll(Int64.Parse(model["VendorId"].ToString()));
                return View(Vendor_Save);
            }
            else
            {
                ViewData["SuccessMessage"] = Session["SuccessMessage"];
                return RedirectToAction("Vendors");
            }




        }

        public ActionResult DeleteVendor(decimal id = 0)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"VendorsDelete";

                cmd.Parameters.AddWithValue("@VendorId", id);

                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Vendor Deleted Completely Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Vendors");
        }
        /// <summary>
        /// ///Leads
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult ActiveInactiveVendor(decimal id = 0, int IsActive = 1)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"VendorsEnableDisable";

                cmd.Parameters.AddWithValue("@Id", id);
                if (IsActive == 1)
                {
                    cmd.Parameters.AddWithValue("@IsActive", true);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsActive", false);
                }
                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Vendor Updated Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Vendors");
        }


        public ActionResult Leads(decimal id = 0, string srchContactName = "", string srchOccupantName = "")
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
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
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

            return View(BindDataLeadAll(id, srchContactName, srchOccupantName));
        }

        public ActionResult AddLead(FormCollection a, Int64 Id = 0)
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
            SqlCommand cmd = new SqlCommand("LeadsSelect", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@LeadsId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Lead Lead_Single = new Lead();
            Lead Lead_Detail = new Lead();
            JsonResult jR = new JsonResult();
            List<Lead> Lead_List = new List<Lead>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dt.Rows.Count > 0)
            {


                //Lead_Single.Address = dt.Rows[0]["Address"].ToString();
                Lead_Single.ContactEmail = dt.Rows[0]["ContactEmail"].ToString();
                Lead_Single.LeadsId = Int64.Parse(dt.Rows[0]["LeadsId"].ToString());
                Lead_Single.Breed = dt.Rows[0]["Breed"].ToString();
                Lead_Single.City = dt.Rows[0]["City"].ToString();
                Lead_Single.ContactNumber = dt.Rows[0]["ContactNumber"].ToString();
                Lead_Single.ContactType = dt.Rows[0]["ContactType"].ToString();
                Lead_Single.FloorPreference = dt.Rows[0]["FloorPreference"].ToString();
                Lead_Single.LeadsName = dt.Rows[0]["LeadsName"].ToString();
                Lead_Single.LeaseTerm = dt.Rows[0]["LeaseTerm"].ToString();
                Lead_Single.MoveInDate = DateTime.Parse(dt.Rows[0]["MoveInDate"].ToString());
                Lead_Single.NoOfAdults = dt.Rows[0]["NoOfAdults"].ToString();
                Lead_Single.NoOfBedRooms = dt.Rows[0]["NoOfBedRooms"].ToString();
                Lead_Single.NoOfChildren = dt.Rows[0]["NoOfChildren"].ToString();
                Lead_Single.NoOfPets = dt.Rows[0]["NoOfPets"].ToString();
                Lead_Single.Notes = dt.Rows[0]["Notes"].ToString();
                Lead_Single.OcupantName = dt.Rows[0]["OcupantName"].ToString();
                Lead_Single.Address = dt.Rows[0]["Address"].ToString();
                Lead_Single.Address2 = dt.Rows[0]["Address2"].ToString();
                Lead_Single.PreferedArea = dt.Rows[0]["PreferedArea"].ToString();
                Lead_Single.PreferedAddress = dt.Rows[0]["PreferedAddress"].ToString();
                Lead_Single.Elevator = dt.Rows[0]["Elevator"].ToString();
                if (dt.Rows[0]["Zip"] == null || dt.Rows[0]["Zip"].ToString() == "" || Int64.Parse(dt.Rows[0]["Zip"].ToString()) == 0)
                {
                    Lead_Single.Zip = null;
                }
                else
                {
                    Lead_Single.Zip = Int64.Parse(dt.Rows[0]["Zip"].ToString());
                }
                //Lead_Single.Zip = decimal.Parse(dt.Rows[0]["Zip"].ToString());
                Lead_Single.State = dt.Rows[0]["State"].ToString();
                //For Company Fill

                List<Company> company_List = new List<Company>();
                Company company_One = new Company();
                company_List = BindDataCompanyAll(0);
                company_List = company_List.Where(x => x.CompanyName == dt.Rows[0]["LeadsName"].ToString()).ToList();
                if (company_List.Count() > 0)
                {
                    company_One = company_List[0];
                    Lead_Single.CompanyContactNumber = company_One.CompanyContact;
                    //For Contact Fill
                    List<Contact> Contact_List = new List<Contact>();
                    Contact_List = BindDataContactAll(0);
                    Contact Contact_One = new Contact();
                    Contact_List = Contact_List.Where(x => x.Company == dt.Rows[0]["LeadsName"].ToString()).ToList();
                    if (Contact_List.Count() > 0)
                    {
                        Contact_One = Contact_List[0];
                        Lead_Single.ContactNumberCompany = Contact_One.Phone;
                        Lead_Single.ContactName = Contact_One.FirstName + " " + Contact_One.LastName;
                    }
                    else
                    {
                        Lead_Single.ContactName = "";
                    }

                }
                //Lead_Single.ContactName = dt.Rows[0]["ContactName"].ToString();

                Lead_Single.ReferelSource = dt.Rows[0]["ReferelSource"].ToString();
                Lead_Single.State = dt.Rows[0]["State"].ToString();
                Lead_Single.Weight = decimal.Parse(dt.Rows[0]["Weight"].ToString());
                //Lead_Single.Zip = decimal.Parse(dt.Rows[0]["Zip"].ToString());

                //Added By Shahab
                Lead_Single.OccupantCity = dt.Rows[0]["OccupantCity"].ToString();
                Lead_Single.OccupantState = dt.Rows[0]["OccupantState"].ToString();
                Lead_Single.OccupantZip = decimal.Parse(dt.Rows[0]["OccupantZip"].ToString());
                if (dt.Rows[0]["OccupantZip"] == null || dt.Rows[0]["OccupantZip"].ToString() == "" || Int64.Parse(dt.Rows[0]["OccupantZip"].ToString()) == 0)
                {
                    Lead_Single.OccupantZip = null;
                }
                else
                {
                    Lead_Single.OccupantZip = Int64.Parse(dt.Rows[0]["OccupantZip"].ToString());
                }


                Lead_Single.CompanyLogo = dt.Rows[0]["CompanyLogo"].ToString();

                ViewBag.ContactType = DropDownListContantType(dt.Rows[0]["ContactType"].ToString());
                ViewBag.LeaseTerm = DropDownListLeaseTerm(dt.Rows[0]["LeaseTerm"].ToString());
                ViewBag.City = DropDownListCity(dt.Rows[0]["City"].ToString());
                ViewBag.ReferelSource = BindDataReferalAll(0).Where(y => y.IsActive == true);//DropDownListReferelSource(dt.Rows[0]["ReferelSource"].ToString());
                ViewBag.NoOfAdults = DropDownListNumber(dt.Rows[0]["NoOfAdults"].ToString());
                ViewBag.NoOfBedRooms = DropDownListBed(dt.Rows[0]["NoOfBedRooms"].ToString());
                ViewBag.NoOfChildren = DropDownListNumber(dt.Rows[0]["NoOfChildren"].ToString());
                ViewBag.NoOfPets = DropDownListNumber(dt.Rows[0]["NoOfPets"].ToString());
                ViewBag.Weight = DropDownListWeight(dt.Rows[0]["Weight"].ToString());
                ViewBag.Company = BindDataCompanyAll(0).Where(y => y.IsActive == true);
                ViewBag.Contact = BindDataContactAll(0).Where(y => y.IsActive == true);
                ViewBag.Elevator = DropDownListYesNo(dt.Rows[0]["Elevator"].ToString());
                //ViewBag.ContactName = DropDownListContactName(dt.Rows[0]["ContactName"].ToString(), dt.Rows[0]["LeadsName"].ToString());
                ViewBag.States = BindDataStatesAll(0);
            }
            else
            {
                Lead_Single = new Lead() { Elevator = "", PreferedAddress = "", IsActive = true, CompanyContactNumber = "", ContactName = "", ContactNumberCompany = "", LeadsId = 0, ContactEmail = "", Breed = "", City = "", ContactNumber = "", ContactType = "", FloorPreference = "", LeadsName = "", LeaseTerm = "30", Address = "", Address2 = "", OccupantCity = "Houston", OccupantState = "TX", OccupantZip = 75881,
                    ContactInfoId = 0, MoveInDate = DateTime.Now, NoOfAdults = "none", NoOfBedRooms = "Studio", NoOfChildren = "none", NoOfPets = "0", Notes = "", OcupantName = "", PreferedArea = "", ReferelSource = "No Referral", State = "", Weight = 0, Zip = 78851, CompanyLogo = "" };
                ViewBag.ContactType = DropDownListContantType("");
                ViewBag.LeaseTerm = DropDownListLeaseTerm("");
                ViewBag.City = DropDownListCity("");
                List<ReferalSource> referalSources = new List<ReferalSource>();
                referalSources = BindDataReferalAll(0).Where(y => y.IsActive == true).ToList();
                ViewBag.ReferelSource = referalSources;
                List<Company> companies = new List<Company>();
                companies = BindDataCompanyAll(0).Where(y => y.IsActive == true).ToList();
                ViewBag.Company = companies;
                List<Contact> contacts = new List<Contact>();
                contacts = BindDataContactAll(0).Where(y => y.IsActive == true).ToList();
                ViewBag.Contact = contacts;
                ViewBag.NoOfAdults = DropDownListNumber("");
                ViewBag.NoOfBedRooms = DropDownListBed("");
                ViewBag.NoOfChildren = DropDownListNumber("");
                ViewBag.NoOfPets = DropDownListNumber("");
                ViewBag.Weight = DropDownListWeight("");
                ViewBag.Elevator = DropDownListYesNo("");
                //ViewBag.ContactName = DropDownListContactName(dt.Rows[0]["ContactName"].ToString(), dt.Rows[0]["LeadsName"].ToString());
                ViewBag.States = BindDataStatesAll(0);
            }



            return View(Lead_Single);
        }

        [HttpPost]
        public ActionResult AddLead(FormCollection model)
        {
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["error"] = null;
            Lead Lead_Save = new Lead();

            Lead_Save.LeadsId = Int64.Parse(model["LeadsId"]);

            Lead_Save.LeadsName = model["LeadsName"].ToString();
            Lead_Save.ContactType = model["ContactType"].ToString();
            Lead_Save.LeaseTerm = model["LeaseTerm"].ToString();
            if (Lead_Save.ContactType == "Company" && Lead_Save.LeadsName == "")
            {
                Session["error"] = "Please Select Company!";
            }
            Lead_Save.FloorPreference = model["FloorPreference"].ToString();
            //Lead_Save.MoveInDate = DateTime.Parse(model["MoveInDate"].ToString());

            try
            {

                Lead_Save.MoveInDate = DateTime.Parse(model["MoveInDate"]);

            }
            catch (Exception ex)
            {

                string date = model["MoveInDate"].ToString();
                string mon = date.Substring(0, 2);
                string da = date.Substring(3, 2);
                string ye = date.Substring(6, 4);
                //date = da + "/" + mon + "/" + ye;
                System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
                Lead_Save.MoveInDate = DateTime.ParseExact(date, "MM/dd/yyyy", provider);

            }

            Lead_Save.NoOfAdults = model["NoOfAdults"].ToString();
            Lead_Save.NoOfBedRooms = model["NoOfBedRooms"].ToString();
            Lead_Save.NoOfChildren = model["NoOfChildren"].ToString();
            Lead_Save.NoOfPets = model["NoOfPets"].ToString();
            //Lead_Save.Zip = decimal.Parse(model["Zip"].ToString());

            if (model["Zip"] == null || model["Zip"].ToString() == "")
            {
                Lead_Save.Zip = 0;
            }
            else
            {
                Lead_Save.Zip = Int64.Parse(model["Zip"].ToString());
            }

            Lead_Save.CompanyLogo = "";
            Lead_Save.Weight = decimal.Parse(model["Weight"].ToString());
            //Lead_Save.State = model["State"].ToString();
            Lead_Save.Elevator = model["Elevator"].ToString();
            Lead_Save.PreferedArea = model["PreferedArea"].ToString();
            Lead_Save.OccupantCity = model["OccupantCity"].ToString();
            Lead_Save.OccupantState = model["OccupantState"].ToString();
            //Lead_Save.OccupantZip = decimal.Parse( model["OccupantZip"].ToString());
            if (model["OccupantZip"] == null || model["OccupantZip"].ToString() == "")
            {
                Lead_Save.OccupantZip = 0;
            }
            else
            {
                Lead_Save.OccupantZip = Int64.Parse(model["OccupantZip"].ToString());
            }

            Lead_Save.ReferelSource = model["ReferelSource"].ToString();


            Lead_Save.OcupantName = model["OcupantName"].ToString();
            //Lead_Save.Notes = model["Notes"].ToString();

            Lead_Save.ContactNumber = model["ContactNumber"];
            if (Lead_Save.ContactNumber == null)
                Lead_Save.ContactNumber = "";

            Lead_Save.ContactEmail = model["ContactEmail"];

            Lead_Save.City = model["City"];
            if (Lead_Save.City == null)
                Lead_Save.City = "";

            Lead_Save.Breed = model["Breed"];
            if (Lead_Save.Breed == null)
                Lead_Save.Breed = "";

            Lead_Save.Address = model["Address"];
            if (Lead_Save.Address == null)
                Lead_Save.Address = "";

            Lead_Save.Address2 = model["Address2"];

            Lead_Save.Notes = model["Notes"];
            Lead_Save.PreferedAddress = model["PreferedAddress"];
            Lead_Save.State = model["State"];
            //if (Session["LoginUserRole"] != null)
            //{
            //    //vwAppUserAsset_Save.last_update_user_id = Session["LoginUserEmail"].ToString();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            if (Session["error"] == null)
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.Text;
                    if (Lead_Save.LeadsId == 0)
                    {
                        if (Lead_Save.ContactType == "Company")
                        {
                            if (!string.IsNullOrEmpty(Lead_Save.ContactNumber))
                            { }
                            cmd.CommandText = "LeadsInsert";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Breed", Lead_Save.Breed);
                            cmd.Parameters.AddWithValue("@City", Lead_Save.City);
                            cmd.Parameters.AddWithValue("@Address", Lead_Save.Address);
                            cmd.Parameters.AddWithValue("@ContactEmail", Lead_Save.ContactEmail);
                            cmd.Parameters.AddWithValue("@Address2", Lead_Save.Address2);
                            cmd.Parameters.AddWithValue("@OccupantCity", Lead_Save.OccupantCity);
                            cmd.Parameters.AddWithValue("@OccupantState", Lead_Save.OccupantState);
                            cmd.Parameters.AddWithValue("@OccupantZip", Lead_Save.OccupantZip);

                            cmd.Parameters.AddWithValue("@ContactNumber", Lead_Save.ContactNumber);

                            cmd.Parameters.AddWithValue("@ContactType", Lead_Save.ContactType);
                            cmd.Parameters.AddWithValue("@FloorPreference", Lead_Save.FloorPreference);
                            cmd.Parameters.AddWithValue("@LeadsName", Lead_Save.LeadsName);
                            cmd.Parameters.AddWithValue("@LeaseTerm", Lead_Save.LeaseTerm);
                            cmd.Parameters.AddWithValue("@MoveInDate", Lead_Save.MoveInDate);
                            cmd.Parameters.AddWithValue("@NoOfAdults", Lead_Save.NoOfAdults);

                            cmd.Parameters.AddWithValue("@NoOfBedRooms", Lead_Save.NoOfBedRooms);

                            cmd.Parameters.AddWithValue("@NoOfChildren", Lead_Save.NoOfChildren);
                            cmd.Parameters.AddWithValue("@NoOfPets", Lead_Save.NoOfPets);
                            cmd.Parameters.AddWithValue("@Notes", Lead_Save.Notes);

                            cmd.Parameters.AddWithValue("@OcupantName", Lead_Save.OcupantName);
                            cmd.Parameters.AddWithValue("@PreferedArea", Lead_Save.PreferedArea);

                            cmd.Parameters.AddWithValue("@ReferelSource", Lead_Save.ReferelSource);

                            cmd.Parameters.AddWithValue("@Weight", Lead_Save.Weight);
                            cmd.Parameters.AddWithValue("@Zip", Lead_Save.Zip);
                            cmd.Parameters.AddWithValue("@PreferedAddress", Lead_Save.PreferedAddress);
                            cmd.Parameters.AddWithValue("@Elevator", Lead_Save.Elevator);
                            cmd.Parameters.AddWithValue("@State", Lead_Save.State);
                            cmd.Parameters.AddWithValue("@CompanyLogo", Lead_Save.CompanyLogo);

                            try
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                                Session["error"] = null;
                                Session["SuccessMessage"] = "Success: Lead Successfully Added";
                            }
                            catch (SqlException e)
                            {

                                ViewBag.error = "Transaction Failure";
                                Session["error"] = ViewBag.error;
                                Session["Message"] = e.Message;
                            }


                        }
                        else
                        {

                            if (USNumberValidatorWithEmptyOK(Lead_Save.ContactNumber))
                            {

                                if (EmailValidatorWithEmptyOk(Lead_Save.ContactEmail))
                                {

                                    cmd.CommandText = "LeadsInsert";
                                    cmd.CommandType = CommandType.StoredProcedure;

                                    cmd.Parameters.AddWithValue("@Breed", Lead_Save.Breed);
                                    cmd.Parameters.AddWithValue("@City", Lead_Save.City);
                                    cmd.Parameters.AddWithValue("@Address", Lead_Save.Address);
                                    cmd.Parameters.AddWithValue("@ContactEmail", Lead_Save.ContactEmail);
                                    cmd.Parameters.AddWithValue("@Address2", Lead_Save.Address2);
                                    cmd.Parameters.AddWithValue("@OccupantCity", Lead_Save.OccupantCity);
                                    cmd.Parameters.AddWithValue("@OccupantState", Lead_Save.OccupantState);
                                    cmd.Parameters.AddWithValue("@OccupantZip", Lead_Save.OccupantZip);
                                    cmd.Parameters.AddWithValue("@ContactNumber", Lead_Save.ContactNumber);



                                    cmd.Parameters.AddWithValue("@ContactType", Lead_Save.ContactType);
                                    cmd.Parameters.AddWithValue("@FloorPreference", Lead_Save.FloorPreference);
                                    cmd.Parameters.AddWithValue("@LeadsName", Lead_Save.LeadsName);
                                    cmd.Parameters.AddWithValue("@LeaseTerm", Lead_Save.LeaseTerm);
                                    cmd.Parameters.AddWithValue("@MoveInDate", Lead_Save.MoveInDate);
                                    cmd.Parameters.AddWithValue("@NoOfAdults", Lead_Save.NoOfAdults);

                                    cmd.Parameters.AddWithValue("@NoOfBedRooms", Lead_Save.NoOfBedRooms);

                                    cmd.Parameters.AddWithValue("@NoOfChildren", Lead_Save.NoOfChildren);
                                    cmd.Parameters.AddWithValue("@NoOfPets", Lead_Save.NoOfPets);
                                    cmd.Parameters.AddWithValue("@Notes", Lead_Save.Notes);

                                    cmd.Parameters.AddWithValue("@OcupantName", Lead_Save.OcupantName);
                                    cmd.Parameters.AddWithValue("@PreferedArea", Lead_Save.PreferedArea);

                                    cmd.Parameters.AddWithValue("@ReferelSource", Lead_Save.ReferelSource);

                                    cmd.Parameters.AddWithValue("@Weight", Lead_Save.Weight);
                                    cmd.Parameters.AddWithValue("@Zip", Lead_Save.Zip);
                                    cmd.Parameters.AddWithValue("@PreferedAddress", Lead_Save.PreferedAddress);
                                    cmd.Parameters.AddWithValue("@Elevator", Lead_Save.Elevator);
                                    cmd.Parameters.AddWithValue("@State", Lead_Save.State);
                                    cmd.Parameters.AddWithValue("@CompanyLogo", Lead_Save.CompanyLogo);

                                    try
                                    {
                                        con.Open();
                                        cmd.ExecuteNonQuery();
                                        Session["error"] = null;
                                        Session["SuccessMessage"] = "Success: Lead Successfully Added";
                                    }
                                    catch (SqlException e)
                                    {

                                        ViewBag.error = "Transaction Failure";
                                        Session["error"] = ViewBag.error;
                                        Session["Message"] = e.Message;
                                    }

                                }
                                else
                                {
                                    ViewBag.Message = "Email is Incorrect!";
                                    Session["Message"] = ViewBag.Message;
                                    Session["error"] = ViewBag.Message;
                                }
                            }
                            else
                            {
                                ViewBag.Message = "Contact Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                                Session["Message"] = ViewBag.Message;
                                Session["error"] = ViewBag.Message;
                            }

                        }


                    }

                    else
                    {

                        if (Lead_Save.ContactType == "Company")
                        {
                            if (USNumberValidatorWithEmptyOK(Lead_Save.ContactNumber))
                            {

                                if (EmailValidatorWithEmptyOk(Lead_Save.ContactEmail))
                                {

                                    cmd.CommandText = "LeadsUpdate";
                                    cmd.CommandType = CommandType.StoredProcedure;

                                    cmd.Parameters.AddWithValue("@LeadsId", Lead_Save.LeadsId);
                                    cmd.Parameters.AddWithValue("@Breed", Lead_Save.Breed);
                                    cmd.Parameters.AddWithValue("@City", Lead_Save.City);
                                    cmd.Parameters.AddWithValue("@Address", Lead_Save.Address);
                                    cmd.Parameters.AddWithValue("@Address2", Lead_Save.Address2);
                                    cmd.Parameters.AddWithValue("@OccupantCity", Lead_Save.OccupantCity);
                                    cmd.Parameters.AddWithValue("@OccupantState", Lead_Save.OccupantState);
                                    cmd.Parameters.AddWithValue("@OccupantZip", Lead_Save.OccupantZip);
                                    cmd.Parameters.AddWithValue("@ContactEmail", Lead_Save.ContactEmail);

                                    cmd.Parameters.AddWithValue("@ContactNumber", Lead_Save.ContactNumber);

                                    cmd.Parameters.AddWithValue("@ContactType", Lead_Save.ContactType);
                                    cmd.Parameters.AddWithValue("@FloorPreference", Lead_Save.FloorPreference);
                                    cmd.Parameters.AddWithValue("@LeadsName", Lead_Save.LeadsName);
                                    cmd.Parameters.AddWithValue("@LeaseTerm", Lead_Save.LeaseTerm);
                                    cmd.Parameters.AddWithValue("@MoveInDate", Lead_Save.MoveInDate);
                                    cmd.Parameters.AddWithValue("@NoOfAdults", Lead_Save.NoOfAdults);

                                    cmd.Parameters.AddWithValue("@NoOfBedRooms", Lead_Save.NoOfBedRooms);

                                    cmd.Parameters.AddWithValue("@NoOfChildren", Lead_Save.NoOfChildren);
                                    cmd.Parameters.AddWithValue("@NoOfPets", Lead_Save.NoOfPets);
                                    cmd.Parameters.AddWithValue("@Notes", Lead_Save.Notes);

                                    cmd.Parameters.AddWithValue("@OcupantName", Lead_Save.OcupantName);
                                    cmd.Parameters.AddWithValue("@PreferedArea", Lead_Save.PreferedArea);

                                    cmd.Parameters.AddWithValue("@ReferelSource", Lead_Save.ReferelSource);

                                    cmd.Parameters.AddWithValue("@Weight", Lead_Save.Weight);
                                    cmd.Parameters.AddWithValue("@Zip", Lead_Save.Zip);
                                    cmd.Parameters.AddWithValue("@Elevator", Lead_Save.Elevator);
                                    cmd.Parameters.AddWithValue("@State", Lead_Save.State);
                                    cmd.Parameters.AddWithValue("@CompanyLogo", Lead_Save.CompanyLogo);
                                    cmd.Parameters.AddWithValue("@PreferedAddress", Lead_Save.PreferedAddress);

                                    try
                                    {
                                        con.Open();
                                        cmd.ExecuteNonQuery();
                                        Session["SuccessMessage"] = "Success: Lead Successfully Updated";
                                    }
                                    catch (SqlException e)
                                    {

                                        ViewBag.error = "Transaction Failure";
                                        Session["Message"] = e.Message;
                                    }
                                }
                                else
                                {
                                    ViewBag.Message = "Email is Incorrect!";
                                    Session["Message"] = ViewBag.Message;
                                    Session["error"] = ViewBag.Message;
                                }
                            }
                            else
                            {
                                ViewBag.Message = "Contact Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                                Session["Message"] = ViewBag.Message;
                                Session["error"] = ViewBag.Message;
                            }
                        }

                        else
                        {
                            if (USNumberValidatorWithEmptyOK(Lead_Save.ContactNumber))
                            {
                                if (EmailValidatorWithEmptyOk(Lead_Save.ContactEmail))
                                {
                                    cmd.CommandText = "LeadsUpdate";
                                    cmd.CommandType = CommandType.StoredProcedure;

                                    cmd.Parameters.AddWithValue("@LeadsId", Lead_Save.LeadsId);
                                    cmd.Parameters.AddWithValue("@Breed", Lead_Save.Breed);
                                    cmd.Parameters.AddWithValue("@City", Lead_Save.City);
                                    cmd.Parameters.AddWithValue("@Address", Lead_Save.Address);
                                    cmd.Parameters.AddWithValue("@ContactEmail", Lead_Save.ContactEmail);
                                    cmd.Parameters.AddWithValue("@Address2", Lead_Save.Address2);
                                    cmd.Parameters.AddWithValue("@OccupantCity", Lead_Save.OccupantCity);
                                    cmd.Parameters.AddWithValue("@OccupantState", Lead_Save.OccupantState);
                                    cmd.Parameters.AddWithValue("@OccupantZip", Lead_Save.OccupantZip);
                                    cmd.Parameters.AddWithValue("@ContactNumber", Lead_Save.ContactNumber);

                                    cmd.Parameters.AddWithValue("@ContactType", Lead_Save.ContactType);
                                    cmd.Parameters.AddWithValue("@FloorPreference", Lead_Save.FloorPreference);
                                    cmd.Parameters.AddWithValue("@LeadsName", Lead_Save.LeadsName);
                                    cmd.Parameters.AddWithValue("@LeaseTerm", Lead_Save.LeaseTerm);
                                    cmd.Parameters.AddWithValue("@MoveInDate", Lead_Save.MoveInDate);
                                    cmd.Parameters.AddWithValue("@NoOfAdults", Lead_Save.NoOfAdults);

                                    cmd.Parameters.AddWithValue("@NoOfBedRooms", Lead_Save.NoOfBedRooms);

                                    cmd.Parameters.AddWithValue("@NoOfChildren", Lead_Save.NoOfChildren);
                                    cmd.Parameters.AddWithValue("@NoOfPets", Lead_Save.NoOfPets);
                                    cmd.Parameters.AddWithValue("@Notes", Lead_Save.Notes);

                                    cmd.Parameters.AddWithValue("@OcupantName", Lead_Save.OcupantName);
                                    cmd.Parameters.AddWithValue("@PreferedArea", Lead_Save.PreferedArea);

                                    cmd.Parameters.AddWithValue("@ReferelSource", Lead_Save.ReferelSource);

                                    cmd.Parameters.AddWithValue("@Weight", Lead_Save.Weight);
                                    cmd.Parameters.AddWithValue("@Zip", Lead_Save.Zip);
                                    cmd.Parameters.AddWithValue("@Elevator", Lead_Save.Elevator);
                                    cmd.Parameters.AddWithValue("@State", Lead_Save.State);
                                    cmd.Parameters.AddWithValue("@CompanyLogo", Lead_Save.CompanyLogo);
                                    cmd.Parameters.AddWithValue("@PreferedAddress", Lead_Save.PreferedAddress);

                                    try
                                    {
                                        con.Open();
                                        cmd.ExecuteNonQuery();
                                        Session["SuccessMessage"] = "Success: Lead Successfully Updated";
                                    }
                                    catch (SqlException e)
                                    {

                                        ViewBag.error = "Transaction Failure";
                                        Session["Message"] = e.Message;
                                    }
                                }
                                else
                                {
                                    ViewBag.Message = "Email is Incorrect!";
                                    Session["Message"] = ViewBag.Message;
                                    Session["error"] = ViewBag.Message;
                                }
                            }
                            else
                            {
                                ViewBag.Message = "Contact Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                                Session["Message"] = ViewBag.Message;
                                Session["error"] = ViewBag.Message;
                            }
                        }


                    }

                }
            }
            con.Close();

            if (Session["error"] != null)
            {
                ViewData["Message"] = Session["error"];
                ViewBag.ContactType = DropDownListContantType(model["ContactType"].ToString());
                ViewBag.LeaseTerm = DropDownListLeaseTerm("");
                ViewBag.City = DropDownListCity("");
                ViewBag.ReferelSource = DropDownListReferelSource("");
                ViewBag.NoOfAdults = DropDownListNumber(model["NoOfAdults"].ToString());
                ViewBag.NoOfBedRooms = DropDownListBed(model["NoOfBedRooms"].ToString());
                ViewBag.NoOfChildren = DropDownListNumber(model["NoOfChildren"].ToString());
                ViewBag.NoOfPets = DropDownListNumber(model["NoOfPets"].ToString());
                ViewBag.Weight = DropDownListWeight("");
                ViewBag.ReferelSource = BindDataReferalAll(0);
                ViewBag.Company = BindDataCompanyAll(0).Where(y => y.IsActive = true);
                ViewBag.Contact = BindDataContactAll(0).Where(y => y.IsActive = true);
                ViewBag.Elevator = DropDownListYesNo(model["Elevator"].ToString());
                ViewBag.States = BindDataStatesAll(0);
                return View(Lead_Save);
            }
            else
            {
                ViewData["SuccessMessage"] = Session["SuccessMessage"];
                return RedirectToAction("Leads");
            }


        }

        public ActionResult DeleteLead(decimal id = 0)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"LeadsDelete";

                cmd.Parameters.AddWithValue("@LeadId", id);

                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Lead Deleted Completely Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Leads");
        }

        public ActionResult ActiveInactiveLead(decimal id = 0, int IsActive = 1)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"LeadEnableDisable";

                cmd.Parameters.AddWithValue("@LeadsId", id);
                if (IsActive == 1)
                {
                    cmd.Parameters.AddWithValue("@IsActive", true);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsActive", false);
                }
                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Lead Updated Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Leads");
        }

        /// <summary>
        /// ///Properties
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult Properties(decimal id = 0, string srchContactName = "", string srchOccupantName = "")
        {

            //string connect = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
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
            ViewBag.PropContactList = BindDataPropContactAll(0);
            return View(BindDataPropertyAll(id, srchContactName, srchOccupantName));
        }

        public ActionResult AddProperty(Int64 Id = 0)
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
            SqlCommand cmd = new SqlCommand("PropertiesSelect", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PropertyId", Id);
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


                //Property_Single.Address = dt.Rows[0]["Address"].ToString();
                Property_Single.AdminFee = decimal.Parse(dt.Rows[0]["AdminFee"].ToString());
                Property_Single.PropertyId = Int64.Parse(dt.Rows[0]["PropertyId"].ToString());
                Property_Single.ApplicationFee = decimal.Parse(dt.Rows[0]["ApplicationFee"].ToString());
                Property_Single.City = dt.Rows[0]["City"].ToString();
                Property_Single.Area = dt.Rows[0]["Area"].ToString();
                if (dt.Rows[0]["BusinessCenterImage"].ToString() == "")
                {
                    Property_Single.BusinessCenterImage = "No Image Saved Yet";
                }
                else
                {
                    Property_Single.BusinessCenterImage = dt.Rows[0]["BusinessCenterImage"].ToString();
                }


                Property_Single.WeightLimit = decimal.Parse(dt.Rows[0]["WeightLimit"].ToString());
                Property_Single.BreakLeasePolicy = dt.Rows[0]["BreakLeasePolicy"].ToString();
                Property_Single.Building = dt.Rows[0]["Building"].ToString();
                Property_Single.CleaningFee = decimal.Parse(dt.Rows[0]["CleaningFee"].ToString());
                Property_Single.CreatedBy = DateTime.Now;
                Property_Single.CreatedDatee = DateTime.Now;
                if (dt.Rows[0]["ElevatorFitnessImage"].ToString() == "")
                {
                    Property_Single.ElevatorFitnessImage = "No Image Saved Yet";
                }
                else
                {
                    Property_Single.ElevatorFitnessImage = dt.Rows[0]["ElevatorFitnessImage"].ToString();
                }

                Property_Single.PetFee = decimal.Parse(dt.Rows[0]["PetFee"].ToString());
                Property_Single.MaxNoOfPets = Int16.Parse(dt.Rows[0]["MaxNoOfPets"].ToString());
                Property_Single.Leased = false;
                if (dt.Rows[0]["MailBoxImage"].ToString() == "")
                {
                    Property_Single.MailBoxImage = "No Image Saved Yet";
                }
                else
                {
                    Property_Single.MailBoxImage = dt.Rows[0]["MailBoxImage"].ToString();
                }

                Property_Single.Floor = dt.Rows[0]["Floor"].ToString();
                if (dt.Rows[0]["ParkingTypeImage"].ToString() == "")
                {
                    Property_Single.ParkingTypeImage = "No Image Saved Yet";
                }
                else
                {
                    Property_Single.ParkingTypeImage = dt.Rows[0]["ParkingTypeImage"].ToString();
                }

                Property_Single.PetBreedRestrictions = dt.Rows[0]["PetBreedRestrictions"].ToString();
                if (dt.Rows[0]["PoolImage"].ToString() == "")
                {
                    Property_Single.PoolImage = "No Image Saved Yet";
                }
                else
                {
                    Property_Single.PoolImage = dt.Rows[0]["PoolImage"].ToString();
                }

                Property_Single.PropertyAddress = dt.Rows[0]["PropertyAddress"].ToString();
                Property_Single.PropertyAddress2 = dt.Rows[0]["PropertyAddress2"].ToString();
                Property_Single.City = dt.Rows[0]["City"].ToString();

                if (dt.Rows[0]["Zip"] == null || dt.Rows[0]["Zip"].ToString() == "" || Int64.Parse(dt.Rows[0]["Zip"].ToString()) == 0)
                {
                    Property_Single.Zip = null;
                }
                else
                {
                    Property_Single.Zip = Int64.Parse(dt.Rows[0]["Zip"].ToString());
                }
                //Property_Single.Zip = Int64.Parse( dt.Rows[0]["Zip"].ToString());
                Property_Single.State = dt.Rows[0]["State"].ToString();
                Property_Single.PropertyDescription = dt.Rows[0]["PropertyDescription"].ToString();
                Property_Single.ValetTrash = dt.Rows[0]["ValetTrash"].ToString();


                Property_Single.UnitSize = dt.Rows[0]["UnitSize"].ToString();
                Property_Single.UnitSquareFootage = dt.Rows[0]["UnitSquareFootage"].ToString();
                Property_Single.UnitType = dt.Rows[0]["UnitType"].ToString();
                if (dt.Rows[0]["UnitType"].ToString() == "Other")
                {
                    Property_Single.UnitType = dt.Rows[0]["UnitCustom"].ToString();
                }
                Property_Single.VendorId = Int64.Parse(dt.Rows[0]["VendorId"].ToString());
                Property_Single.VendorName = dt.Rows[0]["VendorName"].ToString();
                Property_Single.WebSite = dt.Rows[0]["WebSite"].ToString();
                Property_Single.Pool = dt.Rows[0]["Pool"].ToString();
                Property_Single.PhoneNumber = dt.Rows[0]["PhoneNumber"].ToString();
                Property_Single.ParkingType = dt.Rows[0]["ParkingType"].ToString();
                if (dt.Rows[0]["ParkingType"].ToString() == "Other")
                {
                    Property_Single.ParkingType = dt.Rows[0]["PTCustom"].ToString();
                }
                Property_Single.OtherDepositAmount = dt.Rows[0]["OtherDepositAmount"].ToString();
                Property_Single.OtherDeposit = dt.Rows[0]["OtherDeposit"].ToString();
                if (dt.Rows[0]["OtherDeposit"].ToString() == "Other")
                {
                    Property_Single.OtherDeposit = dt.Rows[0]["DepositCustom"].ToString();
                }
                Property_Single.NoticetoVacate = dt.Rows[0]["NoticetoVacate"].ToString();
                if (dt.Rows[0]["NoticetoVacate"].ToString() == "Other")
                {
                    Property_Single.NoticetoVacate = dt.Rows[0]["NTVCustom"].ToString();
                }
                Property_Single.Name = dt.Rows[0]["Name"].ToString();
                Property_Single.MailboxLocation = dt.Rows[0]["MailboxLocation"].ToString();
                Property_Single.LeaseEndDate = dt.Rows[0]["LeaseEndDate"].ToString();
                if (dt.Rows[0]["LeaseEndDate"].ToString() == "Other")
                {
                    Property_Single.LeaseEndDate = dt.Rows[0]["LEDCustom"].ToString();
                }
                Property_Single.Hours = dt.Rows[0]["Hours"].ToString();
                //Property_Single.FloorPlanPic3;
                if (dt.Rows[0]["FloorPlanPic3"].ToString() == "")
                {
                    Property_Single.FloorPlanPic3 = "No Image";
                }
                else
                {
                    Property_Single.FloorPlanPic3 = dt.Rows[0]["FloorPlanPic3"].ToString();
                }
                //Property_Single.FloorPlanPic2;
                if (dt.Rows[0]["FloorPlanPic2"].ToString() == "")
                {
                    Property_Single.FloorPlanPic2 = "No Image";
                }
                else
                {
                    Property_Single.FloorPlanPic2 = dt.Rows[0]["FloorPlanPic2"].ToString();
                }
                //Property_Single.FloorPlanPic1;
                if (dt.Rows[0]["FloorPlanPic1"].ToString() == "")
                {
                    Property_Single.FloorPlanPic1 = "No Image";
                }
                else
                {
                    Property_Single.FloorPlanPic1 = dt.Rows[0]["FloorPlanPic1"].ToString();
                }
                //Property_Single.FloorPlanPic;
                if (dt.Rows[0]["FloorPlanPic"].ToString() == "")
                {
                    Property_Single.FloorPlanPic = "No Image";
                }
                else
                {
                    Property_Single.FloorPlanPic = dt.Rows[0]["FloorPlanPic"].ToString();
                }
                Property_Single.Floor = dt.Rows[0]["Floor"].ToString();
                Property_Single.Fitness = dt.Rows[0]["Fitness"].ToString();
                Property_Single.Features = dt.Rows[0]["Features"].ToString();
                Property_Single.EmergencyPhoneNumber = dt.Rows[0]["EmergencyPhoneNumber"].ToString();

                Property_Single.Elevator = dt.Rows[0]["Elevator"].ToString();

                ViewBag.Elevator = DropDownListYesNo(dt.Rows[0]["Elevator"].ToString());
                ViewBag.Fitness = DropDownListYesNo(dt.Rows[0]["Fitness"].ToString());
                ViewBag.Pool = DropDownListYesNo(dt.Rows[0]["Pool"].ToString());
                ViewBag.BusinessCenter = DropDownListYesNo(dt.Rows[0]["BusinessCenter"].ToString());
                ViewBag.ParkingType = DropDownListParkingType(dt.Rows[0]["ParkingType"].ToString());

                ViewBag.LeaseEndDate = DropDownListLeaseEndDate(dt.Rows[0]["LeaseEndDate"].ToString());
                ViewBag.NoticetoVacate = DropDownListNoticeToVacate(dt.Rows[0]["NoticetoVacate"].ToString());
                ViewBag.OtherDeposit = DropDownListOtherDeposit(dt.Rows[0]["OtherDeposit"].ToString());
                ViewBag.UnitType = DropDownListUnitType(dt.Rows[0]["UnitType"].ToString());
                //ViewBag.ContactType = DropDownListContantType(dt.Rows[0]["ContactType"].ToString());
                //ViewBag.LeaseTerm = DropDownListLeaseTerm(dt.Rows[0]["LeaseTerm"].ToString());
                ViewBag.City = DropDownListCity(dt.Rows[0]["City"].ToString());
                ViewBag.PropContactList = BindDataPropContactAll(Int64.Parse(dt.Rows[0]["PropertyId"].ToString()));
                //ViewBag.ReferelSource = DropDownListReferelSource(dt.Rows[0]["ReferelSource"].ToString());
                ViewBag.Vendor = BindDataVendorAll("").Where(x => x.VendorType == "Management Company").Where(x => x.IsActive);
                ViewBag.States = BindDataStatesAll(0);
                ViewBag.CommunityFeatures = CheckBoxListCommunityFeatures(dt.Rows[0]["CommunityFeatures"].ToString());
                ViewBag.UnitFeatures = CheckBoxListUnitFeatures(dt.Rows[0]["UnitFeatures"].ToString());
            }
            else
            {
                Property_Single = new Property()
                {
                    AdminFee = 0,
                    PropertyId = 0,
                    ApplicationFee = 0,
                    City = "Houston",
                    Area = "",
                    BusinessCenterImage = "No Image",
                    WeightLimit = 0,
                    BreakLeasePolicy = "if one year Lease aggrement then 20% of Total Monthly Rent X 12 will be paid.",
                    Building = "",
                    CleaningFee = 0,
                    CreatedBy = DateTime.Now,
                    CreatedDatee = DateTime.Now,
                    ElevatorFitnessImage = "No Image",
                    PetFee = 0,
                    MaxNoOfPets = 0,
                    Leased = false,
                    MailBoxImage = "No Image",
                    Floor = "Ground",
                    ParkingTypeImage = "No Image",
                    PetBreedRestrictions = "Big Cat Family and Reptiles not allowed",
                    PoolImage = "No Image",
                    PropertyAddress = " Houston TX",
                    State = "TX",
                    PropertyDescription = "2 bed and two wash room",
                    ValetTrash = "0",
                    Amenities = "backyard pool and garden and garadge",
                    BusinessCenter = "",
                    Elevator = "",
                    EmergencyPhoneNumber = "+1-354-315-6815",
                    Features = "Corner villa and facing Garden",
                    Fitness = "",
                    FloorPlanPic = "No Image",
                    FloorPlanPic1 = "No Image",
                    FloorPlanPic2 = "No Image",
                    FloorPlanPic3 = "No Image",
                    Hours = "24 hours",
                    LeaseEndDate = "",
                    MailboxLocation = "Houston Post office",
                    Name = "",
                    NoticetoVacate = "",
                    OtherDeposit = "pet Deposit",
                    OtherDepositAmount = "0",
                    ParkingFee = "0",
                    ParkingType = "",
                    PhoneNumber = "+1-854-325-4875",
                    Pool = "",
                    Status = "New Property",
                    UnitSize = "5000",
                    UnitSquareFootage = "bed and bath and laung",
                    UnitType = "",
                    VendorId = 0,
                    VendorName = "",
                    WebSite = "www.ks.com",
                    Zip = 74588,
                    PropertyAddress2 = ""
                };
                //ViewBag.ContactType = DropDownListContantType("");
                //ViewBag.LeaseTerm = DropDownListLeaseTerm("");

                ViewBag.Elevator = DropDownListYesNo("");
                ViewBag.Fitness = DropDownListYesNo("");
                ViewBag.Pool = DropDownListYesNo("");
                ViewBag.BusinessCenter = DropDownListYesNo("");
                ViewBag.ParkingType = DropDownListParkingType("");

                ViewBag.LeaseEndDate = DropDownListLeaseEndDate("");
                ViewBag.NoticetoVacate = DropDownListNoticeToVacate("");
                ViewBag.OtherDeposit = DropDownListOtherDeposit("");
                ViewBag.UnitType = DropDownListUnitType("");
                ViewBag.City = DropDownListCity("");
                //ViewBag.ReferelSource = DropDownListReferelSource("");
                ViewBag.PropContactList = BindDataPropContactAll(0);
                ViewBag.Vendor = BindDataVendorAll("").Where(x => x.VendorType == "Management Company").Where(x => x.IsActive = true);
                ViewBag.States = BindDataStatesAll(0);
                ViewBag.CommunityFeatures = CheckBoxListCommunityFeatures("");
                ViewBag.UnitFeatures = CheckBoxListUnitFeatures("");
            }



            return View(Property_Single);
        }

        [HttpPost]
        public ActionResult AddProperty(FormCollection model)
        {

            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["error"] = null;
            Property Property_Save = new Property();

            Property_Save.PropertyId = Int64.Parse(model["PropertyId"]);
            if (Request.Files.Count != 0)
            {
                for (int indexer = 0; indexer < Request.Files.Count; indexer++)
                {
                    HttpPostedFileBase file = Request.Files[indexer];
                    if (file != null)
                    {
                        if (file.ContentLength > 0)
                        {
                            string path = Server.MapPath("~/Uploads/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            file.SaveAs(path + Path.GetFileName(file.FileName));
                            if (indexer == 0)
                            {
                                Property_Save.FloorPlanPic = Path.GetFileName(file.FileName);
                            }
                            else if (indexer == 1)
                            {
                                Property_Save.FloorPlanPic1 = Path.GetFileName(file.FileName);
                            }
                            else if (indexer == 2)
                            {
                                Property_Save.FloorPlanPic2 = Path.GetFileName(file.FileName);
                            }
                            else if (indexer == 3)
                            {
                                Property_Save.FloorPlanPic3 = Path.GetFileName(file.FileName);
                            }
                            else if (indexer == 4)
                            {
                                Property_Save.BusinessCenterImage = Path.GetFileName(file.FileName);
                            }
                            else if (indexer == 5)
                            {
                                Property_Save.ElevatorFitnessImage = Path.GetFileName(file.FileName);
                            }
                            else if (indexer == 6)
                            {
                                Property_Save.MailBoxImage = Path.GetFileName(file.FileName);
                            }
                            else if (indexer == 7)
                            {
                                Property_Save.ParkingTypeImage = Path.GetFileName(file.FileName);
                            }
                            else if (indexer == 8)
                            {
                                Property_Save.PoolImage = Path.GetFileName(file.FileName);
                            }

                            ViewBag.Message = "File uploaded successfully.";
                        }
                        else
                        {
                            if (indexer == 0)
                            {
                                Property_Save.FloorPlanPic = model["FloorPlanPic"].ToString();
                            }
                            else if (indexer == 1)
                            {
                                Property_Save.FloorPlanPic1 = model["FloorPlanPic1"].ToString();
                            }
                            else if (indexer == 2)
                            {
                                Property_Save.FloorPlanPic2 = model["FloorPlanPic2"].ToString();
                            }
                            else if (indexer == 3)
                            {
                                Property_Save.FloorPlanPic3 = model["FloorPlanPic3"].ToString();
                            }
                            else if (indexer == 4)
                            {
                                Property_Save.BusinessCenterImage = model["BusinessCenterImage"].ToString();
                            }
                            else if (indexer == 5)
                            {
                                Property_Save.ElevatorFitnessImage = model["ElevatorFitnessImage"].ToString();
                            }
                            else if (indexer == 6)
                            {
                                Property_Save.MailBoxImage = model["MailBoxImage"].ToString();
                            }
                            else if (indexer == 7)
                            {
                                Property_Save.ParkingTypeImage = model["ParkingTypeImage"].ToString();
                            }
                            else if (indexer == 8)
                            {
                                Property_Save.PoolImage = model["PoolImage"].ToString();
                            }
                        }

                    }
                }
            }
            Property_Save.AdminFee = decimal.Parse(model["AdminFee"].ToString());
            Property_Save.PropertyId = Int64.Parse(model["PropertyId"].ToString());
            Property_Save.ApplicationFee = decimal.Parse(model["ApplicationFee"].ToString());
            Property_Save.City = model["City"].ToString();
            Property_Save.Area = model["Area"].ToString();
            //Property_Save.BusinessCenterImage = model["BusinessCenterImage"].ToString();
            Property_Save.WeightLimit = decimal.Parse(model["WeightLimit"].ToString());
            Property_Save.BreakLeasePolicy = model["BreakLeasePolicy"].ToString();
            Property_Save.Building = model["Building"].ToString();
            Property_Save.CleaningFee = decimal.Parse(model["CleaningFee"].ToString());
            Property_Save.CreatedBy = DateTime.Now;
            Property_Save.CreatedDatee = DateTime.Now;
            //Property_Save.ElevatorFitnessImage = model["ElevatorFitnessImage"].ToString();
            Property_Save.PetFee = decimal.Parse(model["PetFee"].ToString());
            Property_Save.MaxNoOfPets = Int16.Parse(model["MaxNoOfPets"].ToString());
            Property_Save.Leased = false;
            //Property_Save.MailBoxImage = model["MailBoxImage"].ToString();
            Property_Save.Floor = model["Floor"].ToString();
            //Property_Save.ParkingTypeImage = model["ParkingTypeImage"].ToString();
            Property_Save.PetBreedRestrictions = model["PetBreedRestrictions"].ToString();
            //Property_Save.PoolImage = model["PoolImage"].ToString();
            Property_Save.PropertyAddress2 = model["PropertyAddress2"].ToString();
            Property_Save.City = model["City"].ToString();
            Property_Save.PropertyAddress = model["PropertyAddress"].ToString();

            //Property_Save.CommunityFeatures = model["CommunityFeatures"].ToString();
            //Property_Save.UnitFeatures = model["UnitFeatures"].ToString();

            Property_Save.State = model["State"].ToString();

            if (model["Zip"] == null || model["Zip"].ToString() == "")
            {
                Property_Save.Zip = 0;
            }
            else
            {
                Property_Save.Zip = Int64.Parse(model["Zip"].ToString());
            }
            //Property_Save.Zip = decimal.Parse( model["Zip"].ToString());
            Property_Save.PropertyDescription = model["PropertyDescription"].ToString();
            Property_Save.ValetTrash = model["ValetTrash"].ToString();

            Property_Save.Amenities = model["Amenities"].ToString();
            Property_Save.BusinessCenter = model["BusinessCenter"].ToString();
            Property_Save.ParkingFee = model["ParkingFee"].ToString();
            //Property_Save.Status = "New Property";
            Property_Save.UnitSize = model["UnitSize"].ToString();
            Property_Save.UnitSquareFootage = model["UnitSquareFootage"].ToString();
            Property_Save.UnitType = model["UnitType"].ToString();
            if (model["UnitType"].ToString() == "Other")
            {
                Property_Save.UnitType = model["UnitCustom"].ToString();
            }
            Property_Save.VendorId = Int64.Parse(model["VendorId"].ToString());
            Property_Save.VendorName = model["VendorName"].ToString();
            Property_Save.WebSite = model["WebSite"].ToString();
            Property_Save.Pool = model["Pool"].ToString();
            Property_Save.PhoneNumber = model["PhoneNumber"].ToString();
            Property_Save.ParkingType = model["ParkingType"].ToString();
            if (model["ParkingType"].ToString() == "Other")
            {
                Property_Save.ParkingType = model["PTCustom"].ToString();
            }
            Property_Save.OtherDepositAmount = model["OtherDepositAmount"].ToString();
            Property_Save.OtherDeposit = model["OtherDeposit"].ToString();
            if (model["OtherDeposit"].ToString() == "Other")
            {
                Property_Save.OtherDeposit = model["DepositCustom"].ToString();
            }
            Property_Save.NoticetoVacate = model["NoticetoVacate"].ToString();
            if (model["NoticetoVacate"].ToString() == "Other")
            {
                Property_Save.NoticetoVacate = model["NTVCustom"].ToString();
            }
            Property_Save.Name = model["Name"].ToString();
            Property_Save.MailboxLocation = model["MailboxLocation"].ToString();
            Property_Save.LeaseEndDate = model["LeaseEndDate"].ToString();
            if (model["LeaseEndDate"].ToString() == "Other")
            {
                Property_Save.LeaseEndDate = model["LEDCustom"].ToString();
            }
            Property_Save.Hours = model["Hours"].ToString();
            Property_Save.Floor = model["Floor"].ToString();
            Property_Save.Fitness = model["Fitness"].ToString();
            Property_Save.Features = model["Features"].ToString();
            Property_Save.EmergencyPhoneNumber = model["EmergencyPhoneNumber"].ToString();

            Property_Save.Elevator = model["Elevator"].ToString();

            Property_Save.CommunityFeatures = string.Join(",", model["SelectedTypes2"]);
            Property_Save.UnitFeatures = string.Join(",", model["SelectedTypes"]);

            List<PropContact> propContactList = new List<PropContact>();
            PropContact propContact = new PropContact();
            string[] keys = model.AllKeys;
            for (int i = 0; i <= 50; i++)
            {
                if (keys.Contains<string>("PropContactId" + i))
                {
                    propContact.PropContactId = Int64.Parse(model["PropContactId" + i].ToString());
                    if (Int64.Parse(model["PropertyId" + i].ToString()) > 0)
                    {
                        propContact.PropertyId = Int64.Parse(model["PropertyId" + i].ToString());
                    }
                    else
                    {
                        propContact.PropertyId = Int64.Parse(model["PropertyId" + i].ToString());
                    }

                    propContact.PropContactEmail = model["PropContactEmail" + i].ToString();
                    propContact.PropContactFirstName = model["PropContactFirstName" + i].ToString();
                    propContact.PropContactLastName = model["PropContactLastName" + i].ToString();
                    propContact.PropContactNumber = model["PropContactNumber" + i].ToString();
                    propContactList.Add(propContact);
                    propContact = new PropContact();
                }

            }

            //if (Session["LoginUserRole"] != null)
            //{
            //    //vwAppUserAsset_Save.last_update_user_id = Session["LoginUserEmail"].ToString();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                if (Property_Save.PropertyId == 0)
                {
                    if (USNumberValidator(Property_Save.PhoneNumber))
                    {
                        if (USNumberValidator(Property_Save.EmergencyPhoneNumber))
                        {
                            if (WebSiteValidator(Property_Save.WebSite))
                            {
                                if (!DuplicateSingleCommon("Properties", "PropertyId", "Name", Property_Save.Name))
                                {

                                    cmd.CommandText = "PropertiesInsert";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    //SqlTransaction transaction;

                                    cmd.Parameters.AddWithValue("@AdminFee", Property_Save.AdminFee);
                                    //cmd.Parameters.AddWithValue("@PropertyId", Property_Save.PropertyId);
                                    cmd.Parameters.AddWithValue("@ApplicationFee", Property_Save.ApplicationFee);
                                    //cmd.Parameters.AddWithValue("@City", Property_Save.City);
                                    cmd.Parameters.AddWithValue("@Area", Property_Save.Area);
                                    cmd.Parameters.AddWithValue("@BusinessCenterImage", Property_Save.BusinessCenterImage);
                                    cmd.Parameters.AddWithValue("@WeightLimit", Property_Save.WeightLimit);
                                    cmd.Parameters.AddWithValue("@BreakLeasePolicy", Property_Save.BreakLeasePolicy);
                                    cmd.Parameters.AddWithValue("@Building", Property_Save.Building);
                                    cmd.Parameters.AddWithValue("@CleaningFee", Property_Save.CleaningFee);
                                    cmd.Parameters.AddWithValue("@CreatedBy", Property_Save.CreatedBy);
                                    cmd.Parameters.AddWithValue("@CreatedDatee", Property_Save.CreatedDatee);
                                    cmd.Parameters.AddWithValue("@ElevatorFitnessImage", Property_Save.ElevatorFitnessImage);
                                    cmd.Parameters.AddWithValue("@PetFee", Property_Save.PetFee);
                                    cmd.Parameters.AddWithValue("@MaxNoOfPets", Property_Save.MaxNoOfPets);
                                    cmd.Parameters.AddWithValue("@Leased", Property_Save.Leased);
                                    cmd.Parameters.AddWithValue("@MailBoxImage", Property_Save.MailBoxImage);
                                    cmd.Parameters.AddWithValue("@Floor", Property_Save.Floor);
                                    cmd.Parameters.AddWithValue("@ParkingTypeImage", Property_Save.ParkingTypeImage);
                                    cmd.Parameters.AddWithValue("@PetBreedRestrictions", Property_Save.PetBreedRestrictions);
                                    cmd.Parameters.AddWithValue("@PoolImage", Property_Save.PoolImage);
                                    cmd.Parameters.AddWithValue("@PropertyAddress", Property_Save.PropertyAddress);
                                    cmd.Parameters.AddWithValue("@PropertyAddress2", Property_Save.PropertyAddress2);
                                    cmd.Parameters.AddWithValue("@City", Property_Save.City);
                                    cmd.Parameters.AddWithValue("@Zip", Property_Save.Zip);
                                    cmd.Parameters.AddWithValue("@State", Property_Save.State);
                                    cmd.Parameters.AddWithValue("@PropertyDescription", Property_Save.PropertyDescription);
                                    cmd.Parameters.AddWithValue("@ValetTrash", Property_Save.ValetTrash);

                                    cmd.Parameters.AddWithValue("@Amenities", Property_Save.Amenities);
                                    cmd.Parameters.AddWithValue("@BusinessCenter", Property_Save.BusinessCenter);
                                    cmd.Parameters.AddWithValue("@Elevator", Property_Save.Elevator);
                                    cmd.Parameters.AddWithValue("@EmergencyPhoneNumber", Property_Save.EmergencyPhoneNumber);
                                    cmd.Parameters.AddWithValue("@Features", Property_Save.Features);
                                    cmd.Parameters.AddWithValue("@Fitness", Property_Save.Fitness);
                                    cmd.Parameters.AddWithValue("@FloorPlanPic", Property_Save.FloorPlanPic);
                                    cmd.Parameters.AddWithValue("@FloorPlanPic1", Property_Save.FloorPlanPic1);
                                    cmd.Parameters.AddWithValue("@FloorPlanPic2", Property_Save.FloorPlanPic2);
                                    cmd.Parameters.AddWithValue("@FloorPlanPic3", Property_Save.FloorPlanPic3);
                                    cmd.Parameters.AddWithValue("@Hours", Property_Save.Hours);
                                    cmd.Parameters.AddWithValue("@LeaseEndDate", Property_Save.LeaseEndDate);
                                    cmd.Parameters.AddWithValue("@MailboxLocation", Property_Save.MailboxLocation);
                                    cmd.Parameters.AddWithValue("@Name", Property_Save.Name);
                                    cmd.Parameters.AddWithValue("@NoticetoVacate", Property_Save.NoticetoVacate);
                                    cmd.Parameters.AddWithValue("@OtherDeposit", Property_Save.OtherDeposit);
                                    cmd.Parameters.AddWithValue("@OtherDepositAmount", Property_Save.OtherDepositAmount);
                                    cmd.Parameters.AddWithValue("@ParkingFee", Property_Save.ParkingFee);
                                    cmd.Parameters.AddWithValue("@ParkingType", Property_Save.ParkingType);
                                    cmd.Parameters.AddWithValue("@PhoneNumber", Property_Save.PhoneNumber);
                                    cmd.Parameters.AddWithValue("@Pool", Property_Save.Pool);
                                    //cmd.Parameters.AddWithValue("@Status", Property_Save.Status);
                                    cmd.Parameters.AddWithValue("@UnitSize", Property_Save.UnitSize);
                                    cmd.Parameters.AddWithValue("@UnitSquareFootage", Property_Save.UnitSquareFootage);
                                    cmd.Parameters.AddWithValue("@UnitType", Property_Save.UnitType);
                                    cmd.Parameters.AddWithValue("@VendorId", Property_Save.VendorId);
                                    cmd.Parameters.AddWithValue("@VendorName", Property_Save.VendorName);
                                    cmd.Parameters.AddWithValue("@WebSite", Property_Save.WebSite);

                                    cmd.Parameters.AddWithValue("@CommunityFeatures", Property_Save.CommunityFeatures);
                                    cmd.Parameters.AddWithValue("@UnitFeatures", Property_Save.UnitFeatures);

                                    con.Open();
                                }
                                else
                                {
                                    ViewBag.Message = "Name is already exists!";
                                    Session["Message"] = ViewBag.Message;
                                    Session["error"] = ViewBag.Message;
                                }

                            }
                            else
                            {
                                ViewBag.Message = "Email is Incorrect!";
                                Session["Message"] = ViewBag.Message;
                                Session["error"] = ViewBag.Message;
                            }

                        }
                        else
                        {
                            ViewBag.Message = "Phone Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                            Session["Message"] = ViewBag.Message;
                            Session["error"] = ViewBag.Message;
                        }

                        //transaction = con.BeginTransaction();
                        try
                        {

                            if (propContactList.Count > 0)
                            {
                                foreach (var contact_Save in propContactList)
                                {

                                    if (USNumberValidator(contact_Save.PropContactNumber))
                                    {
                                        if (EmailValidator(contact_Save.PropContactEmail))
                                        {
                                            if (!DuplicateCommon("PropContact", "PropContactId", "PropContactFirstName", "PropertyId", contact_Save.PropContactFirstName, contact_Save.PropertyId.ToString()))
                                            {
                                                //SavePropContact(contact_Save);
                                            }
                                            else
                                            {
                                                ViewBag.Message = "First Name with same Company is already exists!";
                                                Session["Message"] = ViewBag.Message;
                                                Session["error"] = ViewBag.Message;
                                            }

                                        }
                                        else
                                        {
                                            ViewBag.Message = "Email is Incorrect!";
                                            Session["Message"] = ViewBag.Message;
                                            Session["error"] = ViewBag.Message;
                                        }

                                    }
                                    else
                                    {
                                        ViewBag.Message = "Phone Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                                        Session["Message"] = ViewBag.Message;
                                        Session["error"] = ViewBag.Message;
                                    }
                                }
                            }
                            if (Session["error"] != null)
                            {

                            }
                            else
                            {
                                object id = cmd.ExecuteScalar();
                                //transaction.Commit();
                                Int64 PropertyId = Int64.Parse(id.ToString());
                                SavePropContact(propContactList, PropertyId);
                                Session["error"] = null;
                                Session["SuccessMessage"] = "Success: Property Successfully Added";
                            }




                        }
                        catch (SqlException e)
                        {

                            ViewBag.error = "Transaction Failure";
                            Session["error"] = ViewBag.error;
                            Session["Message"] = e.Message;
                        }

                    }
                    else
                    {
                        ViewBag.Message = "Phone Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                        Session["Message"] = ViewBag.Message;
                        Session["error"] = ViewBag.Message;
                    }
                }
                else
                {
                    if (USNumberValidator(Property_Save.PhoneNumber))
                    {
                        if (USNumberValidator(Property_Save.EmergencyPhoneNumber))
                        {
                            if (WebSiteValidator(Property_Save.WebSite))
                            {
                                cmd.CommandText = "PropertiesUpdate";
                                cmd.CommandType = CommandType.StoredProcedure;
                                //SqlTransaction transaction;
                                cmd.Parameters.AddWithValue("@AdminFee", Property_Save.AdminFee);
                                cmd.Parameters.AddWithValue("@PropertyId", Property_Save.PropertyId);
                                cmd.Parameters.AddWithValue("@ApplicationFee", Property_Save.ApplicationFee);
                                //cmd.Parameters.AddWithValue("@City", Property_Save.City);
                                cmd.Parameters.AddWithValue("@Area", Property_Save.Area);
                                cmd.Parameters.AddWithValue("@BusinessCenterImage", Property_Save.BusinessCenterImage);
                                cmd.Parameters.AddWithValue("@WeightLimit", Property_Save.WeightLimit);
                                cmd.Parameters.AddWithValue("@BreakLeasePolicy", Property_Save.BreakLeasePolicy);
                                cmd.Parameters.AddWithValue("@Building", Property_Save.Building);
                                cmd.Parameters.AddWithValue("@CleaningFee", Property_Save.CleaningFee);
                                cmd.Parameters.AddWithValue("@CreatedBy", Property_Save.CreatedBy);
                                cmd.Parameters.AddWithValue("@CreatedDatee", Property_Save.CreatedDatee);
                                cmd.Parameters.AddWithValue("@ElevatorFitnessImage", Property_Save.ElevatorFitnessImage);
                                cmd.Parameters.AddWithValue("@PetFee", Property_Save.PetFee);
                                cmd.Parameters.AddWithValue("@MaxNoOfPets", Property_Save.MaxNoOfPets);
                                cmd.Parameters.AddWithValue("@Leased", Property_Save.Leased);
                                cmd.Parameters.AddWithValue("@MailBoxImage", Property_Save.MailBoxImage);
                                cmd.Parameters.AddWithValue("@Floor", Property_Save.Floor);
                                cmd.Parameters.AddWithValue("@ParkingTypeImage", Property_Save.ParkingTypeImage);
                                cmd.Parameters.AddWithValue("@PetBreedRestrictions", Property_Save.PetBreedRestrictions);
                                cmd.Parameters.AddWithValue("@PoolImage", Property_Save.PoolImage);
                                cmd.Parameters.AddWithValue("@PropertyAddress", Property_Save.PropertyAddress);
                                cmd.Parameters.AddWithValue("@PropertyAddress2", Property_Save.PropertyAddress2);
                                cmd.Parameters.AddWithValue("@City", Property_Save.City);
                                cmd.Parameters.AddWithValue("@Zip", Property_Save.Zip);
                                cmd.Parameters.AddWithValue("@State", Property_Save.State);
                                cmd.Parameters.AddWithValue("@PropertyDescription", Property_Save.PropertyDescription);
                                cmd.Parameters.AddWithValue("@ValetTrash", Property_Save.ValetTrash);

                                cmd.Parameters.AddWithValue("@Amenities", Property_Save.Amenities);
                                cmd.Parameters.AddWithValue("@BusinessCenter", Property_Save.BusinessCenter);
                                cmd.Parameters.AddWithValue("@Elevator", Property_Save.Elevator);
                                cmd.Parameters.AddWithValue("@EmergencyPhoneNumber", Property_Save.EmergencyPhoneNumber);
                                cmd.Parameters.AddWithValue("@Features", Property_Save.Features);
                                cmd.Parameters.AddWithValue("@Fitness", Property_Save.Fitness);
                                cmd.Parameters.AddWithValue("@FloorPlanPic", Property_Save.FloorPlanPic);
                                cmd.Parameters.AddWithValue("@FloorPlanPic1", Property_Save.FloorPlanPic1);
                                cmd.Parameters.AddWithValue("@FloorPlanPic2", Property_Save.FloorPlanPic2);
                                cmd.Parameters.AddWithValue("@FloorPlanPic3", Property_Save.FloorPlanPic3);
                                cmd.Parameters.AddWithValue("@Hours", Property_Save.Hours);
                                cmd.Parameters.AddWithValue("@LeaseEndDate", Property_Save.LeaseEndDate);
                                cmd.Parameters.AddWithValue("@MailboxLocation", Property_Save.MailboxLocation);
                                cmd.Parameters.AddWithValue("@Name", Property_Save.Name);
                                cmd.Parameters.AddWithValue("@NoticetoVacate", Property_Save.NoticetoVacate);
                                cmd.Parameters.AddWithValue("@OtherDeposit", Property_Save.OtherDeposit);
                                cmd.Parameters.AddWithValue("@OtherDepositAmount", Property_Save.OtherDepositAmount);
                                cmd.Parameters.AddWithValue("@ParkingFee", Property_Save.ParkingFee);
                                cmd.Parameters.AddWithValue("@ParkingType", Property_Save.ParkingType);
                                cmd.Parameters.AddWithValue("@PhoneNumber", Property_Save.PhoneNumber);
                                cmd.Parameters.AddWithValue("@Pool", Property_Save.Pool);
                                //cmd.Parameters.AddWithValue("@Status", Property_Save.Status);
                                cmd.Parameters.AddWithValue("@UnitSize", Property_Save.UnitSize);
                                cmd.Parameters.AddWithValue("@UnitSquareFootage", Property_Save.UnitSquareFootage);
                                cmd.Parameters.AddWithValue("@UnitType", Property_Save.UnitType);
                                cmd.Parameters.AddWithValue("@VendorId", Property_Save.VendorId);
                                cmd.Parameters.AddWithValue("@VendorName", Property_Save.VendorName);
                                cmd.Parameters.AddWithValue("@WebSite", Property_Save.WebSite);

                                cmd.Parameters.AddWithValue("@CommunityFeatures", Property_Save.CommunityFeatures);
                                cmd.Parameters.AddWithValue("@UnitFeatures", Property_Save.UnitFeatures);

                                con.Open();
                            }
                            else
                            {
                                ViewBag.Message = "Name is already exists!";
                                Session["Message"] = ViewBag.Message;
                                Session["error"] = ViewBag.Message;
                            }

                        }
                        else
                        {
                            ViewBag.Message = "Email is Incorrect!";
                            Session["Message"] = ViewBag.Message;
                            Session["error"] = ViewBag.Message;
                        }

                    }
                    else
                    {
                        ViewBag.Message = "Phone Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                        Session["Message"] = ViewBag.Message;
                        Session["error"] = ViewBag.Message;
                    }


                    //transaction = con.BeginTransaction();
                    try
                    {
                        if (propContactList.Count > 0)
                        {
                            foreach (var contact_Save in propContactList)
                            {
                                if (USNumberValidator(contact_Save.PropContactNumber))
                                {
                                    if (EmailValidator(contact_Save.PropContactEmail))
                                    {
                                        if (!DuplicateCommon("PropContact", "PropContactId", "PropContactFirstName", "PropertyId", contact_Save.PropContactFirstName, contact_Save.PropertyId.ToString()))
                                        {
                                            //SavePropContact(contact_Save);  
                                        }
                                        else
                                        {
                                            ViewBag.Message = "First Name with same Company is already exists!";
                                            Session["Message"] = ViewBag.Message;
                                            Session["error"] = ViewBag.Message;
                                        }

                                    }
                                    else
                                    {
                                        ViewBag.Message = "Email is Incorrect!";
                                        Session["Message"] = ViewBag.Message;
                                        Session["error"] = ViewBag.Message;
                                    }

                                }
                                else
                                {
                                    ViewBag.Message = "Phone Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                                    Session["Message"] = ViewBag.Message;
                                    Session["error"] = ViewBag.Message;
                                }
                            }
                        }
                        if (Session["error"] != null)
                        {

                        }
                        else
                        {
                            cmd.ExecuteNonQuery();
                            //transaction.Commit();
                            Int64 PropertyId = Property_Save.PropertyId;
                            SavePropContact(propContactList, PropertyId);
                            Session["SuccessMessage"] = "Success: Property Successfully Updated";
                        }


                    }
                    catch (SqlException e)
                    {

                        ViewBag.error = "Transaction Failure";
                        Session["Message"] = e.Message;
                        //transaction.Rollback();
                    }
                }

            }
            con.Close();

            if (Session["error"] != null)
            {
                ViewData["Message"] = Session["error"];
                ViewBag.ContactType = DropDownListContantType("");
                ViewBag.LeaseTerm = DropDownListLeaseTerm("");
                ViewBag.City = DropDownListCity("");
                ViewBag.ReferelSource = DropDownListReferelSource("");
                ViewBag.PropContactList = BindDataPropContactAll(0);
                ViewBag.States = BindDataStatesAll(0);
                ViewBag.Elevator = DropDownListYesNo(model["Elevator"].ToString());
                ViewBag.Fitness = DropDownListYesNo(model["Fitness"].ToString());
                ViewBag.Pool = DropDownListYesNo(model["Pool"].ToString());
                ViewBag.BusinessCenter = DropDownListYesNo(model["BusinessCenter"].ToString());
                ViewBag.ParkingType = DropDownListParkingType(model["ParkingType"].ToString());

                ViewBag.LeaseEndDate = DropDownListLeaseEndDate(model["LeaseEndDate"].ToString());
                ViewBag.NoticetoVacate = DropDownListNoticeToVacate(model["NoticetoVacate"].ToString());
                ViewBag.OtherDeposit = DropDownListOtherDeposit(model["OtherDeposit"].ToString());
                ViewBag.UnitType = DropDownListUnitType(model["UnitType"].ToString());
                ViewBag.Vendor = BindDataVendorAll("").Where(x => x.VendorType == "Management Company").Where(x => x.IsActive = true);

                ViewBag.CommunityFeatures = CheckBoxListCommunityFeatures(model["CommunityFeatures"].ToString());
                ViewBag.UnitFeatures = CheckBoxListUnitFeatures(model["UnitFeatures"].ToString());


                return View(Property_Save);
            }
            else
            {
                ViewData["SuccessMessage"] = Session["SuccessMessage"];
                return RedirectToAction("Properties");
            }

        }

        public ActionResult DeleteProperty(decimal id = 0)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"PropertiesDelete";

                cmd.Parameters.AddWithValue("@PropertyId", id);

                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Property Deleted Completely Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Properties");
        }

        /// <summary>
        /// ///Quotes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult Quotes(decimal id = 0, string srchContactName = "", string srchOccupantName = "")
        {
            ///Quote All
            /////////
            //string connect = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
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

            return View(BindDataQuoteAll(0));
        }

        public ActionResult AddQuote(Int64 Id = 0, Int64 PropertyId = 0)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("QuotesSelect", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuoteId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Quote Quote_Single = new Quote();
            Quote Quote_Detail = new Quote();
            JsonResult jR = new JsonResult();
            List<Quote> Quote_List = new List<Quote>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dt.Rows.Count > 0)
            {


                //Quote_Single.Address = dt.Rows[0]["Address"].ToString();
                Quote_Single.OneTimeFurnitureDeliveryFee = decimal.Parse(dt.Rows[0]["OneTimeFurnitureDeliveryFee"].ToString());
                Quote_Single.QuoteId = Int64.Parse(dt.Rows[0]["QuoteId"].ToString());
                Quote_Single.KeyID = Int64.Parse(dt.Rows[0]["KeyID"].ToString());
                Quote_Single.CreditCard = dt.Rows[0]["CreditCard"].ToString();
                Quote_Single.MonthlyCableFee = decimal.Parse(dt.Rows[0]["MonthlyCableFee"].ToString());
                Quote_Single.MonthlyFurnitureUsageFee = decimal.Parse(dt.Rows[0]["MonthlyFurnitureUsageFee"].ToString());
                Quote_Single.LeadsId = Int64.Parse(dt.Rows[0]["LeadsId"].ToString());
                Quote_Single.LeaseEndDate = DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString());
                Quote_Single.LeaseStartDate = DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString());
                Quote_Single.MonthlyCableFee = decimal.Parse(dt.Rows[0]["MonthlyCableFee"].ToString());
                Quote_Single.MonthlyCourierFee = decimal.Parse(dt.Rows[0]["MonthlyCourierFee"].ToString());
                Quote_Single.MonthlyElectricFee = decimal.Parse(dt.Rows[0]["MonthlyElectricFee"].ToString());
                Quote_Single.MonthlyFridgeFee = decimal.Parse(dt.Rows[0]["MonthlyFridgeFee"].ToString());
                Quote_Single.MonthlyFurnitureUsageFee = decimal.Parse(dt.Rows[0]["MonthlyFurnitureUsageFee"].ToString());
                Quote_Single.MonthlyGasFee = decimal.Parse(dt.Rows[0]["MonthlyGasFee"].ToString());
                Quote_Single.MonthlyHouseWaversFee = decimal.Parse(dt.Rows[0]["MonthlyHouseWaversFee"].ToString());
                Quote_Single.MonthlyInternetFee = decimal.Parse(dt.Rows[0]["MonthlyInternetFee"].ToString());
                Quote_Single.MonthlyMarketingFee = decimal.Parse(dt.Rows[0]["MonthlyMarketingFee"].ToString());
                Quote_Single.MonthlyMicrowaveFee = decimal.Parse(dt.Rows[0]["MonthlyMicrowaveFee"].ToString());
                Quote_Single.MonthlyPetRentFee = decimal.Parse(dt.Rows[0]["MonthlyPetRentFee"].ToString());

                Quote_Single.MonthlyPropertyRent = decimal.Parse(dt.Rows[0]["MonthlyPropertyRent"].ToString());
                Quote_Single.MonthlyReferalFee = decimal.Parse(dt.Rows[0]["MonthlyReferalFee"].ToString());
                Quote_Single.MonthlyValetTrashFee = decimal.Parse(dt.Rows[0]["MonthlyValetTrashFee"].ToString());
                Quote_Single.MonthlyWasherDrayerFee = decimal.Parse(dt.Rows[0]["MonthlyWasherDrayerFee"].ToString());

                Quote_Single.MonthlyWaterSewerTrashFee = decimal.Parse(dt.Rows[0]["MonthlyWaterSewerTrashFee"].ToString());
                Quote_Single.OneTimeAdminFee = decimal.Parse(dt.Rows[0]["OneTimeAdminFee"].ToString());
                Quote_Single.OneTimeAmnityFee = decimal.Parse(dt.Rows[0]["OneTimeAmnityFee"].ToString());
                Quote_Single.OneTimeHouseWaversSetupFee = decimal.Parse(dt.Rows[0]["OneTimeHouseWaversSetupFee"].ToString());

                Quote_Single.ParkingPlaces = Int16.Parse(dt.Rows[0]["ParkingPlaces"].ToString());
                Quote_Single.ParkingType = dt.Rows[0]["ParkingType"].ToString();
                Quote_Single.PropertyId = Int64.Parse(dt.Rows[0]["PropertyId"].ToString());
                Quote_Single.CreditCard = dt.Rows[0]["CreditCard"].ToString();
                Quote_Single.MonthlyWasherDrayerType = dt.Rows[0]["MonthlyWasherDrayerType"].ToString();
                Quote_Single.ClientEndDate = DateTime.Parse(dt.Rows[0]["ClientEndDate"].ToString());
                Quote_Single.ClientStartDate = DateTime.Parse(dt.Rows[0]["ClientStartDate"].ToString());
                Quote_Single.IsActive = bool.Parse(dt.Rows[0]["IsActive"].ToString());
                Quote_Single.MonthlyBreakLeaseFee = Int64.Parse(dt.Rows[0]["MonthlyBreakLeaseFee"].ToString());
                Quote_Single.MonthlyInsuranceBlanketFee = Int64.Parse(dt.Rows[0]["MonthlyInsuranceBlanketFee"].ToString());
                Quote_Single.MonthlyKSProfitFee = Int64.Parse(dt.Rows[0]["MonthlyKSProfitFee"].ToString());
                Quote_Single.MonthlyParcelServicePropertyFee = Int64.Parse(dt.Rows[0]["MonthlyParcelServicePropertyFee"].ToString());
                Quote_Single.MonthlyParkingPlacesFee = Int64.Parse(dt.Rows[0]["MonthlyParkingPlacesFee"].ToString());
                Quote_Single.OneTimeCable = Int64.Parse(dt.Rows[0]["OneTimeCable"].ToString());
                Quote_Single.OneTimeCleaning = Int64.Parse(dt.Rows[0]["OneTimeCleaning"].ToString());
                Quote_Single.OneTimeElectric = Int64.Parse(dt.Rows[0]["OneTimeElectric"].ToString());
                Quote_Single.OneTimeGas = Int64.Parse(dt.Rows[0]["OneTimeGas"].ToString());
                Quote_Single.OneTimeGiftBasket = Int64.Parse(dt.Rows[0]["OneTimeGiftBasket"].ToString());
                Quote_Single.OneTimeInspection = Int64.Parse(dt.Rows[0]["OneTimeInspection"].ToString());
                Quote_Single.OneTimeInternet = Int64.Parse(dt.Rows[0]["OneTimeInternet"].ToString());
                Quote_Single.OneTimeKSAdminfee = Int64.Parse(dt.Rows[0]["OneTimeKSAdminfee"].ToString());
                Quote_Single.OneTimeKSAppFee = Int64.Parse(dt.Rows[0]["OneTimeKSAppFee"].ToString());
                Quote_Single.OneTimeKSPetDep = Int64.Parse(dt.Rows[0]["OneTimeKSPetDep"].ToString());
                Quote_Single.OneTimeKSPetFee = Int64.Parse(dt.Rows[0]["OneTimeKSPetFee"].ToString());
                Quote_Single.OneTimeKSSecDep = Int64.Parse(dt.Rows[0]["OneTimeKSSecDep"].ToString());
                Quote_Single.OneTimeNonRefFees = Int64.Parse(dt.Rows[0]["OneTimeNonRefFees"].ToString());
                Quote_Single.OneTimeOccupantBackgroundcheck = Int64.Parse(dt.Rows[0]["OneTimeOccupantBackgroundcheck"].ToString());
                Quote_Single.OneTimePropertyCorporateApplicationFee = Int64.Parse(dt.Rows[0]["OneTimePropertyCorporateApplicationFee"].ToString());
                Quote_Single.OneTimePropHoldFees = Int64.Parse(dt.Rows[0]["OneTimePropHoldFees"].ToString());
                Quote_Single.OneTimePropPetDep = Int64.Parse(dt.Rows[0]["OneTimePropPetDep"].ToString());
                Quote_Single.OneTimePropPetFee = Int64.Parse(dt.Rows[0]["OneTimePropPetFee"].ToString());
                Quote_Single.OneTimePropSecDep = Int64.Parse(dt.Rows[0]["OneTimePropSecDep"].ToString());
                Quote_Single.OneTimeRefKSDep = Int64.Parse(dt.Rows[0]["OneTimeRefKSDep"].ToString());
                Quote_Single.OneTimeRefundablePropFees = Int64.Parse(dt.Rows[0]["OneTimeRefundablePropFees"].ToString());
                Quote_Single.OneTimeRemoteFOBKeyCard = Int64.Parse(dt.Rows[0]["OneTimeRemoteFOBKeyCard"].ToString());
                Quote_Single.OneTimeSureDeposit = Int64.Parse(dt.Rows[0]["OneTimeSureDeposit"].ToString());
                Quote_Single.OneTimeTrash = Int64.Parse(dt.Rows[0]["OneTimeTrash"].ToString());
                Quote_Single.OneTimeWater = Int64.Parse(dt.Rows[0]["OneTimeWater"].ToString());
                Quote_Single.Notes = dt.Rows[0]["Notes"].ToString();
                Quote_Single.PropertyEndDate = DateTime.Parse(dt.Rows[0]["PropertyEndDate"].ToString());
                Quote_Single.PropertyStartDate = DateTime.Parse(dt.Rows[0]["PropertyStartDate"].ToString());
                Quote_Single.TotalStay = Int64.Parse(dt.Rows[0]["TotalStay"].ToString());
                Quote_Single.Vacancy = dt.Rows[0]["Vacancy"].ToString();

                Quote_Single.MonthlyCable = dt.Rows[0]["MonthlyCable"].ToString();
                Quote_Single.MonthlyFridge = dt.Rows[0]["MonthlyFridge"].ToString();
                Quote_Single.MonthlyElectric = dt.Rows[0]["MonthlyElectric"].ToString();
                Quote_Single.MonthlyWaterSewerTrash = dt.Rows[0]["MonthlyWaterSewerTrash"].ToString();
                Quote_Single.MonthlyWasherDrayer = dt.Rows[0]["MonthlyWasherDrayer"].ToString();
                Quote_Single.MonthlyValetTrash = dt.Rows[0]["MonthlyValetTrash"].ToString();
                Quote_Single.MonthlyInternet = dt.Rows[0]["MonthlyInternet"].ToString();
                Quote_Single.OneTimeInspectionName = dt.Rows[0]["OneTimeInspectionName"].ToString();
                Quote_Single.MonthlyFurniture = dt.Rows[0]["MonthlyFurniture"].ToString();
                Quote_Single.MonthlyGas = dt.Rows[0]["MonthlyGas"].ToString();
                Quote_Single.MonthlyHouseWavers = dt.Rows[0]["MonthlyHouseWavers"].ToString();
                Quote_Single.MonthlyMicrowave = dt.Rows[0]["MonthlyMicrowave"].ToString();
                Quote_Single.OneTimeCleaningName = dt.Rows[0]["OneTimeCleaningName"].ToString();

                Quote_Single.OneTimeOtherName = dt.Rows[0]["OneTimeOtherName"].ToString();
                Quote_Single.OneTimeOther = decimal.Parse(dt.Rows[0]["OneTimeOther"].ToString());
                Quote_Single.MonthlyOtherName = dt.Rows[0]["MonthlyOtherName"].ToString();
                Quote_Single.MonthlyOther = decimal.Parse(dt.Rows[0]["MonthlyOther"].ToString());


                Quote_Single.TotalMonthly = decimal.Parse(dt.Rows[0]["TotalMonthly"].ToString());
                Quote_Single.TotalOneTime = decimal.Parse(dt.Rows[0]["TotalOneTime"].ToString());
                Quote_Single.TotalMonthlyCCost = decimal.Parse(dt.Rows[0]["TotalMonthlyCCost"].ToString());
                Quote_Single.DailyCash = decimal.Parse(dt.Rows[0]["DailyCash"].ToString());
                Quote_Single.DailyCredit = decimal.Parse(dt.Rows[0]["DailyCredit"].ToString());
                Quote_Single.Charges = decimal.Parse(dt.Rows[0]["Charges"].ToString());
                Quote_Single.RefFinalAmount = decimal.Parse(dt.Rows[0]["RefFinalAmount"].ToString());

                Quote_Single.property = BindDataPropertyAll(0).Where(m => m.PropertyId == Int64.Parse(dt.Rows[0]["PropertyId"].ToString())).FirstOrDefault();
                Quote_Single.lead = BindDataLeadAll(0).Where(m => m.LeadsId == Int64.Parse(dt.Rows[0]["LeadsId"].ToString())).FirstOrDefault();
                Quote_Single.lead.referalSource = BindDataReferalAllWithNoReferral(0).Where(m => m.CompanyName == Quote_Single.lead.ReferelSource).FirstOrDefault();

                //For Lead Fill

                //List<Lead> Lead_List = new List<Lead>();
                //Lead Lead_One = new Lead();
                //Lead_List = BindDataLeadAll(0);
                //Lead_List = Lead_List.Where(x => x.LeadsId == Int64.Parse( dt.Rows[0]["LeadsId"].ToString())).ToList();
                //if (Lead_List.Count() > 0)
                //{
                //    Lead_One = Lead_List[0];
                //    Quote_Single.lead.OcupantName = Lead_One.OcupantName;
                //    Quote_Single.lead.PreferedAddress = Lead_One.PreferedAddress;
                //    Quote_Single.lead.MoveInDate = Lead_One.MoveInDate;

                //}
                ////For Property Fill
                //List<Property> Property_List = new List<Property>();
                //Property_List = BindDataPropertyAll(0);
                //Property Property_One = new Property();
                //Property_List = Property_List.Where(x => x.PropertyId == Int64.Parse( dt.Rows[0]["PropertyId"].ToString())).ToList();
                //if (Property_List.Count() > 0)
                //{
                //    Property_One = Property_List[0];
                //    Quote_Single.property.PropertyDescription = Property_One.PropertyDescription;
                //    Quote_Single.property.PropertyAddress = Property_One.PropertyAddress;
                //    Quote_Single.property.Status = Property_One.Status;
                //}

                //ViewBag.ContactType = DropDownListContantType(dt.Rows[0]["ContactType"].ToString());
                //ViewBag.LeaseTerm = DropDownListLeaseTerm(dt.Rows[0]["LeaseTerm"].ToString());
                //ViewBag.City = DropDownListCity(dt.Rows[0]["City"].ToString());
                //ViewBag.ReferelSource = DropDownListReferelSource(dt.Rows[0]["ReferelSource"].ToString());
                ViewBag.Property = BindDataPropertyAll(0).Where(y => y.IsActive = true);
                ViewBag.WasherDryerType = DropDownListWasherDryerType(dt.Rows[0]["MonthlyWasherDrayerType"].ToString());
                List<Lead> lead_list = BindDataLeadAll(0).Where(x => x.IsActive = true).ToList();
                List<ReferalSource> referal_list = BindDataReferalAllWithNoReferral(0);
                ReferalSource referal_Single = new ReferalSource();
                Lead Lead_Single = new Lead();
                if (lead_list.Count > 0)
                {
                    if (referal_list.Count > 0)
                    {
                        foreach (var lead in lead_list)
                        {
                            var adress = referal_list.Where(x => x.CompanyName == lead.ReferelSource).First().Address;
                            var reflist = referal_list.Where(x => x.CompanyName == lead.ReferelSource);
                            var refone = referal_list.Where(x => x.CompanyName == lead.ReferelSource).First();
                            if (referal_list.Where(x => x.CompanyName == lead.ReferelSource).First().Address == null)
                            {
                                referal_Single.Address = "";
                            }
                            else
                            {
                                referal_Single.Address = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().Address;
                            }

                            if (referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().CostPerDay == null)
                            {
                                referal_Single.CostPerDay = 0;
                            }
                            else
                            {
                                referal_Single.CostPerDay = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().CostPerDay;
                            }

                            if (referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().Number == null)
                            {
                                referal_Single.Number = 0;
                            }
                            else
                            {
                                referal_Single.Number = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().Number;
                            }

                            referal_Single.ReferalType = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().ReferalType;
                            referal_Single.ReferalSourceId = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().ReferalSourceId;
                            lead.referalSource = referal_Single;
                            referal_Single = new ReferalSource();
                        }
                    }
                    else
                    {
                        foreach (var lead in lead_list)
                        {
                            //var adress = referal_list.Where(x => x.CompanyName == lead.ReferelSource).First().Address;
                            //var reflist = referal_list.Where(x => x.CompanyName == lead.ReferelSource);
                            //var refone = referal_list.Where(x => x.CompanyName == lead.ReferelSource).First();

                            referal_Single.Address = "";

                            referal_Single.CostPerDay = 0;

                            referal_Single.Number = 0;



                            referal_Single.ReferalType = "$";
                            referal_Single.ReferalSourceId = 0;
                            lead.referalSource = referal_Single;
                            referal_Single = new ReferalSource();
                        }
                    }

                }
                else
                {
                    Lead_Single = new Lead()
                    {
                        Elevator = "",
                        PreferedAddress = "",
                        IsActive = true,
                        CompanyContactNumber = "",
                        ContactName = "none",
                        ContactNumberCompany = "none",
                        LeadsId = 0,
                        ContactEmail = "",
                        Breed = "",
                        City = "",
                        ContactNumber = "",
                        ContactType = "",
                        FloorPreference = "",
                        LeadsName = "",
                        LeaseTerm = "",
                        Address = "",
                        ContactInfoId = 0,
                        MoveInDate = DateTime.Now,
                        NoOfAdults = "none",
                        NoOfBedRooms = "Studio",
                        NoOfChildren = "none",
                        NoOfPets = "none",
                        Notes = "",
                        OcupantName = "none",
                        PreferedArea = "",
                        ReferelSource = "none",
                        State = "",
                        Weight = 0,
                        Zip = 75581,
                        CompanyLogo = "",
                        Address2 = "",
                        OccupantCity = "Houston",
                        OccupantState = "TX",
                        OccupantZip = 75581,


                    };

                    referal_Single.Address = "";

                    referal_Single.CostPerDay = 0;

                    referal_Single.Number = 0;



                    referal_Single.ReferalType = "$";
                    referal_Single.ReferalSourceId = 0;
                    Lead_Single.referalSource = referal_Single;

                    lead_list.Add(Lead_Single);
                }

                //ViewBag.Lead = BindDataLeadAll(0);
                ViewBag.Lead = lead_list;
                //List l = DropDownListParkingType(dt.Rows[0]["ParkingType"].ToString());
                ViewBag.ParkingType = DropDownListParkingType(dt.Rows[0]["ParkingType"].ToString()).Where(m => m.Value != "Other");
                ViewBag.CreditCard = DropDownListCreditCardFee(dt.Rows[0]["CreditCard"].ToString());
                ViewBag.Vendor = BindDataVendorAll("").Where(y => y.IsActive = true);
            }
            else
            {
                System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
                Quote_Single = new Quote()
                {
                    PropertyId = PropertyId,
                    QuoteId = 0,
                    KeyID = 0,
                    CreditCard = "0",
                    ParkingType = "",
                    ParkingPlaces = 0,
                    OneTimeHouseWaversSetupFee = 0,
                    OneTimeAmnityFee = 0,
                    OneTimeAdminFee = 0,
                    OneTimeFurnitureDeliveryFee = 0,
                    LeadsId = Id,
                    LeaseEndDate = DateTime.Now,
                    LeaseStartDate = DateTime.Now,
                    MonthlyCableFee = 100,
                    MonthlyCourierFee = 30,
                    MonthlyElectricFee = 200,
                    MonthlyFridgeFee = 0,
                    MonthlyFurnitureUsageFee = 0,
                    MonthlyGasFee = 30,
                    MonthlyHouseWaversFee = 0,
                    MonthlyInternetFee = 100,
                    MonthlyMarketingFee = 50,
                    MonthlyMicrowaveFee = 0,
                    MonthlyPetRentFee = 0,
                    MonthlyPropertyRent = 0,
                    MonthlyReferalFee = 0,
                    MonthlyValetTrashFee = 0,
                    MonthlyWasherDrayerFee = 0,
                    MonthlyWaterSewerTrashFee = 30,
                    VacancyDays = 0,

                    ClientEndDate = DateTime.Now,
                    ClientStartDate = DateTime.Now,
                    IsActive = true,
                    MonthlyBreakLeaseFee = 0,
                    MonthlyInsuranceBlanketFee = 10,
                    MonthlyKSProfitFee = 0,
                    MonthlyParcelServicePropertyFee = 20,
                    MonthlyParkingPlacesFee = 0,
                    OneTimeCable = 50,
                    OneTimeCleaning = 0,
                    OneTimeElectric = 50,
                    OneTimeGas = 50,
                    OneTimeGiftBasket = 0,
                    OneTimeInspection = 0,
                    OneTimeInternet = 50,
                    OneTimeKSAdminfee = 0,
                    OneTimeKSAppFee = 0,
                    OneTimeKSPetDep = 0,
                    OneTimeKSPetFee = 0,
                    OneTimeKSSecDep = 0,
                    OneTimeNonRefFees = 0,
                    OneTimeOccupantBackgroundcheck = 0,
                    OneTimePropertyCorporateApplicationFee = 0,
                    OneTimePropHoldFees = 0,
                    OneTimePropPetDep = 0,
                    OneTimePropPetFee = 0,
                    OneTimePropSecDep = 0,
                    OneTimeRefKSDep = 0,
                    OneTimeRefundablePropFees = 0,
                    OneTimeRemoteFOBKeyCard = 0,
                    OneTimeSureDeposit = 0,
                    OneTimeTrash = 50,
                    OneTimeWater = 50,
                    Notes = "",
                    PropertyEndDate = DateTime.Now,
                    PropertyStartDate = DateTime.Now,
                    TotalStay = 30,
                    Vacancy = "Available",
                    MonthlyCable = "",
                    MonthlyFridge = "",
                    MonthlyElectric = "",
                    MonthlyWaterSewerTrash = "",
                    MonthlyWasherDrayer = "",
                    MonthlyValetTrash = "",
                    MonthlyInternet = "",
                    OneTimeInspectionName = "",
                    MonthlyFurniture = "",
                    MonthlyGas = "",
                    MonthlyHouseWavers = "",
                    MonthlyMicrowave = "",
                    OneTimeCleaningName = "",
                    RefFinalAmount = 0,
                    Charges = 0,
                    DailyCredit = 0,
                    DailyCash = 0,
                    TotalMonthlyCCost = 0,
                    TotalOneTime = 0,
                    TotalMonthly = 0,
                    MonthlyOther = 0,
                    OneTimeOther = 0,
                    OneTimeOtherName = "",
                    MonthlyOtherName = "Other Fee",
                    MonthlyWasherDrayerType = "Full Size",
                    lead = new Lead(),
                    referalSource = new ReferalSource() { Number = 0 },




                };
                //ViewBag.ContactType = DropDownListContantType("");
                //ViewBag.LeaseTerm = DropDownListLeaseTerm("");
                //ViewBag.City = DropDownListCity("");
                //ViewBag.ReferelSource = DropDownListReferelSource("");
                ViewBag.Property = BindDataPropertyAll(0).Where(y => y.IsActive = true);
                ViewBag.WasherDryerType = DropDownListWasherDryerType("");


                List<Lead> lead_list = BindDataLeadAll(0).Where(y => y.IsActive = true).ToList();

                ///No Where clause IsActive True Because its for Leads. 
                List<ReferalSource> referal_list = BindDataReferalAllWithNoReferral(0);
                ReferalSource referal_Single = new ReferalSource();
                foreach (var lead in lead_list)
                {
                    var adress = referal_list.Where(x => x.CompanyName == lead.ReferelSource).First().Address;
                    var reflist = referal_list.Where(x => x.CompanyName == lead.ReferelSource);
                    var refone = referal_list.Where(x => x.CompanyName == lead.ReferelSource).First();
                    if (referal_list.Where(x => x.CompanyName == lead.ReferelSource).First().Address == null)
                    {
                        referal_Single.Address = "";
                    }
                    else
                    {
                        referal_Single.Address = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().Address;
                    }

                    if (referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().CostPerDay == null)
                    {
                        referal_Single.CostPerDay = 0;
                    }
                    else
                    {
                        referal_Single.CostPerDay = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().CostPerDay;
                    }

                    if (referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().Number == null)
                    {
                        referal_Single.Number = 0;
                    }
                    else
                    {
                        referal_Single.Number = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().Number;
                    }

                    referal_Single.ReferalType = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().ReferalType;
                    referal_Single.ReferalSourceId = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().ReferalSourceId;
                    lead.referalSource = referal_Single;

                    referal_Single = new ReferalSource();
                }
                //ViewBag.Lead = BindDataLeadAll(0);
                ViewBag.Lead = lead_list;
                ViewBag.ParkingType = DropDownListParkingType("").Where(m => m.Value != "Other");
                ViewBag.CreditCard = DropDownListCreditCardFee("");

                ViewBag.ContactType = DropDownListContantType("");
                ViewBag.LeaseTerm = DropDownListLeaseTerm("");
                ViewBag.City = DropDownListCity("");
                ViewBag.ReferelSource = DropDownListReferelSource("");
                ViewBag.Vendor = BindDataVendorAll("").Where(y => y.IsActive = true);
            }



            return View(Quote_Single);
        }

        [HttpPost]
        public ActionResult AddQuote(FormCollection model)
        {
            Session["Message"] = "";
            Session["SuccessMessage"] = "";

            Session["error"] = null;
            Quote Quote_Save = new Quote();

            Quote_Save.QuoteId = Int64.Parse(model["QuoteId"]);


            Quote_Save.OneTimeFurnitureDeliveryFee = decimal.Parse(model["OneTimeFurnitureDeliveryFee"].ToString());
            Quote_Save.OneTimePropertyCorporateApplicationFee = decimal.Parse(model["OneTimePropertyCorporateApplicationFee"].ToString());
            Quote_Save.QuoteId = Int64.Parse(model["QuoteId"].ToString());
            Quote_Save.KeyID = GenerateKeyID("Quotes", "QuoteId");
            Quote_Save.CreditCard = "0";
            Quote_Save.MonthlyCableFee = decimal.Parse(model["MonthlyCableFee"].ToString());
            Quote_Save.MonthlyFurnitureUsageFee = decimal.Parse(model["MonthlyFurnitureUsageFee"].ToString());
            Quote_Save.LeadsId = Int64.Parse(model["LeadsId"].ToString());
            Quote_Save.CreditCard = model["CreditCard"].ToString();
            //Quote_Save.LeaseEndDate = DateTime.Parse(model["LeaseEndDate"].ToString());
            //try
            //{

            //    Quote_Save.LeaseEndDate = DateTime.Parse(model["LeaseEndDate"]);

            //}
            //catch (Exception ex)
            //{

            //    string date = model["LeaseEndDate"].ToString();
            //    string mon = date.Substring(0, 2);
            //    string da = date.Substring(3, 2);
            //    string ye = date.Substring(6, 4);
            //    //date = da + "/" + mon + "/" + ye;
            //    System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            //    Quote_Save.LeaseEndDate = DateTime.ParseExact(date, "MM/dd/yyyy", provider);

            //}
            //Quote_Save.LeaseStartDate = DateTime.Parse(model["LeaseStartDate"].ToString());
            try
            {

                Quote_Save.LeaseStartDate = DateTime.Parse(model["LeaseStartDate"]);

            }
            catch (Exception ex)
            {

                string date = model["LeaseStartDate"].ToString();
                string mon = date.Substring(0, 2);
                string da = date.Substring(3, 2);
                string ye = date.Substring(6, 4);
                //date = da + "/" + mon + "/" + ye;
                System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
                Quote_Save.LeaseStartDate = DateTime.ParseExact(date, "MM/dd/yyyy", provider);

            }
            Quote_Save.MonthlyCableFee = decimal.Parse(model["MonthlyCableFee"].ToString());
            Quote_Save.MonthlyCourierFee = decimal.Parse(model["MonthlyCourierFee"].ToString());
            Quote_Save.MonthlyElectricFee = decimal.Parse(model["MonthlyElectricFee"].ToString());
            Quote_Save.MonthlyFridgeFee = decimal.Parse(model["MonthlyFridgeFee"].ToString());
            Quote_Save.MonthlyFurnitureUsageFee = decimal.Parse(model["MonthlyFurnitureUsageFee"].ToString());
            Quote_Save.MonthlyGasFee = decimal.Parse(model["MonthlyGasFee"].ToString());
            Quote_Save.MonthlyHouseWaversFee = decimal.Parse(model["MonthlyHouseWaversFee"].ToString());
            Quote_Save.MonthlyInternetFee = decimal.Parse(model["MonthlyInternetFee"].ToString());
            Quote_Save.MonthlyMarketingFee = decimal.Parse(model["MonthlyMarketingFee"].ToString());
            Quote_Save.MonthlyMicrowaveFee = decimal.Parse(model["MonthlyMicrowaveFee"].ToString());
            Quote_Save.MonthlyPetRentFee = decimal.Parse(model["MonthlyPetRentFee"].ToString());

            Quote_Save.MonthlyPropertyRent = decimal.Parse(model["MonthlyPropertyRent"].ToString());
            Quote_Save.MonthlyReferalFee = decimal.Parse(model["MonthlyReferalFee"].ToString());
            Quote_Save.MonthlyValetTrashFee = decimal.Parse(model["MonthlyValetTrashFee"].ToString());
            Quote_Save.MonthlyWasherDrayerFee = decimal.Parse(model["MonthlyWasherDrayerFee"].ToString());

            Quote_Save.MonthlyWaterSewerTrashFee = decimal.Parse(model["MonthlyWaterSewerTrashFee"].ToString());
            Quote_Save.OneTimeAdminFee = decimal.Parse(model["OneTimeAdminFee"].ToString());
            Quote_Save.OneTimeAmnityFee = decimal.Parse(model["OneTimeAmnityFee"].ToString());
            Quote_Save.OneTimeHouseWaversSetupFee = decimal.Parse(model["OneTimeHouseWaversSetupFee"].ToString());

            //Quote_Save.ParkingPlaces = Int16.Parse(model["ParkingPlaces"].ToString());
            Quote_Save.ParkingType = model["ParkingType"].ToString();
            Quote_Save.PropertyId = int.Parse(model["PropertyId"].ToString());
            //Quote_Save.VacancyDays = int.Parse(model["VacancyDays"].ToString());


            //Quote_Save.ClientEndDate = DateTime.Parse(model["ClientEndDate"].ToString());
            try
            {

                Quote_Save.ClientEndDate = DateTime.Parse(model["ClientEndDate"]);

            }
            catch (Exception ex)
            {

                string date = model["ClientEndDate"].ToString();
                string mon = date.Substring(0, 2);
                string da = date.Substring(3, 2);
                string ye = date.Substring(6, 4);
                //date = da + "/" + mon + "/" + ye;
                System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
                Quote_Save.ClientEndDate = DateTime.ParseExact(date, "MM/dd/yyyy", provider);

            }
            //Quote_Save.ClientStartDate = DateTime.Parse(model["ClientStartDate"].ToString());
            try
            {

                Quote_Save.ClientStartDate = DateTime.Parse(model["ClientStartDate"]);

            }
            catch (Exception ex)
            {

                string date = model["ClientStartDate"].ToString();
                string mon = date.Substring(0, 2);
                string da = date.Substring(3, 2);
                string ye = date.Substring(6, 4);
                //date = da + "/" + mon + "/" + ye;
                System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
                Quote_Save.ClientStartDate = DateTime.ParseExact(date, "MM/dd/yyyy", provider);

            }
            //Quote_Save.IsActive = bool.Parse(model["IsActive"].ToString());
            Quote_Save.MonthlyBreakLeaseFee = Int64.Parse(model["MonthlyBreakLeaseFee"].ToString());
            Quote_Save.MonthlyInsuranceBlanketFee = Int64.Parse(model["MonthlyInsuranceBlanketFee"].ToString());
            Quote_Save.MonthlyKSProfitFee = decimal.Parse(model["MonthlyKSProfitFee"].ToString());
            Quote_Save.MonthlyParcelServicePropertyFee = Int64.Parse(model["MonthlyParcelServicePropertyFee"].ToString());
            Quote_Save.MonthlyParkingPlacesFee = Int64.Parse(model["MonthlyParkingPlacesFee"].ToString());
            Quote_Save.OneTimeCable = Int64.Parse(model["OneTimeCable"].ToString());
            Quote_Save.OneTimeCleaning = Int64.Parse(model["OneTimeCleaning"].ToString());
            Quote_Save.OneTimeElectric = Int64.Parse(model["OneTimeElectric"].ToString());
            Quote_Save.OneTimeGas = Int64.Parse(model["OneTimeGas"].ToString());
            Quote_Save.OneTimeGiftBasket = Int64.Parse(model["OneTimeGiftBasket"].ToString());
            Quote_Save.OneTimeInspection = Int64.Parse(model["OneTimeInspection"].ToString());
            Quote_Save.OneTimeInternet = Int64.Parse(model["OneTimeInternet"].ToString());
            Quote_Save.OneTimeKSAdminfee = Int64.Parse(model["OneTimeKSAdminfee"].ToString());
            Quote_Save.OneTimeKSAppFee = Int64.Parse(model["OneTimeKSAppFee"].ToString());
            Quote_Save.OneTimeKSPetDep = Int64.Parse(model["OneTimeKSPetDep"].ToString());
            Quote_Save.OneTimeKSPetFee = Int64.Parse(model["OneTimeKSPetFee"].ToString());
            Quote_Save.OneTimeKSSecDep = Int64.Parse(model["OneTimeKSSecDep"].ToString());
            Quote_Save.OneTimeNonRefFees = Int64.Parse(model["OneTimeNonRefFees"].ToString());
            Quote_Save.OneTimeOccupantBackgroundcheck = Int64.Parse(model["OneTimeOccupantBackgroundcheck"].ToString());
            Quote_Save.OneTimePropHoldFees = Int64.Parse(model["OneTimePropHoldFees"].ToString());
            Quote_Save.OneTimePropPetDep = Int64.Parse(model["OneTimePropPetDep"].ToString());
            Quote_Save.OneTimePropPetFee = Int64.Parse(model["OneTimePropPetFee"].ToString());
            Quote_Save.OneTimePropSecDep = Int64.Parse(model["OneTimePropSecDep"].ToString());
            Quote_Save.OneTimeRefKSDep = Int64.Parse(model["OneTimeRefKSDep"].ToString());
            Quote_Save.OneTimeRefundablePropFees = Int64.Parse(model["OneTimeRefundablePropFees"].ToString());
            Quote_Save.OneTimeRemoteFOBKeyCard = Int64.Parse(model["OneTimeRemoteFOBKeyCard"].ToString());
            Quote_Save.OneTimeSureDeposit = Int64.Parse(model["OneTimeSureDeposit"].ToString());
            Quote_Save.OneTimeTrash = Int64.Parse(model["OneTimeTrash"].ToString());
            Quote_Save.OneTimeWater = Int64.Parse(model["OneTimeWater"].ToString());

            Quote_Save.MonthlyCable = model["MonthlyCable"].ToString();
            Quote_Save.MonthlyFridge = model["MonthlyFridge"].ToString();
            Quote_Save.MonthlyElectric = model["MonthlyElectric"].ToString();
            Quote_Save.MonthlyWaterSewerTrash = model["MonthlyWaterSewerTrash"].ToString();
            Quote_Save.MonthlyWasherDrayer = model["MonthlyWasherDrayer"].ToString();
            Quote_Save.MonthlyValetTrash = model["MonthlyValetTrash"].ToString();
            Quote_Save.MonthlyWasherDrayerType = model["MonthlyWasherDrayerType"].ToString();
            Quote_Save.MonthlyInternet = model["MonthlyInternet"].ToString();
            Quote_Save.OneTimeInspectionName = model["OneTimeInspectionName"].ToString();
            Quote_Save.MonthlyFurniture = model["MonthlyFurniture"].ToString();
            Quote_Save.MonthlyGas = model["MonthlyGas"].ToString();
            Quote_Save.MonthlyHouseWavers = model["MonthlyHouseWavers"].ToString();
            Quote_Save.MonthlyMicrowave = model["MonthlyMicrowave"].ToString();
            Quote_Save.OneTimeCleaningName = model["OneTimeCleaningName"].ToString();


            Quote_Save.Notes = model["Notes"].ToString();
            //Quote_Save.PropertyEndDate = DateTime.Parse(model["PropertyEndDate"].ToString());
            try
            {

                Quote_Save.PropertyEndDate = DateTime.Parse(model["PropertyEndDate"]);

            }
            catch (Exception ex)
            {

                string date = model["PropertyEndDate"].ToString();
                string mon = date.Substring(0, 2);
                string da = date.Substring(3, 2);
                string ye = date.Substring(6, 4);
                //date = da + "/" + mon + "/" + ye;
                System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
                Quote_Save.PropertyEndDate = DateTime.ParseExact(date, "MM/dd/yyyy", provider);

            }
            //Quote_Save.PropertyStartDate = DateTime.Parse(model["PropertyStartDate"].ToString());
            try
            {

                Quote_Save.PropertyStartDate = DateTime.Parse(model["PropertyStartDate"]);

            }
            catch (Exception ex)
            {

                string date = model["PropertyStartDate"].ToString();
                string mon = date.Substring(0, 2);
                string da = date.Substring(3, 2);
                string ye = date.Substring(6, 4);
                //date = da + "/" + mon + "/" + ye;
                System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
                Quote_Save.PropertyStartDate = DateTime.ParseExact(date, "MM/dd/yyyy", provider);

            }
            Quote_Save.TotalStay = Int64.Parse(model["TotalStay"].ToString());
            Quote_Save.Vacancy = model["Vacancy"].ToString();

            Quote_Save.TotalMonthly = decimal.Parse(model["TotalMonthly"].ToString());
            Quote_Save.TotalOneTime = decimal.Parse(model["TotalOneTime"].ToString());
            Quote_Save.TotalMonthlyCCost = decimal.Parse(model["TotalMonthlyCCost"].ToString());
            Quote_Save.DailyCash = decimal.Parse(model["DailyCash"].ToString());
            Quote_Save.DailyCredit = decimal.Parse(model["DailyCredit"].ToString());
            Quote_Save.Charges = decimal.Parse(model["Charges"].ToString());
            Quote_Save.RefFinalAmount = decimal.Parse(model["RefFinalAmount"].ToString());
            Quote_Save.OneTimeOtherName = model["OneTimeOtherName"].ToString();
            Quote_Save.OneTimeOther = decimal.Parse(model["OneTimeOther"].ToString());
            Quote_Save.MonthlyOtherName = model["MonthlyOtherName"].ToString();
            Quote_Save.MonthlyOther = decimal.Parse(model["MonthlyOther"].ToString());

            //if (Session["LoginUserRole"] != null)
            //{
            //    //vwAppUserAsset_Save.last_update_user_id = Session["LoginUserEmail"].ToString();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                if (Quote_Save.QuoteId == 0)
                {


                    cmd.CommandText = "QuotesInsert";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@OneTimeFurnitureDeliveryFee", Quote_Save.OneTimeFurnitureDeliveryFee);
                    cmd.Parameters.AddWithValue("@KeyID", Quote_Save.KeyID);
                    cmd.Parameters.AddWithValue("@CreditCard", Quote_Save.CreditCard);
                    cmd.Parameters.AddWithValue("@MonthlyCableFee", Quote_Save.MonthlyCableFee);
                    cmd.Parameters.AddWithValue("@MonthlyFurnitureUsageFee", Quote_Save.MonthlyFurnitureUsageFee);
                    cmd.Parameters.AddWithValue("@LeadsId", Quote_Save.LeadsId);
                    cmd.Parameters.AddWithValue("@LeaseEndDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@LeaseStartDate", Quote_Save.LeaseStartDate);
                    //cmd.Parameters.AddWithValue("@MonthlyCableFee",  Quote_Save.MonthlyCableFee);
                    cmd.Parameters.AddWithValue("@MonthlyCourierFee", Quote_Save.MonthlyCourierFee);
                    cmd.Parameters.AddWithValue("@MonthlyElectricFee", Quote_Save.MonthlyElectricFee);
                    cmd.Parameters.AddWithValue("@MonthlyFridgeFee", Quote_Save.MonthlyFridgeFee);
                    //cmd.Parameters.AddWithValue("@MonthlyFurnitureUsageFee",  Quote_Save.MonthlyFurnitureUsageFee);
                    cmd.Parameters.AddWithValue("@MonthlyGasFee", Quote_Save.MonthlyGasFee);
                    cmd.Parameters.AddWithValue("@MonthlyHouseWaversFee", Quote_Save.MonthlyHouseWaversFee);
                    cmd.Parameters.AddWithValue("@MonthlyInternetFee", Quote_Save.MonthlyInternetFee);
                    cmd.Parameters.AddWithValue("@MonthlyMarketingFee", Quote_Save.MonthlyMarketingFee);
                    cmd.Parameters.AddWithValue("@MonthlyMicrowaveFee", Quote_Save.MonthlyMicrowaveFee);
                    cmd.Parameters.AddWithValue("@MonthlyPetRentFee", Quote_Save.MonthlyPetRentFee);

                    cmd.Parameters.AddWithValue("@MonthlyPropertyRent", Quote_Save.MonthlyPropertyRent);
                    cmd.Parameters.AddWithValue("@MonthlyReferalFee", Quote_Save.MonthlyReferalFee);
                    cmd.Parameters.AddWithValue("@MonthlyValetTrashFee", Quote_Save.MonthlyValetTrashFee);
                    cmd.Parameters.AddWithValue("@MonthlyWasherDrayerFee", Quote_Save.MonthlyWasherDrayerFee);

                    cmd.Parameters.AddWithValue("@MonthlyWaterSewerTrashFee", Quote_Save.MonthlyWaterSewerTrashFee);
                    cmd.Parameters.AddWithValue("@OneTimeAdminFee", Quote_Save.OneTimeAdminFee);
                    cmd.Parameters.AddWithValue("@OneTimeAmnityFee", Quote_Save.OneTimeAmnityFee);
                    cmd.Parameters.AddWithValue("@OneTimeHouseWaversSetupFee", Quote_Save.OneTimeHouseWaversSetupFee);
                    //cmd.Parameters.AddWithValue("@OneTimeHouseWaversSetupFee", Quote_Save.OneTimeHouseWaversSetupFee);
                    cmd.Parameters.AddWithValue("@ParkingPlaces", 0);
                    cmd.Parameters.AddWithValue("@ParkingType", Quote_Save.ParkingType);
                    cmd.Parameters.AddWithValue("@PropertyId", Quote_Save.PropertyId);
                    cmd.Parameters.AddWithValue("@VacancyDays", 30);


                    cmd.Parameters.AddWithValue("@ClientEndDate", Quote_Save.ClientEndDate);
                    cmd.Parameters.AddWithValue("@ClientStartDate", Quote_Save.ClientStartDate);
                    cmd.Parameters.AddWithValue("@IsActive", Quote_Save.IsActive);
                    cmd.Parameters.AddWithValue("@MonthlyBreakLeaseFee", Quote_Save.MonthlyBreakLeaseFee);
                    cmd.Parameters.AddWithValue("@MonthlyInsuranceBlanketFee", Quote_Save.MonthlyInsuranceBlanketFee);
                    cmd.Parameters.AddWithValue("@MonthlyKSProfitFee", Quote_Save.MonthlyKSProfitFee);
                    cmd.Parameters.AddWithValue("@MonthlyParcelServicePropertyFee", Quote_Save.MonthlyParcelServicePropertyFee);
                    cmd.Parameters.AddWithValue("@MonthlyParkingPlacesFee", Quote_Save.MonthlyParkingPlacesFee);
                    cmd.Parameters.AddWithValue("@OneTimeCable", Quote_Save.OneTimeCable);
                    cmd.Parameters.AddWithValue("@OneTimeCleaning", Quote_Save.OneTimeCleaning);
                    cmd.Parameters.AddWithValue("@OneTimeElectric", Quote_Save.OneTimeElectric);
                    cmd.Parameters.AddWithValue("@OneTimeGas", Quote_Save.OneTimeGas);
                    cmd.Parameters.AddWithValue("@OneTimeGiftBasket", Quote_Save.OneTimeGiftBasket);
                    cmd.Parameters.AddWithValue("@OneTimeInspection", Quote_Save.OneTimeInspection);
                    cmd.Parameters.AddWithValue("@OneTimeInternet", Quote_Save.OneTimeInternet);
                    cmd.Parameters.AddWithValue("@OneTimeKSAdminfee", Quote_Save.OneTimeKSAdminfee);
                    cmd.Parameters.AddWithValue("@OneTimeKSAppFee", Quote_Save.OneTimeKSAppFee);
                    cmd.Parameters.AddWithValue("@OneTimeKSPetDep", Quote_Save.OneTimeKSPetDep);
                    cmd.Parameters.AddWithValue("@OneTimeKSPetFee", Quote_Save.OneTimeKSPetFee);
                    cmd.Parameters.AddWithValue("@OneTimeKSSecDep", Quote_Save.OneTimeKSSecDep);
                    cmd.Parameters.AddWithValue("@OneTimeNonRefFees", Quote_Save.OneTimeNonRefFees);
                    cmd.Parameters.AddWithValue("@OneTimeOccupantBackgroundcheck", Quote_Save.OneTimeOccupantBackgroundcheck);
                    cmd.Parameters.AddWithValue("@OneTimePropertyCorporateApplicationFee", Quote_Save.OneTimePropertyCorporateApplicationFee);
                    cmd.Parameters.AddWithValue("@OneTimePropHoldFees", Quote_Save.OneTimePropHoldFees);
                    cmd.Parameters.AddWithValue("@OneTimePropPetDep", Quote_Save.OneTimePropPetDep);
                    cmd.Parameters.AddWithValue("@OneTimePropPetFee", Quote_Save.OneTimePropPetFee);
                    cmd.Parameters.AddWithValue("@OneTimePropSecDep", Quote_Save.OneTimePropSecDep);
                    cmd.Parameters.AddWithValue("@OneTimeRefKSDep", Quote_Save.OneTimeRefKSDep);
                    cmd.Parameters.AddWithValue("@OneTimeRefundablePropFees", Quote_Save.OneTimeRefundablePropFees);
                    cmd.Parameters.AddWithValue("@OneTimeRemoteFOBKeyCard", Quote_Save.OneTimeRemoteFOBKeyCard);
                    cmd.Parameters.AddWithValue("@OneTimeSureDeposit", Quote_Save.OneTimeSureDeposit);
                    cmd.Parameters.AddWithValue("@OneTimeTrash", Quote_Save.OneTimeTrash);
                    cmd.Parameters.AddWithValue("@OneTimeWater", Quote_Save.OneTimeWater);
                    cmd.Parameters.AddWithValue("@Notes", Quote_Save.Notes);
                    cmd.Parameters.AddWithValue("@PropertyEndDate", Quote_Save.PropertyEndDate);
                    cmd.Parameters.AddWithValue("@PropertyStartDate", Quote_Save.PropertyStartDate);
                    cmd.Parameters.AddWithValue("@TotalStay", Quote_Save.TotalStay);
                    cmd.Parameters.AddWithValue("@Vacancy", Quote_Save.Vacancy);

                    cmd.Parameters.AddWithValue("@MonthlyCable", Quote_Save.MonthlyCable);
                    cmd.Parameters.AddWithValue("@MonthlyFridge", Quote_Save.MonthlyFridge);
                    cmd.Parameters.AddWithValue("@MonthlyElectric", Quote_Save.MonthlyElectric);
                    cmd.Parameters.AddWithValue("@MonthlyWaterSewerTrash", Quote_Save.MonthlyWaterSewerTrash);
                    cmd.Parameters.AddWithValue("@MonthlyWasherDrayer", Quote_Save.MonthlyWasherDrayer);
                    cmd.Parameters.AddWithValue("@MonthlyValetTrash", Quote_Save.MonthlyValetTrash);
                    cmd.Parameters.AddWithValue("@MonthlyInternet", Quote_Save.MonthlyInternet);
                    cmd.Parameters.AddWithValue("@OneTimeInspectionName", Quote_Save.OneTimeInspectionName);
                    cmd.Parameters.AddWithValue("@MonthlyFurniture", Quote_Save.MonthlyFurniture);
                    cmd.Parameters.AddWithValue("@MonthlyGas", Quote_Save.MonthlyGas);
                    cmd.Parameters.AddWithValue("@MonthlyHouseWavers", Quote_Save.MonthlyHouseWavers);
                    cmd.Parameters.AddWithValue("@MonthlyMicrowave", Quote_Save.MonthlyMicrowave);
                    cmd.Parameters.AddWithValue("@OneTimeCleaningName", Quote_Save.OneTimeCleaningName);
                    cmd.Parameters.AddWithValue("@MonthlyWasherDrayerType", Quote_Save.MonthlyWasherDrayerType);
                    cmd.Parameters.AddWithValue("@TotalMonthly", Quote_Save.TotalMonthly);
                    cmd.Parameters.AddWithValue("@TotalOneTime", Quote_Save.TotalOneTime);
                    cmd.Parameters.AddWithValue("@TotalMonthlyCCost", Quote_Save.TotalMonthlyCCost);
                    cmd.Parameters.AddWithValue("@DailyCash", Quote_Save.DailyCash);
                    cmd.Parameters.AddWithValue("@DailyCredit", Quote_Save.DailyCredit);
                    cmd.Parameters.AddWithValue("@Charges", Quote_Save.Charges);
                    cmd.Parameters.AddWithValue("@RefFinalAmount", Quote_Save.RefFinalAmount);

                    cmd.Parameters.AddWithValue("@MonthlyOther", Quote_Save.MonthlyOther);
                    cmd.Parameters.AddWithValue("@OneTimeOther", Quote_Save.OneTimeOther);
                    cmd.Parameters.AddWithValue("@MonthlyOtherName", Quote_Save.MonthlyOtherName);
                    cmd.Parameters.AddWithValue("@OneTimeOtherName", Quote_Save.OneTimeOtherName);


                    try
                    {
                        con.Open();
                        object QouteID = cmd.ExecuteScalar();
                        Session["SuccessMessage"] = "Success: Quote Successfully Updated";
                        AddReservation(Int64.Parse(QouteID.ToString()));

                        SqlCommand cmdStatus = new SqlCommand();
                        cmd.CommandText = "update Properties set Status='Quote Made', Leased=1 where PropertyId=" + Quote_Save.PropertyId;
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();

                        Session["error"] = null;
                        Session["SuccessMessage"] = "Success: Quote Successfully Added";
                    }
                    catch (SqlException e)
                    {

                        ViewBag.error = "Transaction Failure";
                        Session["error"] = ViewBag.error;
                        Session["Message"] = e.Message;
                    }

                }

                else
                {
                    cmd.CommandText = "QuotesUpdate";
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("@OneTimeFurnitureDeliveryFee", Quote_Save.OneTimeFurnitureDeliveryFee);
                    cmd.Parameters.AddWithValue("@QuoteId", Quote_Save.QuoteId);
                    cmd.Parameters.AddWithValue("@KeyID", Quote_Save.KeyID);
                    cmd.Parameters.AddWithValue("@CreditCard", Quote_Save.CreditCard);
                    //cmd.Parameters.AddWithValue("@MonthlyCableFee", Quote_Save.MonthlyCableFee);
                    //cmd.Parameters.AddWithValue("@MonthlyFurnitureUsageFee", Quote_Save.MonthlyFurnitureUsageFee);
                    cmd.Parameters.AddWithValue("@LeadsId", Quote_Save.LeadsId);
                    cmd.Parameters.AddWithValue("@LeaseEndDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@LeaseStartDate", Quote_Save.LeaseStartDate);
                    cmd.Parameters.AddWithValue("@MonthlyCableFee", Quote_Save.MonthlyCableFee);
                    cmd.Parameters.AddWithValue("@MonthlyCourierFee", Quote_Save.MonthlyCourierFee);
                    cmd.Parameters.AddWithValue("@MonthlyElectricFee", Quote_Save.MonthlyElectricFee);
                    cmd.Parameters.AddWithValue("@MonthlyFridgeFee", Quote_Save.MonthlyFridgeFee);
                    cmd.Parameters.AddWithValue("@MonthlyFurnitureUsageFee", Quote_Save.MonthlyFurnitureUsageFee);
                    cmd.Parameters.AddWithValue("@MonthlyGasFee", Quote_Save.MonthlyGasFee);
                    cmd.Parameters.AddWithValue("@MonthlyHouseWaversFee", Quote_Save.MonthlyHouseWaversFee);
                    cmd.Parameters.AddWithValue("@MonthlyInternetFee", Quote_Save.MonthlyInternetFee);
                    cmd.Parameters.AddWithValue("@MonthlyMarketingFee", Quote_Save.MonthlyMarketingFee);
                    cmd.Parameters.AddWithValue("@MonthlyMicrowaveFee", Quote_Save.MonthlyMicrowaveFee);
                    cmd.Parameters.AddWithValue("@MonthlyPetRentFee", Quote_Save.MonthlyPetRentFee);

                    cmd.Parameters.AddWithValue("@MonthlyPropertyRent", Quote_Save.MonthlyPropertyRent);
                    cmd.Parameters.AddWithValue("@MonthlyReferalFee", Quote_Save.MonthlyReferalFee);
                    cmd.Parameters.AddWithValue("@MonthlyValetTrashFee", Quote_Save.MonthlyValetTrashFee);
                    cmd.Parameters.AddWithValue("@MonthlyWasherDrayerFee", Quote_Save.MonthlyWasherDrayerFee);

                    cmd.Parameters.AddWithValue("@MonthlyWaterSewerTrashFee", Quote_Save.MonthlyWaterSewerTrashFee);
                    cmd.Parameters.AddWithValue("@OneTimeAdminFee", Quote_Save.OneTimeAdminFee);
                    cmd.Parameters.AddWithValue("@OneTimeAmnityFee", Quote_Save.OneTimeAmnityFee);
                    cmd.Parameters.AddWithValue("@OneTimeHouseWaversSetupFee", Quote_Save.OneTimeHouseWaversSetupFee);

                    cmd.Parameters.AddWithValue("@ParkingPlaces", 0);
                    cmd.Parameters.AddWithValue("@ParkingType", Quote_Save.ParkingType);
                    cmd.Parameters.AddWithValue("@PropertyId", Quote_Save.PropertyId);
                    cmd.Parameters.AddWithValue("@VacancyDays", 30);

                    cmd.Parameters.AddWithValue("@ClientEndDate", Quote_Save.ClientEndDate);
                    cmd.Parameters.AddWithValue("@ClientStartDate", Quote_Save.ClientStartDate);
                    cmd.Parameters.AddWithValue("@IsActive", Quote_Save.IsActive);
                    cmd.Parameters.AddWithValue("@MonthlyBreakLeaseFee", Quote_Save.MonthlyBreakLeaseFee);
                    cmd.Parameters.AddWithValue("@MonthlyInsuranceBlanketFee", Quote_Save.MonthlyInsuranceBlanketFee);
                    cmd.Parameters.AddWithValue("@MonthlyKSProfitFee", Quote_Save.MonthlyKSProfitFee);
                    cmd.Parameters.AddWithValue("@MonthlyParcelServicePropertyFee", Quote_Save.MonthlyParcelServicePropertyFee);
                    cmd.Parameters.AddWithValue("@MonthlyParkingPlacesFee", Quote_Save.MonthlyParkingPlacesFee);
                    cmd.Parameters.AddWithValue("@OneTimeCable", Quote_Save.OneTimeCable);
                    cmd.Parameters.AddWithValue("@OneTimeCleaning", Quote_Save.OneTimeCleaning);
                    cmd.Parameters.AddWithValue("@OneTimeElectric", Quote_Save.OneTimeElectric);
                    cmd.Parameters.AddWithValue("@OneTimeGas", Quote_Save.OneTimeGas);
                    cmd.Parameters.AddWithValue("@OneTimeGiftBasket", Quote_Save.OneTimeGiftBasket);
                    cmd.Parameters.AddWithValue("@OneTimeInspection", Quote_Save.OneTimeInspection);
                    cmd.Parameters.AddWithValue("@OneTimeInternet", Quote_Save.OneTimeInternet);
                    cmd.Parameters.AddWithValue("@OneTimeKSAdminfee", Quote_Save.OneTimeKSAdminfee);
                    cmd.Parameters.AddWithValue("@OneTimeKSAppFee", Quote_Save.OneTimeKSAppFee);
                    cmd.Parameters.AddWithValue("@OneTimeKSPetDep", Quote_Save.OneTimeKSPetDep);
                    cmd.Parameters.AddWithValue("@OneTimeKSPetFee", Quote_Save.OneTimeKSPetFee);
                    cmd.Parameters.AddWithValue("@OneTimeKSSecDep", Quote_Save.OneTimeKSSecDep);
                    cmd.Parameters.AddWithValue("@OneTimeNonRefFees", Quote_Save.OneTimeNonRefFees);
                    cmd.Parameters.AddWithValue("@OneTimeOccupantBackgroundcheck", Quote_Save.OneTimeOccupantBackgroundcheck);
                    cmd.Parameters.AddWithValue("@OneTimePropertyCorporateApplicationFee", Quote_Save.OneTimePropertyCorporateApplicationFee);
                    cmd.Parameters.AddWithValue("@OneTimePropHoldFees", Quote_Save.OneTimePropHoldFees);
                    cmd.Parameters.AddWithValue("@OneTimePropPetDep", Quote_Save.OneTimePropPetDep);
                    cmd.Parameters.AddWithValue("@OneTimePropPetFee", Quote_Save.OneTimePropPetFee);
                    cmd.Parameters.AddWithValue("@OneTimePropSecDep", Quote_Save.OneTimePropSecDep);
                    cmd.Parameters.AddWithValue("@OneTimeRefKSDep", Quote_Save.OneTimeRefKSDep);
                    cmd.Parameters.AddWithValue("@OneTimeRefundablePropFees", Quote_Save.OneTimeRefundablePropFees);
                    cmd.Parameters.AddWithValue("@OneTimeRemoteFOBKeyCard", Quote_Save.OneTimeRemoteFOBKeyCard);
                    cmd.Parameters.AddWithValue("@OneTimeSureDeposit", Quote_Save.OneTimeSureDeposit);
                    cmd.Parameters.AddWithValue("@OneTimeTrash", Quote_Save.OneTimeTrash);
                    cmd.Parameters.AddWithValue("@OneTimeWater", Quote_Save.OneTimeWater);
                    cmd.Parameters.AddWithValue("@Notes", Quote_Save.Notes);
                    cmd.Parameters.AddWithValue("@PropertyEndDate", Quote_Save.PropertyEndDate);
                    cmd.Parameters.AddWithValue("@PropertyStartDate", Quote_Save.PropertyStartDate);
                    cmd.Parameters.AddWithValue("@TotalStay", Quote_Save.TotalStay);
                    cmd.Parameters.AddWithValue("@Vacancy", Quote_Save.Vacancy);

                    cmd.Parameters.AddWithValue("@MonthlyCable", Quote_Save.MonthlyCable);
                    cmd.Parameters.AddWithValue("@MonthlyFridge", Quote_Save.MonthlyFridge);
                    cmd.Parameters.AddWithValue("@MonthlyElectric", Quote_Save.MonthlyElectric);
                    cmd.Parameters.AddWithValue("@MonthlyWaterSewerTrash", Quote_Save.MonthlyWaterSewerTrash);
                    cmd.Parameters.AddWithValue("@MonthlyWasherDrayer", Quote_Save.MonthlyWasherDrayer);
                    cmd.Parameters.AddWithValue("@MonthlyWasherDrayerType", Quote_Save.MonthlyWasherDrayerType);
                    cmd.Parameters.AddWithValue("@MonthlyValetTrash", Quote_Save.MonthlyValetTrash);
                    cmd.Parameters.AddWithValue("@MonthlyInternet", Quote_Save.MonthlyInternet);
                    cmd.Parameters.AddWithValue("@OneTimeInspectionName", Quote_Save.OneTimeInspectionName);
                    cmd.Parameters.AddWithValue("@MonthlyFurniture", Quote_Save.MonthlyFurniture);
                    cmd.Parameters.AddWithValue("@MonthlyGas", Quote_Save.MonthlyGas);
                    cmd.Parameters.AddWithValue("@MonthlyHouseWavers", Quote_Save.MonthlyHouseWavers);
                    cmd.Parameters.AddWithValue("@MonthlyMicrowave", Quote_Save.MonthlyMicrowave);
                    cmd.Parameters.AddWithValue("@OneTimeCleaningName", Quote_Save.OneTimeCleaningName);

                    cmd.Parameters.AddWithValue("@TotalMonthly", Quote_Save.TotalMonthly);
                    cmd.Parameters.AddWithValue("@TotalOneTime", Quote_Save.TotalOneTime);
                    cmd.Parameters.AddWithValue("@TotalMonthlyCCost", Quote_Save.TotalMonthlyCCost);
                    cmd.Parameters.AddWithValue("@DailyCash", Quote_Save.DailyCash);
                    cmd.Parameters.AddWithValue("@DailyCredit", Quote_Save.DailyCredit);
                    cmd.Parameters.AddWithValue("@Charges", Quote_Save.Charges);
                    cmd.Parameters.AddWithValue("@RefFinalAmount", Quote_Save.RefFinalAmount);

                    cmd.Parameters.AddWithValue("@MonthlyOther", Quote_Save.MonthlyOther);
                    cmd.Parameters.AddWithValue("@OneTimeOther", Quote_Save.OneTimeOther);
                    cmd.Parameters.AddWithValue("@MonthlyOtherName", Quote_Save.MonthlyOtherName);
                    cmd.Parameters.AddWithValue("@OneTimeOtherName", Quote_Save.OneTimeOtherName);
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        Session["SuccessMessage"] = "Success: Quote Successfully Updated";

                    }
                    catch (SqlException e)
                    {

                        ViewBag.error = "Transaction Failure";
                        Session["Message"] = e.Message;
                    }
                }

            }
            con.Close();

            if (Session["error"] != null)
            {
                ViewData["Message"] = Session["error"];
                ViewBag.Vendor = BindDataVendorAll("").Where(y => y.IsActive = true);
                ViewBag.Property = BindDataPropertyAll(0).Where(y => y.IsActive = true);
                ViewBag.ContactType = DropDownListContantType("");
                ViewBag.LeaseTerm = DropDownListLeaseTerm("");
                ViewBag.City = DropDownListCity("");
                ViewBag.ReferelSource = DropDownListReferelSource("");
                ViewBag.ParkingType = DropDownListParkingType(model["ParkingType"].ToString()).Where(m => m.Value != "Other");
                ViewBag.CreditCard = DropDownListCreditCardFee(model["CreditCard"].ToString());
                List<Lead> lead_list = BindDataLeadAll(0).Where(y => y.IsActive = true).ToList();
                List<ReferalSource> referal_list = BindDataReferalAll(0).Where(y => y.IsActive = true).ToList();
                foreach (var lead in lead_list)
                {
                    if (referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().Address == null)
                    {
                        lead.referalSource.Address = "";
                    }
                    else
                    {
                        lead.referalSource.Address = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().Address;
                    }

                    if (referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().CostPerDay == null)
                    {
                        lead.referalSource.CostPerDay = 0;
                    }
                    else
                    {
                        lead.referalSource.CostPerDay = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().CostPerDay;
                    }

                    if (referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().Number == null)
                    {
                        lead.referalSource.Number = 0;
                    }
                    else
                    {
                        lead.referalSource.Number = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().Number;
                    }

                    lead.referalSource.ReferalType = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().ReferalType;
                    lead.referalSource.ReferalSourceId = referal_list.Where(x => x.CompanyName == lead.ReferelSource).FirstOrDefault().ReferalSourceId;
                }
                //ViewBag.Lead = BindDataLeadAll(0);
                ViewBag.Lead = lead_list;
                return View(Quote_Save);
            }
            else
            {
                ViewData["SuccessMessage"] = Session["SuccessMessage"];
                return RedirectToAction("Quotes");
            }


        }

        public ActionResult DeleteQuote(decimal id = 0)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"QuotesDelete";

                cmd.Parameters.AddWithValue("@QuoteId", id);

                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    //Select Property
                    SqlCommand cmdStatus = new SqlCommand();
                    cmdStatus.CommandType = CommandType.Text;
                    cmdStatus.CommandText = "Select PropertyId from Properties where QuoteId=" + id;
                    con.Close();
                    SqlDataReader Dr;
                    con.Open();
                    Dr = cmdStatus.ExecuteReader();
                    DataTable dt = new DataTable("Vw");
                    dt.Load(Dr);

                    //Update Status    
                    cmdStatus = new SqlCommand();
                    cmdStatus.CommandText = "update Properties set Status='Vacant', Leased=0 where PropertyId=" + Int64.Parse(dt.Rows[0]["PropertyId"].ToString());
                    cmdStatus.CommandType = CommandType.Text;
                    cmdStatus.ExecuteNonQuery();


                    Session["SuccessMessage"] = "Quote Deleted Completely Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Quotes");
        }

        public ActionResult ActiveInactiveQuote(decimal id = 0, int IsActive = 1)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"QuotesEnableDisable";

                cmd.Parameters.AddWithValue("@Id", id);
                if (IsActive == 1)
                {
                    cmd.Parameters.AddWithValue("@IsActive", true);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsActive", false);
                }
                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Quote Updated Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Quotes");
        }


        /// <summary>
        /// ///Reservation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Reservations(decimal id = 0, string srchContactName = "", string srchOccupantName = "")
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
            SqlCommand cmd = new SqlCommand("ReservationsSelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Reservation Reservation_Single = new Reservation();
            Reservation Reservation_Detail = new Reservation();
            JsonResult jR = new JsonResult();
            List<Reservation> Reservation_List = new List<Reservation>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    //Reservation_Single.Address = dt.Rows[i]["Address"].ToString();
                    Reservation_Single.RId = Int64.Parse(dt.Rows[i]["RId"].ToString());
                    Reservation_Single.TotalMonthly = decimal.Parse(dt.Rows[i]["TotalMonthly"].ToString());
                    if (dt.Rows[i]["ArrivalInstructions"].ToString() == null)
                    {
                        Reservation_Single.ArrivalInstructions = "";
                    }
                    else
                    {
                        Reservation_Single.ArrivalInstructions = dt.Rows[i]["ArrivalInstructions"].ToString();
                    }
                    if (dt.Rows[i]["CheckOutTime"] == null)
                    {
                        Reservation_Single.CheckOutTime = DateTime.Parse(dt.Rows[i]["LeaseEndDate"].ToString());
                    }
                    else
                    {
                        Reservation_Single.CheckOutTime = DateTime.Parse(dt.Rows[i]["CheckOutTime"].ToString());
                    }

                    if (dt.Rows[i]["DepartureInstructions"].ToString() == null)
                    {
                        Reservation_Single.DepartureInstructions = "";
                    }
                    else
                    {
                        Reservation_Single.DepartureInstructions = dt.Rows[i]["DepartureInstructions"].ToString();
                    }

                    if (dt.Rows[i]["CheckInTime"] == null)
                    {
                        Reservation_Single.CheckInTime = DateTime.Parse(dt.Rows[i]["LeaseStartDate"].ToString());
                    }
                    else
                    {
                        Reservation_Single.CheckInTime = DateTime.Parse(dt.Rows[i]["CheckInTime"].ToString());
                    }

                    Reservation_Single.LeaseEndDate = DateTime.Parse(dt.Rows[i]["LeaseEndDate"].ToString());
                    Reservation_Single.LeaseStartDate = DateTime.Parse(dt.Rows[i]["LeaseStartDate"].ToString());

                    Reservation_Single.QouteId = Int64.Parse(dt.Rows[i]["QouteId"].ToString());
                    Reservation_Single.GuestName = dt.Rows[i]["GuestName"].ToString();
                    // Reservation_Single.GuestName = dt.Rows[i]["GuestName"].ToString();

                    Reservation_Single.PropertyId = Int64.Parse(dt.Rows[i]["PropertyId"].ToString());

                    Reservation_Single.IsActive = bool.Parse(dt.Rows[i]["IsActive"].ToString());
                    Reservation_Single.Status = dt.Rows[i]["Status"].ToString();
                    Reservation_Single.property = BindDataPropertyAll(0).Where(m => m.PropertyId == Int64.Parse(dt.Rows[i]["PropertyId"].ToString())).FirstOrDefault();
                    Reservation_Single.quote = BindDataQuoteAll(0).Where(m => m.LeadsId == Int64.Parse(dt.Rows[i]["QouteId"].ToString())).FirstOrDefault();
                    //Reservation_Single.quote.lead = BindDataLeadAll(0).Where(m => m.LeadsId == Int64.Parse(dt.Rows[i]["QouteId"].ToString())).FirstOrDefault();
                    Reservation_List.Add(Reservation_Single);
                    Reservation_Single = new Reservation();

                }

                if (id == 0)
                {
                    Reservation_Detail = Reservation_List[0];
                }
                else
                {
                    Reservation_Detail = Reservation_List.Where(a => a.RId == id).FirstOrDefault();
                }
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

            return View(Reservation_List);
        }


        public ActionResult AddReservation(Int64 id)
        {

            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            Session["Message"] = "";
            Session["SuccessMessage"] = "";

            string connect = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("QuotesSelect", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuoteId", id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            con = new SqlConnection(connect);
            cmd = new SqlCommand("QuotesSelectTotals", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuoteId", id);
            SqlDataReader DrTotal;
            con.Open();
            DrTotal = cmd.ExecuteReader();
            DataTable dt_Total = new DataTable("Vw");
            dt_Total.Load(DrTotal);
            con.Close();

            Reservation Reservation_Save = new Reservation();

            Quote Quote_Single = new Quote();

            List<Quote> Quote_List = new List<Quote>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dt.Rows.Count > 0)
            {

                Reservation_Save.QouteId = Int64.Parse(dt.Rows[0]["QuoteId"].ToString());

                Reservation_Save.LeaseEndDate = DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString());
                Reservation_Save.LeaseStartDate = DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString());

                Reservation_Save.PropertyId = Int64.Parse(dt.Rows[0]["PropertyId"].ToString());
                Reservation_Save.TotalOneTime = Decimal.Parse(dt_Total.Rows[0]["TotalOneTime"].ToString());
                Reservation_Save.TotalMonthly = Decimal.Parse(dt_Total.Rows[0]["TotalMonthly"].ToString());
            }




            //if (Session["LoginUserRole"] != null)
            //{
            //    //vwAppUserAsset_Save.last_update_user_id = Session["LoginUserEmail"].ToString();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                if (Reservation_Save.RId == 0)
                {


                    cmd.CommandText = "ReservationsInsert";
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("@TotalMonthly", Reservation_Save.TotalMonthly);
                    cmd.Parameters.AddWithValue("@TotalOneTime", Reservation_Save.TotalOneTime);
                    //cmd.Parameters.AddWithValue("@ArrivalInstructions", Reservation_Save.ArrivalInstructions);
                    //cmd.Parameters.AddWithValue("@CheckOutTime", Reservation_Save.CheckOutTime);
                    //cmd.Parameters.AddWithValue("@DepartureInstructions", Reservation_Save.DepartureInstructions);
                    //cmd.Parameters.AddWithValue("@CheckInTime", Reservation_Save.CheckInTime);
                    cmd.Parameters.AddWithValue("@LeaseEndDate", Reservation_Save.LeaseEndDate);
                    cmd.Parameters.AddWithValue("@LeaseStartDate", Reservation_Save.LeaseStartDate);

                    cmd.Parameters.AddWithValue("@QouteId", Reservation_Save.QouteId);

                    cmd.Parameters.AddWithValue("@PropertyId", Reservation_Save.PropertyId);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();

                        SqlCommand cmdStatus = new SqlCommand();
                        cmd.CommandText = "update Properties set Status='Reserved for Lease' , Leased=1 where PropertyId=" + Reservation_Save.PropertyId;
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();


                        Session["error"] = null;
                        Session["SuccessMessage"] = "Success: Reservation Successfully Added";
                    }
                    catch (SqlException e)
                    {

                        ViewBag.error = "Transaction Failure";
                        Session["error"] = ViewBag.error;
                        Session["Message"] = e.Message;
                    }


                }

                else
                {
                    cmd.CommandText = "ReservationsUpdate";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RId", Reservation_Save.RId);
                    cmd.Parameters.AddWithValue("@TotalMonthly", Reservation_Save.TotalMonthly);
                    cmd.Parameters.AddWithValue("@ArrivalInstructions", Reservation_Save.ArrivalInstructions);
                    cmd.Parameters.AddWithValue("@CheckOutTime", Reservation_Save.CheckOutTime);
                    cmd.Parameters.AddWithValue("@DepartureInstructions", Reservation_Save.DepartureInstructions);
                    cmd.Parameters.AddWithValue("@CheckInTime", Reservation_Save.CheckInTime);
                    cmd.Parameters.AddWithValue("@LeaseEndDate", Reservation_Save.LeaseEndDate);
                    cmd.Parameters.AddWithValue("@LeaseStartDate", Reservation_Save.LeaseStartDate);

                    cmd.Parameters.AddWithValue("@QouteId", Reservation_Save.QouteId);

                    cmd.Parameters.AddWithValue("@PropertyId", Reservation_Save.PropertyId);


                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        Session["SuccessMessage"] = "Success: Reservation Successfully Updated";
                    }
                    catch (SqlException e)
                    {

                        ViewBag.error = "Transaction Failure";
                        Session["Message"] = e.Message;
                    }
                }

            }
            con.Close();

            if (Session["error"] != null)
            {
                ViewData["Message"] = Session["error"];
                ViewBag.ContactType = DropDownListContantType("");
                ViewBag.LeaseTerm = DropDownListLeaseTerm("");
                ViewBag.City = DropDownListCity("");
                ViewBag.ReferelSource = DropDownListReferelSource("");
                return View(Reservation_Save);
            }
            else
            {
                ViewData["SuccessMessage"] = Session["SuccessMessage"];
                return RedirectToAction("Reservations");
            }


        }

        public ActionResult DeleteReservation(decimal id = 0)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"ReservationsDelete";

                cmd.Parameters.AddWithValue("@RId", id);

                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    //Select Property
                    SqlCommand cmdStatus = new SqlCommand();
                    cmdStatus.CommandType = CommandType.Text;
                    cmdStatus.CommandText = "Select PropertyId from Reservations where RId=" + id;

                    SqlDataReader Dr;
                    con.Open();
                    Dr = cmdStatus.ExecuteReader();
                    DataTable dt = new DataTable("Vw");
                    dt.Load(Dr);

                    //Update Status    
                    cmdStatus = new SqlCommand();
                    cmdStatus.CommandText = "update Properties set Status='Quote Made', Leased=1 where PropertyId=" + Int64.Parse(dt.Rows[0]["PropertyId"].ToString());
                    cmdStatus.CommandType = CommandType.Text;
                    cmdStatus.ExecuteNonQuery();


                    Session["SuccessMessage"] = "Reservation Deleted Completely Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Reservations");
        }

        public ActionResult ActiveInactiveReservation(decimal id = 0, int IsActive = 1)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"ReservationsEnableDisable";

                cmd.Parameters.AddWithValue("@Id", id);
                if (IsActive == 1)
                {
                    cmd.Parameters.AddWithValue("@IsActive", true);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsActive", false);
                }
                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Reservation Updated Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Reservations");
        }


        /// <summary>
        /// ///Arrival
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult ChangeStatus(Int64 Id = 0)
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
            SqlCommand cmd = new SqlCommand("ReservationsSelect", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Reservation Reservation_Single = new Reservation();
            Reservation Reservation_Detail = new Reservation();

            JsonResult jR = new JsonResult();
            List<Reservation> Reservation_List = new List<Reservation>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dt.Rows.Count > 0)
            {

                Reservation_Single.RId = Int64.Parse(dt.Rows[0]["RId"].ToString());
                Reservation_Single.ArrivalInstructions = "Q-" + Int64.Parse(dt.Rows[0]["QouteId"].ToString()) + " Arrival Instructions";//dt.Rows[0]["ArrivalInstructions"].ToString();
                //Reservation_Single.CheckInTime = DateTime.Parse(dt.Rows[0]["CheckInTime"].ToString());
                Reservation_Single.CheckInTime = DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString());
                TimeSpan ts = new TimeSpan(09, 30, 0);
                Reservation_Single.CheckInTime = Reservation_Single.CheckInTime.Value.Date + ts;
                Reservation_Single.property = BindDataPropertyAll(0).Where(x => x.PropertyId == Int64.Parse(dt.Rows[0]["PropertyId"].ToString())).FirstOrDefault();
                Reservation_Single.quote = BindDataQuoteAll(0).Where(x => x.QuoteId == Int64.Parse(dt.Rows[0]["QouteId"].ToString())).FirstOrDefault();
                Reservation_Single.quote.lead = BindDataLeadAll(0).Where(x => x.LeadsId == Int64.Parse(Reservation_Single.quote.LeadsId.ToString())).FirstOrDefault();
                Reservation_Single.LeaseStartDate = DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString());
                Reservation_Single.LeaseEndDate = DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString());
                Reservation_Single.CheckinKeyArrangements = dt.Rows[0]["CheckinKeyArrangements"].ToString();
                Reservation_Single.CustomerServiceNumber = dt.Rows[0]["CustomerServiceNumber"].ToString();
                Reservation_Single.Emergencynumber = dt.Rows[0]["Emergencynumber"].ToString();
                Reservation_Single.EntryGateCode = dt.Rows[0]["EntryGateCode"].ToString();
                Reservation_Single.ParkingPoolHours = dt.Rows[0]["ParkingPoolHours"].ToString();
                Reservation_Single.ParkingNumberofspaces = dt.Rows[0]["ParkingNumberofspaces"].ToString();
                Reservation_Single.ParkingFitnessCenterHours = dt.Rows[0]["ParkingFitnessCenterHours"].ToString();
                Reservation_Single.ParkingBusinessCenterHours = dt.Rows[0]["ParkingBusinessCenterHours"].ToString();
                Reservation_Single.ParkingAssignedSpace = dt.Rows[0]["ParkingAssignedSpace"].ToString();
                Reservation_Single.Housekeeping = dt.Rows[0]["Housekeeping"].ToString();
                Reservation_Single.GuestName = dt.Rows[0]["GuestName"].ToString();
                Reservation_Single.MailboxLocation = dt.Rows[0]["MailboxLocation"].ToString();
                Reservation_Single.MailboxNumber = dt.Rows[0]["MailboxNumber"].ToString();
                Reservation_Single.TrashDisposal = dt.Rows[0]["TrashDisposal"].ToString();
                Reservation_Single.WifiNetworkName = dt.Rows[0]["WifiNetworkName"].ToString();
                Reservation_Single.WifiPassword = dt.Rows[0]["WifiPassword"].ToString();
                Reservation_Single.Status = dt.Rows[0]["Status"].ToString();

                ViewBag.Status = BindDataStatusAll(0);
            }
            else
            {
                Reservation_Single = new Reservation()
                {
                    RId = 0,
                    ArrivalInstructions = "",
                    CheckInTime = DateTime.Now.Date,
                    CheckinKeyArrangements = "",
                    CustomerServiceNumber = "",
                    Emergencynumber = "",
                    EntryGateCode = "",
                    ParkingPoolHours = "",
                    ParkingNumberofspaces = "",
                    ParkingFitnessCenterHours = "",
                    ParkingBusinessCenterHours = "",
                    ParkingAssignedSpace = "",
                    Housekeeping = "",
                    GuestName = "",
                    MailboxLocation = "",
                    MailboxNumber = "",
                    TrashDisposal = "",
                    WifiNetworkName = "",
                    WifiPassword = ""
                };

            }



            return View(Reservation_Single);
        }


        [HttpPost]
        public ActionResult ChangeStatus(FormCollection model)
        {
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["error"] = null;
            Reservation Reservation_Save = new Reservation();

            Reservation_Save.RId = Int64.Parse(model["RId"]);

            //Reservation_Save.RId = Int64.Parse(model["RId"].ToString());
            Reservation_Save.Status = model["Status"];



            //if (Session["LoginUserRole"] != null)
            //{
            //    //vwAppUserAsset_Save.last_update_user_id = Session["LoginUserEmail"].ToString();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "ReservationsStatusUpdate";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@RId", Reservation_Save.RId);

                cmd.Parameters.AddWithValue("@Status", Reservation_Save.Status);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    //Select Property
                    //SqlCommand cmdStatus = new SqlCommand();

                    ////Update Status
                    ////

                    //cmdStatus = new SqlCommand();
                    //cmdStatus.Connection = con;
                    //cmdStatus.CommandText = "update Properties set Status='"+ Reservation_Save.Status + "', Leased=true where  PropertyId=" + Reservation_Save.PropertyId;
                    //cmdStatus.CommandType = CommandType.Text;
                    //cmdStatus.ExecuteNonQuery();


                    Session["SuccessMessage"] = "Success: Status Successfully Changed";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["Message"] = e.Message;
                }


            }
            con.Close();

            if (Session["error"] != null)
            {
                ViewData["Message"] = Session["error"];
                ViewBag.Status = BindDataStatusAll(0);
                return View(Reservation_Save);
            }
            else
            {
                ViewData["SuccessMessage"] = Session["SuccessMessage"];
                return RedirectToAction("Reservations");
            }


        }


        public ActionResult AddArrival(Int64 Id = 0)
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
            SqlCommand cmd = new SqlCommand("ReservationsSelect", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Reservation Reservation_Single = new Reservation();
            Reservation Reservation_Detail = new Reservation();
            JsonResult jR = new JsonResult();
            List<Reservation> Reservation_List = new List<Reservation>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dt.Rows.Count > 0)
            {

                Reservation_Single.RId = Int64.Parse(dt.Rows[0]["RId"].ToString());
                Reservation_Single.ArrivalInstructions = "Q-" + Int64.Parse(dt.Rows[0]["QouteId"].ToString()) + " Arrival Instructions";//dt.Rows[0]["ArrivalInstructions"].ToString();
                //Reservation_Single.CheckInTime = DateTime.Parse(dt.Rows[0]["CheckInTime"].ToString());
                Reservation_Single.CheckInTime = DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString());
                TimeSpan ts = new TimeSpan(09, 30, 0);
                Reservation_Single.CheckInTime = Reservation_Single.CheckInTime.Value.Date + ts;
                Reservation_Single.property = BindDataPropertyAll(0).Where(x => x.PropertyId == Int64.Parse(dt.Rows[0]["PropertyId"].ToString())).FirstOrDefault();

                Reservation_Single.LeaseStartDate = DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString());
                Reservation_Single.LeaseEndDate = DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString());
                Reservation_Single.CheckinKeyArrangements = dt.Rows[0]["CheckinKeyArrangements"].ToString();
                Reservation_Single.CustomerServiceNumber = dt.Rows[0]["CustomerServiceNumber"].ToString();
                Reservation_Single.Emergencynumber = dt.Rows[0]["Emergencynumber"].ToString();
                Reservation_Single.EntryGateCode = dt.Rows[0]["EntryGateCode"].ToString();
                Reservation_Single.ParkingPoolHours = dt.Rows[0]["ParkingPoolHours"].ToString();
                Reservation_Single.ParkingNumberofspaces = dt.Rows[0]["ParkingNumberofspaces"].ToString();
                Reservation_Single.ParkingFitnessCenterHours = dt.Rows[0]["ParkingFitnessCenterHours"].ToString();
                Reservation_Single.ParkingBusinessCenterHours = dt.Rows[0]["ParkingBusinessCenterHours"].ToString();
                Reservation_Single.ParkingAssignedSpace = dt.Rows[0]["ParkingAssignedSpace"].ToString();
                Reservation_Single.Housekeeping = dt.Rows[0]["Housekeeping"].ToString();
                Reservation_Single.GuestName = dt.Rows[0]["GuestName"].ToString();
                Reservation_Single.MailboxLocation = dt.Rows[0]["MailboxLocation"].ToString();
                Reservation_Single.MailboxNumber = dt.Rows[0]["MailboxNumber"].ToString();
                Reservation_Single.TrashDisposal = dt.Rows[0]["TrashDisposal"].ToString();
                Reservation_Single.WifiNetworkName = dt.Rows[0]["WifiNetworkName"].ToString();
                Reservation_Single.WifiPassword = dt.Rows[0]["WifiPassword"].ToString();
            }
            else
            {
                Reservation_Single = new Reservation()
                {
                    RId = 0,
                    ArrivalInstructions = "",
                    CheckInTime = DateTime.Now.Date,
                    CheckinKeyArrangements = "",
                    CustomerServiceNumber = "",
                    Emergencynumber = "",
                    EntryGateCode = "",
                    ParkingPoolHours = "",
                    ParkingNumberofspaces = "",
                    ParkingFitnessCenterHours = "",
                    ParkingBusinessCenterHours = "",
                    ParkingAssignedSpace = "",
                    Housekeeping = "",
                    GuestName = "",
                    MailboxLocation = "",
                    MailboxNumber = "",
                    TrashDisposal = "",
                    WifiNetworkName = "",
                    WifiPassword = ""
                };

            }



            return View(Reservation_Single);
        }

        [HttpPost]
        public ActionResult AddArrival(FormCollection model)
        {
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["error"] = null;
            Reservation Reservation_Save = new Reservation();

            Reservation_Save.RId = Int64.Parse(model["RId"]);

            //Reservation_Save.RId = Int64.Parse(model["RId"].ToString());

            //Reservation_Save.ArrivalInstructions = model["ArrivalInstructions"].ToString();

            Reservation_Save.CheckinKeyArrangements = model["CheckinKeyArrangements"].ToString();
            Reservation_Save.CustomerServiceNumber = model["CustomerServiceNumber"].ToString();
            Reservation_Save.Emergencynumber = model["Emergencynumber"].ToString();
            Reservation_Save.EntryGateCode = model["EntryGateCode"].ToString();
            Reservation_Save.ParkingPoolHours = model["ParkingPoolHours"].ToString();
            Reservation_Save.ParkingNumberofspaces = model["ParkingNumberofspaces"].ToString();
            Reservation_Save.ParkingFitnessCenterHours = model["ParkingFitnessCenterHours"].ToString();
            Reservation_Save.ParkingBusinessCenterHours = model["ParkingBusinessCenterHours"].ToString();
            Reservation_Save.ParkingAssignedSpace = model["ParkingAssignedSpace"].ToString();
            Reservation_Save.Housekeeping = model["Housekeeping"].ToString();
            Reservation_Save.GuestName = model["GuestName"].ToString();
            Reservation_Save.MailboxLocation = model["MailboxLocation"].ToString();
            Reservation_Save.MailboxNumber = model["MailboxNumber"].ToString();
            Reservation_Save.TrashDisposal = model["TrashDisposal"].ToString();
            Reservation_Save.WifiNetworkName = model["WifiNetworkName"].ToString();
            Reservation_Save.WifiPassword = model["WifiPassword"].ToString();


            Reservation_Save.LeaseStartDate = DateTime.Parse(model["LeaseStartDate"].ToString());
            Reservation_Save.LeaseEndDate = DateTime.Parse(model["LeaseEndDate"].ToString());
            //try
            //{

            //    Reservation_Save.CheckInTime = DateTime.Parse(model["CheckInTime"]);

            //}
            //catch (Exception ex)
            //{

            //    string date = model["CheckInTime"].ToString();
            //    string mon = date.Substring(0, 2);
            //    string da = date.Substring(3, 2);
            //    string ye = date.Substring(6, 4);
            //    //date = da + "/" + mon + "/" + ye;
            //    System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            //    Reservation_Save.CheckInTime = DateTime.ParseExact(date, "MM/dd/yyyy", provider);

            //}

            //Reservation_Save.CheckInTime = DateTime.Parse(model["CheckInTime"].ToString());



            //if (Session["LoginUserRole"] != null)
            //{
            //    //vwAppUserAsset_Save.last_update_user_id = Session["LoginUserEmail"].ToString();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "ArrivalUpdate";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@RId", Reservation_Save.RId);

                //cmd.Parameters.AddWithValue("@ArrivalInstructions", Reservation_Save.ArrivalInstructions);
                //cmd.Parameters.AddWithValue("@CheckInTime", Reservation_Save.CheckInTime);


                cmd.Parameters.AddWithValue("@CheckinKeyArrangements", Reservation_Save.CheckinKeyArrangements);
                cmd.Parameters.AddWithValue("@CustomerServiceNumber", Reservation_Save.CustomerServiceNumber);
                cmd.Parameters.AddWithValue("@Emergencynumber", Reservation_Save.Emergencynumber);
                cmd.Parameters.AddWithValue("@EntryGateCode", Reservation_Save.EntryGateCode);
                cmd.Parameters.AddWithValue("@ParkingPoolHours", Reservation_Save.ParkingPoolHours);
                cmd.Parameters.AddWithValue("@ParkingNumberofspaces", Reservation_Save.ParkingNumberofspaces);
                cmd.Parameters.AddWithValue("@ParkingFitnessCenterHours", Reservation_Save.ParkingFitnessCenterHours);
                cmd.Parameters.AddWithValue("@ParkingBusinessCenterHours", Reservation_Save.ParkingBusinessCenterHours);
                cmd.Parameters.AddWithValue("@ParkingAssignedSpace", Reservation_Save.ParkingAssignedSpace);
                cmd.Parameters.AddWithValue("@Housekeeping", Reservation_Save.Housekeeping);
                cmd.Parameters.AddWithValue("@GuestName", Reservation_Save.GuestName);
                cmd.Parameters.AddWithValue("@MailboxLocation", Reservation_Save.MailboxLocation);
                cmd.Parameters.AddWithValue("@MailboxNumber", Reservation_Save.MailboxNumber);
                cmd.Parameters.AddWithValue("@TrashDisposal", Reservation_Save.TrashDisposal);
                cmd.Parameters.AddWithValue("@WifiNetworkName", Reservation_Save.WifiNetworkName);
                cmd.Parameters.AddWithValue("@WifiPassword", Reservation_Save.WifiPassword);

                ;

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    //Select Property
                    SqlCommand cmdStatus = new SqlCommand();
                    cmdStatus.Connection = con;
                    cmdStatus.CommandType = CommandType.Text;
                    cmdStatus.CommandText = "Select PropertyId from Reservations where RId=" + Reservation_Save.RId;

                    SqlDataReader Dr;
                    //con.Open();
                    Dr = cmdStatus.ExecuteReader();
                    DataTable dt = new DataTable("Vw");
                    dt.Load(Dr);

                    //Update Status    
                    cmdStatus = new SqlCommand();
                    cmdStatus.Connection = con;
                    cmdStatus.CommandText = "update Properties set Status='Leased', Leased=true where PropertyId=" + Int64.Parse(dt.Rows[0]["PropertyId"].ToString());
                    cmdStatus.CommandType = CommandType.Text;
                    cmdStatus.ExecuteNonQuery();


                    Session["SuccessMessage"] = "Success: Arrival Successfully Updated";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["Message"] = e.Message;
                }


            }
            con.Close();

            if (Session["error"] != null)
            {
                ViewData["Message"] = Session["error"];
                ViewBag.ContactType = DropDownListContantType("");
                ViewBag.LeaseTerm = DropDownListLeaseTerm("");
                ViewBag.City = DropDownListCity("");
                ViewBag.ReferelSource = DropDownListReferelSource("");
                return View(Reservation_Save);
            }
            else
            {
                ViewData["SuccessMessage"] = Session["SuccessMessage"];
                return RedirectToAction("Reservations");
            }


        }

        /// <summary>
        /// ///Departure
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult AddDeparture(Int64 Id = 0)
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
            SqlCommand cmd = new SqlCommand("ReservationsSelect", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Reservation Reservation_Single = new Reservation();
            Reservation Reservation_Detail = new Reservation();

            List<Reservation> Reservation_List = new List<Reservation>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dt.Rows.Count > 0)
            {

                Reservation_Single.RId = Int16.Parse(dt.Rows[0]["RId"].ToString());
                Reservation_Single.DepartureInstructions = "Q-" + Int64.Parse(dt.Rows[0]["QouteId"].ToString()) + " Departure Instructions";//dt.Rows[0]["DepartureInstructions"].ToString();
                //Reservation_Single.CheckOutTime = DateTime.Parse(dt.Rows[0]["CheckOutTime"].ToString());
                Reservation_Single.CheckOutTime = DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString());

                TimeSpan ts = new TimeSpan(16, 30, 0);
                Reservation_Single.CheckOutTime = Reservation_Single.CheckOutTime.Value.Date + ts;
                //Reservation_Single.CheckOutTime = DateTime.Parse(Reservation_Single.CheckOutTime.Value.Day + "/" + Reservation_Single.CheckOutTime.Value.Month + "/" + Reservation_Single.CheckOutTime.Value.Year + "04:00:00 PM");
                Reservation_Single.property = BindDataPropertyAll(0).Where(x => x.PropertyId == Int64.Parse(dt.Rows[0]["PropertyId"].ToString())).FirstOrDefault();


            }
            else
            {
                Reservation_Single = new Reservation()
                {
                    RId = 0,
                    DepartureInstructions = "",
                    CheckOutTime = DateTime.Now.Date,

                };

            }



            return View(Reservation_Single);
        }

        [HttpPost]
        public ActionResult AddDeparture(FormCollection model)
        {
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["error"] = null;
            Reservation Reservation_Save = new Reservation();

            Reservation_Save.RId = Int64.Parse(model["RId"]);
            //Reservation_Save.RId = Int64.Parse(model["RId"].ToString());

            Reservation_Save.DepartureInstructions = model["DepartureInstructions"].ToString();

            try
            {

                Reservation_Save.CheckOutTime = DateTime.Parse(model["CheckOutTime"]);

            }
            catch (Exception ex)
            {

                string date = model["CheckOutTime"].ToString();
                string mon = date.Substring(0, 2);
                string da = date.Substring(3, 2);
                string ye = date.Substring(6, 4);
                //date = da + "/" + mon + "/" + ye;
                System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
                Reservation_Save.CheckOutTime = DateTime.ParseExact(date, "MM/dd/yyyy", provider);

            }

            //Reservation_Save.CheckOutTime = DateTime.Parse(model["CheckOutTime"].ToString());



            //if (Session["LoginUserRole"] != null)
            //{
            //    //vwAppUserAsset_Save.last_update_user_id = Session["LoginUserEmail"].ToString();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "DepartureUpdate";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@RId", Reservation_Save.RId);

                cmd.Parameters.AddWithValue("@DepartureInstructions", Reservation_Save.DepartureInstructions);
                cmd.Parameters.AddWithValue("@CheckOutTime", Reservation_Save.CheckOutTime);


                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    //Select Property
                    SqlCommand cmdStatus = new SqlCommand();
                    cmdStatus.CommandType = CommandType.Text;
                    cmdStatus.CommandText = "Select PropertyId from Reservations where RId=" + Reservation_Save.RId;

                    SqlDataReader Dr;
                    con.Open();
                    Dr = cmdStatus.ExecuteReader();
                    DataTable dt = new DataTable("Vw");
                    dt.Load(Dr);

                    //Update Status    
                    cmdStatus = new SqlCommand();
                    cmdStatus.CommandText = "update Properties set Status='Vacant', Leased=true where PropertyId=" + Int64.Parse(dt.Rows[0]["PropertyId"].ToString());
                    cmdStatus.CommandType = CommandType.Text;
                    cmdStatus.ExecuteNonQuery();

                    Session["SuccessMessage"] = "Success: Departure Successfully Updated";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["Message"] = e.Message;
                }


            }
            con.Close();

            if (Session["error"] != null)
            {
                ViewData["Message"] = Session["error"];
                ViewBag.ContactType = DropDownListContantType("");
                ViewBag.LeaseTerm = DropDownListLeaseTerm("");
                ViewBag.City = DropDownListCity("");
                ViewBag.ReferelSource = DropDownListReferelSource("");
                return View(Reservation_Save);
            }
            else
            {
                ViewData["SuccessMessage"] = Session["SuccessMessage"];
                return RedirectToAction("Reservations");
            }


        }


        /// <summary>
        /// ///Companys
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult Company(decimal id = 0, string srchContactName = "", string srchOccupantName = "")
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
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

            return View(BindDataCompanyAll(id, srchContactName, srchOccupantName));
        }

        public ActionResult AddCompany(FormCollection a, Int64 Id = 0)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View(BindDataCompany(Id));
        }

        [HttpPost]
        public ActionResult AddCompany(FormCollection model)
        {
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["error"] = null;
            Company Company_Save = new Company();

            Company_Save.CompanyId = Int64.Parse(model["CompanyId"]);

            //HttpPostedFileBase file = Request.Files[0];
            //if (file != null)
            //{ 
            //    string path = Server.MapPath("~/Uploads/");
            //    if (!Directory.Exists(path))
            //    {
            //        Directory.CreateDirectory(path);
            //    }

            //    file.SaveAs(path + Path.GetFileName(file.FileName));

            //    ViewBag.Message = "File uploaded successfully.";
            //}

            Company_Save.CompanyEmail = model["CompanyEmail"].ToString();
            Company_Save.CompanyId = Int64.Parse(model["CompanyId"].ToString());

            Company_Save.City = "";//model["City"].ToString();   Removed by Client

            //Again added by Shahahb
            Company_Save.City = model["City"].ToString();

            Company_Save.CompanyContact = model["CompanyContact"].ToString();
            Company_Save.Address = model["Address"].ToString();
            //Added by Shahab
            Company_Save.Address2 = model["Address2"].ToString();
            Company_Save.Website = model["Website"].ToString();
            Company_Save.CompanyName = model["CompanyName"].ToString();
            Company_Save.PreferedArea = "";//model["PreferedArea"].ToString(); Removed by Client
            Company_Save.State = model["State"].ToString();
            if (model["Zip"] == null || model["Zip"] == "")
            {
                Company_Save.Zip = 0;
            }
            else
            {
                Company_Save.Zip = decimal.Parse(model["Zip"].ToString());
            }



            Company_Save.IsActive = true;


            //if (Session["LoginUserRole"] != null)
            //{
            //    //vwAppUserAsset_Save.last_update_user_id = Session["LoginUserEmail"].ToString();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                if (Company_Save.CompanyId == 0)
                {
                    if (USNumberValidator(Company_Save.CompanyContact))
                    {
                        if (WebSiteValidator(Company_Save.Website))
                        {
                            if (EmailValidator(Company_Save.CompanyEmail))
                            {
                                if (!DuplicateAsset(Company_Save.CompanyName, Company_Save.Website))
                                {
                                    cmd.CommandText = "CompanyInsert";
                                    cmd.CommandType = CommandType.StoredProcedure;


                                    cmd.Parameters.AddWithValue("@City", Company_Save.City);
                                    cmd.Parameters.AddWithValue("@Address", Company_Save.Address);
                                    cmd.Parameters.AddWithValue("@Address2", Company_Save.Address2);
                                    cmd.Parameters.AddWithValue("@CompanyEmail", Company_Save.CompanyEmail);

                                    cmd.Parameters.AddWithValue("@CompanyContact", Company_Save.CompanyContact);

                                    cmd.Parameters.AddWithValue("@CompanyName", Company_Save.CompanyName);
                                    cmd.Parameters.AddWithValue("@PreferedArea", Company_Save.PreferedArea);
                                    cmd.Parameters.AddWithValue("@IsActive", Company_Save.IsActive);
                                    cmd.Parameters.AddWithValue("@Website", Company_Save.Website);
                                    cmd.Parameters.AddWithValue("@Zip", Company_Save.Zip);
                                    cmd.Parameters.AddWithValue("@State", Company_Save.State);

                                    try
                                    {
                                        con.Open();
                                        cmd.ExecuteNonQuery();
                                        Session["error"] = null;
                                        Session["SuccessMessage"] = "Success: Company Successfully Added";
                                    }
                                    catch (SqlException e)
                                    {

                                        ViewBag.error = "Transaction Failure";
                                        Session["error"] = ViewBag.error;
                                        Session["Message"] = e.Message;
                                    }
                                }
                                else
                                {
                                    ViewBag.Message = "Company Name with same WebSite is already exist!";
                                    Session["Message"] = ViewBag.Message;
                                    Session["error"] = ViewBag.Message;
                                }
                            }
                            else
                            {
                                ViewBag.Message = "Email is Incorrect!";
                                Session["Message"] = ViewBag.Message;
                                Session["error"] = ViewBag.Message;
                            }
                        }
                        else
                        {
                            ViewBag.Message = "Website is Incorrect!";
                            Session["Message"] = ViewBag.Message;
                            Session["error"] = ViewBag.Message;
                        }

                    }
                    else
                    {
                        ViewBag.Message = "Number is in a wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                        Session["Message"] = ViewBag.Message;
                        Session["error"] = ViewBag.Message;
                    }
                }

                else
                {
                    if (WebSiteValidator(Company_Save.Website))
                    {
                        if (EmailValidator(Company_Save.CompanyEmail))
                        {
                            if (USNumberValidator(Company_Save.CompanyContact))
                            {
                                cmd.CommandText = "CompanyUpdate";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@City", Company_Save.City);
                                cmd.Parameters.AddWithValue("@Address", Company_Save.Address);
                                cmd.Parameters.AddWithValue("@Address2", Company_Save.Address2);
                                cmd.Parameters.AddWithValue("@CompanyEmail", Company_Save.CompanyEmail);

                                cmd.Parameters.AddWithValue("@CompanyContact", Company_Save.CompanyContact);

                                cmd.Parameters.AddWithValue("@CompanyName", Company_Save.CompanyName);
                                cmd.Parameters.AddWithValue("@PreferedArea", Company_Save.PreferedArea);
                                //cmd.Parameters.AddWithValue("@IsActive", Company_Save.IsActive);
                                cmd.Parameters.AddWithValue("@Website", Company_Save.Website);
                                cmd.Parameters.AddWithValue("@Zip", Company_Save.Zip);
                                cmd.Parameters.AddWithValue("@State", Company_Save.State);
                                cmd.Parameters.AddWithValue("@CompanyId", Company_Save.CompanyId);

                                try
                                {
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    Session["SuccessMessage"] = "Success: Company Successfully Updated";
                                }
                                catch (SqlException e)
                                {

                                    ViewBag.error = "Transaction Failure";
                                    Session["Message"] = e.Message;
                                }
                            }
                            else
                            {
                                ViewBag.Message = "Number is in a wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                                Session["Message"] = ViewBag.Message;
                                Session["error"] = ViewBag.Message;
                            }
                        }
                        else
                        {
                            ViewBag.Message = "Email is Incorrect!";
                            Session["Message"] = ViewBag.Message;
                            Session["error"] = ViewBag.Message;
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Website is Incorrect!";
                        Session["Message"] = ViewBag.Message;
                        Session["error"] = ViewBag.Message;
                    }
                }

            }
            con.Close();

            if (Session["error"] != null)
            {
                ViewData["Message"] = Session["error"];
                ViewBag.City = DropDownListCity("");
                ViewBag.States = BindDataStatesAll(0);
                return View(Company_Save);
            }
            else
            {
                ViewData["SuccessMessage"] = Session["SuccessMessage"];
                return RedirectToAction("Company");
            }


        }

        public ActionResult DeleteCompany(decimal id = 0)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"CompanyDelete";

                cmd.Parameters.AddWithValue("@CompanyId", id);

                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Company Deleted Completely Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Company");
        }

        public ActionResult ActiveInactiveCompany(decimal id = 0, int IsActive = 1)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"CompanyEnableDisable";

                cmd.Parameters.AddWithValue("@CompanyId", id);
                if (IsActive == 1)
                {
                    cmd.Parameters.AddWithValue("@IsActive", true);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsActive", false);
                }

                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Company Updated Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Company");
        }

        /// <summary>
        /// ///Contacts
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult Contact(decimal id = 0, string srchContactName = "", string srchOccupantName = "")
        {

            //string connect = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
            //SqlConnection con = new SqlConnection("Server =.\\MSSQLSERVER19; Initial Catalog = simplicity; Persist Security Info = False; User ID = sa; Password = sa; MultipleActiveResultSets = False; Encrypt = false; TrustServerCertificate = true; Connection Timeout = 200");
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
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

            return View(BindDataContactAll(id));
        }

        public ActionResult AddContact(FormCollection a, Int64 Id = 0)
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
            SqlCommand cmd = new SqlCommand("ContactsSelect", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Contact Contact_Single = new Contact();
            Contact Contact_Detail = new Contact();
            JsonResult jR = new JsonResult();
            List<Contact> Contact_List = new List<Contact>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dt.Rows.Count > 0)
            {
                Contact_Single.Email = dt.Rows[0]["Email"].ToString();
                Contact_Single.Id = Int64.Parse(dt.Rows[0]["Id"].ToString());

                Contact_Single.Phone = dt.Rows[0]["Phone"].ToString();
                Contact_Single.Address = dt.Rows[0]["Address"].ToString();
                Contact_Single.FirstName = dt.Rows[0]["FirstName"].ToString();
                Contact_Single.LastName = dt.Rows[0]["LastName"].ToString();
                Contact_Single.IsActive = bool.Parse(dt.Rows[0]["IsActive"].ToString());
                Contact_Single.DOB = DateTime.Parse(dt.Rows[0]["DOB"].ToString());
                Contact_Single.Company = dt.Rows[0]["Company"].ToString();
                Contact_Single.CompanyId = Int64.Parse(dt.Rows[0]["CompanyId"].ToString());
                Contact_Single.Address2 = dt.Rows[0]["Address2"].ToString();
                Contact_Single.City = dt.Rows[0]["City"].ToString();
                Contact_Single.State = dt.Rows[0]["State"].ToString();


                if (dt.Rows[0]["Zip"] == null || dt.Rows[0]["Zip"].ToString() == "" || Int64.Parse(dt.Rows[0]["Zip"].ToString()) == 0)
                {
                    Contact_Single.Zip = null;
                }
                else
                {
                    Contact_Single.Zip = Int64.Parse(dt.Rows[0]["Zip"].ToString());
                }
                //Contact_Single.Zip = decimal.Parse( dt.Rows[0]["Zip"].ToString());


                //ViewBag.City = DropDownListCity(dt.Rows[0]["City"].ToString());
                ViewBag.States = BindDataStatesAll(0);
            }
            else
            {
                Contact_Single = new Contact()
                {
                    Id = 0,
                    Email = "",


                    Phone = "",
                    FirstName = "",
                    IsActive = true,
                    LastName = "",

                    Address = "",
                    DOB = DateTime.Now,
                    Company = "",

                    CompanyId = 0,
                    Zip = 75581,
                    State = "TX",
                    City = "Houston",
                    Address2 = ""


                };
                ViewBag.City = DropDownListCity("");
                ViewBag.States = BindDataStatesAll(0);
            }


            cmd = new SqlCommand("CompanySelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader DrCompany;
            con.Open();
            DrCompany = cmd.ExecuteReader();
            DataTable dtCompany = new DataTable("Vw");
            dtCompany.Load(DrCompany);

            con.Close();

            Company Company_Single = new Company();
            Company Company_Detail = new Company();

            List<Company> Company_List = new List<Company>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dtCompany.Rows.Count > 0)
            {
                for (int i = 0; i < dtCompany.Rows.Count; i++)
                {

                    //Company_Single.Address = dt.Rows[i]["Address"].ToString();
                    Company_Single.CompanyEmail = dtCompany.Rows[i]["CompanyEmail"].ToString();
                    Company_Single.CompanyId = Int64.Parse(dtCompany.Rows[i]["CompanyId"].ToString());

                    Company_Single.City = dtCompany.Rows[i]["City"].ToString();
                    Company_Single.CompanyContact = dtCompany.Rows[i]["CompanyContact"].ToString();
                    Company_Single.Address = dtCompany.Rows[i]["Address"].ToString();
                    Company_Single.Website = dtCompany.Rows[i]["Website"].ToString();
                    Company_Single.CompanyName = dtCompany.Rows[i]["CompanyName"].ToString();
                    Company_Single.PreferedArea = dtCompany.Rows[i]["PreferedArea"].ToString();
                    Company_Single.State = "";


                    Company_Single.IsActive = bool.Parse(dtCompany.Rows[i]["IsActive"].ToString());

                    Company_List.Add(Company_Single);
                    Company_Single = new Company();

                }
            }

            Company_List = Company_List.Where(x => x.IsActive == true).ToList();
            Tuple<Contact, IEnumerable<Company>> tuple = new Tuple<Contact, IEnumerable<Company>>(Contact_Single, Company_List);
            //tuple = (Contact_Single,Company_List);
            return View(tuple);
        }

        [HttpPost]
        public ActionResult AddContact(FormCollection model)
        {
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            Session["error"] = null;
            Contact Contact_Save = new Contact();

            Contact_Save.Id = Int64.Parse(model["Item1.Id"]);


            Contact_Save.Email = model["Item1.Email"].ToString();
            Contact_Save.Id = Int64.Parse(model["Item1.Id"].ToString());

            Contact_Save.Phone = model["Item1.Phone"].ToString();
            Contact_Save.Address = model["Item1.Address"].ToString();
            Contact_Save.FirstName = model["Item1.FirstName"].ToString();
            Contact_Save.LastName = model["Item1.LastName"].ToString();
            Contact_Save.IsActive = true;
            Contact_Save.Company = model["Item1.Company"].ToString();
            Contact_Save.CompanyId = Int64.Parse(model["Item1.CompanyId"].ToString());

            Contact_Save.Address2 = model["Item1.Address2"].ToString();
            Contact_Save.City = model["Item1.City"].ToString();
            Contact_Save.State = model["Item1.State"].ToString();
            //Contact_Save.Zip = decimal.Parse(model["Item1.Zip"].ToString());
            if (model["Item1.Zip"] == null || model["Item1.Zip"].ToString() == "")
            {
                Contact_Save.Zip = 0;
            }
            else
            {
                Contact_Save.Zip = Int64.Parse(model["Item1.Zip"].ToString());
            }

            //Contact_Save.DOB = DateTime.Parse(model["DOB"].ToString());
            try
            {

                Contact_Save.DOB = DateTime.Parse(model["Item1.DOB"]);

            }
            catch (Exception ex)
            {

                string date = model["Item1.DOB"].ToString();
                string mon = date.Substring(0, 2);
                string da = date.Substring(3, 2);
                string ye = date.Substring(6, 4);
                //date = da + "/" + mon + "/" + ye;
                System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
                Contact_Save.DOB = DateTime.ParseExact(date, "MM/dd/yyyy", provider);

            }

            //if (Session["LoginUserRole"] != null)
            //{
            //    //vwAppUserAsset_Save.last_update_user_id = Session["LoginUserEmail"].ToString();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                if (Contact_Save.Id == 0)
                {
                    if (USNumberValidator(Contact_Save.Phone))
                    {
                        if (EmailValidator(Contact_Save.Email))
                        {
                            if (!DuplicateCommon("Contacts", "Id", "FirstName", "Company", Contact_Save.FirstName, Contact_Save.Company))
                            {

                                cmd.CommandText = "ContactsInsert";
                                cmd.CommandType = CommandType.StoredProcedure;



                                cmd.Parameters.AddWithValue("@Address", Contact_Save.Address);
                                cmd.Parameters.AddWithValue("@Email", Contact_Save.Email);

                                cmd.Parameters.AddWithValue("@Phone", Contact_Save.Phone);

                                cmd.Parameters.AddWithValue("@FirstName", Contact_Save.FirstName);
                                //cmd.Parameters.AddWithValue("@ContactId", Contact_Save.ContactId);
                                cmd.Parameters.AddWithValue("@LastName", Contact_Save.LastName);
                                cmd.Parameters.AddWithValue("@IsActive", Contact_Save.IsActive);
                                cmd.Parameters.AddWithValue("@DOB", Contact_Save.DOB);
                                cmd.Parameters.AddWithValue("@Company", Contact_Save.Company);
                                cmd.Parameters.AddWithValue("@CompanyId", Contact_Save.CompanyId);

                                cmd.Parameters.AddWithValue("@Address2", Contact_Save.Address2);
                                cmd.Parameters.AddWithValue("@City", Contact_Save.City);
                                cmd.Parameters.AddWithValue("@State", Contact_Save.State);
                                cmd.Parameters.AddWithValue("@Zip", Contact_Save.Zip);

                                try
                                {
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    Session["error"] = null;
                                    Session["SuccessMessage"] = "Success: Contact Successfully Added";
                                }
                                catch (SqlException e)
                                {

                                    ViewBag.error = "Transaction Failure";
                                    Session["error"] = ViewBag.error;
                                    Session["Message"] = e.Message;
                                }
                            }
                            else
                            {
                                ViewBag.Message = "First Name with same Company is already exists!";
                                Session["Message"] = ViewBag.Message;
                                Session["error"] = ViewBag.Message;
                            }

                        }
                        else
                        {
                            ViewBag.Message = "Email is Incorrect!";
                            Session["Message"] = ViewBag.Message;
                            Session["error"] = ViewBag.Message;
                        }

                    }
                    else
                    {
                        ViewBag.Message = "Phone Number is in a Wrong Pattern! Please Follow this Pattern (234)-123-4657 or +1-212-456-7890";
                        Session["Message"] = ViewBag.Message;
                        Session["error"] = ViewBag.Message;
                    }
                }

                else
                {
                    if (USNumberValidator(Contact_Save.Phone))
                    {
                        if (EmailValidator(Contact_Save.Email))
                        {

                            cmd.CommandText = "ContactsUpdate";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Address", Contact_Save.Address);
                            cmd.Parameters.AddWithValue("@Email", Contact_Save.Email);

                            cmd.Parameters.AddWithValue("@Phone", Contact_Save.Phone);

                            cmd.Parameters.AddWithValue("@FirstName", Contact_Save.FirstName);
                            cmd.Parameters.AddWithValue("@Id", Contact_Save.Id);
                            cmd.Parameters.AddWithValue("@LastName", Contact_Save.LastName);
                            //cmd.Parameters.AddWithValue("@IsActive", Contact_Save.IsActive);
                            cmd.Parameters.AddWithValue("@DOB", Contact_Save.DOB);
                            cmd.Parameters.AddWithValue("@Company", Contact_Save.Company);
                            cmd.Parameters.AddWithValue("@CompanyId", Contact_Save.CompanyId);


                            cmd.Parameters.AddWithValue("@Address2", Contact_Save.Address2);
                            cmd.Parameters.AddWithValue("@City", Contact_Save.City);
                            cmd.Parameters.AddWithValue("@State", Contact_Save.State);
                            cmd.Parameters.AddWithValue("@Zip", Contact_Save.Zip);


                            try
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                                Session["SuccessMessage"] = "Success: Contact Successfully Updated";
                            }
                            catch (SqlException e)
                            {

                                ViewBag.error = "Transaction Failure";
                                Session["Message"] = e.Message;
                            }
                        }
                        else
                        {
                            ViewBag.Message = "Email is Incorrect!";
                            Session["Message"] = ViewBag.Message;
                            Session["error"] = ViewBag.Message;
                        }

                    }
                    else
                    {
                        ViewBag.Message = "Phone Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                        Session["Message"] = ViewBag.Message;
                        Session["error"] = ViewBag.Message;
                    }

                }

            }
            con.Close();

            if (Session["error"] != null)
            {
                ViewData["Message"] = Session["error"];
                ViewBag.City = DropDownListCity("");
                ViewBag.States = BindDataStatesAll(0);

                SqlCommand cmd = new SqlCommand("CompanySelectAll", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader DrCompany;
                con.Open();
                DrCompany = cmd.ExecuteReader();
                DataTable dtCompany = new DataTable("Vw");
                dtCompany.Load(DrCompany);

                con.Close();

                Company Company_Single = new Company();
                Company Company_Detail = new Company();

                List<Company> Company_List = new List<Company>();
                //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

                if (dtCompany.Rows.Count > 0)
                {
                    for (int i = 0; i < dtCompany.Rows.Count; i++)
                    {

                        //Company_Single.Address = dt.Rows[i]["Address"].ToString();
                        Company_Single.CompanyEmail = dtCompany.Rows[i]["CompanyEmail"].ToString();
                        Company_Single.CompanyId = Int64.Parse(dtCompany.Rows[i]["CompanyId"].ToString());

                        Company_Single.City = dtCompany.Rows[i]["City"].ToString();
                        Company_Single.CompanyContact = dtCompany.Rows[i]["CompanyContact"].ToString();
                        Company_Single.Address = dtCompany.Rows[i]["Address"].ToString();
                        Company_Single.Website = dtCompany.Rows[i]["Website"].ToString();
                        Company_Single.CompanyName = dtCompany.Rows[i]["CompanyName"].ToString();
                        Company_Single.PreferedArea = dtCompany.Rows[i]["PreferedArea"].ToString();
                        Company_Single.State = "";

                        Company_Single.IsActive = bool.Parse(dtCompany.Rows[i]["IsActive"].ToString());

                        Company_List.Add(Company_Single);
                        Company_Single = new Company();

                    }
                }


                Tuple<Contact, IEnumerable<Company>> tuple = new Tuple<Contact, IEnumerable<Company>>(Contact_Save, Company_List.Where(x => x.IsActive = true));
                //tuple = (Contact_Single,Company_List);
                return View(tuple);
                //return View(Contact_Save);
            }
            else
            {
                ViewData["SuccessMessage"] = Session["SuccessMessage"];
                return RedirectToAction("Contact");
            }


        }

        public ActionResult DeleteContact(decimal id = 0)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"ContactDelete";

                cmd.Parameters.AddWithValue("@Id", id);

                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Contact Deleted Completely Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Contact");
        }

        public ActionResult ActiveInactiveContact(decimal id = 0, int IsActive = 1)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"ContactsEnableDisable";

                cmd.Parameters.AddWithValue("@Id", id);
                if (IsActive == 1)
                {
                    cmd.Parameters.AddWithValue("@IsActive", true);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsActive", false);
                }
                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Contact Updated Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Contact");
        }

        /// <summary>
        /// /Property Contact
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult PropContact()
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            IEnumerable<PropContact> propContactList = BindDataPropContactAll(0);

            PropContact propcontact = new PropContact();

            Tuple<PropContact, IEnumerable<PropContact>> tuple = new Tuple<PropContact, IEnumerable<PropContact>>(propcontact, propContactList);

            return View(tuple);
        }

        [HttpPost]
        public ActionResult AddPropContact(FormCollection model)
        {
            Session["Message"] = "";
            Session["SuccessMessage"] = "";

            PropContact Contact_Save = new PropContact();

            Contact_Save.PropContactId = Int64.Parse(model["PropContactId"]);

            Contact_Save.PropContactEmail = model["PropContactEmail"].ToString();
            Contact_Save.PropertyId = Int64.Parse(model["PropertyId"].ToString());

            Contact_Save.PropContactNumber = model["PropContactNumber"].ToString();

            Contact_Save.PropContactFirstName = model["PropContactFirstName"].ToString();
            Contact_Save.PropContactLastName = model["PropContactLastName"].ToString();


            //if (Session["LoginUserRole"] != null)
            //{
            //    //vwAppUserAsset_Save.last_update_user_id = Session["LoginUserEmail"].ToString();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                if (Contact_Save.PropContactId == 0)
                {
                    if (USNumberValidator(Contact_Save.PropContactNumber))
                    {
                        if (EmailValidator(Contact_Save.PropContactEmail))
                        {
                            if (!DuplicateCommon("PropContact", "PropContactId", "PropContactFirstName", "PropertyId", Contact_Save.PropContactFirstName, Contact_Save.PropertyId.ToString()))
                            {

                                cmd.CommandText = "PropContactInsert";
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.AddWithValue("@PropContactEmail", Contact_Save.PropContactEmail);

                                cmd.Parameters.AddWithValue("@PropContactNumber", Contact_Save.PropContactNumber);

                                cmd.Parameters.AddWithValue("@PropContactFirstName", Contact_Save.PropContactFirstName);

                                cmd.Parameters.AddWithValue("@PropContactLastName", Contact_Save.PropContactLastName);

                                cmd.Parameters.AddWithValue("@PropertyId", Contact_Save.PropertyId);

                                try
                                {
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    Session["error"] = null;
                                    Session["SuccessMessage"] = "Success: Contact Successfully Added";
                                }
                                catch (SqlException e)
                                {

                                    ViewBag.error = "Transaction Failure";
                                    Session["error"] = ViewBag.error;
                                    Session["Message"] = e.Message;
                                }
                            }
                            else
                            {
                                ViewBag.Message = "First Name with same Company is already exists!";
                                Session["Message"] = ViewBag.Message;
                                Session["error"] = ViewBag.Message;
                            }

                        }
                        else
                        {
                            ViewBag.Message = "Email is Incorrect!";
                            Session["Message"] = ViewBag.Message;
                            Session["error"] = ViewBag.Message;
                        }

                    }
                    else
                    {
                        ViewBag.Message = "Phone Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                        Session["Message"] = ViewBag.Message;
                        Session["error"] = ViewBag.Message;
                    }
                }

                else
                {
                    cmd.CommandText = "PropContactUpdate";
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("@PropContactEmail", Contact_Save.PropContactEmail);

                    cmd.Parameters.AddWithValue("@PropContactNumber", Contact_Save.PropContactNumber);

                    cmd.Parameters.AddWithValue("@PropContactFirstName", Contact_Save.PropContactFirstName);

                    cmd.Parameters.AddWithValue("@PropContactLastName", Contact_Save.PropContactLastName);

                    cmd.Parameters.AddWithValue("@PropertyId", Contact_Save.PropertyId);

                    cmd.Parameters.AddWithValue("@PropContactId", Contact_Save.PropContactId);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        Session["SuccessMessage"] = "Success: Contact Successfully Updated";
                    }
                    catch (SqlException e)
                    {

                        ViewBag.error = "Transaction Failure";
                        Session["Message"] = e.Message;
                    }
                }

            }
            con.Close();
            return RedirectToAction("AddProperty");
        }

        public ActionResult DeletePropContact(decimal id = 0, Int64 propertyId = 0)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"PropContactDelete";

                cmd.Parameters.AddWithValue("@PropContactId", id);

                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Contact Deleted Completely Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("AddProperty", new { id = propertyId });
            //return Redirect("/Setup/AddProperty/" + propertyId);
        }

        public ActionResult ActiveInactiveProperty(decimal id = 0, int IsActive = 1)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"PropertiesEnableDisable";

                cmd.Parameters.AddWithValue("@Id", id);
                if (IsActive == 1)
                {
                    cmd.Parameters.AddWithValue("@IsActive", true);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsActive", false);
                }
                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Property Updated Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Properties");
        }

        public ActionResult DeleteVendContact(decimal id = 0, Int64 VendorId = 0)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = @"VendContactDelete";

                cmd.Parameters.AddWithValue("@VendContactId", id);

                //
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Session["SuccessMessage"] = "Contact Deleted Completely Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("AddVendor", new { id = VendorId });
            //return Redirect("/Setup/AddVenderty/" + VendertyId);
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
        public ActionResult Edit(decimal id = 0)
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            Vendor Vendor_Single = new Vendor();

            return View("AddVendor", Vendor_Single);
        }

        public ActionResult AlertClose()
        {
            if (Session["LoginUserRole"] != null)
            {

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            Session["SuccessMessage"] = "";
            return RedirectToAction("SensorsList");
        }

        public ActionResult CancelVendor(decimal id = 0)
        {

            Session["error"] = "";
            Session["Message"] = "";
            Session["SuccessMessage"] = "";
            //return View("AddPartyContact", g_PartyContact_Single);
            return RedirectToAction("Vendors");
        }
        #endregion

        #endregion

        #region Dependent Methods
        [HttpPost]
        [ValidateInput(false)]
        public FileResult ExportReferalSource(string GridHtml)
        {
            return File(Encoding.ASCII.GetBytes(GridHtml), "application/vnd.ms-excel", "ReferalSource-" + DateTime.Now + ".xls");
        }

        [HttpPost]
        [ValidateInput(false)]
        public void Export(Int64 Id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<header class='clearfix'><h1>INVOICE</h1><div id='company' class='clearfix'><div>Company Name</div><div>455 John Tower,<br /> AZ 85004, US</div></div></header>");
            sb.Append("<header class='clearfix'>");
            sb.Append("<h1>INVOICE</h1>");
            sb.Append("<div id='company' class='clearfix'>");
            sb.Append("<div>Company Name</div>");
            sb.Append("<div>455 John Tower,<br /> AZ 85004, US</div>");
            sb.Append("<div>(602) 519-0450</div>");
            sb.Append("<div><a href='mailto:company@example.com'>company@example.com</a></div>");
            sb.Append("</div>");
            sb.Append("<div id='project'>");
            sb.Append("<div><span>PROJECT</span> Website development</div>");
            sb.Append("<div><span>CLIENT</span> John Doe</div>");
            sb.Append("<div><span>ADDRESS</span> 796 Silver Harbour, TX 79273, US</div>");
            sb.Append("<div><span>EMAIL</span> <a href='mailto:john@example.com'>john@example.com</a></div>");
            sb.Append("<div><span>DATE</span> April 13, 2016</div>");
            sb.Append("<div><span>DUE DATE</span> May 13, 2016</div>");
            sb.Append("</div>");
            sb.Append("</header>");
            sb.Append("<main>");
            sb.Append("<table>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th class='service'>SERVICE</th>");
            sb.Append("<th class='desc'>DESCRIPTION</th>");
            sb.Append("<th>PRICE</th>");
            sb.Append("<th>QTY</th>");
            sb.Append("<th>TOTAL</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<tbody>");
            sb.Append("<tr>");
            sb.Append("<td class='service'>Design</td>");
            sb.Append("<td class='desc'>Creating a recognizable design solution based on the company's existing visual identity</td>");
            sb.Append("<td class='unit'>$400.00</td>");
            sb.Append("<td class='qty'>2</td>");
            sb.Append("<td class='total'>$800.00</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan='4'>SUBTOTAL</td>");
            sb.Append("<td class='total'>$800.00</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan='4'>TAX 25%</td>");
            sb.Append("<td class='total'>$200.00</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan='4' class='grand total'>GRAND TOTAL</td>");
            sb.Append("<td class='grand total'>$1,000.00</td>");
            sb.Append("</tr>");
            sb.Append("</tbody>");
            sb.Append("</table>");
            sb.Append("<div id='notices'>");
            sb.Append("<div>NOTICE:</div>");
            sb.Append("<div class='notice'>A finance charge of 1.5% will be made on unpaid balances after 30 days.</div>");
            sb.Append("</div>");
            sb.Append("</main>");
            sb.Append("<footer>");
            sb.Append("Invoice was created on a computer and is valid without the signature and seal.");
            sb.Append("</footer>");



            StringReader sr = new StringReader(sb.ToString());
            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
            //    pdfDoc.Open();

            //    htmlparser.Parse(sr);
            //    pdfDoc.Close();

            //    byte[] bytes = memoryStream.ToArray();
            //    memoryStream.Close();


            //    Response.Clear();
            //    // Gets or sets the HTTP MIME type of the output stream.
            //    Response.ContentType = "application/pdf";
            //    // Adds an HTTP header to the output stream
            //    Response.AddHeader("Content-Disposition", "attachment; filename=Invoice.pdf");

            //    //Gets or sets a value indicating whether to buffer output and send it after
            //    // the complete response is finished processing.
            //    Response.Buffer = true;
            //    // Sets the Cache-Control header to one of the values of System.Web.HttpCacheability.
            //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //    // Writes a string of binary characters to the HTTP output stream. it write the generated bytes .
            //    Response.BinaryWrite(bytes);

            //    // Sends all currently buffered output to the client, stops execution of the
            //    // page, and raises the System.Web.HttpApplication.EndRequest event.
            //    Response.End();
            //    // Closes the socket connection to a client. it is a necessary step as you must close the response after doing work.its best approach.
            //    Response.Close();

            //    //SmtpClient smtpServer = new SmtpClient("smtp.office365.com");

            //    //smtpServer.Port = 587;
            //    //smtpServer.UseDefaultCredentials = false;
            //    //smtpServer.Credentials = new System.Net.NetworkCredential("sql@pegasusresources.com", "Puw4565311!!", "pegasusresources.com"); // new NetworkCredential("adminni", "Qs409ess!") as ICredentialsByHost;
            //    //smtpServer.EnableSsl = true;
            //    //ServicePointManager.ServerCertificateValidationCallback =
            //    //        delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            //    //        { return true; };

            //    ////SmtpClient smtpServer = new SmtpClient("smtp.office365.com");

            //    //smtpServer.Send("nfaruqi@idominium.com", "nt_faruqi@yahoo.com", "Quote1", "hihihi");

            //    //return View();
            //    //return File(Encoding.ASCII.GetBytes(GridHtml), "application/pdf", "ReferalSource-" + DateTime.Now + ".pdf");

            //    //using Microsoft.Office.Interop.Word;

            //}
            word wordApp = new word();
            //Application = wordApp.app;
            //using Range = Microsoft.Office.Interop.Word.Range;

            //wordApp.app = new Microsoft.Office.Interop.Word.Application();
            wordApp.app.Visible = false; //Hide the Word.Application object.
            string path = Server.MapPath("~/Uploads/");
            wordApp.document = wordApp.app.Documents.Open(path + "Texas-Standard-Residential-Lease-Agreement-1.docx");
            string docName = "Q-1_" + "Texas-Standard-Residential-Lease-Agreement.docx";
            wordApp.document.SaveAs2(path + docName);

            wordApp.document.Close(); //close the document.
            wordApp.app.Quit(); //close the Word.Application object. otherwise, you'll get a ReadOnly error later.
                                /////Duplicate Qoute Doc Saved

            ////Current Qoute Doc Load
            //wordApp = new word();
            wordApp.app = new Microsoft.Office.Interop.Word.Application();
            wordApp.app.Visible = false; //Hide the Word.Application object.

            wordApp.document = wordApp.app.Documents.Open(path + docName);

            string bookmark = "LeaseStartDate";
            string Text2 = "Text2";
            string Text3 = "Text3";
            string Text4 = "Text4";
            string Text5 = "Text5";
            string Text6 = "Text6";
            string Text7 = "Text7";
            string Text8 = "Text8";
            string Text9 = "Text9";
            string Text10 = "Text10";
            string Text11 = "Text11";
            string Text12 = "Text12";
            string Text13 = "Text13";
            string Text14 = "Text14";
            string Text15 = "Text15";
            string Text16 = "Text16";
            string Text17 = "Text17";
            string Text18 = "Text18";
            string Text19 = "Text19";
            string Text20 = "Text20";
            string Text21 = "Text21";
            string Text22 = "Text22";
            string Text23 = "Text23";

            string Text24 = "Text24";
            string Text25 = "Text25";

            string Img1 = "Img1";
            string Img2 = "Img2";
            string Img3 = "Img3";
            string Img4 = "Img4";
            string Img5 = "Img5";


            //wordApp.ccl = wordApp.document.ContentControls;
            //wordApp.cc = wordApp.document.ContentControls[""];
            wordApp.bookmarks = wordApp.document.Bookmarks;

            ////Add Book Mark Change
            wordApp.bookmark = wordApp.document.Bookmarks[bookmark];
            //wordApp.range = wordApp.bookmark.Range;
            wordApp.range = wordApp.bookmark.Range.Duplicate;
            //Select the text.
            wordApp.bookmark.Select();

            //Overwrite the selection.
            wordApp.app.Selection.TypeText(DateTime.Now.ToString());

            //wordApp.bookmark.Range.Text = DateTime.Now.ToString();

            wordApp.document.Bookmarks.Add(bookmark, wordApp.range);

            ////End


            ////Add Book Mark Change
            wordApp.bookmark = wordApp.document.Bookmarks[Text2];
            //wordApp.range = wordApp.bookmark.Range;
            wordApp.range = wordApp.bookmark.Range.Duplicate;
            //Select the text.
            wordApp.bookmark.Select();

            //Overwrite the selection.
            wordApp.app.Selection.TypeText("Jorge");

            //wordApp.bookmark.Range.Text = DateTime.Now.ToString();

            wordApp.document.Bookmarks.Add(bookmark, wordApp.range);

            ////End

            ////Add Book Mark Change

            wordApp.range = wordApp.document.Bookmarks[Img1].Range;
            wordApp.range.InlineShapes.AddPicture(path + "bk1-brochure.png");
            wordApp.range.InlineShapes[2].Delete();

            ////End


            object format = wordApp.wdSave;

            wordApp.document.SaveAs2(path + "Texas-Standard-Residential-Lease-Agreement.pdf", format, false);

            //wordApp.document.Save(); //save the document.
            wordApp.document.Close(); //close the document.
            wordApp.app.Quit(); //close the Word.Application object. otherwise, you'll get a ReadOnly error later.


        }

        public void DownloadArrivalDocument(Int64 Id)
        {

            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("sp_Arrival", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            string path = Server.MapPath("~/Uploads/");

            string docName = "R-" + Id.ToString() + "ArrivalInstructions";
            string LeaseStartDate = "LeaseStartDate";
            string Name = "Name";
            string LeaseEndDate = "LeaseEndDate";
            string ParkingNumberofspaces = "ParkingNumberofspaces";
            string PropertyAddress = "PropertyAddress";
            string PropertyAddress2 = "PropertyAddress2";
            StringBuilder sbArrivalInstructions = new StringBuilder();
            if (dt.Rows.Count > 0)
            {
                LeaseStartDate = DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).ToString();

                Name = dt.Rows[0]["Name"].ToString();
                LeaseEndDate = dt.Rows[0]["LeaseEndDate"].ToString();
                ParkingNumberofspaces = dt.Rows[0]["ParkingNumberofspaces"].ToString();

                string CheckinKeyArrangements = dt.Rows[0]["CheckinKeyArrangements"].ToString();

                string EntryGateCode = dt.Rows[0]["EntryGateCode"].ToString();
                string MailboxNumber = dt.Rows[0]["MailboxNumber"].ToString();
                string MailboxLocation = dt.Rows[0]["MailboxLocation"].ToString();
                string TrashDisposal = dt.Rows[0]["TrashDisposal"].ToString();
                string WifiNetworkName = dt.Rows[0]["WifiNetworkName"].ToString();
                string WifiPassword = dt.Rows[0]["WifiPassword"].ToString();
                PropertyAddress = dt.Rows[0]["PropertyAddress"].ToString();

                string Housekeeping = dt.Rows[0]["Housekeeping"].ToString();
                string ParkingAssignedSpace = dt.Rows[0]["ParkingAssignedSpace"].ToString();
                string ParkingBusinessCenterHours = dt.Rows[0]["ParkingBusinessCenterHours"].ToString();
                ///////////////
                string ParkingFitnessCenterHours = dt.Rows[0]["ParkingFitnessCenterHours"].ToString();
                string ParkingPoolHours = dt.Rows[0]["ParkingPoolHours"].ToString();
                string CustomerServiceNumber = dt.Rows[0]["CustomerServiceNumber"].ToString();
                string Emergencynumber = dt.Rows[0]["Emergencynumber"].ToString();
                string GuestName = dt.Rows[0]["GuestName"].ToString();
                string RId = Int64.Parse(dt.Rows[0]["RId"].ToString()).ToString();
                string PhoneNumber1 = dt.Rows[0]["PhoneNumber"].ToString();
                string Floor = dt.Rows[0]["Floor"].ToString();


                sbArrivalInstructions.Append(@"<header class='clearfix'><h1>Arrival Instructions </h1></header>");


                sbArrivalInstructions.Append("<div id='company' class='clearfix' style='font-size:10.0pt;'>");
                sbArrivalInstructions.Append("<div>Reservation/Lease ID: <b>R-" + Id + "</b></div>");
                sbArrivalInstructions.Append("<div>Tenant/Guest Name: <b>" + GuestName + "</b></div>");
                sbArrivalInstructions.Append("<div>Reservation/Lease Start Date: <b>" + LeaseStartDate + "</b></div>");
                sbArrivalInstructions.Append("<div>Reservation/Lease End Date: <b>" + LeaseEndDate + "</b></div>");
                sbArrivalInstructions.Append("<div>Check-in time: <b>4:00 pm</b></div>");
                sbArrivalInstructions.Append("<div>Check-out time: <b>11 am</b></div>");
                sbArrivalInstructions.Append("<div>Property Name: <b>" + Name + "</b></div>");
                sbArrivalInstructions.Append("<div>Property Address: <b>" + PropertyAddress + "</b></div>");
                sbArrivalInstructions.Append("<div>Property Phone Number: <b>" + PhoneNumber1 + "</b></div>");
                sbArrivalInstructions.Append("<div>Floor Level: <b>" + Floor + "</b></div><br />");
                sbArrivalInstructions.Append("<div><b>Community Office Hours:</b></div> <br />");
                sbArrivalInstructions.Append("<div><b>60-day</b> Notice to Vacate is Required.</div>");
                sbArrivalInstructions.Append("<div>Check-in/Key Arrangements: <b>" + CheckinKeyArrangements + "</b></div>");
                sbArrivalInstructions.Append("<div>Entry Gate Code: <b>" + EntryGateCode + "</b></div>");
                sbArrivalInstructions.Append("<div>Mailbox Number: <b>" + MailboxNumber + "</b></div>");
                sbArrivalInstructions.Append("<div>Mailbox Location: <b>" + MailboxLocation + "</b></div>");
                sbArrivalInstructions.Append("<div>Trash Disposal: <b>" + TrashDisposal + "</b></div>");
                sbArrivalInstructions.Append("<div>Wifi Network Name: <b>" + WifiNetworkName + "</b></div>");
                sbArrivalInstructions.Append("<div>Wifi Password: <b>" + WifiPassword + "</b></div>");
                sbArrivalInstructions.Append("<div>Housekeeping: <b>" + Housekeeping + "</b></div><br />");
                sbArrivalInstructions.Append("<div><b>Parking Information:</b></div><br />");
                sbArrivalInstructions.Append("<div>Number of spaces: <b>" + ParkingNumberofspaces + "</b></div>");
                sbArrivalInstructions.Append("<div>Assigned Space: <b>" + ParkingAssignedSpace + "</b></div>");
                sbArrivalInstructions.Append("<div>Business Center Hours: <b>" + ParkingBusinessCenterHours + "</b></div>");
                sbArrivalInstructions.Append("<div>Fitness Center Hours: <b>" + ParkingFitnessCenterHours + "</b></div>");
                sbArrivalInstructions.Append("<div>Pool Hours: <b>" + ParkingPoolHours + "</b></div><br />");
                sbArrivalInstructions.Append("<div>Please call <b>" + CustomerServiceNumber + "</b> for Customer Service Issues during your stay with <b>Keyluxe Suites</b>.</div>");
                sbArrivalInstructions.Append("<div>After Hours Emergency number <b>" + Emergencynumber + "</b>.</div>");

                sbArrivalInstructions.Append("</div>");

                sbArrivalInstructions.Append("<footer>");
                sbArrivalInstructions.Append("Keyluxe Suite Arrival Instructions Document.");
                sbArrivalInstructions.Append("</footer>");

            }

            StringReader sr = new StringReader(sbArrivalInstructions.ToString());
            Document pdfDoc = new Document(PageSize.A3, 10f, 10f, 10f, 0f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();

                htmlparser.Parse(sr);
                pdfDoc.Close();

                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();


                Response.Clear();
                // Gets or sets the HTTP MIME type of the output stream.
                Response.ContentType = "application/pdf";
                // Adds an HTTP header to the output stream
                Response.AddHeader("Content-Disposition", "attachment; filename=" + docName + ".pdf");

                //Gets or sets a value indicating whether to buffer output and send it after
                // the complete response is finished processing.
                Response.Buffer = true;
                // Sets the Cache-Control header to one of the values of System.Web.HttpCacheability.
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                // Writes a string of binary characters to the HTTP output stream. it write the generated bytes .
                Response.BinaryWrite(bytes);

                // Sends all currently buffered output to the client, stops execution of the
                // page, and raises the System.Web.HttpApplication.EndRequest event.
                Response.End();
                // Closes the socket connection to a client. it is a necessary step as you must close the response after doing work.its best approach.
                Response.Close();
            }

        }
        public FileResult DownloadArrivalDocumentWord(Int64 Id)
        {

            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("sp_Arrival", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();


            word wordApp = new word();
            //Application = wordApp.app;
            //using Range = Microsoft.Office.Interop.Word.Range;

            //wordApp.app = new Microsoft.Office.Interop.Word.Application();
            wordApp.app.Visible = false; //Hide the Word.Application object.
            string path = Server.MapPath("~/Uploads/");
            wordApp.document = wordApp.app.Documents.Open(path + "ArrivalInstructions.docx");
            string docName = "R-" + Id.ToString() + "ArrivalInstructions.docx";
            wordApp.document.SaveAs2(path + docName);

            wordApp.document.Close(); //close the document.
            wordApp.app.Quit(); //close the Word.Application object. otherwise, you'll get a ReadOnly error later.
                                /////Duplicate Qoute Doc Saved

            ////Current Qoute Doc Load
            //wordApp = new word();
            wordApp.app = new Microsoft.Office.Interop.Word.Application();
            wordApp.app.Visible = false; //Hide the Word.Application object.

            wordApp.document = wordApp.app.Documents.Open(path + docName);
            string LeaseStartDate = "LeaseStartDate";
            string Name = "Name";
            string LeaseEndDate = "LeaseEndDate";
            string ParkingNumberofspaces = "ParkingNumberofspaces";
            string PropertyAddress = "PropertyAddress";
            string PropertyAddress2 = "PropertyAddress2";

            if (dt.Rows.Count > 0)
            {
                LeaseStartDate = DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;
                List lst = new List();
                for (int i = 1; i <= wordApp.bookmarks.Count; i++)
                {
                    lst.Add(wordApp.bookmarks[i].Name);
                }


                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["LeaseStartDate"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(DateTime.Now.ToString());

                wordApp.document.Bookmarks.Add("LeaseStartDate", wordApp.range);
                /////////////////

                Name = dt.Rows[0]["Name"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["Name"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(Name);

                wordApp.document.Bookmarks.Add("Name", wordApp.range);
                /////////////////////

                LeaseEndDate = dt.Rows[0]["LeaseEndDate"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["LeaseEndDate"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(LeaseEndDate);

                wordApp.document.Bookmarks.Add("LeaseEndDate", wordApp.range);
                /////////////////
                ParkingNumberofspaces = dt.Rows[0]["ParkingNumberofspaces"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["ParkingNumberofspace"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(ParkingNumberofspaces);

                wordApp.document.Bookmarks.Add("ParkingNumberofspace", wordApp.range);
                ///////////////

                string CheckinKeyArrangements = dt.Rows[0]["CheckinKeyArrangements"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["CheckinKeyArrangemen"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(CheckinKeyArrangements);

                wordApp.document.Bookmarks.Add("CheckinKeyArrangemen", wordApp.range);
                ///////////////

                string EntryGateCode = dt.Rows[0]["EntryGateCode"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["EntryGateCode"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(EntryGateCode);

                wordApp.document.Bookmarks.Add("EntryGateCode", wordApp.range);
                ///////////////
                string MailboxNumber = dt.Rows[0]["MailboxNumber"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["MailboxNumber"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(MailboxNumber);

                wordApp.document.Bookmarks.Add("MailboxNumber", wordApp.range);
                ///////////////
                string MailboxLocation = dt.Rows[0]["MailboxLocation"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["MailboxLocation"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(MailboxLocation);

                wordApp.document.Bookmarks.Add("MailboxLocation", wordApp.range);
                ///////////////
                string TrashDisposal = dt.Rows[0]["TrashDisposal"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["TrashDisposal"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(TrashDisposal);

                wordApp.document.Bookmarks.Add("TrashDisposal", wordApp.range);
                ///////////////
                string WifiNetworkName = dt.Rows[0]["WifiNetworkName"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["WifiNetworkName"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(WifiNetworkName);

                wordApp.document.Bookmarks.Add("WifiNetworkName", wordApp.range);
                ///////////////
                string WifiPassword = dt.Rows[0]["WifiPassword"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["WifiPassword"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(WifiPassword);

                wordApp.document.Bookmarks.Add("WifiPassword", wordApp.range);
                ///////////////
                PropertyAddress = dt.Rows[0]["PropertyAddress"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["PropertyAddress"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(PropertyAddress);

                wordApp.document.Bookmarks.Add("PropertyAddress", wordApp.range);
                ///////////////
                ////PropertyAddress2 = dt.Rows[0]["PropertyAddress"].ToString();
                ////wordApp.bookmarks = wordApp.document.Bookmarks;

                ////////Add Book Mark Change
                ////wordApp.bookmark = wordApp.document.Bookmarks["PropertyAddress1"];
                //////wordApp.range = wordApp.bookmark.Range;
                ////wordApp.range = wordApp.bookmark.Range.Duplicate;
                //////Select the text.
                ////wordApp.bookmark.Select();

                //////Overwrite the selection.
                ////wordApp.app.Selection.TypeText(PropertyAddress2);

                ////wordApp.document.Bookmarks.Add("PropertyAddress1", wordApp.range);
                ///////////////////

                string Housekeeping = dt.Rows[0]["Housekeeping"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["Housekeeping"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(Housekeeping);

                wordApp.document.Bookmarks.Add("Housekeeping", wordApp.range);
                ///////////////
                string ParkingAssignedSpace = dt.Rows[0]["ParkingAssignedSpace"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["ParkingAssignedSpace"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(ParkingAssignedSpace);

                wordApp.document.Bookmarks.Add("ParkingAssignedSpace", wordApp.range);
                ///////////////
                string ParkingBusinessCenterHours = dt.Rows[0]["ParkingBusinessCenterHours"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["ParkingBusinessCente"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(ParkingBusinessCenterHours);

                wordApp.document.Bookmarks.Add("ParkingBusinessCente", wordApp.range);
                ///////////////
                string ParkingFitnessCenterHours = dt.Rows[0]["ParkingFitnessCenterHours"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["ParkingFitnessCenter"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(ParkingFitnessCenterHours);

                wordApp.document.Bookmarks.Add("ParkingFitnessCenter", wordApp.range);
                ///////////////
                string ParkingPoolHours = dt.Rows[0]["ParkingPoolHours"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["ParkingPoolHours"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(ParkingPoolHours);

                wordApp.document.Bookmarks.Add("ParkingPoolHours", wordApp.range);
                ///////////////
                string CustomerServiceNumber = dt.Rows[0]["CustomerServiceNumber"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["CustomerServiceNumbe"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(CustomerServiceNumber);

                wordApp.document.Bookmarks.Add("CustomerServiceNumbe", wordApp.range);
                ///////////////
                string Emergencynumber = dt.Rows[0]["Emergencynumber"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["Emergencynumber"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(Emergencynumber);

                wordApp.document.Bookmarks.Add("Emergencynumber", wordApp.range);
                ///////////////
                string GuestName = dt.Rows[0]["GuestName"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["GuestName"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(GuestName);

                wordApp.document.Bookmarks.Add("GuestName", wordApp.range);
                ///////////////
                string RId = Int64.Parse(dt.Rows[0]["RId"].ToString()).ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["RId"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(RId);

                wordApp.document.Bookmarks.Add("RId", wordApp.range);
                ///////////////
                string PhoneNumber1 = dt.Rows[0]["PhoneNumber"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["PhoneNumber"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(PhoneNumber1);

                wordApp.document.Bookmarks.Add("PhoneNumber", wordApp.range);
                ///////////////
                //string PhoneNumber2 = dt.Rows[0]["PhoneNumber"].ToString();
                //wordApp.bookmarks = wordApp.document.Bookmarks;

                //////Add Book Mark Change
                //wordApp.bookmark = wordApp.document.Bookmarks["PhoneNumber1"];
                ////wordApp.range = wordApp.bookmark.Range;
                //wordApp.range = wordApp.bookmark.Range.Duplicate;
                ////Select the text.
                //wordApp.bookmark.Select();

                ////Overwrite the selection.
                //wordApp.app.Selection.TypeText(PhoneNumber2);

                //wordApp.document.Bookmarks.Add("PhoneNumber1", wordApp.range);
                /////////////////
                ///////////////
                string Floor = dt.Rows[0]["Floor"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["Floor"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(Floor);

                wordApp.document.Bookmarks.Add("Floor", wordApp.range);

                ///////////////
                //string Building = dt.Rows[0]["Building"].ToString();
                //wordApp.bookmarks = wordApp.document.Bookmarks;

                //////Add Book Mark Change
                //wordApp.bookmark = wordApp.document.Bookmarks["Building"];
                ////wordApp.range = wordApp.bookmark.Range;
                //wordApp.range = wordApp.bookmark.Range.Duplicate;
                ////Select the text.
                //wordApp.bookmark.Select();

                ////Overwrite the selection.
                //wordApp.app.Selection.TypeText(Building);

                //wordApp.document.Bookmarks.Add("Building", wordApp.range);

            }


            object format = wordApp.wdSave;

            wordApp.document.SaveAs2(path + "R-" + Id.ToString() + "ArrivalInstructions.pdf", format, false);
            wordApp.document.SaveAs2(path + "R-" + Id.ToString() + "ArrivalInstructions.html", wordApp.wdSaveHTML, false);
            wordApp.document.SaveAs2(path + "R-" + Id.ToString() + "ArrivalInstructions.pdf", wordApp.wdSaveOpenDocText, false);
            wordApp.document.SaveAs2(path + "R-" + Id.ToString() + "ArrivalInstructions.xml", wordApp.wdSaveOpenXMLDoc, false);
            wordApp.document.SaveAs2(path + "R-" + Id.ToString() + "ArrivalInstructions.rtf", wordApp.wdSaveRTF, false);
            wordApp.document.SaveAs2(path + "R-" + Id.ToString() + "ArrivalInstructions.txt", wordApp.wdSaveText, false);
            wordApp.document.SaveAs2(path + "R-" + Id.ToString() + "ArrivalInstructions.pdf", wordApp.wdSaveWebArchive, false);
            wordApp.document.SaveAs2(path + "R-" + Id.ToString() + "ArrivalInstructions.xml", wordApp.wdSaveXMLDoc, false);

            //wordApp.document.Save(); //save the document.
            wordApp.document.Close(); //close the document.
            wordApp.app.Quit(); //close the Word.Application object. otherwise, you'll get a ReadOnly error later.


            return File(path + "R-" + Id.ToString() + "ArrivalInstructions.pdf", "text/plain", "R-" + Id.ToString() + "ArrivalInstructions.pdf");


        }
        public void DownloadDepartureDocument(Int64 Id)
        {

            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("sp_Arrival", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();


            string path = Server.MapPath("~/Uploads/");

            string docName = "R-" + Id.ToString() + "DepartureInstructions";
            string Name = "Name";
            string GuestName = "GuestName";
            if (dt.Rows.Count > 0)
            {

                Name = dt.Rows[0]["Name"].ToString();
                GuestName = dt.Rows[0]["GuestName"].ToString();

            }


            StringBuilder sb = new StringBuilder();
            sb.Append(@"<header class='clearfix'><h1>Departure Instructions </h1></header>");


            sb.Append("<div id='company' class='clearfix' style='font-size:10.0pt;'>");
            sb.Append("<div>Guest Occupant Name: <b>" + GuestName + "</b></div>");
            sb.Append("<div>Community Name: <b>" + Name + "</b></div>");
            sb.Append("<div>We hope you enjoyed your stay with Keyluxe Suites! </div>");
            sb.Append(@"<div>Departure Time: 11 am on Departure Date 
Need to Extend ? Please contact"); sb.Append(@"Keyluxe Suites immediately so that we can assist you 214.277.9119.
Key Return: Please return keys to leasing office.If checking out after hours prior key return arrangements must be made.
Please fill out key return checklist and email or text a picture of the completed form. 
Remove all personal items from apartment.
</div>");
            sb.Append(@"<div><p><br />1.	The apartment should be returned in the same condition as when you moved in. 
"); sb.Append(@"<br />2.Please remove all food and groceries from the fridge and pantry.
"); sb.Append(@"<br />3.Turn off lights, ceiling fans and electronics.
"); sb.Append(@"<br />4.In the Winter, please set the thermostat to 59 degrees Fahrenheit before leaving.
"); sb.Append(@"<br />5.In the Summer, please set the thermostat to 85 degrees Fahrenheit before leaving.
"); sb.Append(@"<br />6.Please close and lock doors and windows. 
"); sb.Append(@"<br />7.Return all keys, remotes, fobs or parking passes to property. <b>Please note if all items are not returned return will be charged until returned as well as other charges could apply.</b>
</p></div>");
            sb.Append("</div>");
            sb.Append("<div id='project'>");
            sb.Append(@"<div><br />     a.Additional Rent Days at market rent"); sb.Append(@"<br />     b.Lock Change $300.00.
"); sb.Append(@"<br />     c.Lost or Misplaced Keys $100.00
"); sb.Append(@"<br />     d.Replacement of Fob, Remotes or permit $150.00 per item.
</div>");
            sb.Append(@"<div><br />  8.	Checking out early?  If you vacate early, please contact Keyluxe Suites and let us know as soon as possible so that we can notify the community for security purposes.   
"); sb.Append(@"<br />  9.<b>Please note: If there is damage beyond normal wear and tear or excessive cleaning charges Keyluxe Suites will deduct the charges from your security deposit on file.The security deposit is typically returned within 45 days of move -out.
</b></div>");
            sb.Append("</div>");

            sb.Append("<footer>");
            sb.Append("<br />Keyluxe Suite Detarture Instructions.");
            sb.Append("</footer>");


            StringReader sr = new StringReader(sb.ToString());
            Document pdfDoc = new Document(PageSize.A3, 10f, 10f, 10f, 0f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();

                htmlparser.Parse(sr);
                pdfDoc.Close();

                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();


                Response.Clear();
                // Gets or sets the HTTP MIME type of the output stream.
                Response.ContentType = "application/pdf";
                // Adds an HTTP header to the output stream
                Response.AddHeader("Content-Disposition", "attachment; filename=" + docName + ".pdf");

                //Gets or sets a value indicating whether to buffer output and send it after
                // the complete response is finished processing.
                Response.Buffer = true;
                // Sets the Cache-Control header to one of the values of System.Web.HttpCacheability.
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                // Writes a string of binary characters to the HTTP output stream. it write the generated bytes .
                Response.BinaryWrite(bytes);

                // Sends all currently buffered output to the client, stops execution of the
                // page, and raises the System.Web.HttpApplication.EndRequest event.
                Response.End();
                // Closes the socket connection to a client. it is a necessary step as you must close the response after doing work.its best approach.
                Response.Close();
            }

        }

        private void btnReplaceBookmarkText_Click()
        {

            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("sp_Arrival", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RId", 5);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            string path = Server.MapPath("~/Uploads/");

            string docName = "R-" + "5".ToString() + "ArrivalInstructions.docx";
            string LeaseStartDate = "LeaseStartDate";
            string Name = "Name";
            string LeaseEndDate = "LeaseEndDate";
            string ParkingNumberofspaces = "ParkingNumberofspaces";
            string PropertyAddress = "PropertyAddress";
            string PropertyAddress2 = "PropertyAddress2";
            StringBuilder sbArrivalInstructions = new StringBuilder();
            if (dt.Rows.Count > 0)
            {
                LeaseStartDate = DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).ToString();

                Name = dt.Rows[0]["Name"].ToString();
                LeaseEndDate = dt.Rows[0]["LeaseEndDate"].ToString();
                ParkingNumberofspaces = dt.Rows[0]["ParkingNumberofspaces"].ToString();

                string CheckinKeyArrangements = dt.Rows[0]["CheckinKeyArrangements"].ToString();

                string EntryGateCode = dt.Rows[0]["EntryGateCode"].ToString();
                string MailboxNumber = dt.Rows[0]["MailboxNumber"].ToString();
                string MailboxLocation = dt.Rows[0]["MailboxLocation"].ToString();
                string TrashDisposal = dt.Rows[0]["TrashDisposal"].ToString();
                string WifiNetworkName = dt.Rows[0]["WifiNetworkName"].ToString();
                string WifiPassword = dt.Rows[0]["WifiPassword"].ToString();
                PropertyAddress = dt.Rows[0]["PropertyAddress"].ToString();

                string Housekeeping = dt.Rows[0]["Housekeeping"].ToString();
                string ParkingAssignedSpace = dt.Rows[0]["ParkingAssignedSpace"].ToString();
                string ParkingBusinessCenterHours = dt.Rows[0]["ParkingBusinessCenterHours"].ToString();
                ///////////////
                string ParkingFitnessCenterHours = dt.Rows[0]["ParkingFitnessCenterHours"].ToString();
                string ParkingPoolHours = dt.Rows[0]["ParkingPoolHours"].ToString();
                string CustomerServiceNumber = dt.Rows[0]["CustomerServiceNumber"].ToString();
                string Emergencynumber = dt.Rows[0]["Emergencynumber"].ToString();
                string GuestName = dt.Rows[0]["GuestName"].ToString();
                string RId = Int64.Parse(dt.Rows[0]["RId"].ToString()).ToString();
                string PhoneNumber1 = dt.Rows[0]["PhoneNumber"].ToString();
                string Floor = dt.Rows[0]["Floor"].ToString();

                using (DocumentFormat.OpenXml.Packaging.WordprocessingDocument pkgDoc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(path + docName, true))
                {
                    var document = pkgDoc.MainDocumentPart.Document;
                    foreach (var text in document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
                    {
                        if (text.Text.Contains("[LeaseStartDate]"))
                        {
                            text.Text = text.Text.Replace("[LeaseStartDate]", LeaseStartDate);

                        }
                        else if (text.Text.Contains("[GuestName]"))
                        {
                            text.Text = text.Text.Replace("[GuestName]", GuestName);

                        }
                        else if (text.Text.Contains("[Name]"))
                        {
                            text.Text = text.Text.Replace("[Name]", Name);

                        }
                        else if (text.Text.Contains("[LeaseEndDate]"))
                        {
                            text.Text = text.Text.Replace("[LeaseEndDate]", LeaseEndDate);

                        }
                        else if (text.Text.Contains("[ParkingNumberofspaces]"))
                        {
                            text.Text = text.Text.Replace("[ParkingNumberofspaces]", ParkingNumberofspaces);

                        }
                        else if (text.Text.Contains("[PropertyAddress]"))
                        {
                            text.Text = text.Text.Replace("[PropertyAddress]", PropertyAddress);

                        }
                        else if (text.Text.Contains("[PropertyAddress2]"))
                        {
                            text.Text = text.Text.Replace("[PropertyAddress2]", PropertyAddress2);

                        }
                        else if (text.Text.Contains("[CheckinKeyArrangements]"))
                        {
                            text.Text = text.Text.Replace("[CheckinKeyArrangements]", CheckinKeyArrangements);

                        }
                        else if (text.Text.Contains("[EntryGateCode]"))
                        {
                            text.Text = text.Text.Replace("[EntryGateCode]", EntryGateCode);

                        }
                        else if (text.Text.Contains("[MailboxNumber]"))
                        {
                            text.Text = text.Text.Replace("[MailboxNumber]", MailboxNumber);

                        }
                        else if (text.Text.Contains("[MailboxLocation]"))
                        {
                            text.Text = text.Text.Replace("[MailboxLocation]", MailboxLocation);

                        }
                        else if (text.Text.Contains("[TrashDisposal]"))
                        {
                            text.Text = text.Text.Replace("[TrashDisposal]", TrashDisposal);

                        }
                        else if (text.Text.Contains("[WifiNetworkName]"))
                        {
                            text.Text = text.Text.Replace("[WifiNetworkName]", WifiNetworkName);

                        }
                        else if (text.Text.Contains("[WifiPassword]"))
                        {
                            text.Text = text.Text.Replace("[WifiPassword]", WifiPassword);

                        }
                        else if (text.Text.Contains("[Housekeeping]"))
                        {
                            text.Text = text.Text.Replace("[Housekeeping]", Housekeeping);

                        }
                        else if (text.Text.Contains("[ParkingAssignedSpace]"))
                        {
                            text.Text = text.Text.Replace("[ParkingAssignedSpace]", ParkingAssignedSpace);

                        }
                        else if (text.Text.Contains("[ParkingBusinessCenterHours]"))
                        {
                            text.Text = text.Text.Replace("[ParkingBusinessCenterHours]", ParkingBusinessCenterHours);

                        }
                        ////////
                        ///
                        else if (text.Text.Contains("[ParkingFitnessCenterHours]"))
                        {
                            text.Text = text.Text.Replace("[ParkingFitnessCenterHours]", ParkingFitnessCenterHours);

                        }
                        else if (text.Text.Contains("[ParkingPoolHours]"))
                        {
                            text.Text = text.Text.Replace("[ParkingPoolHours]", ParkingPoolHours);

                        }
                        else if (text.Text.Contains("[CustomerServiceNumber]"))
                        {
                            text.Text = text.Text.Replace("[CustomerServiceNumber]", CustomerServiceNumber);

                        }
                        else if (text.Text.Contains("[Emergencynumber]"))
                        {
                            text.Text = text.Text.Replace("[Emergencynumber]", Emergencynumber);

                        }
                        else if (text.Text.Contains("[RID]"))
                        {
                            text.Text = text.Text.Replace("[RID]", RId);

                        }
                        else if (text.Text.Contains("[PhoneNumber]"))
                        {
                            text.Text = text.Text.Replace("[PhoneNumber]", PhoneNumber1);

                        }
                        else if (text.Text.Contains("[Floor]"))
                        {
                            text.Text = text.Text.Replace("[Floor]", Floor);

                        }

                    }
                    pkgDoc.Save();
                }


                //DocumentFormat.OpenXml.Wordprocessing.Body body = pkgDoc.MainDocumentPart.Document.Body;

                //    DocumentFormat.OpenXml.Wordprocessing.BookmarkStart bkmStart = body.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart>().Where(bkm => bkm.Name == bkmName).FirstOrDefault();
                //    bkmID = bkmStart.Id;
                //    DocumentFormat.OpenXml.Wordprocessing.BookmarkEnd bkmEnd = body.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkEnd>().Where(bkm => bkm.Id == bkmID).FirstOrDefault();
                //    parentTypeStart = bkmStart.Parent.LocalName;
                //    parentTypeEnd = bkmEnd.Parent.LocalName;
                //    int counter = 0;
                //    if (parentTypeStart == "" && parentTypeEnd == "")
                //    { //bookmark starts at a paragraph and ends within a paragraph
                //        DocumentFormat.OpenXml.Wordprocessing.Paragraph bkmParaStart = (DocumentFormat.OpenXml.Wordprocessing.Paragraph)bkmStart.Parent;
                //        DocumentFormat.OpenXml.Wordprocessing.Paragraph bkmParaEnd = (DocumentFormat.OpenXml.Wordprocessing.Paragraph)bkmEnd.Parent;
                //        DocumentFormat.OpenXml.Wordprocessing.Paragraph bkmParaNext = (DocumentFormat.OpenXml.Wordprocessing.Paragraph)bkmParaStart;
                //        List<DocumentFormat.OpenXml.Wordprocessing.Paragraph> paras = new List<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
                //        paras.Add(bkmParaStart);

                //        DocumentFormat.OpenXml.Wordprocessing.BookmarkEnd x = bkmParaNext.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkEnd>().Where(bkm => bkm.Id == bkmID).FirstOrDefault();
                //        while (x == null)
                //        {
                //            DocumentFormat.OpenXml.Wordprocessing.Paragraph nextPara = (DocumentFormat.OpenXml.Wordprocessing.Paragraph)bkmParaNext.NextSibling();
                //            if (nextPara != null)
                //            {
                //                paras.Add(nextPara);
                //                bkmParaNext = (DocumentFormat.OpenXml.Wordprocessing.Paragraph)nextPara.Clone();
                //                x = bkmParaNext.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkEnd>().Where(bkm => bkm.Id == bkmID).FirstOrDefault();
                //            }
                //        }
                //        foreach (DocumentFormat.OpenXml.Wordprocessing.Paragraph para in paras)
                //        {
                //            string t = "changed string once more " + counter;
                //            DocumentFormat.OpenXml.Wordprocessing.Run firstRun = para.Descendants<DocumentFormat.OpenXml.Wordprocessing.Run>().FirstOrDefault();
                //            DocumentFormat.OpenXml.Wordprocessing.Run newRun = (DocumentFormat.OpenXml.Wordprocessing.Run)firstRun.Clone();
                //            newRun.RemoveAllChildren<DocumentFormat.OpenXml.Wordprocessing.Text>();
                //            para.RemoveAllChildren<DocumentFormat.OpenXml.Wordprocessing.Run>();
                //            para.RemoveAllChildren<DocumentFormat.OpenXml.Wordprocessing.Text>();
                //            para.AppendChild<DocumentFormat.OpenXml.Wordprocessing.Run>(newRun).AppendChild<DocumentFormat.OpenXml.Wordprocessing.Text>(new DocumentFormat.OpenXml.Wordprocessing.Text(t));
                //        }
                //        //After replacing the runs and text the bookmark is at the beginning
                //        //of the paragraph, we want it at the end
                //        DocumentFormat.OpenXml.Wordprocessing.BookmarkEnd newBkmEnd = new DocumentFormat.OpenXml.Wordprocessing.BookmarkEnd() { Id = bkmID };
                //        DocumentFormat.OpenXml.Wordprocessing.Paragraph p = paras.Last<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
                //        p.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkEnd>().Where(bkm => bkm.Id == bkmID).FirstOrDefault().Remove();
                //        p.Append(newBkmEnd);
                //    }
            }
        }

        public void DownloadQuoteSheetOpenXml(Int64 Id)
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("sp_QuoteSheet", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuoteId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();
            string path = Server.MapPath("~/Uploads/DownloadableDocs/");
            string Filepath = path + "CreateOpenXmlDoc.docx";
            //string SourcedocName = "Keyluxe Suites PDF reservation quote.docx";
            string SourcedocName = "Keyluxe suites Quote Sheet Template - Data Fields.docx";
            string docName = "Q-" + Id.ToString() + "Keyluxe Suites PDF reservation quote.docx";
            string LeaseStartDate = "LeaseStartDate";
            string Name = "Name";
            string LeaseEndDate = "LeaseEndDate";
            string LeadsName = "LeadsName";
            string MonthlyPetRentFee = "MonthlyPetRentFee";
            string NoOfBedRooms = "NoOfBedRooms";
            string NoOfPets = "NoOfPets";
            string OcupantName = "OcupantName";
            string OcupantName2 = "OcupantName2";
            string Address = "Address";
            string PhoneNumber = "PhoneNumber";
            string PropertyAddress = "PropertyAddress";
            string PropertyAddress2 = "PropertyAddress2";
            string ContactNumber = "ContactNumber";
            string ToDate = "ToDate";
            string TotalMonthly = "TotalMonthly";
            string TotalMonthlyCCost = "TotalMonthlyCCost";
            string ContactEmail = "ContactEmail";
            string GuestName = "GuestName";
            string RId = "RId";
            string LeadsName1 = "LeadsName1";
            string OneTimeRefundablePro = "OneTimeRefundablePro";
            string OneTimeNonRefFees = "OneTimeNonRefFees";
            string Date = "Date";
            string TotalStay = "TotalStay";
            string DailyCash = "DailyCash";
            string OneTimeKSSecDep = "OneTimeKSSecDep";
            string OneTimePropPetDep = "OneTimePropPetDep";
            string OneTimeKSPetFee = "OneTimeKSPetFee";
            string OneTimeKSAppFee = "OneTimeKSAppFee";

            string CommunityFeatures = "CommunityFeatures";

            string UnitFeatures = "UnitFeatures";

            string Charges = "Charges";
            
            string ToDate1 = "ToDate1";
            //OpenXmlImage.InsertAPicture(Filepath, path + "PropImage4.jpg");
            MemoryStream destStream = FileCloner.ReadAllBytesToMemoryStream(path + SourcedocName);
            if (dt.Rows.Count > 0)
            {
                ToDate1 = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString();

                OneTimeKSAppFee = dt.Rows[0]["OneTimeKSAppFee"].ToString();

                OneTimeNonRefFees = dt.Rows[0]["OneTimeNonRefFees"].ToString();

                OneTimeRefundablePro = dt.Rows[0]["OneTimeRefundablePropFees"].ToString();

                LeadsName1 = dt.Rows[0]["LeadsName"].ToString();
                if (LeadsName1 == "")
                {
                    LeadsName1 = dt.Rows[0]["OcupantName"].ToString();
                }

                //RId = "R-" + dt.Rows[0]["RId"].ToString();

                //GuestName = dt.Rows[0]["GuestName"].ToString();

                ContactNumber = dt.Rows[0]["ContactNumber"].ToString();

                Address = dt.Rows[0]["Address"].ToString();

                LeadsName = dt.Rows[0]["LeadsName"].ToString();
                if (LeadsName == "")
                {
                    LeadsName = dt.Rows[0]["OcupantName"].ToString();
                }

                Name = dt.Rows[0]["LeadsName"].ToString();
                if (Name == "")
                {
                    Name = dt.Rows[0]["OcupantName"].ToString();
                }

                LeaseStartDate = DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Month.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Day.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Year.ToString();

                LeaseEndDate = DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Month.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Day.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Year.ToString();

                MonthlyPetRentFee = dt.Rows[0]["MonthlyPetRentFee"].ToString();
                NoOfPets = dt.Rows[0]["NoOfPets"].ToString();
                OcupantName = dt.Rows[0]["OcupantName"].ToString();
                OcupantName2 = dt.Rows[0]["OcupantName"].ToString();
                PhoneNumber = dt.Rows[0]["PhoneNumber"].ToString();
                PropertyAddress = dt.Rows[0]["PropertyAddress"].ToString();
                PropertyAddress2 = dt.Rows[0]["PropertyAddress"].ToString();
                ToDate = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString();
                Charges = dt.Rows[0]["Charges"].ToString();
                ///////////////
                ContactEmail = dt.Rows[0]["ContactEmail"].ToString();

                Date = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString();
                TotalStay = dt.Rows[0]["TotalStay"].ToString();
                DailyCash = dt.Rows[0]["DailyCash"].ToString();
                OneTimeKSSecDep = dt.Rows[0]["OneTimeKSSecDep"].ToString();
                OneTimePropPetDep = dt.Rows[0]["OneTimePropPetDep"].ToString();
                OneTimeKSPetFee = dt.Rows[0]["OneTimeKSPetFee"].ToString();
                OneTimeKSAppFee = dt.Rows[0]["OneTimeKSAppFee"].ToString();

                

                //OpenXmlImage.InsertAPicture(path + "Keyluxe Suites PDF reservation quote.docx", path + "PropImage5.jpg");

                //using (var wordprocessingDocument = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Create(Filepath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                //{
                //    //OpenXmlImage openXmlImage = new OpenXmlImage();
                //    //OpenXmlImage.InsertAPicture(Filepath, path + "PropImage4");
                //    DocumentFormat.OpenXml.Packaging.MainDocumentPart mainPart = wordprocessingDocument.MainDocumentPart;
                //    mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                //    var body = mainPart.Document.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Body());
                //    DocumentFormat.OpenXml.Wordprocessing.Paragraph para = body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph());
                //    DocumentFormat.OpenXml.Wordprocessing.Run run = para.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run());
                //    run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text("Qoute Sheet"));

                //    wordprocessingDocument.MainDocumentPart.Document.Save();
                //}


                using (DocumentFormat.OpenXml.Packaging.WordprocessingDocument pkgDoc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(destStream, true))
                {
                    DocumentFormat.OpenXml.Packaging.MainDocumentPart mainPart = pkgDoc.MainDocumentPart;


                    var document = pkgDoc.MainDocumentPart.Document;

                    //    IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Drawing> drawings = mainPart.Document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Drawing>().ToList();
                    //    foreach (DocumentFormat.OpenXml.Wordprocessing.Drawing drawing in drawings)
                    //    {
                    //        DocumentFormat.OpenXml.Drawing.Wordprocessing.DocProperties dpr = drawing.Descendants<DocumentFormat.OpenXml.Drawing.Wordprocessing.DocProperties>().FirstOrDefault();
                    //        if (dpr != null && dpr.Name == "Picture 1")
                    //        {
                    //            foreach (DocumentFormat.OpenXml.Drawing.Blip b in drawing.Descendants<DocumentFormat.OpenXml.Drawing.Blip>().ToList())
                    //            {
                    //                DocumentFormat.OpenXml.Packaging.OpenXmlPart imagePart = pkgDoc.MainDocumentPart.GetPartById(b.Embed);
                    //                using (var writer = new BinaryWriter(imagePart.GetStream()))
                    //                {
                    //                    writer.Write(System.IO.File.ReadAllBytes(path + "PropImage5.jpg"));
                    //                }
                    //            }
                    //        }
                    //    }
                    //    using (FileStream fs = new FileStream(path + "test1.docx", FileMode.CreateNew, FileAccess.Write))
                    //    {
                    //        destStream.CopyTo(fs);
                    //    }

                    //var xname = document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Header>().Where(x => x.XName == "RId").FirstOrDefault();

                    foreach (var text in document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
                    {
                        if (text.Text.Equals("Date"))
                        {
                            text.Text = text.Text.Replace("Date", Date);
                        }
                        else if (text.Text.Contains("TotalStay"))
                        {
                            text.Text = text.Text.Replace("TotalStay", TotalStay);
                        }
                        else if (text.Text.Contains("DailyCash"))
                        {
                            text.Text = text.Text.Replace("DailyCash", DailyCash);
                        }
                        else if (text.Text.Contains("OneTimeKSSecDep"))
                        {
                            text.Text = text.Text.Replace("OneTimeKSSecDep", OneTimeKSSecDep);
                        }
                        else if (text.Text.Contains("OneTimePropPetDep"))
                        {
                            text.Text = text.Text.Replace("OneTimePropPetDep", OneTimePropPetDep);
                        }
                        else if (text.Text.Contains("OneTimeKSPetFee"))
                        {
                            text.Text = text.Text.Replace("OneTimeKSPetFee", OneTimeKSPetFee);
                        }
                        else if (text.Text.Contains("LeaseStartDate"))
                        {
                            text.Text = text.Text.Replace("LeaseStartDate", LeaseStartDate);

                        }
                        else if (text.Text.Contains("GuestName"))
                        {
                            text.Text = text.Text.Replace("GuestName", GuestName);

                        }
                        else if (text.Text.Contains("Name"))
                        {
                            text.Text = text.Text.Replace("Name", Name);

                        }
                        else if (text.Text.Contains("LeaseEndDate"))
                        {
                            text.Text = text.Text.Replace("LeaseEndDate", LeaseEndDate);

                        }
                        else if (text.Text.Contains("ToDate"))
                        {
                            text.Text = text.Text.Replace("ToDate", ToDate1);

                        }
                        else if (text.Text.Contains("PropertyAddress"))
                        {
                            text.Text = text.Text.Replace("[PropertyAddress]", PropertyAddress);

                        }
                        else if (text.Text.Contains("[PropertyAddress2]"))
                        {
                            text.Text = text.Text.Replace("[PropertyAddress2]", PropertyAddress2);

                        }
                        else if (text.Text.Contains("[OneTimeKSAppFee]"))
                        {
                            text.Text = text.Text.Replace("[OneTimeKSAppFee]", OneTimeKSAppFee);

                        }
                        else if (text.Text.Contains("[NonRefundable]"))
                        {
                            text.Text = text.Text.Replace("[NonRefundable]", OneTimeNonRefFees);

                        }
                        else if (text.Text.Contains("[Refundable]"))
                        {
                            text.Text = text.Text.Replace("[Refundable]", OneTimeRefundablePro);

                        }
                        else if (text.Text.Contains("[Name1]"))
                        {
                            text.Text = text.Text.Replace("[Name1]", LeadsName1);

                        }
                        else if (text.Text.Contains("[TODATE1]"))
                        {
                            text.Text = text.Text.Replace("[TODATE1]", ToDate1);

                        }
                        else if (text.Text.Contains("[ContactNumber]"))
                        {
                            text.Text = text.Text.Replace("[ContactNumber]", ContactNumber);

                        }
                        else if (text.Text.Contains("[ADDRESS]"))
                        {
                            text.Text = text.Text.Replace("[ADDRESS]", Address);

                        }
                        else if (text.Text.Contains("[OCCUPANTNAME]"))
                        {
                            text.Text = text.Text.Replace("[OCCUPANTNAME]", LeadsName);

                        }
                        else if (text.Text.Contains("[OcupantName]"))
                        {
                            text.Text = text.Text.Replace("[OcupantName]", LeadsName1);

                        }
                        else if (text.Text.Contains("[PetFee]"))
                        {
                            text.Text = text.Text.Replace("[PetFee]", MonthlyPetRentFee);

                        }
                        else if (text.Text.Contains("[NoOfPets]"))
                        {
                            text.Text = text.Text.Replace("[NoOfPets]", NoOfPets);

                        }
                        else if (text.Text.Contains("[Charges]"))
                        {
                            text.Text = text.Text.Replace("[Charges]", Charges);

                        }
                        else if (text.Text.Contains("[CONTACTEMAIL]"))
                        {
                            text.Text = text.Text.Replace("[CONTACTEMAIL]", ContactEmail);

                        }

                        else if (text.Text.Contains("RId"))
                        {
                            text.Text = text.Text.Replace("RId", RId);

                        }
                        else if (text.Text.Contains("[PhoneNumber]"))
                        {
                            text.Text = text.Text.Replace("[PhoneNumber]", PhoneNumber);

                        }
                        else
                        {
                            switch (text.Text)
                            {
                                case "Crown Molding":
                                    if(CommunityFeatures.Contains(text.Text))
                                    { 
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    
                                    break;
                                case "Formal Entry/ Foyer":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Spa / Hot Tub":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Split Level":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Washer & dryer":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Microwave":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Dishwasher":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Air conditioning":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Dog park":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Pool":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Spa":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Fitness center":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Business center":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Clubhouse":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Tennis and basketball court":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Playground":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Fishing pond":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Bicycle trails":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Charging station":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Bicycle rental":
                                    if (CommunityFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;

                                
                                case "Central A/ C":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Ceiling Fan":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Walkin Shower":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Fire Extinguisher":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Stainless Steel Appliances":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Patio or Balcony":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Private backyards":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Wood Style Flooring":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                
                                case "Wine Chiller":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Granite Countertops":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Fireplace":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Garden Tub":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Keyless Entry":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Fully Equipped Kitchen":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Patio/OutdoorStorageCloset":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                
                                case "Garbage Disposal":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;
                                case "Walkin Closet":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        text.Remove();
                                    }
                                    break;


                            }
                            switch (text.Text)
                            {
                                case "Crown Molding":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        if(text.Parent != null)
                                        text.Remove();
                                    }

                                    break;
                                case "Dishwasher":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        if (text.Parent != null)
                                            text.Remove();
                                    }
                                    break;
                                case "Microwave":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        if (text.Parent != null)
                                            text.Remove();
                                    }
                                    break;

                                case "Washer & dryer":
                                    if (UnitFeatures.Contains(text.Text))
                                    {
                                    }
                                    else
                                    {
                                        if (text.Parent != null)
                                            text.Remove();
                                    }
                                    break;
                                }


                        }


                    }



                    Response.Clear();
                    // Gets or sets the HTTP MIME type of the output stream.
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    // Adds an HTTP header to the output stream
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + docName);

                    //Gets or sets a value indicating whether to buffer output and send it after
                    // the complete response is finished processing.
                    Response.Buffer = true;
                    // Sets the Cache-Control header to one of the values of System.Web.HttpCacheability.
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    // Writes a string of binary characters to the HTTP output stream. it write the generated bytes .
                    Response.BinaryWrite(destStream.GetBuffer());

                    // Sends all currently buffered output to the client, stops execution of the
                    // page, and raises the System.Web.HttpApplication.EndRequest event.
                    Response.End();
                    // Closes the socket connection to a client. it is a necessary step as you must close the response after doing work.its best approach.
                    Response.Close();


                    //pkgDoc.Save();

                }

            }
        }
    


        public void DownloadLeaseAgreementOpenXml(Int64 Id)
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("sp_ReservationAgreement", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RId", 5);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();
            string path = Server.MapPath("~/Uploads/");
            string SourcedocName = "KLS Reservation Agreement.docx";
            string docName = "R-" + "5".ToString() + "KLS Reservation Agreement-Open.docx";
            string LeaseStartDate = "LeaseStartDate";
            string Name = "Name";
            string LeaseEndDate = "LeaseEndDate";
            string LeadsName = "LeadsName";
            string MonthlyPetRentFee = "MonthlyPetRentFee";
            string NoOfBedRooms = "NoOfBedRooms";
            string NoOfPets = "NoOfPets";
            string OcupantName = "OcupantName";
            string OcupantName2 = "OcupantName2";
            string Address = "Address";
            string PhoneNumber = "PhoneNumber";
            string PropertyAddress = "PropertyAddress";
            string PropertyAddress2 = "PropertyAddress2";
            string ContactNumber = "ContactNumber";
            string ToDate = "ToDate";
            string TotalMonthly = "TotalMonthly";
            string TotalMonthlyCCost = "TotalMonthlyCCost";
            string ContactEmail = "ContactEmail";
            string GuestName = "GuestName";
            string RId = "RId";
            string LeadsName1 = "LeadsName1";
            string OneTimeRefundablePro = "OneTimeRefundablePro";
            string OneTimeNonRefFees = "OneTimeNonRefFees";

            string Charges = "Charges";
            string OneTimeKSAppFee = "OneTimeKSAppFee";
            string ToDate1 = "ToDate1";
            MemoryStream destStream = FileCloner.ReadAllBytesToMemoryStream(path + SourcedocName);
            if (dt.Rows.Count > 0)
            {
                ToDate1 = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString();

                OneTimeKSAppFee = dt.Rows[0]["OneTimeKSAppFee"].ToString();

                OneTimeNonRefFees = dt.Rows[0]["OneTimeNonRefFees"].ToString();

                OneTimeRefundablePro = dt.Rows[0]["OneTimeRefundablePropFees"].ToString();

                LeadsName1 = dt.Rows[0]["LeadsName"].ToString();
                if (LeadsName1 == "")
                {
                    LeadsName1 = dt.Rows[0]["OcupantName"].ToString();
                }

                RId = "R-" + dt.Rows[0]["RId"].ToString();

                GuestName = dt.Rows[0]["GuestName"].ToString();

                ContactNumber = dt.Rows[0]["ContactNumber"].ToString();

                Address = dt.Rows[0]["Address"].ToString();

                LeadsName = dt.Rows[0]["LeadsName"].ToString();
                if (LeadsName == "")
                {
                    LeadsName = dt.Rows[0]["OcupantName"].ToString();
                }

                Name = dt.Rows[0]["LeadsName"].ToString();
                if (Name == "")
                {
                    Name = dt.Rows[0]["OcupantName"].ToString();
                }

                LeaseStartDate = DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Month.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Day.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Year.ToString();

                LeaseEndDate = DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Month.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Day.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Year.ToString();

                MonthlyPetRentFee = dt.Rows[0]["MonthlyPetRentFee"].ToString();
                NoOfPets = dt.Rows[0]["NoOfPets"].ToString();
                OcupantName = dt.Rows[0]["OcupantName"].ToString();
                OcupantName2 = dt.Rows[0]["OcupantName"].ToString();
                PhoneNumber = dt.Rows[0]["PhoneNumber"].ToString();
                PropertyAddress = dt.Rows[0]["PropertyAddress"].ToString();
                PropertyAddress2 = dt.Rows[0]["PropertyAddress"].ToString();
                ToDate = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString();
                Charges = dt.Rows[0]["Charges"].ToString();
                ///////////////
                ContactEmail = dt.Rows[0]["ContactEmail"].ToString();



                using (DocumentFormat.OpenXml.Packaging.WordprocessingDocument pkgDoc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(destStream, true))
                {


                    var document = pkgDoc.MainDocumentPart.Document;

                    var xname = document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Header>().Where(x => x.XName == "RId").FirstOrDefault();

                    foreach (var text in document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
                    {
                        if (text.Text.Contains("[MOVEINDATE]"))
                        {
                            text.Text = text.Text.Replace("[MOVEINDATE]", LeaseStartDate);

                        }
                        else if (text.Text.Contains("GuestName"))
                        {
                            text.Text = text.Text.Replace("GuestName", GuestName);

                        }
                        else if (text.Text.Contains("[NAME]"))
                        {
                            text.Text = text.Text.Replace("[NAME]", Name);

                        }
                        else if (text.Text.Contains("[MOVEOUTDATE]"))
                        {
                            text.Text = text.Text.Replace("[MOVEOUTDATE]", LeaseEndDate);

                        }
                        else if (text.Text.Contains("[TODATE]"))
                        {
                            text.Text = text.Text.Replace("[TODATE]", ToDate1);

                        }
                        else if (text.Text.Contains("[PropertyAddress]"))
                        {
                            text.Text = text.Text.Replace("[PropertyAddress]", PropertyAddress);

                        }
                        else if (text.Text.Contains("[PropertyAddress2]"))
                        {
                            text.Text = text.Text.Replace("[PropertyAddress2]", PropertyAddress2);

                        }
                        else if (text.Text.Contains("[OneTimeKSAppFee]"))
                        {
                            text.Text = text.Text.Replace("[OneTimeKSAppFee]", OneTimeKSAppFee);

                        }
                        else if (text.Text.Contains("[NonRefundable]"))
                        {
                            text.Text = text.Text.Replace("[NonRefundable]", OneTimeNonRefFees);

                        }
                        else if (text.Text.Contains("[Refundable]"))
                        {
                            text.Text = text.Text.Replace("[Refundable]", OneTimeRefundablePro);

                        }
                        else if (text.Text.Contains("[Name1]"))
                        {
                            text.Text = text.Text.Replace("[Name1]", LeadsName1);

                        }
                        else if (text.Text.Contains("[TODATE1]"))
                        {
                            text.Text = text.Text.Replace("[TODATE1]", ToDate1);

                        }
                        else if (text.Text.Contains("[ContactNumber]"))
                        {
                            text.Text = text.Text.Replace("[ContactNumber]", ContactNumber);

                        }
                        else if (text.Text.Contains("[ADDRESS]"))
                        {
                            text.Text = text.Text.Replace("[ADDRESS]", Address);

                        }
                        else if (text.Text.Contains("[OCCUPANTNAME]"))
                        {
                            text.Text = text.Text.Replace("[OCCUPANTNAME]", LeadsName);

                        }
                        else if (text.Text.Contains("[OcupantName]"))
                        {
                            text.Text = text.Text.Replace("[OcupantName]", LeadsName1);

                        }
                        else if (text.Text.Contains("[PetFee]"))
                        {
                            text.Text = text.Text.Replace("[PetFee]", MonthlyPetRentFee);

                        }
                        else if (text.Text.Contains("[NoOfPets]"))
                        {
                            text.Text = text.Text.Replace("[NoOfPets]", NoOfPets);

                        }
                        else if (text.Text.Contains("[Charges]"))
                        {
                            text.Text = text.Text.Replace("[Charges]", Charges);

                        }
                        else if (text.Text.Contains("[CONTACTEMAIL]"))
                        {
                            text.Text = text.Text.Replace("[CONTACTEMAIL]", ContactEmail);

                        }

                        else if (text.Text.Contains("RId"))
                        {
                            text.Text = text.Text.Replace("RId", RId);

                        }
                        else if (text.Text.Contains("[PhoneNumber]"))
                        {
                            text.Text = text.Text.Replace("[PhoneNumber]", PhoneNumber);

                        }


                    }



                    Response.Clear();
                    // Gets or sets the HTTP MIME type of the output stream.
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    // Adds an HTTP header to the output stream
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + docName);

                    //Gets or sets a value indicating whether to buffer output and send it after
                    // the complete response is finished processing.
                    Response.Buffer = true;
                    // Sets the Cache-Control header to one of the values of System.Web.HttpCacheability.
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    // Writes a string of binary characters to the HTTP output stream. it write the generated bytes .
                    Response.BinaryWrite(destStream.GetBuffer());

                    // Sends all currently buffered output to the client, stops execution of the
                    // page, and raises the System.Web.HttpApplication.EndRequest event.
                    Response.End();
                    // Closes the socket connection to a client. it is a necessary step as you must close the response after doing work.its best approach.
                    Response.Close();
                

                //pkgDoc.Save();

            }

            }
        }
        private byte[] btnReplaceBookmarkText2_Click()
        {

            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("sp_ReservationAgreement", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RId", 5);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();
            string path = Server.MapPath("~/Uploads/");
            string SourcedocName = "KLS Reservation Agreement.docx";
            string docName = "R-" + "5".ToString() + "KLS Reservation Agreement-Open.docx";
            string LeaseStartDate = "LeaseStartDate";
            string Name = "Name";
            string LeaseEndDate = "LeaseEndDate";
            string LeadsName = "LeadsName";
            string MonthlyPetRentFee = "MonthlyPetRentFee";
            string NoOfBedRooms = "NoOfBedRooms";
            string NoOfPets = "NoOfPets";
            string OcupantName = "OcupantName";
            string OcupantName2 = "OcupantName2";
            string Address = "Address";
            string PhoneNumber = "PhoneNumber";
            string PropertyAddress = "PropertyAddress";
            string PropertyAddress2 = "PropertyAddress2";
            string ContactNumber = "ContactNumber";
            string ToDate = "ToDate";
            string TotalMonthly = "TotalMonthly";
            string TotalMonthlyCCost = "TotalMonthlyCCost";
            string ContactEmail = "ContactEmail";
            string GuestName = "GuestName";
            string RId = "RId";
            string LeadsName1 = "LeadsName1";
            string OneTimeRefundablePro = "OneTimeRefundablePro";
            string OneTimeNonRefFees = "OneTimeNonRefFees";

            string Charges = "Charges";
            string OneTimeKSAppFee = "OneTimeKSAppFee";
            string ToDate1 = "ToDate1";
            MemoryStream destStream = FileCloner.ReadAllBytesToMemoryStream(path + SourcedocName);
            if (dt.Rows.Count > 0)
            {
                ToDate1 = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString();

                OneTimeKSAppFee = dt.Rows[0]["OneTimeKSAppFee"].ToString();

                OneTimeNonRefFees = dt.Rows[0]["OneTimeNonRefFees"].ToString();

                OneTimeRefundablePro = dt.Rows[0]["OneTimeRefundablePropFees"].ToString();

                LeadsName1 = dt.Rows[0]["LeadsName"].ToString();
                if (LeadsName1 == "")
                {
                    LeadsName1 = dt.Rows[0]["OcupantName"].ToString();
                }

                RId = "R-" + dt.Rows[0]["RId"].ToString();

                GuestName = dt.Rows[0]["GuestName"].ToString();

                ContactNumber = dt.Rows[0]["ContactNumber"].ToString();

                Address = dt.Rows[0]["Address"].ToString();

                LeadsName = dt.Rows[0]["LeadsName"].ToString();
                if (LeadsName == "")
                {
                    LeadsName = dt.Rows[0]["OcupantName"].ToString();
                }

                Name = dt.Rows[0]["LeadsName"].ToString();
                if (Name == "")
                {
                    Name = dt.Rows[0]["OcupantName"].ToString();
                }

                LeaseStartDate = DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Month.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Day.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Year.ToString();

                LeaseEndDate = DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Month.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Day.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Year.ToString();

                MonthlyPetRentFee = dt.Rows[0]["MonthlyPetRentFee"].ToString();
                NoOfPets = dt.Rows[0]["NoOfPets"].ToString();
                OcupantName = dt.Rows[0]["OcupantName"].ToString();
                OcupantName2 = dt.Rows[0]["OcupantName"].ToString();
                PhoneNumber = dt.Rows[0]["PhoneNumber"].ToString();
                PropertyAddress = dt.Rows[0]["PropertyAddress"].ToString();
                PropertyAddress2 = dt.Rows[0]["PropertyAddress"].ToString();
                ToDate = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString();
                Charges = dt.Rows[0]["Charges"].ToString();
                ///////////////
                ContactEmail = dt.Rows[0]["ContactEmail"].ToString();

                

                using (DocumentFormat.OpenXml.Packaging.WordprocessingDocument pkgDoc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(destStream, true))
                {
                    

                    var document = pkgDoc.MainDocumentPart.Document;
                    
                    var xname = document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Header>().Where(x => x.XName == "RId").FirstOrDefault();
                    
                    foreach (var text in document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
                    {
                        if (text.Text.Contains("[MOVEINDATE]"))
                        {
                            text.Text = text.Text.Replace("[MOVEINDATE]", LeaseStartDate);

                        }
                        else if (text.Text.Contains("GuestName"))
                        {
                            text.Text = text.Text.Replace("GuestName", GuestName);

                        }
                        else if (text.Text.Contains("[NAME]"))
                        {
                            text.Text = text.Text.Replace("[NAME]", Name);

                        }
                        else if (text.Text.Contains("[MOVEOUTDATE]"))
                        {
                            text.Text = text.Text.Replace("[MOVEOUTDATE]", LeaseEndDate);

                        }
                        else if (text.Text.Contains("[TODATE]"))
                        {
                            text.Text = text.Text.Replace("[TODATE]", ToDate1);

                        }
                        else if (text.Text.Contains("[PropertyAddress]"))
                        {
                            text.Text = text.Text.Replace("[PropertyAddress]", PropertyAddress);

                        }
                        else if (text.Text.Contains("[PropertyAddress2]"))
                        {
                            text.Text = text.Text.Replace("[PropertyAddress2]", PropertyAddress2);

                        }
                        else if (text.Text.Contains("[OneTimeKSAppFee]"))
                        {
                            text.Text = text.Text.Replace("[OneTimeKSAppFee]", OneTimeKSAppFee);

                        }
                        else if (text.Text.Contains("[NonRefundable]"))
                        {
                            text.Text = text.Text.Replace("[NonRefundable]", OneTimeNonRefFees);

                        }
                        else if (text.Text.Contains("[Refundable]"))
                        {
                            text.Text = text.Text.Replace("[Refundable]", OneTimeRefundablePro);

                        }
                        else if (text.Text.Contains("[Name1]"))
                        {
                            text.Text = text.Text.Replace("[Name1]", LeadsName1);

                        }
                        else if (text.Text.Contains("[TODATE1]"))
                        {
                            text.Text = text.Text.Replace("[TODATE1]", ToDate1);

                        }
                        else if (text.Text.Contains("[ContactNumber]"))
                        {
                            text.Text = text.Text.Replace("[ContactNumber]", ContactNumber);

                        }
                        else if (text.Text.Contains("[ADDRESS]"))
                        {
                            text.Text = text.Text.Replace("[ADDRESS]", Address);

                        }
                        else if (text.Text.Contains("[OCCUPANTNAME]"))
                        {
                            text.Text = text.Text.Replace("[OCCUPANTNAME]", LeadsName);

                        }
                        else if (text.Text.Contains("[OcupantName]"))
                        {
                            text.Text = text.Text.Replace("[OcupantName]", LeadsName1);

                        }
                        else if (text.Text.Contains("[PetFee]"))
                        {
                            text.Text = text.Text.Replace("[PetFee]", MonthlyPetRentFee);

                        }
                        else if (text.Text.Contains("[NoOfPets]"))
                        {
                            text.Text = text.Text.Replace("[NoOfPets]", NoOfPets);

                        }
                        else if (text.Text.Contains("[Charges]"))
                        {
                            text.Text = text.Text.Replace("[Charges]", Charges);

                        }
                        else if (text.Text.Contains("[CONTACTEMAIL]"))
                        {
                            text.Text = text.Text.Replace("[CONTACTEMAIL]", ContactEmail);

                        }
                        
                        else if (text.Text.Contains("RId"))
                        {
                            text.Text = text.Text.Replace("RId", RId);

                        }
                        else if (text.Text.Contains("[PhoneNumber]"))
                        {
                            text.Text = text.Text.Replace("[PhoneNumber]", PhoneNumber);

                        }
                        

                    }
                    
                    
                    //pkgDoc.Save();
                    
                }

            }
            return destStream.GetBuffer();
            //return File(destStream.GetBuffer(), "text/plain", docName);
        }
        public void DownloadLeaseAgreement(Int64 Id)
        {

            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("sp_ReservationAgreement", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();
            string path = Server.MapPath("~/Uploads/");
            string docName = "R-" + Id.ToString() + "KLS Reservation Agreement";
            string LeaseStartDate = "LeaseStartDate";
            string Name = "Name";
            string LeaseEndDate = "LeaseEndDate";
            string LeadsName = "LeadsName";
            string MonthlyPetRentFee = "MonthlyPetRentFee";
            string NoOfBedRooms = "NoOfBedRooms";
            string NoOfPets = "NoOfPets";
            string OcupantName = "OcupantName";
            string OcupantName2 = "OcupantName2";
            string Address = "Address";
            string PhoneNumber = "PhoneNumber";
            string PropertyAddress = "PropertyAddress";
            string PropertyAddress2 = "PropertyAddress2";
            string ContactNumber = "ContactNumber";
            string ToDate = "ToDate";
            string TotalMonthly = "TotalMonthly";
            string TotalMonthlyCCost = "TotalMonthlyCCost";
            string ContactEmail = "ContactEmail";
            string GuestName = "GuestName";
            string RId = "RId";
            string LeadsName1 = "LeadsName1";
            string OneTimeRefundablePro = "OneTimeRefundablePro";
            string OneTimeNonRefFees = "OneTimeNonRefFees";

            string Charges = "Charges";
            string OneTimeKSAppFee = "OneTimeKSAppFee";
            string ToDate1 = "ToDate1";


            if (dt.Rows.Count > 0)
            {

                ////////////////
                ToDate1 = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString();
                
                OneTimeKSAppFee = dt.Rows[0]["OneTimeKSAppFee"].ToString();

                OneTimeNonRefFees = dt.Rows[0]["OneTimeNonRefFees"].ToString();

                OneTimeRefundablePro = dt.Rows[0]["OneTimeRefundablePropFees"].ToString();

                LeadsName1 = dt.Rows[0]["LeadsName"].ToString();
                if (LeadsName1 == "")
                {
                    LeadsName1 = dt.Rows[0]["OcupantName"].ToString();
                }

                RId = "R-" + dt.Rows[0]["RId"].ToString();

                GuestName = dt.Rows[0]["GuestName"].ToString();

                ContactNumber = dt.Rows[0]["ContactNumber"].ToString();

                Address = dt.Rows[0]["Address"].ToString();

                LeadsName = dt.Rows[0]["LeadsName"].ToString();
                if (LeadsName == "")
                {
                    LeadsName = dt.Rows[0]["OcupantName"].ToString();
                }
                LeaseStartDate = DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Month.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Day.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Year.ToString();

                LeaseEndDate = DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Month.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Day.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Year.ToString();

                MonthlyPetRentFee = dt.Rows[0]["MonthlyPetRentFee"].ToString();
                NoOfPets = dt.Rows[0]["NoOfPets"].ToString();
                OcupantName = dt.Rows[0]["OcupantName"].ToString();
                OcupantName2 = dt.Rows[0]["OcupantName"].ToString();
                PhoneNumber = dt.Rows[0]["PhoneNumber"].ToString();
                PropertyAddress = dt.Rows[0]["PropertyAddress"].ToString();
                PropertyAddress2 = dt.Rows[0]["PropertyAddress"].ToString();
                ToDate = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString();
                Charges = dt.Rows[0]["Charges"].ToString();
                ///////////////
                ContactEmail = dt.Rows[0]["ContactEmail"].ToString();


            }

            string guest = "";
            string pathImage = Server.MapPath("~/Uploads/");
            StringBuilder RAgreement = new StringBuilder();
            RAgreement.Append(@"<header class='clearfix'></header>");
            RAgreement.Append("<h1>Reservation Agreement</h1><div>This Reservation Agreement is between <b>" + LeadsName + "</b> and <b>KEYLUXE SUITES</b>.</div><br />");

            RAgreement.Append("<div id='company' class='clearfix'>");
            RAgreement.Append("<table border=1>");
            RAgreement.Append("<tr>");
            RAgreement.Append(@"<td width=114 colspan=2 style='width:85.25pt;padding:0cm 5.4pt 0cm 5.4pt;
  height:21.6pt' style='font-size:10.0pt;font-family:Arial,sans-serif'>Guest Name:</td>");
            RAgreement.Append(@"<td width='510'  style='width:382.25pt;border-bottom:solid;padding:0cm 5.4pt 0cm 5.4pt;
  height:21.6pt'><input style='border-bottom:solid'>" + OcupantName + "</input></td></tr>");

            RAgreement.Append("<tr>");
            RAgreement.Append(@"<td width=114 colspan=2 style='width:85.25pt;padding:0cm 5.4pt 0cm 5.4pt;
  height:21.6pt' style='font-size:10.0pt;font-family:Arial,sans-serif'>Billing Address:</td>");
            RAgreement.Append(@"<td width='510'  style='width:382.25pt;border-bottom:solid;padding:0cm 5.4pt 0cm 5.4pt;
  height:21.6pt'><input style='border-bottom:solid'>" + Address + "</td>");
            RAgreement.Append("</tr>");

            RAgreement.Append("</table>");
            RAgreement.Append("</div>");

            RAgreement.Append("<div>");
            RAgreement.Append("<table border=1>");
            RAgreement.Append("<tr>");
            RAgreement.Append("<td >Phone:</td>");
            RAgreement.Append("<td >" + ContactNumber + "</td>");
            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");
            RAgreement.Append("<td>Email:</td>");
            RAgreement.Append("<td>" + ContactEmail + "</td>");

            RAgreement.Append("</tr>");

            RAgreement.Append("</table>");
            RAgreement.Append("</div><br />");
            RAgreement.Append("<h1>RESERVATION INFORMATION</h1><br />");
            RAgreement.Append("<div id='reservationInformation' class='clearfix'>");
            RAgreement.Append("<table border=1>");
            RAgreement.Append("<tr>");
            RAgreement.Append(@"<td width=114  style='width:85.25pt;padding:0cm 5.4pt 0cm 5.4pt;
  height: 21.6pt' style='font -size:10.0pt;font-family:Arial,sans-serif'>Move-in date:</td>");
            RAgreement.Append("<td>" + LeaseStartDate + "</td>");
            RAgreement.Append("<td>Move-out date:</td>");
            RAgreement.Append("<td>" + LeaseEndDate + "</td>");
            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");

            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");
            RAgreement.Append("<td>Move-in time:</td>");
            RAgreement.Append("<td>4:00 PM</td>");
            RAgreement.Append("<td>Move-out time:</td>");
            RAgreement.Append("<td>11:00 AM</td>");
            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");
            RAgreement.Append("<td >Property address:</td>");
            RAgreement.Append("<td>" + PropertyAddress + "</td>");

            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");
            RAgreement.Append("<td >Unit address</td>");
            RAgreement.Append("<td>" + PropertyAddress2 + "</td>");

            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");
            RAgreement.Append("<td>Unit number:</td>");
            RAgreement.Append("<td>" + PhoneNumber + "</td>");
            RAgreement.Append("<td>Rental rate:</td>");
            RAgreement.Append("<td>$" + Charges + "</td>");
            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");
            RAgreement.Append("<td >Permitted occupants:</td>");
            RAgreement.Append("<td>" + OcupantName2 + "</td>");

            RAgreement.Append("</tr>");

            RAgreement.Append("<tr>");
            RAgreement.Append("<td>Application fee:</td>");
            RAgreement.Append("<td>$" + OneTimeKSAppFee + "</td>");
            RAgreement.Append("<td>Refundable security deposit:</td>");
            RAgreement.Append("<td>$" + OneTimeRefundablePro + "</td>");
            RAgreement.Append("<td>Non-refundable fee:</td>");
            RAgreement.Append("<td>$" + OneTimeNonRefFees + "</td>");

            RAgreement.Append("</tr>");

            RAgreement.Append("<tr>");
            RAgreement.Append("<td>Number of pets:</td>");
            RAgreement.Append("<td>" + NoOfPets + "</td>");
            RAgreement.Append("<td>Refundable pet deposit:</td>");
            RAgreement.Append("<td>$0</td>");
            RAgreement.Append("<td>Non-refundable pet fee:</td>");
            RAgreement.Append("<td>$" + MonthlyPetRentFee + "</td>");

            RAgreement.Append("</tr>");

            RAgreement.Append("</table>");
            RAgreement.Append("</div><br />");

            RAgreement.Append("<h1>CREDIT CARD AUTHORIZATION</h1>");

            RAgreement.Append(@"<div>Guest authorizes KEYLUXE SUITES to charge the credit card listed below for rent, deposits, and fees. The credit card will remain on file for 120 days after move-out. Please note that should you have any unpaid rent, late fees, utility overages, damages, and/or missing items you are authorizing KEYLUXE SUITES to charge your credit card.</div><br />");
            //3rd Table
            RAgreement.Append("<div id='creditCardInfo' class='clearfix'>");
            RAgreement.Append("<table border=1>");
            RAgreement.Append("<tr>");
            RAgreement.Append(@"<td width=114 colspan=2 style='width:85.25pt;padding:0cm 5.4pt 0cm 5.4pt;
  height: 21.6pt' style='font -size:10.0pt;font-family:Arial,sans-serif'>Name (as it appears on card):</td>");
            RAgreement.Append("<td>" + guest + "</td>");
            RAgreement.Append("<td>Phone:</td>");
            RAgreement.Append("<td>" + guest + "</td>");
            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");

            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");
            RAgreement.Append("<td >Billing address:</td>");
            RAgreement.Append("<td colspan=4>" + guest + "</td>");

            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");
            RAgreement.Append("<td>Card type:</td>");
            RAgreement.Append("<td>[] Visa</td>");
            RAgreement.Append("<td>[] Mastercard</td>");
            RAgreement.Append("<td>[] Discover</td>");
            RAgreement.Append("<td>[] American Express</td>");
            RAgreement.Append("</tr>");

            RAgreement.Append("<tr>");
            RAgreement.Append("<td >Card Number:</td>");
            RAgreement.Append("<td colspan=4>" + guest + "</td>");
            RAgreement.Append("</tr>");

            RAgreement.Append("<tr>");
            RAgreement.Append("<td>Expiration (MM/YY):</td>");
            RAgreement.Append("<td>" + guest + "</td>");
            RAgreement.Append("<td>Security Code:</td>");
            RAgreement.Append("<td>" + guest + "</td>");

            RAgreement.Append("</tr>");

            RAgreement.Append("</table>");
            RAgreement.Append("</div>");

            RAgreement.Append("<div>");
            RAgreement.Append("<table border=1>");

            RAgreement.Append("<tr>");
            RAgreement.Append("<td>Expiration (MM/YY):</td>");
            RAgreement.Append("<td>" + guest + "</td>");
            RAgreement.Append("<td>Security Code:</td>");
            RAgreement.Append("<td>" + guest + "</td>");

            RAgreement.Append("</tr>");

            RAgreement.Append("</table>");
            RAgreement.Append("</div><br />");


            RAgreement.Append("<h1>NOTICE TO VACATE</h1>");
            RAgreement.Append("<div>");
            RAgreement.Append("<table>");

            RAgreement.Append("<tr>");

            RAgreement.Append("<td>[]I wish to provide notice today:</td>");
            RAgreement.Append("<td><table border=1  width: 5px;height: 5px;><tr><td></td></tr></table></td>");

            RAgreement.Append("<tr>");

            RAgreement.Append("<td>[]I wish to provide notice at a later date.</td>");
            RAgreement.Append("<td></td>");
            RAgreement.Append("</tr>");
            RAgreement.Append("</table>");
            RAgreement.Append("</div><br />");


            RAgreement.Append(@"<div>1)	<b>Rent Includes:</b> apartment rent, furniture, housewares, electricity, basic expanded cable or streaming services, water, trash, and high-speed internet.</div>");
            RAgreement.Append(@"<div>2)	<b>Occupants and Authorized Persons:</b> Guest agrees that only the persons listed as \""Permitted Occupants\"" (which shall be deemed to include Guest, if an individual) on the first page of this Agreement will occupy the Unit and that the Permitted Occupants shall use the Unit for residential purposes only pursuant to the terms of this Agreement and the Addendum as if the persons listed as Permitted Occupants had executed a counterpart of this Agreement. Guest represents that:</div>");
            RAgreement.Append(@"<div>a)	only the Permitted Occupants will occupy the Unit and the Unit will be used solely for residential purposes.</div>");
            RAgreement.Append(@"<div>b)	no Permitted Occupants have a residence history which involves disturbance of neighbors, destruction of property, living or keeping unauthorized occupants or pets; and</div>");
            RAgreement.Append(@"<div>c)	no Permitted Occupants have a criminal history that reflects any prior convictions or deferred adjudication for felony offenses or any sex-related offenses that require registration under applicable law.</div>");
            RAgreement.Append(@"<div>3)	<b>Termination:</b> both parties may terminate this Agreement and Occupant's right to occupy the Unit at the end of the initial Term or at any time thereafter by giving at least sixty (60) days prior written notice of termination to the other. Such termination shall be effective at the end of the Initial Term or the end of the Renewal Term after the notice is provided, as applicable. KEYLUXE SUITES must receive this notice during normal business hours otherwise the notice will not be effective until the next business day. If resident fails to give such written notice once the required written notice is given, amount due will continue to accrue through the notice period until the termination is effective.</div>");
            RAgreement.Append(@"<div>4)	<b>Rent and other charges.</b></div>");
            RAgreement.Append(@"<div>a)	Reservation Confirmation: Client shall pay to KEYLUXE SUITES the Reservation Confirmation rate as described on the first page of this Agreement, for the unit in accordance with the terms of this Agreement (the \""Rent\"").</div>");

            RAgreement.Append(@"<div>b)	Reservation Includes Basic Services: The reservation includes charges for the services that have been listed on the first page of this Agreement under \""Rent Includes\"". Resident acknowledges that the rent paid by KEYLUXE SUITES for the Unit includes a water allowance of $45 per month, based upon the number of bedrooms in the Unit in accordance with the following schedule: $125 for one bedroom; $150 for a two bedroom, $200 for a three bedroom. In addition to the rent and other charges due hereunder, Client shall pay, within ten (10) days of being billed for such amount, the water, and electric services charge to the Unit in excess of the Threshold Amount, including a prorated amount for any excess usage during a partial month of occupancy. Resident's execution of the Credit Card Authorization attached at the end of this Reservation shall entitle KEYLUXE SUITES to assess such water, and electric services including but not limited to, long distance telephone service and premium cable television, if available.  All utilities and services shall be used for ordinary household purpose only.</ div>");
            RAgreement.Append(@"<div>c)	Pets: No Pets of any kind shall be permitted in the Unit or on Complex premises without prior written consent from KEYLUXE SUITES and/or complex management, a signed pet agreement and payment of a pet deposit and/or non-refundable pet fee. The presence of pets of resident, any Permitted Occupant, or any of their guests or invitees in the Unit or on the complex premises without prior written permission from KEYLUXE SUITES shall be deemed a default of this Agreement, and KEYLUXE SUITES thereafter shall have the right to terminate this Agreement immediately on written notice.</div>");
            RAgreement.Append(@"<div>d)	Late Charges: Rent is due on the 25th of every month. Rent received on the first is considered late and Guest will be charged $25 per day until Guest account is brought current. All payments received will first be applied towards late fees and then to rent.</div>");
            RAgreement.Append(@"<div>5)	<b>Security Deposit.</b></div>");
            RAgreement.Append(@"<div>6)	<b>Smoking/Vaping Policy:</b> Smoking of any kind is prohibited in all units.  Violation of this policy will result in a $1,500.00 fine in addition to any other cleaning charges. Guest and Guest Visitors agree to comply with this policy and the individual property’s smoking policy.</div>");
            RAgreement.Append(@"<div>7)	<b>Lockouts:</b> Should you lose the apartment keys and become locked out there will be a $350.00 charge for assistance.</div>");

            RAgreement.Append(@"<div>8)	<b>Trash:</b> The Complex may provide trash dumpster and/or compactors for the Complex. If such services are offered, it is each Permitted Occupants responsibility to ensure that all trash is disposed of properly. For sanitary reasons, trash is NEVER to be left outside the unit or anywhere else in the Complex. There will be a $50.00 fine assessed by KEYLUXE SUITES to Guest and/or any Permitted Occupant for the removal of any trash that is not disposed in the dumpsters or compactors for the Complex.</div>");
            RAgreement.Append(@"<div>9)	<b>Maid Service:</b> If included an addendum will be provided with the occurrence of when the service will occur.</div>");
            RAgreement.Append(@"<div>10) <b>Entry:</b> KEYLUXE SUITES and Complex management shall have the right to enter the Unit for any reasonable business purpose. KEYLUXE SUITES agrees to abide by state and local laws regarding repairs.</div>");
            RAgreement.Append(@"<div>11) <b>Security and Indemnification:</b> KEYLUXE SUITES shall not be held liable to Resident, or their guests for any damage, injury or loss to person or property caused by other person or vandalism or other crimes, (furniture, jewelry, clothing, etc.) from fire, flood, water leaks, rain, hail, ice, snow, smoke, lighting, wind, explosions, interruption of utilities or other occurrences unless such damage, injury or loss is caused exclusively by the negligence of Owner or KEYLUXE SUITES: in which case, the liability of either Owner or KEYLUXE SUITES, as the case may be, shall be limited to the extent of their respective negligence. Guest is strongly advised to secure Resident’s own insurance to protect against all the above. KEYLUXE SUITES does not insure Guest personal property. Repair requests for smoke detectors, locks, or latches must be in writing to KEYLUXE SUITES. Guest acknowledges that neither Property Owner nor KEYLUXE SUITES are equipped or trained to prove personal security services to Guest, occupants, or their guests. Guest represents that it will not rely upon any security measures taken by Owner or KEYLUXE SUITES and will call the local law enforcement authority in the event of any security needs and will call 911 or any other applicable emergency number in the event of an emergency.</div>");

            RAgreement.Append(@"<div>12) <b>Move-Out Procedures:</b> Guest must provide KEYLUXE SUITES a written 60-day notice prior to vacating apartment. Oral move-out notices are not sufficient and do not constitute notice. The move-out date cannot be changed unless both parties agree in writing. Before moving out, Guest must pay all Rent and other charges due through the end of the initial Term or then-current Renewal Term, as applicable. Guest and any Permitted Occupant should follow the Check-out Procedures supplied to them, if any. Guest will be liable for reasonable cleaning charges, including charges for cleaning carpets, furniture, walls, etc., that are soiled due to negligence, carelessness, accident, or abuse.</div>");
            RAgreement.Append(@"<div>13) <b>Holdover:</b> If Guest or any Permitted Occupant shall holdover possession of the Unit beyond the date indicated in any notice to vacate given by KEYLUXE SUITES in accordance with the terms of this Agreement, or a different move-out date agreed to by the parties in writing for any reason, then Rent and any charges for Optional Services for the holdover period will be increased by 50% over the existing Rent and charges for all rent for the full term of the Agreement of a new resident who cannot occupy because of the holdover.</div>");
            RAgreement.Append(@"<div>14) <b>Bed Bugs.</b></div>");
            RAgreement.Append(@"<div>  a)	  Guest acknowledges that KEYLUXE SUITES has inspected the unit and is not aware of any bed bug infestation. Guest agrees that all furnishings and personal properties that will be moved into the premises will be free of bed bugs. Resident herby agrees to prevent and control possible infestation by adhering to the below list of responsibilities:</div>");

            RAgreement.Append(@"<div>         i)	    Check for hitch-hiking bed bugs. If you stay in a hotel or another home, inspect your clothing, luggage shoes and personal belongings for signs of bed bugs before reentering your apartment.</div>");
            RAgreement.Append(@"<div>         ii)	    Check backpacks, shoes and clothing after using public transportation or visiting theaters.</div>");

            RAgreement.Append(@"<div>         iii)	    After guests visit make sure to inspect beds, bedding and upholstered furniture for signs of bed-bug infestation.</div>");
            RAgreement.Append(@"<div>         ii)	    Check backpacks, shoes and clothing after using public transportation or visiting theaters.</div>");
            RAgreement.Append(@"<div>         ii)	    Check backpacks, shoes and clothing after using public transportation or visiting theaters.</div>");
            RAgreement.Append(@"<div>         ii)	    Check backpacks, shoes and clothing after using public transportation or visiting theaters.</div>");

            RAgreement.Append(@"<div>         ii)	    Check backpacks, shoes and clothing after using public transportation or visiting theaters.</div>");

            RAgreement.Append(@"<div>  b)	  Guest shall report any problems immediately to KEYLUXE SUITES. Guest shall cooperate with pest control efforts. If your unit or a neighbor’s unit is infested, a pest management professional may be called in to eradicate the problem. Your unit must be properly prepared for treatment.  Guest must comply with recommendations and requests from the pest management specialist prior to professional treatment.</div>");
            RAgreement.Append(@"<div>  c)	  Guest agrees to reimburse KEYLUXE SUITES for expenses including but not limited to attorney fees and pest management fees that KEYLUXE SUITES may incur as a result of infestation of bed bugs in the dwelling.</div>");
            RAgreement.Append(@"<div>  d)	  Guest agrees to hold KEYLUXE SUITES harmless from any actions, claims, losses, damages and expenses that may incur as a result of a bed bug infestation.</div>");
            RAgreement.Append(@"<div>  e)	  It is acknowledged that KEYLUXE SUITES shall not be liable for any loss of personal property to the resident as a result of an infestation of bed bugs.  Resident agrees to have personal property insurance to cover such losses.</div>");
            RAgreement.Append(@"<div>If any signs of bed bugs are found upon move-in, we must receive written acknowledgement within 48 hours of your move-in date.The Parties hereby agree to the terms and conditions set forth in this Agreement and such is demonstrated throughout by their signatures below:</div><br />");

            //Client Table
            RAgreement.Append("<h1>CLIENT</h1><br />");
            RAgreement.Append("<div id='clientTable' class='clearfix'>");
            RAgreement.Append("<table border=1>");
            RAgreement.Append("<tr>");
            RAgreement.Append(@"<td width=114 colspan=1 style='width:85.25pt;padding:0cm 5.4pt 0cm 5.4pt;
  height: 21.6pt' style='font -size:10.0pt;font-family:Arial,sans-serif'>Signature:</td>");
            RAgreement.Append(@"<td width='510' colspan='1' style='width:382.25pt;border:none;border-bottom:solid;padding:0cm 5.4pt 0cm 5.4pt;
  height:21.6pt'></td>");
            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");
            RAgreement.Append("<td>Name:</td>");
            RAgreement.Append("<td>" + LeadsName1 + "</td>");
            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");
            RAgreement.Append("<td>Date:</td>");
            RAgreement.Append("<td>" + ToDate + "</td>");
            RAgreement.Append("</tr>");
            RAgreement.Append("</table>");
            RAgreement.Append("</div><br /><br /><br />");

            //Keyluxe Table
            RAgreement.Append("<h1>KEYLUXE SUITES</h1><br />");
            RAgreement.Append("<div id='keyluxeTable' class='clearfix'>");
            RAgreement.Append("<table border=1>");
            RAgreement.Append("<tr>");
            RAgreement.Append(@"<td width=114 colspan=1 style='width:85.25pt;padding:0cm 5.4pt 0cm 5.4pt;
  height: 21.6pt' style='font -size:10.0pt;font-family:Arial,sans-serif'>Signature:</td>");
            RAgreement.Append(@"<td width='510' colspan='1' style='width:382.25pt;border:none;border-bottom:solid;padding:0cm 5.4pt 0cm 5.4pt;
  height:21.6pt'></td>");
            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");
            RAgreement.Append("<td>Name:</td>");
            RAgreement.Append("<td></td>");
            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");
            RAgreement.Append("<td>Title:</td>");
            RAgreement.Append("<td></td>");
            RAgreement.Append("</tr>");
            RAgreement.Append("<tr>");
            RAgreement.Append("<td>Date:</td>");
            RAgreement.Append("<td>" + ToDate1 + "</td>");
            RAgreement.Append("</tr>");
            RAgreement.Append("</table>");
            RAgreement.Append("</div>");


            RAgreement.Append("<footer>");
            RAgreement.Append("Invoice was created on a computer and is valid without the signature and seal.");
            RAgreement.Append("</footer>");


            StringReader sr = new StringReader(RAgreement.ToString());
            Document pdfDoc = new Document(PageSize.A3, 10f, 10f, 10f, 0f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();

                htmlparser.Parse(sr);
                pdfDoc.Close();

                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();


                Response.Clear();
                // Gets or sets the HTTP MIME type of the output stream.
                Response.ContentType = "application/pdf";
                // Adds an HTTP header to the output stream
                Response.AddHeader("Content-Disposition", "attachment; filename=" + docName + ".pdf");

                //Gets or sets a value indicating whether to buffer output and send it after
                // the complete response is finished processing.
                Response.Buffer = true;
                // Sets the Cache-Control header to one of the values of System.Web.HttpCacheability.
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                // Writes a string of binary characters to the HTTP output stream. it write the generated bytes .
                Response.BinaryWrite(bytes);

                // Sends all currently buffered output to the client, stops execution of the
                // page, and raises the System.Web.HttpApplication.EndRequest event.
                Response.End();
                // Closes the socket connection to a client. it is a necessary step as you must close the response after doing work.its best approach.
                Response.Close();
            }
            //byte[] bytes = btnReplaceBookmarkText2_Click();
            //Response.Clear();
            //// Gets or sets the HTTP MIME type of the output stream.
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            //// Adds an HTTP header to the output stream
            //Response.AddHeader("Content-Disposition", "attachment; filename=" + docName + ".docx");

            ////Gets or sets a value indicating whether to buffer output and send it after
            //// the complete response is finished processing.
            //Response.Buffer = true;
            //// Sets the Cache-Control header to one of the values of System.Web.HttpCacheability.
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //// Writes a string of binary characters to the HTTP output stream. it write the generated bytes .
            //Response.BinaryWrite(bytes);

            //// Sends all currently buffered output to the client, stops execution of the
            //// page, and raises the System.Web.HttpApplication.EndRequest event.
            //Response.End();
            //// Closes the socket connection to a client. it is a necessary step as you must close the response after doing work.its best approach.
            //Response.Close();

            //btnReplaceBookmarkText_Click();

        }
        public FileResult DownloadLeaseAgreementWord(Int64 Id)
        {

            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("sp_ReservationAgreement", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();


            word wordApp = new word();
            //Application = wordApp.app;
            //using Range = Microsoft.Office.Interop.Word.Range;

            //wordApp.app = new Microsoft.Office.Interop.Word.Application();
            wordApp.app.Visible = false; //Hide the Word.Application object.
            string path = Server.MapPath("~/Uploads/");
            wordApp.document = wordApp.app.Documents.Open(path + "KLS Reservation Agreement.docx");
            string docName = "R-" + Id.ToString() + "KLS Reservation Agreement.docx";
            wordApp.document.SaveAs2(path + docName);

            wordApp.document.Close(); //close the document.
            wordApp.app.Quit(); //close the Word.Application object. otherwise, you'll get a ReadOnly error later.
                                /////Duplicate Qoute Doc Saved

            ////Current Qoute Doc Load
            //wordApp = new word();
            wordApp.app = new Microsoft.Office.Interop.Word.Application();
            wordApp.app.Visible = false; //Hide the Word.Application object.

            wordApp.document = wordApp.app.Documents.Open(path + docName);
            string LeaseStartDate = "LeaseStartDate";
            string Name = "Name";
            string LeaseEndDate = "LeaseEndDate";
            string LeadsName = "LeadsName";
            string MonthlyPetRentFee = "MonthlyPetRentFee";
            string NoOfBedRooms = "NoOfBedRooms";
            string NoOfPets = "NoOfPets";
            string OcupantName = "OcupantName";
            string OcupantName2 = "OcupantName2";
            string Address = "Address";
            string PhoneNumber = "PhoneNumber";
            string PropertyAddress = "PropertyAddress";
            string PropertyAddress2 = "PropertyAddress2";
            string ContactNumber = "ContactNumber";
            string ToDate = "ToDate";
            string TotalMonthly = "TotalMonthly";
            string TotalMonthlyCCost = "TotalMonthlyCCost";
            string ContactEmail = "ContactEmail";
            string GuestName = "GuestName";
            string RId = "RId";
            string LeadsName1 = "LeadsName1";
            string OneTimeRefundablePro = "OneTimeRefundablePro";
            string OneTimeNonRefFees = "OneTimeNonRefFees";

            string Charges = "Charges";
            string OneTimeKSAppFee = "OneTimeKSAppFee";
            string ToDate1 = "ToDate1";


            if (dt.Rows.Count > 0)
            {

                ////////////////
                ToDate1 = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["ToDate1"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(ToDate1);

                wordApp.document.Bookmarks.Add("ToDate1", wordApp.range);
                /////////////////


                ////////////////

                OneTimeKSAppFee = dt.Rows[0]["OneTimeKSAppFee"].ToString();

                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["OneTimeKSAppFee"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(OneTimeKSAppFee);

                wordApp.document.Bookmarks.Add("OneTimeKSAppFee", wordApp.range);
                /////////////////


                ////////////////

                OneTimeNonRefFees = dt.Rows[0]["OneTimeNonRefFees"].ToString();

                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["OneTimeNonRefFees"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(OneTimeNonRefFees);

                wordApp.document.Bookmarks.Add("OneTimeNonRefFees", wordApp.range);
                /////////////////


                ////////////////

                OneTimeRefundablePro = dt.Rows[0]["OneTimeRefundablePropFees"].ToString();

                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["OneTimeRefundablePro"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(OneTimeRefundablePro);

                wordApp.document.Bookmarks.Add("OneTimeRefundablePro", wordApp.range);
                /////////////////

                ////////////////

                LeadsName1 = dt.Rows[0]["LeadsName"].ToString();
                if (LeadsName1 == "")
                {
                    LeadsName1 = DateTime.Parse(dt.Rows[0]["OccupantName"].ToString()).ToString();
                }
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["LeadsName1"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(LeadsName1);

                wordApp.document.Bookmarks.Add("LeadsName1", wordApp.range);
                /////////////////


                ////////////////

                RId = "R-" + dt.Rows[0]["RId"].ToString();

                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["RId"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(RId);

                wordApp.document.Bookmarks.Add("RId", wordApp.range);
                /////////////////


                ////////////////

                GuestName = dt.Rows[0]["GuestName"].ToString();

                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["GuestName"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(GuestName);

                wordApp.document.Bookmarks.Add("GuestName", wordApp.range);
                /////////////////

                ////////////////

                ContactNumber = dt.Rows[0]["ContactNumber"].ToString();

                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["ContactNumber"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(ContactNumber);

                wordApp.document.Bookmarks.Add("ContactNumber", wordApp.range);
                /////////////////

                ////////////////

                Address = dt.Rows[0]["Address"].ToString();
                
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["Address"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(Address);

                wordApp.document.Bookmarks.Add("Address", wordApp.range);
                /////////////////


                ////////////////

                LeadsName = dt.Rows[0]["LeadsName"].ToString();
                if (LeadsName == "")
                {
                    LeadsName = DateTime.Parse(dt.Rows[0]["OccupantName"].ToString()).ToString();
                }
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["LeadsName"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(LeadsName);

                wordApp.document.Bookmarks.Add("LeadsName", wordApp.range);
                /////////////////


                ////////////////
                LeaseStartDate = DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Month.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Day.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseStartDate"].ToString()).Year.ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["LeaseStartDate"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(LeaseStartDate);

                wordApp.document.Bookmarks.Add("LeaseStartDate", wordApp.range);
                /////////////////
                /////////////////////

                LeaseEndDate = DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Month.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Day.ToString() + "/" + DateTime.Parse(dt.Rows[0]["LeaseEndDate"].ToString()).Year.ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["LeaseEndDate"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(LeaseEndDate);

                wordApp.document.Bookmarks.Add("LeaseEndDate", wordApp.range);
                /////////////////

                MonthlyPetRentFee = dt.Rows[0]["MonthlyPetRentFee"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["MonthlyPetRentFee"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(MonthlyPetRentFee);

                wordApp.document.Bookmarks.Add("MonthlyPetRentFee", wordApp.range);
                ///////////////

                ///////////////
                NoOfPets = dt.Rows[0]["NoOfPets"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["NoOfPets"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(NoOfPets);

                wordApp.document.Bookmarks.Add("NoOfPets", wordApp.range);
                ///////////////
                OcupantName = dt.Rows[0]["OcupantName"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["OcupantName"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(OcupantName);

                wordApp.document.Bookmarks.Add("OcupantName", wordApp.range);
                ///////////////
                OcupantName2 = dt.Rows[0]["OcupantName"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["OcupantName2"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(OcupantName2);

                wordApp.document.Bookmarks.Add("OcupantName2", wordApp.range);
                ///////////////
                ///////////////
                PhoneNumber = dt.Rows[0]["PhoneNumber"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["PhoneNumber"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(PhoneNumber);

                wordApp.document.Bookmarks.Add("PhoneNumber", wordApp.range);
                ///////////////
                PropertyAddress = dt.Rows[0]["PropertyAddress"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["PropertyAddress"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(PropertyAddress);

                wordApp.document.Bookmarks.Add("PropertyAddress", wordApp.range);
                ///////////////
                PropertyAddress2 = dt.Rows[0]["PropertyAddress"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["PropertyAddress1"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(PropertyAddress2);

                wordApp.document.Bookmarks.Add("PropertyAddress1", wordApp.range);
                ///////////////
                ///////////////
                ToDate = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["ToDate"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(ToDate);

                wordApp.document.Bookmarks.Add("ToDate", wordApp.range);
                ///////////////
                ///////////////
                ///////////////
                Charges = dt.Rows[0]["Charges"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["Charges"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(Charges);

                wordApp.document.Bookmarks.Add("Charges", wordApp.range);
                ///////////////
                ContactEmail = dt.Rows[0]["ContactEmail"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["ContactEmail"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(ContactEmail);

                wordApp.document.Bookmarks.Add("ContactEmail", wordApp.range);
                ///////////////
                ///////////////


            }


            object format = wordApp.wdSave;

            wordApp.document.SaveAs2(path + "R-" + Id.ToString() + "KLS Reservation Agreement.pdf", format, false);
            wordApp.document.SaveAs2(path + "R-" + Id.ToString() + "KLS Reservation Agreement.html", wordApp.wdSaveHTML, false);

            //wordApp.document.Save(); //save the document.
            wordApp.document.Close(); //close the document.
            wordApp.app.Quit(); //close the Word.Application object. otherwise, you'll get a ReadOnly error later.


            return File(path + "R-" + Id.ToString() + "KLS Reservation Agreement.pdf", "text/plain", "R-" + Id.ToString() + "KLS Reservation Agreement.pdf");
            

        }
        public FileResult DownloadLeaseAgreement1(Int64 Id)
        {

            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("sp_leaseAgreement", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuoteId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();


            word wordApp = new word();
            //Application = wordApp.app;
            //using Range = Microsoft.Office.Interop.Word.Range;

            //wordApp.app = new Microsoft.Office.Interop.Word.Application();
            wordApp.app.Visible = false; //Hide the Word.Application object.
            string path = Server.MapPath("~/Uploads/");
            wordApp.document = wordApp.app.Documents.Open(path + "Texas-Standard-Residential-Lease-Agreement-1.docx");
            string docName = "Q-" + Id.ToString() + "Texas-Standard-Residential-Lease-Agreement.docx";
            wordApp.document.SaveAs2(path + docName);

            wordApp.document.Close(); //close the document.
            wordApp.app.Quit(); //close the Word.Application object. otherwise, you'll get a ReadOnly error later.
                                /////Duplicate Qoute Doc Saved

            ////Current Qoute Doc Load
            //wordApp = new word();
            wordApp.app = new Microsoft.Office.Interop.Word.Application();
            wordApp.app.Visible = false; //Hide the Word.Application object.
            
            wordApp.document = wordApp.app.Documents.Open(path + docName);
            string LeaseStartDate = "LeaseStartDate";
            string Name = "Name";
            string LeaseEndDate = "LeaseEndDate";
            string MonthlyParkingPlaces = "MonthlyParkingPlaces";
            string MonthlyPetRentFee = "MonthlyPetRentFee";
            string NoOfBedRooms = "NoOfBedRooms";
            string NoOfPets = "NoOfPets";
            string OcupantName = "OcupantName";
            string OcupantName2 = "OcupantName2";
            string ParkingPlaces = "ParkingPlaces";
            string PhoneNumber = "PhoneNumber";
            string PropertyAddress = "PropertyAddress";
            string PropertyAddress2 = "PropertyAddress2";
            string RentDay = "RentDay";
            string ToDate = "ToDate";
            string TotalMonthly = "TotalMonthly";
            string TotalMonthlyCCost = "TotalMonthlyCCost";
            string TotalOneTime = "TotalOneTime";
            string UnitSize = "UnitSize";
            string UnitType = "UnitType";
            string VendorName = "VendorName";
            string Weight = "Weight";
            string Breed = "Breed";

            string Charges = "Charges";
            string ContactEmail = "ContactEmail";
            string EmergencyPhoneNumber = "EmergencyPhoneNumber";

            string Img1 = "Img1";
            string Img2 = "Img2";
            string Img3 = "Img3";
            string Img4 = "Img4";
            string Img5 = "Img5";

            if (dt.Rows.Count > 0)
            {
                LeaseStartDate = DateTime.Parse( dt.Rows[0]["LeaseStartDate"].ToString()).ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["LeaseStartDate"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(DateTime.Now.ToString());

                wordApp.document.Bookmarks.Add("LeaseStartDate", wordApp.range);
                /////////////////

                Name = dt.Rows[0]["Name"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["Name"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(Name);

                wordApp.document.Bookmarks.Add("Name", wordApp.range);
                /////////////////////

                LeaseEndDate = dt.Rows[0]["LeaseEndDate"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["LeaseEndDate"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(LeaseEndDate);

                wordApp.document.Bookmarks.Add("LeaseEndDate", wordApp.range);
                /////////////////
                
                MonthlyParkingPlaces = dt.Rows[0]["ParkingPlaces"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["MonthlyParkingPlaces"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(MonthlyParkingPlaces);

                wordApp.document.Bookmarks.Add("MonthlyParkingPlaces", wordApp.range);
                ///////////////

                MonthlyPetRentFee = dt.Rows[0]["MonthlyPetRentFee"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["MonthlyPetRentFee"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(MonthlyPetRentFee);

                wordApp.document.Bookmarks.Add("MonthlyPetRentFee", wordApp.range);
                ///////////////

                NoOfBedRooms = dt.Rows[0]["NoOfBedRooms"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["NoOfBedRooms"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(NoOfBedRooms);

                wordApp.document.Bookmarks.Add("NoOfBedRooms", wordApp.range);
                ///////////////
                NoOfPets = dt.Rows[0]["NoOfPets"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["NoOfPets"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(NoOfPets);

                wordApp.document.Bookmarks.Add("NoOfPets", wordApp.range);
                ///////////////
                OcupantName = dt.Rows[0]["OcupantName"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["OcupantName"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(OcupantName);

                wordApp.document.Bookmarks.Add("OcupantName", wordApp.range);
                ///////////////
                OcupantName2 = dt.Rows[0]["OcupantName"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["OcupantName2"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(OcupantName2);

                wordApp.document.Bookmarks.Add("OcupantName2", wordApp.range);
                ///////////////
                ParkingPlaces = dt.Rows[0]["ParkingPlaces"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["ParkingPlaces"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(ParkingPlaces);

                wordApp.document.Bookmarks.Add("ParkingPlaces", wordApp.range);
                ///////////////
                PhoneNumber = dt.Rows[0]["PhoneNumber"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["PhoneNumber"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(PhoneNumber);

                wordApp.document.Bookmarks.Add("PhoneNumber", wordApp.range);
                ///////////////
                PropertyAddress = dt.Rows[0]["PropertyAddress"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["PropertyAddress"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(PropertyAddress);

                wordApp.document.Bookmarks.Add("PropertyAddress", wordApp.range);
                ///////////////
                PropertyAddress2 = dt.Rows[0]["PropertyAddress"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["PropertyAddress2"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(PropertyAddress2);

                wordApp.document.Bookmarks.Add("PropertyAddress2", wordApp.range);
                ///////////////
                RentDay = "5th";
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["RentDay"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(RentDay);

                wordApp.document.Bookmarks.Add("RentDay", wordApp.range);
                ///////////////
                ToDate = DateTime.Now.ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["ToDate"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(ToDate);

                wordApp.document.Bookmarks.Add("ToDate", wordApp.range);
                ///////////////
                TotalMonthly = dt.Rows[0]["TotalMonthly"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["TotalMonthly"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(TotalMonthly);

                wordApp.document.Bookmarks.Add("TotalMonthly", wordApp.range);
                ///////////////
                TotalMonthlyCCost = dt.Rows[0]["TotalMonthlyCCost"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["TotalMonthlyCCost"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(TotalMonthlyCCost);

                wordApp.document.Bookmarks.Add("TotalMonthlyCCost", wordApp.range);
                ///////////////
                TotalOneTime = dt.Rows[0]["TotalOneTime"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["TotalOneTime"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(TotalOneTime);

                wordApp.document.Bookmarks.Add("TotalOneTime", wordApp.range);
                ///////////////
                UnitSize = dt.Rows[0]["UnitSize"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["UnitSize"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(UnitSize);

                wordApp.document.Bookmarks.Add("UnitSize", wordApp.range);
                ///////////////
                UnitType = dt.Rows[0]["UnitType"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["UnitType"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(UnitType);

                wordApp.document.Bookmarks.Add("UnitType", wordApp.range);
                ///////////////
                VendorName = dt.Rows[0]["VendorName"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["VendorName"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(VendorName);

                wordApp.document.Bookmarks.Add("VendorName", wordApp.range);
                ///////////////
                Weight = dt.Rows[0]["Weight"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["Weight"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(Weight);

                wordApp.document.Bookmarks.Add("Weight", wordApp.range);
                ///////////////
                Breed = dt.Rows[0]["Breed"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["Breed"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(Breed);

                wordApp.document.Bookmarks.Add("Breed", wordApp.range);
                ///////////////
                Charges = dt.Rows[0]["Charges"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["Charges"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(Charges);

                wordApp.document.Bookmarks.Add("Charges", wordApp.range);
                ///////////////
                ContactEmail = dt.Rows[0]["ContactEmail"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["ContactEmail"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(ContactEmail);

                wordApp.document.Bookmarks.Add("ContactEmail", wordApp.range);
                ///////////////
                EmergencyPhoneNumber = dt.Rows[0]["EmergencyPhoneNumber"].ToString();
                wordApp.bookmarks = wordApp.document.Bookmarks;

                ////Add Book Mark Change
                wordApp.bookmark = wordApp.document.Bookmarks["EmergencyPhoneNumber"];
                //wordApp.range = wordApp.bookmark.Range;
                wordApp.range = wordApp.bookmark.Range.Duplicate;
                //Select the text.
                wordApp.bookmark.Select();

                //Overwrite the selection.
                wordApp.app.Selection.TypeText(EmergencyPhoneNumber);

                wordApp.document.Bookmarks.Add("EmergencyPhoneNumber", wordApp.range);
                ///////////////

                ////Add Book Mark Image
                Img1 = dt.Rows[0]["MailBoxImage"].ToString();
                wordApp.range = wordApp.document.Bookmarks["Img1"].Range;
                wordApp.range.InlineShapes.AddPicture(path + Img1);
                wordApp.range.InlineShapes[2].Delete();

                ////End
                ///
                ////Add Book Mark Image
                Img2 = dt.Rows[0]["BusinessCenterImage"].ToString();
                wordApp.range = wordApp.document.Bookmarks["Img2"].Range;
                wordApp.range.InlineShapes.AddPicture(path + Img2);
                wordApp.range.InlineShapes[2].Delete();

                ////End
                ///
                ////Add Book Mark Image
                Img3 = dt.Rows[0]["ElevatorFitnessImage"].ToString();
                wordApp.range = wordApp.document.Bookmarks["Img3"].Range;
                wordApp.range.InlineShapes.AddPicture(path + Img3);
                wordApp.range.InlineShapes[2].Delete();

                ////End
                ///
                ////Add Book Mark Image
                Img4 = dt.Rows[0]["ParkingTypeImage"].ToString();
                wordApp.range = wordApp.document.Bookmarks["Img4"].Range;
                wordApp.range.InlineShapes.AddPicture(path + Img4);
                wordApp.range.InlineShapes[2].Delete();

                ////End
                ///
                ////Add Book Mark Image
                Img5 = dt.Rows[0]["PoolImage"].ToString();
                wordApp.range = wordApp.document.Bookmarks["Img5"].Range;
                wordApp.range.InlineShapes.AddPicture(path + Img5);
                wordApp.range.InlineShapes[2].Delete();

                ////End

            }


            object format = wordApp.wdSave;

            wordApp.document.SaveAs2(path + "Q-" + Id.ToString() + "Texas-Standard-Residential-Lease-Agreement.pdf", format, false);

            //wordApp.document.Save(); //save the document.
            wordApp.document.Close(); //close the document.
            wordApp.app.Quit(); //close the Word.Application object. otherwise, you'll get a ReadOnly error later.

            
            return File(path + "Q-" + Id.ToString() + "Texas-Standard-Residential-Lease-Agreement.pdf", "text/plain", "Q-" + Id.ToString() + "Texas-Standard-Residential-Lease-Agreement.pdf");


        }
        public Int64 GenerateKeyID(string table, string id)
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = @"SELECT count(" + id + ") as cnt FROM [dbo]." + table;
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);
            con.Close();
            if (dt.Rows.Count > 0)
            {
                Int64 keyCount = Int64.Parse(dt.Rows[0]["cnt"].ToString());
                string KeyID = (keyCount + 1).ToString();
                int cnt  = KeyID.Length;
                if(cnt == 1)
                {
                    KeyID = "00000" + KeyID;
                }
                else if (cnt == 2)
                {
                    KeyID = "0000" + KeyID;
                }
                else if (cnt == 3)
                {
                    KeyID = "000" + KeyID;
                }
                else if (cnt == 4)
                {
                    KeyID = "00" + KeyID;
                }
                else if (cnt == 5)
                {
                    KeyID = "0" + KeyID;
                }
                else if (cnt == 6)
                {
                    
                }
                else
                {
                    
                }
                return Int64.Parse(KeyID);
            }
            else
            {
                return 0;
            }
        }
        public void SaveVendContact(List<VendContact> VendContactList, Int64 VendorId)
        {
            Session["Message"] = "";
            Session["SuccessMessage"] = "";


            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                if (VendContactList.Count > 0)
                {
                    foreach (var contact_Save in VendContactList)
                    {


                        if (contact_Save.VendContactId == 0)
                        {
                            if (USNumberValidator(contact_Save.VendContactNumber))
                            {
                                if (EmailValidator(contact_Save.VendContactEmail))
                                {
                                    if (!DuplicateCommon("VendContact", "VendContactId", "VendContactFirstName", "VendorId", contact_Save.VendContactFirstName, contact_Save.VendorId.ToString()))
                                    {

                                        cmd.CommandText = "VendContactInsert";
                                        cmd.CommandType = CommandType.StoredProcedure;

                                        cmd.Parameters.AddWithValue("@VendContactEmail", contact_Save.VendContactEmail);

                                        cmd.Parameters.AddWithValue("@VendContactNumber", contact_Save.VendContactNumber);

                                        cmd.Parameters.AddWithValue("@VendContactFirstName", contact_Save.VendContactFirstName);

                                        cmd.Parameters.AddWithValue("@VendContactLastName", contact_Save.VendContactLastName);

                                        cmd.Parameters.AddWithValue("@VendorId", VendorId);

                                        try
                                        {
                                            con.Open();
                                            cmd.ExecuteNonQuery();
                                            Session["error"] = null;
                                            Session["SuccessMessage"] = "Success: Contact Successfully Added";
                                        }
                                        catch (SqlException e)
                                        {

                                            ViewBag.error = "Transaction Failure";
                                            Session["error"] = ViewBag.error;
                                            Session["Message"] = e.Message;
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.Message = "Contact Name of this Vendor is already exists!";
                                        Session["Message"] = ViewBag.Message;
                                        Session["error"] = ViewBag.Message;
                                    }

                                }
                                else
                                {
                                    ViewBag.Message = "Email is Incorrect!";
                                    Session["Message"] = ViewBag.Message;
                                    Session["error"] = ViewBag.Message;
                                }

                            }
                            else
                            {
                                ViewBag.Message = "Phone Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                                Session["Message"] = ViewBag.Message;
                                Session["error"] = ViewBag.Message;
                            }
                        }

                        else
                        {
                            if (USNumberValidator(contact_Save.VendContactNumber))
                            {
                                if (EmailValidator(contact_Save.VendContactEmail))
                                {
                                    cmd.CommandText = "VendContactUpdate";
                                    cmd.CommandType = CommandType.StoredProcedure;


                                    cmd.Parameters.AddWithValue("@VendContactEmail", contact_Save.VendContactEmail);

                                    cmd.Parameters.AddWithValue("@VendContactNumber", contact_Save.VendContactNumber);

                                    cmd.Parameters.AddWithValue("@VendContactFirstName", contact_Save.VendContactFirstName);

                                    cmd.Parameters.AddWithValue("@VendContactLastName", contact_Save.VendContactLastName);

                                    cmd.Parameters.AddWithValue("@VendorId", contact_Save.VendorId);

                                    cmd.Parameters.AddWithValue("@VendContactId", contact_Save.VendContactId);

                                    try
                                    {
                                        con.Open();
                                        cmd.ExecuteNonQuery();
                                        Session["SuccessMessage"] = "Success: Contact Successfully Updated";
                                    }
                                    catch (SqlException e)
                                    {

                                        ViewBag.error = "Transaction Failure";
                                        Session["Message"] = e.Message;
                                        throw e;
                                    }
                                }
                                else
                                {
                                    ViewBag.Message = "Email is Incorrect!";
                                    Session["Message"] = ViewBag.Message;
                                    Session["error"] = ViewBag.Message;
                                }

                            }
                            else
                            {
                                ViewBag.Message = "Phone Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                                Session["Message"] = ViewBag.Message;
                                Session["error"] = ViewBag.Message;
                            }
                        }
                    }
                }
            }
            con.Close();

        }
        public void SavePropContact(List<PropContact> propContactList, Int64 PropertyId)
        {
            Session["Message"] = "";
            Session["SuccessMessage"] = "";


            SqlConnection con = new SqlConnection(connect);
            //SqlCommand cmd = new SqlCommand("select manufacturer_id, asset_id, asset_name,  deviceSN, folder_Id, external_id, active_ind, asset_description, latitude, longitude, street_address_1, street_address_2, asset_created_by, logical_termination_dt from dbo.asset where activated_by = 'admin@simplicityintegration.com'", con);

            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                if (propContactList.Count > 0)
                {
                    foreach (var contact_Save in propContactList)
                    {


                        if (contact_Save.PropContactId == 0)
                        {
                            if (USNumberValidator(contact_Save.PropContactNumber))
                            {
                                if (EmailValidator(contact_Save.PropContactEmail))
                                {
                                    if (!DuplicateCommon("PropContact", "PropContactId", "PropContactFirstName", "PropertyId", contact_Save.PropContactFirstName, contact_Save.PropertyId.ToString()))
                                    {

                                        cmd.CommandText = "PropContactInsert";
                                        cmd.CommandType = CommandType.StoredProcedure;

                                        cmd.Parameters.AddWithValue("@PropContactEmail", contact_Save.PropContactEmail);

                                        cmd.Parameters.AddWithValue("@PropContactNumber", contact_Save.PropContactNumber);

                                        cmd.Parameters.AddWithValue("@PropContactFirstName", contact_Save.PropContactFirstName);

                                        cmd.Parameters.AddWithValue("@PropContactLastName", contact_Save.PropContactLastName);

                                        cmd.Parameters.AddWithValue("@PropertyId", PropertyId);

                                        try
                                        {
                                            con.Open();
                                            cmd.ExecuteNonQuery();
                                            Session["error"] = null;
                                            Session["SuccessMessage"] = "Success: Contact Successfully Added";
                                        }
                                        catch (SqlException e)
                                        {

                                            ViewBag.error = "Transaction Failure";
                                            Session["error"] = ViewBag.error;
                                            Session["Message"] = e.Message;
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.Message = "First Name with same Company is already exists!";
                                        Session["Message"] = ViewBag.Message;
                                        Session["error"] = ViewBag.Message;
                                    }

                                }
                                else
                                {
                                    ViewBag.Message = "Email is Incorrect!";
                                    Session["Message"] = ViewBag.Message;
                                    Session["error"] = ViewBag.Message;
                                }

                            }
                            else
                            {
                                ViewBag.Message = "Phone Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                                Session["Message"] = ViewBag.Message;
                                Session["error"] = ViewBag.Message;
                            }
                        }

                        else
                        {
                            if (USNumberValidator(contact_Save.PropContactNumber))
                            {
                                if (EmailValidator(contact_Save.PropContactEmail))
                                {
                                    cmd.CommandText = "PropContactUpdate";
                                    cmd.CommandType = CommandType.StoredProcedure;


                                    cmd.Parameters.AddWithValue("@PropContactEmail", contact_Save.PropContactEmail);

                                    cmd.Parameters.AddWithValue("@PropContactNumber", contact_Save.PropContactNumber);

                                    cmd.Parameters.AddWithValue("@PropContactFirstName", contact_Save.PropContactFirstName);

                                    cmd.Parameters.AddWithValue("@PropContactLastName", contact_Save.PropContactLastName);

                                    cmd.Parameters.AddWithValue("@PropertyId", contact_Save.PropertyId);

                                    cmd.Parameters.AddWithValue("@PropContactId", contact_Save.PropContactId);

                                    try
                                    {
                                        con.Open();
                                        cmd.ExecuteNonQuery();
                                        Session["SuccessMessage"] = "Success: Contact Successfully Updated";
                                    }
                                    catch (SqlException e)
                                    {

                                        ViewBag.error = "Transaction Failure";
                                        Session["Message"] = e.Message;
                                        throw e;
                                    }
                                }
                                else
                                {
                                    ViewBag.Message = "Email is Incorrect!";
                                    Session["Message"] = ViewBag.Message;
                                    Session["error"] = ViewBag.Message;
                                }

                            }
                            else
                            {
                                ViewBag.Message = "Phone Number is in a Wrong Pattern! Please Follow this Pattern: (234)-123-4657 or +1-212-456-7890";
                                Session["Message"] = ViewBag.Message;
                                Session["error"] = ViewBag.Message;
                            }
                        }
                    }
                }
            }
            con.Close();
            
        }

        public List<Vendor> BindDataVendorAll(string VendorType)
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("VendorsSelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Vendor Vendor_Single = new Vendor();
            Vendor Vendor_Detail = new Vendor();
            JsonResult jR = new JsonResult();
            List<Vendor> Vendor_List = new List<Vendor>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    Vendor_Single.Address = dt.Rows[i]["Address"].ToString();
                    Vendor_Single.Address2 = dt.Rows[i]["Address2"].ToString();
                    Vendor_Single.Email = dt.Rows[i]["Email"].ToString();
                    Vendor_Single.VendorId = Int64.Parse(dt.Rows[i]["VendorId"].ToString());
                    Vendor_Single.VendorType = dt.Rows[i]["VendorType"].ToString();
                    Vendor_Single.Website = dt.Rows[i]["Website"].ToString();
                    Vendor_Single.Notes = dt.Rows[i]["Notes"].ToString();
                    Vendor_Single.CompanyName = dt.Rows[i]["CompanyName"].ToString();
                    Vendor_Single.State = dt.Rows[i]["State"].ToString();
                    Vendor_Single.Street = dt.Rows[i]["Street"].ToString();
                    Vendor_Single.City = dt.Rows[i]["City"].ToString();
                    Vendor_Single.PhoneNumber = dt.Rows[i]["PhoneNumber"].ToString();
                    Vendor_Single.Zip = dt.Rows[i]["Zip"].ToString();
                    Vendor_Single.DollarAmount = decimal.Parse(dt.Rows[i]["DollarAmount"].ToString());
                    Vendor_Single.PercentageAmount = decimal.Parse(dt.Rows[i]["PercentageAmount"].ToString());
                    Vendor_Single.IsActive = bool.Parse(dt.Rows[i]["IsActive"].ToString());
                    Vendor_List.Add(Vendor_Single);
                    Vendor_Single = new Vendor();

                }

                //if (id == 0)
                //{
                //    Vendor_Detail = Vendor_List[0];
                //}
                //else
                //{
                //    Vendor_Detail = Vendor_List.Where(a => a.VendorId == id).FirstOrDefault();
                //}
            }

            if(VendorType == "All")
            {
                
            }
            else
            {
                Vendor_List = Vendor_List.Where(x => x.Street.Contains(VendorType) == true || x.VendorType.Contains(VendorType) == true).ToList();
            }

            return Vendor_List;
        }


        public List<Status> BindDataStatusAll(decimal id = 0, string srchContactName = "", string srchOccupantName = "")
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("Select * From Statuses where IsActive = 1",con);
            cmd.CommandType = CommandType.Text;
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Status Status_Single = new Status();
            List<Status> Status_List = new List<Status>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    //Company_Single.Address = dt.Rows[i]["Address"].ToString();
                    Status_Single.StatusId = Int64.Parse( dt.Rows[i]["StatusId"].ToString());
                    Status_Single.Statuses = dt.Rows[i]["Status"].ToString();

                    Status_List.Add(Status_Single);
                    Status_Single = new Status();

                }

                
            }

            return Status_List;
        }

        public List<VendContact> BindDataVendContactAll(decimal id)
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("VendContactSelect", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@VendorId", id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            VendContact Contact_Single = new VendContact();
            VendContact Contact_Detail = new VendContact();
            JsonResult jR = new JsonResult();
            List<VendContact> Contact_List = new List<VendContact>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    //Contact_Single.Address = dt.Rows[i]["Address"].ToString();
                    Contact_Single.VendContactEmail = dt.Rows[i]["VendContactEmail"].ToString();
                    Contact_Single.VendContactId = Int64.Parse(dt.Rows[i]["VendContactId"].ToString());

                    Contact_Single.VendContactNumber = dt.Rows[i]["VendContactNumber"].ToString();

                    Contact_Single.VendContactFirstName = dt.Rows[i]["VendContactFirstName"].ToString();
                    Contact_Single.VendContactLastName = dt.Rows[i]["VendContactLastName"].ToString();
                    Contact_Single.VendorId = Int64.Parse(dt.Rows[i]["VendorId"].ToString());

                    Contact_List.Add(Contact_Single);
                    Contact_Single = new VendContact();

                }

                if (id == 0)
                {
                    Contact_Detail = Contact_List[0];
                }
                //else if(id == 1)
                //{
                //    ViewData["ProcessQuote"] = "p";
                //}
                else
                {
                    Contact_Detail = Contact_List.Where(a => a.VendContactId == id).FirstOrDefault();
                }
            }

            return Contact_List;
        }
        public List<PropContact> BindDataPropContactAll(decimal id)
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("PropContactSelect", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PropertyId", id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            PropContact Contact_Single = new PropContact();
            PropContact Contact_Detail = new PropContact();
            JsonResult jR = new JsonResult();
            List<PropContact> Contact_List = new List<PropContact>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    //Contact_Single.Address = dt.Rows[i]["Address"].ToString();
                    Contact_Single.PropContactEmail = dt.Rows[i]["PropContactEmail"].ToString();
                    Contact_Single.PropContactId = Int64.Parse(dt.Rows[i]["PropContactId"].ToString());

                    Contact_Single.PropContactNumber = dt.Rows[i]["PropContactNumber"].ToString();
                    
                    Contact_Single.PropContactFirstName = dt.Rows[i]["PropContactFirstName"].ToString();
                    Contact_Single.PropContactLastName = dt.Rows[i]["PropContactLastName"].ToString();
                    Contact_Single.PropertyId = Int64.Parse(dt.Rows[i]["PropertyId"].ToString());

                    Contact_List.Add(Contact_Single);
                    Contact_Single = new PropContact();

                }

                if (id == 0)
                {
                    Contact_Detail = Contact_List[0];
                }
                //else if(id == 1)
                //{
                //    ViewData["ProcessQuote"] = "p";
                //}
                else
                {
                    Contact_Detail = Contact_List.Where(a => a.PropContactId == id).FirstOrDefault();
                }
            }

            return Contact_List;
        }
        public List<Contact> BindDataContactAll(decimal id)
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("ContactsSelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Contact Contact_Single = new Contact();
            Contact Contact_Detail = new Contact();
            JsonResult jR = new JsonResult();
            List<Contact> Contact_List = new List<Contact>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    //Contact_Single.Address = dt.Rows[i]["Address"].ToString();
                    Contact_Single.Email = dt.Rows[i]["Email"].ToString();
                    Contact_Single.Id = Int64.Parse(dt.Rows[i]["Id"].ToString());

                    Contact_Single.Phone = dt.Rows[i]["Phone"].ToString();
                    Contact_Single.Address = dt.Rows[i]["Address"].ToString();
                    Contact_Single.FirstName = dt.Rows[i]["FirstName"].ToString();
                    Contact_Single.LastName = dt.Rows[i]["LastName"].ToString();
                    Contact_Single.IsActive = bool.Parse(dt.Rows[i]["IsActive"].ToString());
                    Contact_Single.Company = dt.Rows[i]["Company"].ToString();
                    Contact_Single.CompanyId = Int64.Parse(dt.Rows[i]["CompanyId"].ToString());
                    Contact_Single.DOB = DateTime.Parse(dt.Rows[i]["DOB"].ToString());

                    Contact_Single.City = dt.Rows[i]["City"].ToString();
                    Contact_Single.State = dt.Rows[i]["State"].ToString();
                    Contact_Single.Zip = Int64.Parse(dt.Rows[i]["Zip"].ToString());
                    Contact_Single.Address2 = dt.Rows[i]["Address2"].ToString();

                    Contact_List.Add(Contact_Single);
                    Contact_Single = new Contact();

                }

                if (id == 0)
                {
                    Contact_Detail = Contact_List[0];
                }
                //else if(id == 1)
                //{
                //    ViewData["ProcessQuote"] = "p";
                //}
                else
                {
                    Contact_Detail = Contact_List.Where(a => a.Id == id).FirstOrDefault();
                }
            }

            return Contact_List;
        }

        public List<ReferalSource> BindDataReferalAllWithNoReferral(decimal Id)
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("ReferalSourcesSelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            ReferalSource ReferalSource_Single = new ReferalSource();
            ReferalSource ReferalSource_Detail = new ReferalSource();
            JsonResult jR = new JsonResult();
            List<ReferalSource> ReferalSource_List = new List<ReferalSource>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    ReferalSource_Single.Address = dt.Rows[i]["Address"].ToString();

                    ReferalSource_Single.ReferalSourceId = Int64.Parse(dt.Rows[i]["ReferalSourceId"].ToString());

                    ReferalSource_Single.CompanyName = dt.Rows[i]["CompanyName"].ToString();

                    ReferalSource_Single.ReferalType = dt.Rows[i]["ReferalType"].ToString();

                    ReferalSource_Single.Number = decimal.Parse( dt.Rows[i]["Number"].ToString());

                    ReferalSource_Single.CostPerDay = decimal.Parse(dt.Rows[i]["CostPerDay"].ToString());

                    ReferalSource_Single.IsActive = bool.Parse(dt.Rows[i]["IsActive"].ToString());

                    ReferalSource_List.Add(ReferalSource_Single);
                    ReferalSource_Single = new ReferalSource();

                }

                if (Id == 0)
                {
                    ReferalSource_Detail = ReferalSource_List[0];
                }
                else
                {
                    ReferalSource_Detail = ReferalSource_List.Where(a => a.ReferalSourceId == Id).FirstOrDefault();
                }
            }
            //ReferalSource_List = ReferalSource_List.Where(v => v.CompanyName != "No Referral").ToList();
            return ReferalSource_List;
        }
        public List<ReferalSource> BindDataReferalAll(decimal Id)
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("ReferalSourcesSelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            ReferalSource ReferalSource_Single = new ReferalSource();
            ReferalSource ReferalSource_Detail = new ReferalSource();
            JsonResult jR = new JsonResult();
            List<ReferalSource> ReferalSource_List = new List<ReferalSource>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    ReferalSource_Single.Address = dt.Rows[i]["Address"].ToString();

                    ReferalSource_Single.ReferalSourceId = Int64.Parse(dt.Rows[i]["ReferalSourceId"].ToString());

                    ReferalSource_Single.CompanyName = dt.Rows[i]["CompanyName"].ToString();

                    ReferalSource_Single.ReferalType = dt.Rows[i]["ReferalType"].ToString();

                    ReferalSource_Single.Number = decimal.Parse(dt.Rows[i]["Number"].ToString());

                    ReferalSource_Single.CostPerDay = decimal.Parse( dt.Rows[i]["CostPerDay"].ToString());

                    ReferalSource_Single.IsActive = bool.Parse(dt.Rows[i]["IsActive"].ToString());

                    ReferalSource_List.Add(ReferalSource_Single);
                    ReferalSource_Single = new ReferalSource();

                }

                if (Id == 0)
                {
                    ReferalSource_Detail = ReferalSource_List[0];
                }
                else
                {
                    ReferalSource_Detail = ReferalSource_List.Where(a => a.ReferalSourceId == Id).FirstOrDefault();
                }
            }
            ReferalSource_List = ReferalSource_List.Where(v => v.CompanyName != "No Referral").ToList();
            return ReferalSource_List;
        }
        public List<Company> BindDataCompanyAll(decimal id = 0, string srchContactName = "", string srchOccupantName = "")
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("CompanySelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Company Company_Single = new Company();
            Company Company_Detail = new Company();
            JsonResult jR = new JsonResult();
            List<Company> Company_List = new List<Company>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    //Company_Single.Address = dt.Rows[i]["Address"].ToString();
                    Company_Single.CompanyEmail = dt.Rows[i]["CompanyEmail"].ToString();
                    Company_Single.CompanyId = Int64.Parse(dt.Rows[i]["CompanyId"].ToString());

                    Company_Single.City = dt.Rows[i]["City"].ToString();
                    Company_Single.CompanyContact = dt.Rows[i]["CompanyContact"].ToString();
                    Company_Single.Address = dt.Rows[i]["Address"].ToString();
                    Company_Single.Website = dt.Rows[i]["Website"].ToString();
                    Company_Single.CompanyName = dt.Rows[i]["CompanyName"].ToString();
                    Company_Single.PreferedArea = dt.Rows[i]["PreferedArea"].ToString();
                    Company_Single.State = "";

                    Company_Single.IsActive = bool.Parse(dt.Rows[i]["IsActive"].ToString());

                    Company_List.Add(Company_Single);
                    Company_Single = new Company();

                }

                if (id == 0)
                {
                    Company_Detail = Company_List[0];
                }
                //else if(id == 1)
                //{
                //    ViewData["ProcessQuote"] = "p";
                //}
                else
                {
                    Company_Detail = Company_List.Where(a => a.CompanyId == id).FirstOrDefault();
                }
            }

            return Company_List;
        }
        public Company BindDataCompany(Int64 Id)
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("CompanySelect", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyId", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Company Company_Single = new Company();
            Company Company_Detail = new Company();
            JsonResult jR = new JsonResult();
            List<Company> Company_List = new List<Company>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dt.Rows.Count > 0)
            {


                Company_Single.CompanyEmail = dt.Rows[0]["CompanyEmail"].ToString();
                Company_Single.CompanyId = Int64.Parse(dt.Rows[0]["CompanyId"].ToString());

                Company_Single.City = "";//dt.Rows[0]["City"].ToString(); Removed By Client
                Company_Single.CompanyContact = dt.Rows[0]["CompanyContact"].ToString();
                Company_Single.Address = dt.Rows[0]["Address"].ToString();
                Company_Single.Website = dt.Rows[0]["Website"].ToString();
                Company_Single.CompanyName = dt.Rows[0]["CompanyName"].ToString();
                Company_Single.PreferedArea = dt.Rows[0]["PreferedArea"].ToString();
                Company_Single.State = dt.Rows[0]["State"].ToString();

                Company_Single.Address2 = dt.Rows[0]["Address2"].ToString();
                
                //Added these new fields By Shahab

                Company_Single.City = dt.Rows[0]["City"].ToString();
                if(dt.Rows[0]["Zip"] == null || dt.Rows[0]["Zip"].ToString() == "" || decimal.Parse( dt.Rows[0]["Zip"].ToString()) == 0)
                {
                    Company_Single.Zip = null;
                }
                else
                {
                    Company_Single.Zip = decimal.Parse(dt.Rows[0]["Zip"].ToString());
                }
                
                
                Company_Single.IsActive = bool.Parse(dt.Rows[0]["IsActive"].ToString());

                ViewBag.City = DropDownListCity(dt.Rows[0]["City"].ToString());
                ViewBag.States = BindDataStatesAll(0);
            }
            else
            {
                Company_Single = new Company()
                {
                    CompanyId = 0,
                    CompanyEmail = "",

                    City = "",
                    CompanyContact = "",
                    CompanyName = "",
                    IsActive = true,
                    Website = "",

                    Address = "",
                    Address2 = "",
                    Zip = 75581,

                    PreferedArea = "",

                    State = ""

                };
                ViewBag.City = DropDownListCity("");
                ViewBag.States = BindDataStatesAll(0);
            }

            return Company_Single;
        }

        public List<Property> BindDataPropertyAll(decimal id = 0, string srchContactName = "", string srchOccupantName = "")
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("PropertiesSelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
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
                    Property_Single.Leased = bool.Parse(dt.Rows[i]["Leased"].ToString());
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
                    Property_Single.ValetTrash = dt.Rows[i]["ValetTrash"].ToString();

                    Property_Single.UnitSize = dt.Rows[i]["UnitSize"].ToString();
                    Property_Single.UnitSquareFootage = dt.Rows[i]["UnitSquareFootage"].ToString();
                    Property_Single.UnitType = dt.Rows[i]["UnitType"].ToString();
                    if(dt.Rows[i]["UnitType"].ToString() == "Other")
                    {
                        Property_Single.UnitType = dt.Rows[i]["UnitCustom"].ToString();
                    }
                    Property_Single.VendorId = Int64.Parse(dt.Rows[i]["VendorId"].ToString());
                    Property_Single.VendorName = dt.Rows[i]["VendorName"].ToString();
                    Property_Single.WebSite = dt.Rows[i]["WebSite"].ToString();
                    Property_Single.Pool = dt.Rows[i]["Pool"].ToString();
                    Property_Single.PhoneNumber = dt.Rows[i]["PhoneNumber"].ToString();
                    Property_Single.ParkingType = dt.Rows[i]["ParkingType"].ToString();
                    if (dt.Rows[i]["ParkingType"].ToString() == "Other")
                    {
                        Property_Single.ParkingType = dt.Rows[i]["PTCustom"].ToString();
                    }
                    Property_Single.OtherDepositAmount = dt.Rows[i]["OtherDepositAmount"].ToString();
                    Property_Single.OtherDeposit = dt.Rows[i]["OtherDeposit"].ToString();
                    if (dt.Rows[i]["OtherDeposit"].ToString() == "Other")
                    {
                        Property_Single.OtherDeposit = dt.Rows[i]["DepositCustom"].ToString();
                    }
                    Property_Single.NoticetoVacate = dt.Rows[i]["NoticetoVacate"].ToString();
                    if (dt.Rows[i]["NoticetoVacate"].ToString() == "Other")
                    {
                        Property_Single.NoticetoVacate = dt.Rows[i]["NTVCustom"].ToString();
                    }
                    Property_Single.Name = dt.Rows[i]["Name"].ToString();
                    Property_Single.MailboxLocation = dt.Rows[i]["MailboxLocation"].ToString();
                    Property_Single.LeaseEndDate = dt.Rows[i]["LeaseEndDate"].ToString();
                    if (dt.Rows[i]["LeaseEndDate"].ToString() == "Other")
                    {
                        Property_Single.LeaseEndDate = dt.Rows[i]["LEDCustom"].ToString();
                    }
                    Property_Single.Hours = dt.Rows[i]["Hours"].ToString();
                    //Property_Single.FloorPlanPic3;
                    if (dt.Rows[i]["FloorPlanPic3"].ToString() == "")
                    {
                        Property_Single.FloorPlanPic3 = "No Image";
                    }
                    else
                    {
                        Property_Single.FloorPlanPic3 = dt.Rows[i]["FloorPlanPic3"].ToString();
                    }
                    //Property_Single.FloorPlanPic2;
                    if (dt.Rows[i]["FloorPlanPic2"].ToString() == "")
                    {
                        Property_Single.FloorPlanPic2 = "No Image";
                    }
                    else
                    {
                        Property_Single.FloorPlanPic2 = dt.Rows[i]["FloorPlanPic2"].ToString();
                    }
                    //Property_Single.FloorPlanPic1;
                    if (dt.Rows[i]["FloorPlanPic1"].ToString() == "")
                    {
                        Property_Single.FloorPlanPic1 = "No Image";
                    }
                    else
                    {
                        Property_Single.FloorPlanPic1 = dt.Rows[i]["FloorPlanPic1"].ToString();
                    }
                    //Property_Single.FloorPlanPic;
                    if (dt.Rows[i]["FloorPlanPic"].ToString() == "")
                    {
                        Property_Single.FloorPlanPic = "No Image";
                    }
                    else
                    {
                        Property_Single.FloorPlanPic = dt.Rows[i]["FloorPlanPic"].ToString();
                    }
                    Property_Single.Floor = dt.Rows[i]["Floor"].ToString();
                    Property_Single.Fitness = dt.Rows[i]["Fitness"].ToString();
                    Property_Single.Features = dt.Rows[i]["Features"].ToString();
                    Property_Single.EmergencyPhoneNumber = dt.Rows[i]["EmergencyPhoneNumber"].ToString();

                    Property_Single.Elevator = dt.Rows[i]["Elevator"].ToString();

                    Property_Single.IsActive = bool.Parse(dt.Rows[i]["IsActive"].ToString());



                    Property_List.Add(Property_Single);
                    Property_Single = new Property();

                }

                if (id == 0)
                {
                    ViewData["ProcessQuote"] = null;
                    Property_Detail = Property_List[0];
                }
                else
                {
                    ViewData["ProcessQuote"] = "p";
                    ViewData["LeadId"] = id;
                    Property_List = Property_List.Where(x => x.Leased == false).ToList();
                    //Property_Detail = Property_List.Where(a => a.PropertyId == id).FirstOrDefault();
                }
            }
            return Property_List;
        }
        public List<Lead> BindDataLeadAll(decimal id = 0, string srchContactName = "", string srchOccupantName = "")
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("LeadsSelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            Lead Lead_Single = new Lead();
            ReferalSource Referal_Single = new ReferalSource();
            Lead Lead_Detail = new Lead();
            JsonResult jR = new JsonResult();
            List<Lead> Lead_List = new List<Lead>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    //Lead_Single.Address = dt.Rows[i]["Address"].ToString();
                    Lead_Single.ContactEmail = dt.Rows[i]["ContactEmail"].ToString();
                    Lead_Single.LeadsId = Int64.Parse(dt.Rows[i]["LeadsId"].ToString());
                    Lead_Single.Breed = dt.Rows[i]["Breed"].ToString();
                    Lead_Single.City = dt.Rows[i]["City"].ToString();
                    Lead_Single.ContactNumber = dt.Rows[i]["ContactNumber"].ToString();
                    Lead_Single.ContactType = dt.Rows[i]["ContactType"].ToString();
                    Lead_Single.FloorPreference = dt.Rows[i]["FloorPreference"].ToString();
                    Lead_Single.LeadsName = dt.Rows[i]["LeadsName"].ToString();
                    Lead_Single.LeaseTerm = dt.Rows[i]["LeaseTerm"].ToString();
                    Lead_Single.MoveInDate = DateTime.Parse(dt.Rows[i]["MoveInDate"].ToString());
                    Lead_Single.NoOfAdults = dt.Rows[i]["NoOfAdults"].ToString();
                    Lead_Single.NoOfBedRooms = dt.Rows[i]["NoOfBedRooms"].ToString();
                    Lead_Single.NoOfChildren = dt.Rows[i]["NoOfChildren"].ToString();
                    Lead_Single.NoOfPets = dt.Rows[i]["NoOfPets"].ToString();
                    Lead_Single.Notes = dt.Rows[i]["Notes"].ToString();
                    Lead_Single.OcupantName = dt.Rows[i]["OcupantName"].ToString();
                    Lead_Single.PreferedArea = dt.Rows[i]["PreferedArea"].ToString();
                    Lead_Single.PreferedAddress = dt.Rows[i]["PreferedAddress"].ToString();
                    Lead_Single.CompanyLogo = dt.Rows[i]["CompanyLogo"].ToString();
                    Lead_Single.ReferelSource = dt.Rows[i]["ReferelSource"].ToString();
                    Lead_Single.Address = dt.Rows[i]["Address"].ToString();
                    Lead_Single.Address2 = dt.Rows[i]["Address2"].ToString();
                    Lead_Single.City = dt.Rows[i]["City"].ToString();
                    Lead_Single.State = dt.Rows[i]["State"].ToString();

                    Lead_Single.Weight = decimal.Parse(dt.Rows[i]["Weight"].ToString());
                    Lead_Single.Zip = decimal.Parse(dt.Rows[i]["Zip"].ToString());
                    Lead_Single.IsActive = bool.Parse(dt.Rows[i]["IsActive"].ToString());
                    Lead_List.Add(Lead_Single);
                    Lead_Single = new Lead();

                }

                if (id == 0)
                {
                    Lead_Detail = Lead_List[0];
                }
                //else if(id == 1)
                //{
                //    ViewData["ProcessQuote"] = "p";
                //}
                else
                {
                    Lead_Detail = Lead_List.Where(a => a.LeadsId == id).FirstOrDefault();
                }
            }

            return Lead_List;
        }

        public List<Quote> BindDataQuoteAll(decimal id = 0, string srchContactName = "", string srchOccupantName = "")
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("QuotesSelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();


            /////Reservation All
            ///////////


            //con = new SqlConnection(connect);
            //cmd = new SqlCommand("ReservationsSelectAll", con);
            //cmd.CommandType = CommandType.StoredProcedure;
            //SqlDataReader Dr_Reser;
            //con.Open();
            //Dr_Reser = cmd.ExecuteReader();
            //DataTable dt_Reser = new DataTable("Vw");
            //dt_Reser.Load(Dr_Reser);

            //con.Close();

            Quote Quote_Single = new Quote();
            Quote Quote_Detail = new Quote();
            JsonResult jR = new JsonResult();
            List<Quote> Quote_List = new List<Quote>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    //Quote_Single.Address = dt.Rows[i]["Address"].ToString();
                    Quote_Single.OneTimeFurnitureDeliveryFee = decimal.Parse(dt.Rows[i]["OneTimeFurnitureDeliveryFee"].ToString());
                    Quote_Single.QuoteId = Int64.Parse(dt.Rows[i]["QuoteId"].ToString());
                    Quote_Single.CreditCard = dt.Rows[i]["CreditCard"].ToString();
                    Quote_Single.MonthlyCableFee = decimal.Parse(dt.Rows[i]["MonthlyCableFee"].ToString());
                    Quote_Single.MonthlyFurnitureUsageFee = decimal.Parse(dt.Rows[i]["MonthlyFurnitureUsageFee"].ToString());
                    Quote_Single.LeadsId = Int64.Parse(dt.Rows[i]["LeadsId"].ToString());
                    Quote_Single.LeaseEndDate = DateTime.Parse(dt.Rows[i]["LeaseEndDate"].ToString());
                    Quote_Single.LeaseStartDate = DateTime.Parse(dt.Rows[i]["LeaseStartDate"].ToString());
                    Quote_Single.MonthlyCableFee = decimal.Parse(dt.Rows[i]["MonthlyCableFee"].ToString());
                    Quote_Single.MonthlyCourierFee = decimal.Parse(dt.Rows[i]["MonthlyCourierFee"].ToString());
                    Quote_Single.MonthlyElectricFee = decimal.Parse(dt.Rows[i]["MonthlyElectricFee"].ToString());
                    Quote_Single.MonthlyFridgeFee = decimal.Parse(dt.Rows[i]["MonthlyFridgeFee"].ToString());
                    Quote_Single.MonthlyFurnitureUsageFee = decimal.Parse(dt.Rows[i]["MonthlyFurnitureUsageFee"].ToString());
                    Quote_Single.MonthlyGasFee = decimal.Parse(dt.Rows[i]["MonthlyGasFee"].ToString());
                    Quote_Single.MonthlyHouseWaversFee = decimal.Parse(dt.Rows[i]["MonthlyHouseWaversFee"].ToString());
                    Quote_Single.MonthlyInternetFee = decimal.Parse(dt.Rows[i]["MonthlyInternetFee"].ToString());
                    Quote_Single.MonthlyMarketingFee = decimal.Parse(dt.Rows[i]["MonthlyMarketingFee"].ToString());
                    Quote_Single.MonthlyMicrowaveFee = decimal.Parse(dt.Rows[i]["MonthlyMicrowaveFee"].ToString());
                    Quote_Single.MonthlyPetRentFee = decimal.Parse(dt.Rows[i]["MonthlyPetRentFee"].ToString());

                    Quote_Single.OneTimeOtherName = dt.Rows[i]["OneTimeOtherName"].ToString();
                    Quote_Single.OneTimeOther = decimal.Parse(dt.Rows[i]["OneTimeOther"].ToString());
                    Quote_Single.MonthlyOtherName = dt.Rows[i]["MonthlyOtherName"].ToString();
                    Quote_Single.MonthlyOther = decimal.Parse(dt.Rows[i]["MonthlyOther"].ToString());


                    Quote_Single.MonthlyPropertyRent = decimal.Parse(dt.Rows[i]["MonthlyPropertyRent"].ToString());
                    Quote_Single.MonthlyReferalFee = decimal.Parse(dt.Rows[i]["MonthlyReferalFee"].ToString());
                    Quote_Single.MonthlyValetTrashFee = decimal.Parse(dt.Rows[i]["MonthlyValetTrashFee"].ToString());
                    Quote_Single.MonthlyWasherDrayerFee = decimal.Parse(dt.Rows[i]["MonthlyWasherDrayerFee"].ToString());

                    Quote_Single.MonthlyWaterSewerTrashFee = decimal.Parse(dt.Rows[i]["MonthlyWaterSewerTrashFee"].ToString());
                    Quote_Single.OneTimeAdminFee = decimal.Parse(dt.Rows[i]["OneTimeAdminFee"].ToString());
                    Quote_Single.OneTimeAmnityFee = decimal.Parse(dt.Rows[i]["OneTimeAmnityFee"].ToString());
                    Quote_Single.OneTimeHouseWaversSetupFee = decimal.Parse(dt.Rows[i]["OneTimeHouseWaversSetupFee"].ToString());
                    Quote_Single.IsActive = bool.Parse(dt.Rows[i]["IsActive"].ToString());
                    Quote_Single.ParkingPlaces = Int16.Parse(dt.Rows[i]["ParkingPlaces"].ToString());
                    Quote_Single.ParkingType = dt.Rows[i]["ParkingType"].ToString();
                    Quote_Single.PropertyId = Int64.Parse(dt.Rows[i]["PropertyId"].ToString());
                    Quote_Single.property = BindDataPropertyAll(0).Where(m => m.PropertyId == Int64.Parse(dt.Rows[i]["PropertyId"].ToString())).FirstOrDefault();
                    Quote_Single.lead = BindDataLeadAll(0).Where(m => m.LeadsId == Int64.Parse(dt.Rows[i]["LeadsId"].ToString())).FirstOrDefault();
                    Quote_List.Add(Quote_Single);
                    Quote_Single = new Quote();

                }

                if (id == 0)
                {
                    Quote_Detail = Quote_List[0];
                }
                else
                {
                    Quote_Detail = Quote_List.Where(a => a.QuoteId == id).FirstOrDefault();
                }
            }

            return Quote_List;
        }

        public List<States> BindDataStatesAll(decimal Id)
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("StatesSelectALL", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            States States_Single = new States();
            States States_Detail = new States();
            JsonResult jR = new JsonResult();
            List<States> States_List = new List<States>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    States_Single.Code =  dt.Rows[i]["Code"].ToString();

                    States_Single.Name = dt.Rows[i]["Name"].ToString();

                    States_List.Add(States_Single);
                    States_Single = new States();

                }

                if (Id == 0)
                {
                    States_Detail = States_List[0];
                }
                else
                {
                    States_Detail = States_List.Where(a => a.Code == "TX").FirstOrDefault();
                }
            }
            //States_List = States_List.Where(v => v.CompanyName != "No Referral").ToList();
            return States_List;
        }

        /// <summary>
        /// /////Common Validators
        /// </summary>
        /// <param name="str_name"></param>
        /// <returns></returns>
        public bool EmailValidator(string str_name)
        {
            var ind = str_name.IndexOf("@") + 1;
            var len = (str_name.Length - ind);
            

            if(str_name.Length>= 6)
            { 
                
                if (!str_name.Contains("@") == true)
                {
                    return false;
                }
                else if (str_name.ElementAt(0).ToString().Contains("@") || str_name.ElementAt(0).ToString().Contains("."))
                {
                    return false;
                }
                else if (str_name.Substring(ind, len).ElementAt(0).ToString().Contains(".") == true)
                {

                    return false;
                }
                else if (!str_name.Substring(ind, len).Contains(".") == true)
                {

                    return false;
                }
                else if (str_name.ElementAt(str_name.Length - 1).ToString().Contains(".") == true)
                {

                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

        }

        public bool EmailValidatorWithEmptyOk(string str_name)
        {
            var ind = str_name.IndexOf("@") + 1;
            var len = (str_name.Length - ind);

            if (!string.IsNullOrEmpty(str_name))
            {
                if (str_name.Length >= 6)
                {

                    if (!str_name.Contains("@") == true)
                    {
                        return false;
                    }
                    else if (str_name.ElementAt(0).ToString().Contains("@") || str_name.ElementAt(0).ToString().Contains("."))
                    {
                        return false;
                    }
                    else if (str_name.Substring(ind, len).ElementAt(0).ToString().Contains(".") == true)
                    {

                        return false;
                    }
                    else if (!str_name.Substring(ind, len).Contains(".") == true)
                    {

                        return false;
                    }
                    else if (str_name.ElementAt(str_name.Length - 1).ToString().Contains(".") == true)
                    {

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            { return true; }
                

        }
        public bool USNumberValidatorWithEmptyOK(string str_name)
        {

            //string motif = @"[0-9 ]{1,9}";
            string without = str_name.Replace("(+1)", "");
            if(!string.IsNullOrEmpty(str_name))
            {
                if (str_name.Length == 14)
                {

                    if (str_name.ElementAt(0).ToString().Contains("("))
                    {
                        if (Regex.IsMatch(str_name.Substring(1, 3), "[0-9]{3}"))
                        {
                            if (str_name.ElementAt(4).ToString().Contains(")") && str_name.ElementAt(5).ToString().Contains("-"))
                            {
                                if (Regex.IsMatch(str_name.Substring(6, 3), "[0-9]{3}"))
                                {
                                    if (str_name.ElementAt(9).ToString().Contains("-"))
                                    {
                                        if (Regex.IsMatch(str_name.Substring(10, 4), "[0-9]{4}"))
                                        {
                                            return true;
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }

                    }
                    else
                    {
                        return false;
                    }
                }
                else if (str_name.Length == 15)
                {

                    if (str_name.ElementAt(0).ToString().Contains("+") && str_name.ElementAt(1).ToString().Contains("1") && str_name.ElementAt(2).ToString().Contains("-"))
                    {
                        if (Regex.IsMatch(str_name.Substring(3, 3), "[0-9]{3}"))
                        {
                            if (str_name.ElementAt(6).ToString().Contains("-"))
                            {
                                if (Regex.IsMatch(str_name.Substring(7, 3), "[0-9]{3}"))
                                {
                                    if (str_name.ElementAt(10).ToString().Contains("-"))
                                    {
                                        if (Regex.IsMatch(str_name.Substring(11, 4), "[0-9]{4}"))
                                        {
                                            return true;
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }

                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            { return true; }
            

        }
        public bool USNumberValidator(string str_name)
        {
            
            //string motif = @"[0-9 ]{1,9}";
            string without = str_name.Replace("(+1)", "");
            
            if (str_name.Length == 14)
            {

                if (str_name.ElementAt(0).ToString().Contains("("))
                {
                    if (Regex.IsMatch(str_name.Substring(1, 3), "[0-9]{3}"))
                    {
                        if (str_name.ElementAt(4).ToString().Contains(")") && str_name.ElementAt(5).ToString().Contains("-"))
                        {
                            if (Regex.IsMatch(str_name.Substring(6, 3), "[0-9]{3}"))
                            {
                                if (str_name.ElementAt(9).ToString().Contains("-"))
                                {
                                    if (Regex.IsMatch(str_name.Substring(10, 4), "[0-9]{4}"))
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            else if (str_name.Length == 15)
            {

                if (str_name.ElementAt(0).ToString().Contains("+") && str_name.ElementAt(1).ToString().Contains("1") && str_name.ElementAt(2).ToString().Contains("-"))
                {
                    if (Regex.IsMatch(str_name.Substring(3, 3), "[0-9]{3}"))
                    {
                        if (str_name.ElementAt(6).ToString().Contains("-"))
                        {
                            if (Regex.IsMatch(str_name.Substring(7, 3), "[0-9]{3}"))
                            {
                                if (str_name.ElementAt(10).ToString().Contains("-"))
                                {
                                    if (Regex.IsMatch(str_name.Substring(11, 4), "[0-9]{4}"))
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public bool WebSiteValidator(string str_name)
        {
            var ind = str_name.IndexOf("www.")+4;
            var len = (str_name.Length - ind);


            if (str_name.Length >= 6)
            {
                if (!str_name.Contains("..") && !str_name.Contains("\\") && !str_name.Contains("@"))
                {
                    if (ind != 4)
                    {
                        return false;
                    }
                    //else if (str_name.Substring(4, len).Contains("."))
                    //{
                    //    return false;
                    //}
                    else if (str_name.ElementAt(5).ToString().Contains(".") == true)
                    {

                        return false;
                    }
                    else if (!str_name.Substring(4, len).Contains(".") == true)
                    {

                        return false;
                    }
                    
                    else if (str_name.ElementAt(str_name.Length - 1).ToString().Contains(".") == true)
                    {

                        return false;
                    }
                    else if (str_name.ElementAt(str_name.Length - 2).ToString().Contains(".") == true)
                    {

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// ///Vendor Duplicate
        /// </summary>
        /// <param name="str_name"></param>
        /// <param name="str2_name"></param>
        /// <returns></returns>
        public bool DuplicateAsset(string str_name, string str2_name)
        {

            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = @"SELECT VendorId
                                FROM [dbo].[Vendors] where CompanyName='" + str_name + "' and VendorType='" + str2_name + "'";
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

        public bool DuplicateCommon(string str_table, string str_id,string str_chk1, string str_chk2,string str_name, string str2_name)
        {

            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = @"SELECT " + str_id + " FROM [dbo]." + str_table + " where " + str_chk1 + "='" + str_name + "' and " + str_chk2 + "='" + str2_name + "'";
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

        public bool DuplicateSingleCommon(string str_table, string str_id, string str_chk1, string str_name)
        {

            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = @"SELECT " + str_id + " FROM [dbo]." + str_table + " where " + str_chk1 + "='" + str_name + "'";
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

        /// <summary>
        /// ///////Referel Duplicate
        /// </summary>
        /// <param name="str_name"></param>
        /// <returns></returns>
        public bool DuplicateReferel(string str_name)
        {

            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = @"SELECT ReferalSourceId
                                FROM [dbo].[ReferalSources] where CompanyName='" + str_name + "'";
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

        /// <summary>
        /// ///////Vendor DropDown List
        /// </summary>
        /// <param name="vendorType"></param>
        /// <returns></returns>
        /// 

        public List<SelectListItem> DropDownListUnitType(string ReferelType)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();
   
            SelectListItem item = new SelectListItem();
            if (ReferelType != "")
            {
                item = new SelectListItem()
                {
                    Value = ReferelType,
                    Text = ReferelType
                };
                ProductList1.Add(item);
            }
            if (ReferelType == "Property")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Property",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Property"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            if (ReferelType == "Turnkey")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Turnkey",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Turnkey"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (ReferelType == "Private Owner")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Private Owner",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Private Owner"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (ReferelType == "Hotel")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Hotel",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Hotel"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (ReferelType == "Other")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Other",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Other"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            return ProductList1;
        }

        public List<SelectListItem> DropDownListOtherDeposit(string ReferelType)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (ReferelType != "")
            {
                item = new SelectListItem()
                {
                    Value = ReferelType,
                    Text = ReferelType
                };
                ProductList1.Add(item);
            }
            if (ReferelType == "Refundable")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Refundable",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Refundable"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            if (ReferelType == "Non-Refundable")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Non-Refundable",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Non-Refundable"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (ReferelType == "Other")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Other",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Other"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            return ProductList1;
        }

        public List<SelectListItem> DropDownListNoticeToVacate(string ReferelType)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (ReferelType != "")
            {
                item = new SelectListItem()
                {
                    Value = ReferelType,
                    Text = ReferelType
                };
                ProductList1.Add(item);
            }
            if (ReferelType == "20")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "20",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "20"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            if (ReferelType == "30")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "30",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "30"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (ReferelType == "60")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "60",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "60"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (ReferelType == "Other")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Other",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Other"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            return ProductList1;
        }

        public List<SelectListItem> DropDownListCreditCardFee(string ReferelType)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            
            if (ReferelType != "")
            {
                string val = "";
                string txt = "";
                if (ReferelType == "N/A" || ReferelType == "0")
                {
                    val = "0";
                    txt = "N/A";
                }
                else if (ReferelType == "2%" || ReferelType == "0.02")
                {
                    val = "0.02";
                    txt = "2%";
                }
                else if (ReferelType == "3%" || ReferelType == "0.03")
                {
                    val = "0.03";
                    txt = "3%";
                }
                else if (ReferelType == "4%" || ReferelType == "0.04")
                {
                    val = "0.04";
                    txt = "4%";
                }
                item = new SelectListItem()
                {
                    Value = val,
                    Text = txt
                };
                ProductList1.Add(item);
            }
            if (ReferelType == "N/A" || ReferelType == "0")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "0",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "N/A"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            if (ReferelType == "2%" || ReferelType == "0.02")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "0.02",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "2%"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (ReferelType == "3%" || ReferelType == "0.03")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "0.03",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "3%"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (ReferelType == "4%" || ReferelType == "0.04")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "0.04",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "4%"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            return ProductList1;
        }

        public List<SelectListItem> DropDownListLeaseEndDate(string ReferelType)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (ReferelType != "")
            {
                item = new SelectListItem()
                {
                    Value = ReferelType,
                    Text = ReferelType
                };
                ProductList1.Add(item);
            }
            if (ReferelType == "Last Day of Month")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Last Day of Month",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Last Day of Month"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            if (ReferelType == "Anniversary Date")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Anniversary Date",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Anniversary Date"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (ReferelType == "Other")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Other",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Other"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            return ProductList1;
        }

        public List<SelectListItem> DropDownListWasherDryerType(string ReferelType)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (ReferelType != "")
            {
                item = new SelectListItem()
                {
                    Value = ReferelType,
                    Text = ReferelType
                };
                ProductList1.Add(item);
            }
            if (ReferelType == "Full Size")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Full Size",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Full Size"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            if (ReferelType == "Stackable")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Stackable",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Stackable"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
           


            return ProductList1;
        }
        public List<SelectListItem> DropDownListParkingType(string ReferelType)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (ReferelType != "")
            {
                item = new SelectListItem()
                {
                    Value = ReferelType,
                    Text = ReferelType
                };
                ProductList1.Add(item);
            }
            if (ReferelType == "Assigned")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Assigned",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Assigned"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            if (ReferelType == "Open")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Open",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Open"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            if (ReferelType == "Reserved")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Reserved",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Reserved"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            if (ReferelType == "Street")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Street",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Street"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            
            if (ReferelType == "Garage")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Garage",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Garage"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            if (ReferelType == "Detach")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Detach",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Detach"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            if (ReferelType == "Attached")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Attached",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Attached"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            if (ReferelType == "Other")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Other",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Other"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            return ProductList1;
        }

        public List<SelectListItem> DropDownListYesNo(string ReferelType)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (ReferelType != "")
            {
                item = new SelectListItem()
                {
                    Value = ReferelType,
                    Text = ReferelType
                };
                ProductList1.Add(item);
            }
            if (ReferelType == "Yes")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Yes",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Yes"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            if (ReferelType == "No")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "No",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "No"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            return ProductList1;
        }
        public List<SelectListItem> DropDownListVendorType(string vendorType)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (vendorType != "")
            {
                item = new SelectListItem()
                {
                    Value = vendorType,
                    Text = vendorType
                };
                ProductList1.Add(item);
            }

            if (vendorType == "ValetTrash")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "ValetTrash",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "ValetTrash"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }

            if (vendorType == "Furniture")
            {
                
            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Furniture",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Furniture"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }
            
            
            if (vendorType == "HouseWavers")
            {
                
            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "HouseWavers",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "HouseWavers"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "Turnkey")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Turnkey",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Turnkey"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            

            if (vendorType == "WaterSewerTrash")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "WaterSewerTrash",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "WaterSewerTrash"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "Concierge")
            {
                
            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Concierge",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Concierge"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "Parcel Service")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Parcel Service",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Parcel Service"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "Utility Concierge")
            {
                
            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Utility Concierge",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Utility Concierge"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            
            

            if (vendorType == "Maid")
            {
                
            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Maid",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Maid"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            
            

            if (vendorType == "Cable")
            {
                
            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Cable",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Cable"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            
            
            if (vendorType == "Internet")
            {
                
            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Internet",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Internet"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            
            

            if (vendorType == "Parking")
            {
                
            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Parking",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Parking"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            
            

            if (vendorType == "Gas")
            {
                
            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Gas",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Gas"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            

            if (vendorType == "Water")
            {
                
            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Water",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Water"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            
            

            if (vendorType == "Electric")
            {
                
            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Electric",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Electric"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            
            

            if (vendorType == "Trash")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Trash",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Trash"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            
            

            if (vendorType == "Management Company")
            {
                
            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Management Company",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Management Company"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "Parcel Service")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Parcel Service",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Parcel Service"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "Fridge")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Fridge",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Fridge"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "Microwave")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Microwave",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Microwave"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            
            if (vendorType == "Inspection")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Inspection",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Inspection"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            
            if (vendorType == "Cleaning")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Cleaning",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Cleaning"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            //for (int i = 0; i < dt3.Rows.Count; i++)
            //{
            //    item = new SelectListItem()
            //    {
            //        Value = dt3.Rows[i]["Column 0"].ToString(),

            //        Text = dt3.Rows[i]["Column 0"].ToString()
            //    };
            //    ProductList1.Add(item);
            //}
            //SelectListItem itemTemp = new SelectListItem(){ Value = vendorType, Text = vendorType };
            //ProductList1.Find(itemTemp). = 
            //string str_firstElement  = ProductList1.ElementAt(0).Text;

            return ProductList1;
        }

        public List<SelectListItem> DropDownListVendorType_Change(string vendorValue, string vendorType)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (vendorType != "")
            {
                switch (vendorType)
                {
                    case "0":
                        item = new SelectListItem()
                        {
                            Value = vendorType,
                            Text = "All"
                        };
                        ProductList1.Add(item);
                        break;
                    case "1":
                        item = new SelectListItem()
                        {
                            Value = "1",//dt3.Rows[0]["Column 0"].ToString(),

                            Text = "ValetTrash"//dt3.Rows[0]["Column 0"].ToString()


                        };
                        ProductList1.Add(item);
                        break;
                    case "2":
                        item = new SelectListItem()
                        {
                            Value = "2",//dt3.Rows[0]["Column 0"].ToString(),

                            Text = "Furniture"//dt3.Rows[0]["Column 0"].ToString()


                        };
                        ProductList1.Add(item);
                        break;
                    case "3":
                        item = new SelectListItem()
                        {
                            Value = "3",//dt3.Rows[0]["Column 0"].ToString(),

                            Text = "HouseWavers"//dt3.Rows[0]["Column 0"].ToString()
                        };
                        ProductList1.Add(item);
                        break;
                    case "4":
                        item = new SelectListItem()
                        {
                            Value = "4",//dt3.Rows[0]["Column 0"].ToString(),

                            Text = "Turnkey"//dt3.Rows[0]["Column 0"].ToString()
                        };
                        ProductList1.Add(item);
                        break;
                    case "5":
                        item = new SelectListItem()
                        {
                            Value = "5",//dt3.Rows[0]["Column 0"].ToString(),

                            Text = "WaterSewerTrash"//dt3.Rows[0]["Column 0"].ToString()
                        };
                        ProductList1.Add(item);
                        break;
                    case "6":
                        
                            item = new SelectListItem()
                            {
                                Value = "6",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Concierge"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);

                        break;
                    case "7":
                        item = new SelectListItem()
                            {
                                Value = "7",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Parcel Service"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "8":
                        item = new SelectListItem()
                            {
                                Value = "8",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Utility Concierge"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "9":
                        item = new SelectListItem()
                            {
                                Value = "9",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Maid"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "10":
                        item = new SelectListItem()
                            {
                                Value = "10",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Cable"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "11":
                        item = new SelectListItem()
                            {
                                Value = "11",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Internet"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "12":
                        item = new SelectListItem()
                            {
                                Value = "12",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Parking"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "13":
                        item = new SelectListItem()
                            {
                                Value = "13",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Gas"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "14":
                        item = new SelectListItem()
                            {
                                Value = "14",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Water"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "15":
                        item = new SelectListItem()
                            {
                                Value = "15",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Electric"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "16":
                        item = new SelectListItem()
                            {
                                Value = "16",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Trash"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "17":
                        item = new SelectListItem()
                            {
                                Value = "17",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Management Company"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "18":
                        item = new SelectListItem()
                            {
                                Value = "18",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Parcel Service"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "19":
                        item = new SelectListItem()
                            {
                                Value = "19",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Fridge"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "20":
                        item = new SelectListItem()
                            {
                                Value = "20",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Microwave"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "21":
                        item = new SelectListItem()
                            {
                                Value = "21",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Inspection"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        break;
                    case "22":
                        item = new SelectListItem()
                            {
                                Value = "22",//dt3.Rows[0]["Column 0"].ToString(),

                                Text = "Cleaning"//dt3.Rows[0]["Column 0"].ToString()
                            };
                            ProductList1.Add(item);
                        
                        break;
                    default:
                        // code block
                        break;
                }

                
            }

            if (vendorType == "0")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "0",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "All"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }

            if (vendorType == "1")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "1",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "ValetTrash"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }

            if (vendorType == "2")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "2",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Furniture"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            if (vendorType == "3")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "3",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "HouseWavers"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "4")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "4",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Turnkey"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            if (vendorType == "5")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "5",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "WaterSewerTrash"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "6")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "6",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Concierge"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "7")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "7",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Parcel Service"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "8")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "8",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Utility Concierge"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (vendorType == "9")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "9",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Maid"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (vendorType == "10")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "10",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Cable"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            if (vendorType == "11")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "11",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Internet"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (vendorType == "12")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "12",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Parking"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (vendorType == "13")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "13",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Gas"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            if (vendorType == "14")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "14",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Water"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (vendorType == "15")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "15",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Electric"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (vendorType == "16")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "16",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Trash"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (vendorType == "17")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "17",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Management Company"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "18")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "18",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Parcel Service"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "19")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "19",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Fridge"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "20")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "20",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Microwave"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (vendorType == "21")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "21",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Inspection"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            if (vendorType == "22")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "22",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Cleaning"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
            //for (int i = 0; i < dt3.Rows.Count; i++)
            //{
            //    item = new SelectListItem()
            //    {
            //        Value = dt3.Rows[i]["Column 0"].ToString(),

            //        Text = dt3.Rows[i]["Column 0"].ToString()
            //    };
            //    ProductList1.Add(item);
            //}
            //SelectListItem itemTemp = new SelectListItem(){ Value = vendorType, Text = vendorType };
            //ProductList1.Find(itemTemp). = 
            //string str_firstElement  = ProductList1.ElementAt(0).Text;

            return ProductList1;
        }

        public List<SelectListItem> CheckBoxListVendorType(string Street)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();
            string[] lst = Street.Split(',');


            foreach (var vendorType in Street.Split(','))
            {

                SelectListItem itemChecked = new SelectListItem();
                if (vendorType != "")
                {
                    itemChecked = new SelectListItem()
                    {
                        Value = "Checked",
                        Text = vendorType
                    };
                    ProductList1.Add(itemChecked);
                }
            }
            

            SelectListItem item = new SelectListItem();
            
            if (lst.Contains("ValetTrash"))
            {

            }
            else
            {

                item = new SelectListItem()
                {
                    Value = "ValetTrash",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "ValetTrash"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Furniture"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Furniture",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Furniture"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            if (lst.Contains("HouseWavers"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "HouseWavers",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "HouseWavers"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Turnkey"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Turnkey",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Turnkey"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            if (lst.Contains("WaterSewerTrash"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "WaterSewerTrash",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "WaterSewerTrash"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Concierge"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Concierge",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Concierge"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Parcel Service"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Parcel Service",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Parcel Service"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Utility Concierge"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Utility Concierge",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Utility Concierge"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (lst.Contains("Maid"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Maid",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Maid"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (lst.Contains("Cable"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Cable",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Cable"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            if (lst.Contains("Internet"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Internet",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Internet"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (lst.Contains("Parking"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Parking",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Parking"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (lst.Contains("Gas"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Gas",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Gas"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            if (lst.Contains("Water"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Water",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Water"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (lst.Contains("Electric"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Electric",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Electric"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (lst.Contains("Trash"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Trash",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Trash"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (lst.Contains("Management Company"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Management Company",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Management Company"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Parcel Service"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Parcel Service",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Parcel Service"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Fridge"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Fridge",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Fridge"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Microwave"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Microwave",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Microwave"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Inspection"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Inspection",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Inspection"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            if (lst.Contains("Cleaning"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Cleaning",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Cleaning"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }
                //for (int i = 0; i < dt3.Rows.Count; i++)
                //{
                //    item = new SelectListItem()
                //    {
                //        Value = dt3.Rows[i]["Column 0"].ToString(),

                //        Text = dt3.Rows[i]["Column 0"].ToString()
                //    };
                //    ProductList1.Add(item);
                //}
                //SelectListItem itemTemp = new SelectListItem(){ Value = vendorType, Text = vendorType };
                //ProductList1.Find(itemTemp). = 
                //string str_firstElement  = ProductList1.ElementAt(0).Text;
            
            return ProductList1;
        }

        public List<SelectListItem> CheckBoxListCommunityFeatures(string CommunityFeatures)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();
            string[] lst = CommunityFeatures.Split(',');


            foreach (var vendorType in CommunityFeatures.Split(','))
            {

                SelectListItem itemChecked = new SelectListItem();
                if (vendorType != "")
                {
                    itemChecked = new SelectListItem()
                    {
                        Value = "Checked",
                        Text = vendorType
                    };
                    ProductList1.Add(itemChecked);
                }
            }


            SelectListItem item = new SelectListItem();
            if (lst.Contains("Crown Molding"))
            {

            }
            else
            {

                item = new SelectListItem()
                {
                    Value = "Crown Molding",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Crown Molding"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Formal Entry/Foyer"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Formal Entry/Foyer",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Formal Entry/Foyer"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            if (lst.Contains("Spa/Hot Tub"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Spa/Hot Tub",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Spa/Hot Tub"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Split Level"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Split Level",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Split Level"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            if (lst.Contains("Washer & dryer"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Washer & dryer",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Washer & dryer"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Microwave"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Microwave",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Microwave"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Dishwasher"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Dishwasher",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Dishwasher"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Air conditioning"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Air conditioning",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Air conditioning"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (lst.Contains("Dog park"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Dog park",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Dog park"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (lst.Contains("Pool"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Pool",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Pool"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            if (lst.Contains("Spa"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Spa",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Spa"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (lst.Contains("Fitness center"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Fitness center",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Fitness center"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (lst.Contains("Business center"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Business center",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Business center"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            if (lst.Contains("Clubhouse"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Clubhouse",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Clubhouse"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (lst.Contains("Tennis and basketball court"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Tennis and basketball court",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Tennis and basketball court"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Playground"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Playground",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Playground"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            if (lst.Contains("Fishing pond"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Fishing pond",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Fishing pond"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Bicycle trails"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Bicycle trails",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Bicycle trails"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Charging station"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Charging station",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Charging station"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            
            //for (int i = 0; i < dt3.Rows.Count; i++)
            //{
            //    item = new SelectListItem()
            //    {
            //        Value = dt3.Rows[i]["Column 0"].ToString(),

            //        Text = dt3.Rows[i]["Column 0"].ToString()
            //    };
            //    ProductList1.Add(item);
            //}
            //SelectListItem itemTemp = new SelectListItem(){ Value = vendorType, Text = vendorType };
            //ProductList1.Find(itemTemp). = 
            //string str_firstElement  = ProductList1.ElementAt(0).Text;

            return ProductList1;
        }

        public List<SelectListItem> CheckBoxListUnitFeatures(string UnitFeatures)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();
            string[] lst = UnitFeatures.Split(',');


            foreach (var vendorType in UnitFeatures.Split(','))
            {

                SelectListItem itemChecked = new SelectListItem();
                if (vendorType != "")
                {
                    itemChecked = new SelectListItem()
                    {
                        Value = "Checked",
                        Text = vendorType
                    };
                    ProductList1.Add(itemChecked);
                }
            }


            SelectListItem item = new SelectListItem();
            if (lst.Contains("Back Yard Fenced"))
            {

            }
            else
            {

                item = new SelectListItem()
                {
                    Value = "Back Yard Fenced",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Back Yard Fenced"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Covered Patio/Deck"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Covered Patio/Deck",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Covered Patio/Deck"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            if (lst.Contains("Spa/Hot Tub"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Spa/Hot Tub",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Spa/Hot Tub"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Fire/Smoke Alarm"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Fire/Smoke Alarm",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Fire/Smoke Alarm"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Washer & dryer"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Washer & dryer",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Washer & dryer"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Microwave"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Microwave",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Microwave"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Central A / C"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Central A / C",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Central A / C"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Ceiling Fan"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Ceiling Fan",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Ceiling Fan"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Walkin Shower"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Walkin Shower",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Walkin Shower"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            if (lst.Contains("Fire Extinguisher"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Fire Extinguisher",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Fire Extinguisher"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Stainless Steel Appliances"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Stainless Steel Appliances",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Stainless Steel Appliances"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Patio or Balcony"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Patio or Balcony",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Patio or Balcony"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Private backyards"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Private backyards",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Private backyards"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Wood Style Flooring"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Wood Style Flooring",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Wood Style Flooring"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Crown Molding"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Crown Molding",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Crown Molding"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Wine Chiller"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Wine Chiller",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Wine Chiller"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Granite Countertops"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Granite Countertops",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Granite Countertops"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Fireplace"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Fireplace",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Fireplace"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Garden Tub"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Garden Tub",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Garden Tub"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Keyless Entry"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Keyless Entry",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Keyless Entry"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Fully Equipped Kitchen"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Fully Equipped Kitchen",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Fully Equipped Kitchen"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Patio/OutdoorStorageCloset"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Patio/OutdoorStorageCloset",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Patio/OutdoorStorageCloset"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Dishwasher"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Dishwasher",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Dishwasher"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Garbage Disposal"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Garbage Disposal",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Garbage Disposal"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (lst.Contains("Walkin Closet"))
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Walkin Closet",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Walkin Closet"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            

            //for (int i = 0; i < dt3.Rows.Count; i++)
            //{
            //    item = new SelectListItem()
            //    {
            //        Value = dt3.Rows[i]["Column 0"].ToString(),

            //        Text = dt3.Rows[i]["Column 0"].ToString()
            //    };
            //    ProductList1.Add(item);
            //}
            //SelectListItem itemTemp = new SelectListItem(){ Value = vendorType, Text = vendorType };
            //ProductList1.Find(itemTemp). = 
            //string str_firstElement  = ProductList1.ElementAt(0).Text;

            return ProductList1;
        }
        public List<SelectListItem> DropDownListReferelType(string ReferelType)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (ReferelType != "")
            {
                item = new SelectListItem()
                {
                    Value = ReferelType,
                    Text = ReferelType
                };
                ProductList1.Add(item);
            }
            if (ReferelType == "%")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "%",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "%"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            if (ReferelType == "$")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "$",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "$"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }



            //for (int i = 0; i < dt3.Rows.Count; i++)
            //{
            //    item = new SelectListItem()
            //    {
            //        Value = dt3.Rows[i]["Column 0"].ToString(),

            //        Text = dt3.Rows[i]["Column 0"].ToString()
            //    };
            //    ProductList1.Add(item);
            //}
            //SelectListItem itemTemp = new SelectListItem(){ Value = vendorType, Text = vendorType };
            //ProductList1.Find(itemTemp). = 
            //string str_firstElement  = ProductList1.ElementAt(0).Text;

            return ProductList1;
        }

        /// <summary>
        /// ///////Leads DropDown List
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public List<SelectListItem> DropDownListContantType(string contantType)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if(contantType != "")
            {
                item = new SelectListItem()
                {
                    Value = contantType,
                    Text = contantType
                };
                ProductList1.Add(item);
            }

            if (contantType == "Company")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Company",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Company"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            if (contantType == "Individual")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Individual",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Individual"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            


            //for (int i = 0; i < dt3.Rows.Count; i++)
            //{
            //    item = new SelectListItem()
            //    {
            //        Value = dt3.Rows[i]["Column 0"].ToString(),

            //        Text = dt3.Rows[i]["Column 0"].ToString()
            //    };
            //    ProductList1.Add(item);
            //}
            //SelectListItem itemTemp = new SelectListItem(){ Value = vendorType, Text = vendorType };
            //ProductList1.Find(itemTemp). = 
            //string str_firstElement  = ProductList1.ElementAt(0).Text;

            return ProductList1;
        }

        public List<SelectListItem> DropDownListLeaseTerm(string LeaseTerm)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (LeaseTerm != "")
            {

                item = new SelectListItem()
                {
                    Value = LeaseTerm,
                    Text = LeaseTerm
                };
                ProductList1.Add(item);
            }
            if (LeaseTerm == "Yearly")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Yearly",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Yearly"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }


            if (LeaseTerm == "Monthly")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Monthly",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Monthly"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }

            if (LeaseTerm == "Daily")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Daily",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Daily"//dt3.Rows[0]["Column 0"].ToString()


                };
                ProductList1.Add(item);
            }

            //for (int i = 0; i < dt3.Rows.Count; i++)
            //{
            //    item = new SelectListItem()
            //    {
            //        Value = dt3.Rows[i]["Column 0"].ToString(),

            //        Text = dt3.Rows[i]["Column 0"].ToString()
            //    };
            //    ProductList1.Add(item);
            //}
            //SelectListItem itemTemp = new SelectListItem(){ Value = vendorType, Text = vendorType };
            //ProductList1.Find(itemTemp). = 
            //string str_firstElement  = ProductList1.ElementAt(0).Text;

            return ProductList1;
        }

        public List<SelectListItem> DropDownListCity(string City)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if(City != "")
            {

                item = new SelectListItem()
                {
                    Value = City,
                    Text = City
                };
                ProductList1.Add(item);
            }
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = @"SELECT CityId, City
                                FROM [dbo].[City]";
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);
            con.Close();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (City == dt.Rows[i]["City"].ToString())
                {

                }
                else
                {

                    item = new SelectListItem()
                    {
                        Value = dt.Rows[i]["City"].ToString(),

                        Text = dt.Rows[i]["City"].ToString()
                    };
                    ProductList1.Add(item);
                }
            }
            
            return ProductList1;
        }

        public List<SelectListItem> DropDownListReferelSource(string ReferelSource)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (ReferelSource != "")
            { 
                item = new SelectListItem()
                {
                    Value = ReferelSource,
                    Text = ReferelSource
                };
                ProductList1.Add(item);
            }
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = @"SELECT  CompanyName
                                FROM [dbo].[ReferalSources]";
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);
            con.Close();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (ReferelSource == dt.Rows[i]["CompanyName"].ToString())
                {

                }
                else
                {

                    item = new SelectListItem()
                    {
                        Value = dt.Rows[i]["CompanyName"].ToString(),

                        Text = dt.Rows[i]["CompanyName"].ToString()
                    };
                    ProductList1.Add(item);
                }
            }

            return ProductList1;
        }

        public List<SelectListItem> DropDownListContactName(string ContactName, string company)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (ContactName != "")
            {
                item = new SelectListItem()
                {
                    Value = ContactName,
                    Text = ContactName
                };
                ProductList1.Add(item);
            }
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = @"SELECT  FirstName + ' '+ LastName as name
                                FROM [dbo].[Contacts] where Company='" + company + "'";
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);
            con.Close();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (ContactName == dt.Rows[i]["name"].ToString())
                {

                }
                else
                {

                    item = new SelectListItem()
                    {
                        Value = dt.Rows[i]["name"].ToString(),

                        Text = dt.Rows[i]["name"].ToString()
                    };
                    ProductList1.Add(item);
                }
            }

            return ProductList1;
        }
        public List<SelectListItem> DropDownListNumber(string number)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            SelectListItem item0 = new SelectListItem();
            if (number != "")
            {
                if(number == "None")
                { 
                }
                else
                { 
                    item = new SelectListItem()
                    {
                        Value = number,
                        Text = number
                    };
                ProductList1.Add(item);
                }
            }

            item0 = new SelectListItem()
            {
                Value = "None",
                Text = "None"
            };
            ProductList1.Add(item0);

            for (int i = 1; i <= 10; i++)
            {
                if (number == i.ToString())
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

        public List<SelectListItem> DropDownListBed(string number)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            SelectListItem item0 = new SelectListItem();
            if (number != "")
            {
                item = new SelectListItem()
                {
                    Value = number,
                    Text = number
                };
                ProductList1.Add(item);
            }

            item0 = new SelectListItem()
            {
                Value = "Studio",
                Text = "Studio"
            };
            ProductList1.Add(item0);

            for (int i = 1; i <= 4; i++)
            {
                if (number == i.ToString())
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
        public List<SelectListItem> DropDownListWeight(string number)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (number != "")
            {
                item = new SelectListItem()
                {
                    Value = number,
                    Text = number
                };
                ProductList1.Add(item);
            }


            for (int i = 0; i <= 5; i++)
            {
                if (number == i.ToString())
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

        [HttpPost]
        public JsonResult GetCountries(string Prefix)
        {

            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            //if (ReferelSource != "")
            //{
            //    item = new SelectListItem()
            //    {
            //        Value = ReferelSource,
            //        Text = ReferelSource
            //    };
            //    ProductList1.Add(item);
            //}
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = @"SELECT  CompanyName
                                FROM [dbo].[ReferalSources]";
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);
            con.Close();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                

                    item = new SelectListItem()
                    {
                        Value = dt.Rows[i]["CompanyName"].ToString(),

                        Text = dt.Rows[i]["CompanyName"].ToString()
                    };
                    ProductList1.Add(item);
                
            }

            //return ProductList1;
            return Json(ProductList1, JsonRequestBehavior.AllowGet);
        }
        ////////////////////
        #endregion
    }
}