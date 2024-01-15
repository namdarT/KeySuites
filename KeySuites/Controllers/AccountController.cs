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
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using SecurePipe;
using System.Text;


namespace Vidly.Controllers
{
    public class AccountController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<Login> userManager;
        private readonly Microsoft.AspNetCore.Identity.SignInManager<Login> signInManager;
        static DbContextOptions<VidlyContext> options = new DbContextOptions<VidlyContext>();
        TempDataDictionary keyValues = new TempDataDictionary();
        public string error = "";
        VidlyContext db_context = new VidlyContext(options);
        private readonly VidlyContext dbContext;
        public string connect = Injector.SConnection;

        // GET: Account
        public ActionResult Index()
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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = null)
        {
                        
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection model, string returnUrl)
        {
            //return RedirectToAction("Index", "Home");
            //string connect = System.Configuration.ConfigurationManager.ConnectionStrings["simplicityCon"].ConnectionString;
            var encryptpassword = Injector.Encrypt("123", "sblw-3hn8-sqoy19");
            ////var decryptpassword = Decrypt("GVTuIMZLrxc=", "sblw-3hn8-sqoy19");
            SqlConnection con = new SqlConnection(connect);
            Login user = new Login();
            user.UserName = model["UserName"];
            user.PasswordHash = model["PasswordHash"];
            user.PasswordHash = user.PasswordHash.Replace(",", "");
            
            if (CriticalCheck())
            {
                if (IsLocalUrl(returnUrl))
                {
                    if (UserPasswordNotInject(user.UserName, user.PasswordHash))
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandType = CommandType.Text;

                            //cmd.CommandText = @"Select Id, UserName, PasswordHash From [dbo].[AspNetUsers] where [UserName]=" + "'" + user.UserName + "'";
                            cmd.CommandText = "EXEC sp_AspNetSignIn '" + user.UserName + "'";
                            //
                            try
                            {
                                con.Open();
                                SqlDataReader Dr;
                                Dr = cmd.ExecuteReader();
                                DataTable dt = new DataTable("Vw");
                                dt.Load(Dr);
                                if (dt.Rows.Count > 0)
                                {
                                    var decryptpassword = Injector.Decrypt(dt.Rows[0]["PasswordHash"].ToString(), "sblw-3hn8-sqoy19");
                                    if (user.PasswordHash == decryptpassword)
                                    {
                                        ViewBag.error = "";
                                        //SqlCommand cmdRole = new SqlCommand("Select * from [dbo].[party_contact] WHERE (account_status_cd='Active' OR account_status_cd='active') and [email]='" + user.UserName + "'", con);
                                        //SqlCommand cmdRole = new SqlCommand("exec ins_ksSignIn '" + user.UserName + "'", con);
                                        //cmdRole.CommandType = CommandType.StoredProcedure;
                                        //SqlDataReader DrRole;
                                        //con.Open();
                                        //DrRole = cmdRole.ExecuteReader();
                                        //DataTable dtRole = new DataTable("VwRole");
                                        //dtRole.Load(DrRole);
                                        //con.Close();
                                        //if (dtRole.Rows.Count > 0)
                                        //{
                                            Session["LoginUserID"] = dt.Rows[0]["Id"].ToString();
                                            Session["LoginUserName"] = user.UserName;
                                            Session["LoginUserEmail"] = user.UserName;
                                            Session["LoginUserRole"] = dt.Rows[0]["UserType"];

                                            SqlCommand command = new SqlCommand("EXEC ins_ksSignIn '" + user.UserName + "',  '" + Session["LoginUserID"].ToString() + "', '" + Session["LoginUserRole"].ToString() + "', 1, 'SignIn Successfull', 'safe'", con);
                                            command.ExecuteNonQuery();
                                        //}
                                        //else
                                        //{
                                            //ViewBag.error = "Account InActive/Disabled!";
                                            //SqlCommand command = new SqlCommand("EXEC ins_ksSignIn '" + user.UserName + "',  'Not Created', 'Not Created', 1, 'Account InActive/Disabled!', 'warning'", con);
                                            //command.ExecuteNonQuery();
                                        //}

                                    }
                                    else
                                    {
                                        ViewBag.error = "User Name/Password Incorrect!";
                                        SqlCommand command = new SqlCommand("EXEC ins_ksSignIn '" + user.UserName + "',  'Not Created', 'Not Created', 1, 'User Name/Password Incorrect!', 'warning'", con);
                                        command.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    ViewBag.error = "User Name/Password Incorrect!";
                                    SqlCommand command = new SqlCommand("EXEC ins_ksSignIn '" + user.UserName + "',  'Not Created', 'Not Created', 1, 'User Name/Password Incorrect!', 'warning'", con);
                                    command.ExecuteNonQuery();
                                }

                            }
                            catch (SqlException e)
                            {

                                ViewBag.error = "Transaction Failure!";
                                SqlCommand command = new SqlCommand("EXEC ins_ksSignIn '" + user.UserName + "',  'Not Created', 'Not Created', 0, 'Transaction Failure!', 'warning'", con);
                                command.ExecuteNonQuery();
                            }

                        }
                    }
                    else
                    {
                        con.Open();
                        ViewBag.error = "Transaction Failure!";
                        SqlCommand command = new SqlCommand("EXEC ins_ksSignIn '" + user.UserName + "',  'Not Created', 'Not Created', 0, 'Transaction Failure!  as Password used like : 1 equal 1, or equal, drop table, tag script tag or return url used in a request', 'critical'", con);
                        command.ExecuteNonQuery();
                    }

                }
                else
                {
                    con.Open();
                    ViewBag.error = "Transaction Failure!";
                    SqlCommand command = new SqlCommand("EXEC ins_ksSignIn '" + user.UserName + "',  'Not Created', 'Not Created', 0, 'Transaction Failure!  due to return url used in a request', 'critical'", con);
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                ViewBag.error = "Transaction Failure!";
            }


            con.Close();
            if (ViewBag.error != string.Empty)
            {
                return View();
            }
            else
            {

                return RedirectToAction("Index", "Home");
            }

        }



        public ActionResult Users(decimal id = 0, string srchContactName = "", string srchOccupantName = "")
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

            return View(BindDataUserAll(id));
        }

        public ActionResult AddUser(FormCollection a, Int64 Id = 0)
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
            SqlCommand cmd = new SqlCommand("UserSelect", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", Id);
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            User User_Single = new User();
            
            if (dt.Rows.Count > 0)
            {
                User_Single.Email = dt.Rows[0]["Email"].ToString();
                User_Single.Id = Int64.Parse(dt.Rows[0]["Id"].ToString());

                User_Single.Phone = dt.Rows[0]["PhoneNumber"].ToString();
                User_Single.UserName = dt.Rows[0]["UserName"].ToString();
                User_Single.FirstName = dt.Rows[0]["FirstName"].ToString();
                User_Single.LastName = dt.Rows[0]["LastName"].ToString();
                User_Single.IsActive = bool.Parse(dt.Rows[0]["IsActive"].ToString());
                User_Single.Password = dt.Rows[0]["Password"].ToString();
                User_Single.PasswordHash = dt.Rows[0]["PasswordHash"].ToString();
                User_Single.UserType = dt.Rows[0]["UserType"].ToString();
                ViewBag.UserType = DropDownListUserType(dt.Rows[0]["UserType"].ToString());
            }
            else
            {
                User_Single = new User()
                {
                    Id = 0,
                    Email = "",


                    Phone = "",
                    FirstName = "",
                    IsActive = true,
                    LastName = "",

                    UserName = "",

                    Password = "",

                    PasswordHash = "",
                    UserType = "User"


                };

                ViewBag.UserType = DropDownListUserType("");
            }

            return View(User_Single);         
        }

        [HttpPost]
        public ActionResult AddUser(FormCollection model)
        {
            Session["Message"] = "";
            Session["SuccessMessage"] = "";

            User User_Save = new User();

            User_Save.Id = Int64.Parse(model["Id"]);

            User_Save.Email = model["Email"].ToString();
            
            User_Save.Phone = model["Phone"].ToString();
            User_Save.UserName = model["UserName"].ToString();
            User_Save.FirstName = model["FirstName"].ToString();
            User_Save.LastName = model["LastName"].ToString();
            User_Save.IsActive = true;
            User_Save.Password = model["Password"].ToString();
            User_Save.UserType = model["UserType"].ToString();
            User_Save.PasswordHash = Injector.Encrypt(User_Save.Password, Injector.PasswordHash); ;


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
            SetupController setup = new SetupController();
            //con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                if (User_Save.Id == 0)
                {
                    if (setup.USNumberValidator(User_Save.Phone))
                    {
                        if (setup.EmailValidator(User_Save.Email))
                        {
                            if (!setup.DuplicateCommon("AspNetUser", "Id", "FirstName", "Department", User_Save.FirstName, User_Save.Department))
                            {

                                cmd.CommandText = "sp_UserRegistration";
                                cmd.CommandType = CommandType.StoredProcedure;



                                cmd.Parameters.AddWithValue("@UserName", User_Save.UserName);
                                cmd.Parameters.AddWithValue("@Email", User_Save.Email);

                                cmd.Parameters.AddWithValue("@PhoneNumber", User_Save.Phone);

                                cmd.Parameters.AddWithValue("@FirstName", User_Save.FirstName);
                                //cmd.Parameters.AddWithValue("@UserId", User_Save.UserId);
                                cmd.Parameters.AddWithValue("@LastName", User_Save.LastName);
                                //cmd.Parameters.AddWithValue("@IsActive", User_Save.IsActive);
                                cmd.Parameters.AddWithValue("@Password", User_Save.Password);
                                cmd.Parameters.AddWithValue("@UserType", User_Save.UserType);
                                cmd.Parameters.AddWithValue("@PasswordHash", User_Save.PasswordHash);

                                try
                                {
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    Session["error"] = null;
                                    Session["SuccessMessage"] = "Success: User Successfully Added";
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
                        ViewBag.Message = "Phone Number is in a Wrong Pattern!";
                        Session["Message"] = ViewBag.Message;
                        Session["error"] = ViewBag.Message;
                    }
                }

                else
                {
                    if (setup.USNumberValidator(User_Save.Phone))
                    {
                        if (setup.EmailValidator(User_Save.Email))
                        {

                            cmd.CommandText = "sp_UserUpdate";
                            cmd.CommandType = CommandType.StoredProcedure;


                            cmd.Parameters.AddWithValue("@UserName", User_Save.UserName);
                            cmd.Parameters.AddWithValue("@Email", User_Save.Email);

                            cmd.Parameters.AddWithValue("@PhoneNumber", User_Save.Phone);

                            cmd.Parameters.AddWithValue("@FirstName", User_Save.FirstName);
                            cmd.Parameters.AddWithValue("@Id", User_Save.Id);
                            cmd.Parameters.AddWithValue("@LastName", User_Save.LastName);
                            //cmd.Parameters.AddWithValue("@IsActive", User_Save.IsActive);
                            cmd.Parameters.AddWithValue("@Password", User_Save.Password);
                            cmd.Parameters.AddWithValue("@PasswordHash", User_Save.PasswordHash);
                            cmd.Parameters.AddWithValue("@UserType", User_Save.UserType);
                            try
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                                Session["SuccessMessage"] = "Success: User Successfully Updated";
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
                        ViewBag.Message = "Phone Number is in a Wrong Pattern!";
                        Session["Message"] = ViewBag.Message;
                        Session["error"] = ViewBag.Message;
                    }

                }

            }
            con.Close();

            if (Session["error"] != null)
            {
                ViewData["Message"] = Session["error"];
                ViewBag.UserType = DropDownListUserType(model["UserType"].ToString());

                return View(User_Save);
            }
            else
            {
                ViewData["SuccessMessage"] = Session["SuccessMessage"];
                return RedirectToAction("Users");
            }


        }


        public ActionResult ActiveInactiveUser(decimal id = 0, int IsActive = 1)
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

                cmd.CommandText = @"UserEnableDisable";

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
                    Session["SuccessMessage"] = "User Updated Successfully";
                }
                catch (SqlException e)
                {

                    ViewBag.error = "Transaction Failure";
                    Session["SuccessMessage"] = e.Message;
                }

            }
            con.Close();
            return RedirectToAction("Users");
        }


        private bool IsLocalUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return true;
            }
            else
            {
                return ((url[0] == '/' && (url.Length == 1 ||
                        (url[1] != '/' && url[1] != '\\'))) ||   // "/" or "/foo" but not "//" or "/\"
                        (url.Length > 1 &&
                         url[0] == '~' && url[1] == '/'));   // "~/" or "~/foo"
            }
        }

        private bool UserPasswordNotInject(string user, string Password)
        {
            bool yesNo = true;
            if (string.IsNullOrEmpty(user))
            {
                return true;
            }
            else
            {
                string[] options = {"1=1", "\" or \"\"=\"", "; DROP TABLE", "; drop table", "<script>" };
                foreach(var x in options)
                {
                    if (user.Contains(x) || Password.Contains(x))
                    {
                        yesNo = false;
                        break;
                    }
                    
                }
                return yesNo;
            }
        }
        public bool CriticalCheck()
        {
            
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("exec sp_SecureSignCheck", con);
            
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);
            con.Close();
            if (bool.Parse(dt.Rows[0][0].ToString()) == false)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public List<SelectListItem> DropDownListUserType(string contantType)
        {
            List<SelectListItem> ProductList1 = new List<SelectListItem>();

            SelectListItem item = new SelectListItem();
            if (contantType != "")
            {
                item = new SelectListItem()
                {
                    Value = contantType,
                    Text = contantType
                };
                ProductList1.Add(item);
            }

            if (contantType == "User")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "User",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "User"//dt3.Rows[0]["Column 0"].ToString()
                };
                ProductList1.Add(item);
            }


            if (contantType == "Admin")
            {

            }
            else
            {
                item = new SelectListItem()
                {
                    Value = "Admin",//dt3.Rows[0]["Column 0"].ToString(),

                    Text = "Admin"//dt3.Rows[0]["Column 0"].ToString()


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
        public List<User> BindDataUserAll(decimal id)
        {
            SqlConnection con = new SqlConnection(connect);
            SqlCommand cmd = new SqlCommand("UserSelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Vw");
            dt.Load(Dr);

            con.Close();

            User User_Single = new User();
            User User_Detail = new User();
            JsonResult jR = new JsonResult();
            List<User> User_List = new List<User>();
            //var items = dbContext.VwAppUserAssets.AsNoTracking().AsQueryable<Models.VwAppUserAsset>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    //User_Single.Address = dt.Rows[i]["Address"].ToString();
                    User_Single.Email = dt.Rows[i]["Email"].ToString();
                    User_Single.Id = Int64.Parse(dt.Rows[i]["Id"].ToString());

                    User_Single.Phone = dt.Rows[i]["PhoneNumber"].ToString();
                    User_Single.UserName = dt.Rows[i]["UserName"].ToString();
                    User_Single.UserType = dt.Rows[i]["UserType"].ToString();
                    User_Single.FirstName = dt.Rows[i]["FirstName"].ToString();
                    User_Single.LastName = dt.Rows[i]["LastName"].ToString();
                    User_Single.IsActive = bool.Parse(dt.Rows[i]["IsActive"].ToString());
                    User_Single.Password = dt.Rows[i]["Password"].ToString();
                    User_Single.PasswordHash = dt.Rows[i]["PasswordHash"].ToString();


                    User_List.Add(User_Single);
                    User_Single = new User();

                }

                if (id == 0)
                {
                    User_Detail = User_List[0];
                }
                //else if(id == 1)
                //{
                //    ViewData["ProcessQuote"] = "p";
                //}
                else
                {
                    User_Detail = User_List.Where(a => a.Id == id).FirstOrDefault();
                }
            }

            return User_List;
        }
    }
}