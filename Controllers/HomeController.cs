using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.IO;
using CCA.Util;
//using Finc24.FincService;
using System.Configuration;
using Finc24.ServiceReference1;

namespace Finc24.Controllers
{
    public class HomeController : Controller
    {
        CCACrypto ccaCrypto = new CCACrypto();

        string workingKey = ConfigurationManager.AppSettings["working_key"];//put in the 32bit alpha numeric key in the quotes provided here 	
        string ccaRequest = "";
        public string strEncRequest = "";
        public string strAccessCode = ConfigurationManager.AppSettings["access_code"]; // put the access code in the quotes provided here.

        Service1Client client = new Service1Client();

        // GET: Home
        public ActionResult Index(string loginId, string password)
        {
            // ViewBag.StatusMessage = status;
            //int q = 0;
            ////int q = client.Dologin("ramesh@finc.com", "123456");
            //if (!string.IsNullOrEmpty(loginId) && !string.IsNullOrEmpty(password))
            //{
            //     q = client.Dologin(loginId, password);

            //}
            //if (q > 0)
            //{
            //    Session["userlogin"] = loginId;
            //  //  return RedirectToAction("DashBoard", "Home");
            //    return View("DashBoard");
            //}
            //else


            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            try
            {
                string userName = string.Empty;
                string password = string.Empty;
                if (collection["UserName"] != null)
                    userName = Convert.ToString(collection["UserName"]);
                if (collection["Password"] != null)
                    password = Convert.ToString(collection["Password"]);

                int userId = 0;
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                {
                    userId = client.Dologin(userName, password);

                }
                //if (q > 0)
                if (userId > 0)
                {
                    // Session["userlogin"] = user;
                    //  return RedirectToAction("DashBoard", "Home");
                    // return View("DashBoard");
                    Session["UserId"] = userId;
                    return RedirectToAction("DashBoard");
                }
                else
                {
                    // return ViewBag.StatusMessage = "danger";
                    Session["UserId"] = 0;
                    return RedirectToAction("Index");
                }
                //return View();
            }
            catch (Exception ex)
            {
                Session["UserId"] = 0;
                return RedirectToAction("Index");
            }
        }

        public ActionResult Registration()
        {
            return View();
        }

        public ActionResult CheckMailSubscription(string userName, string password)
        {
            try
            {
                int userId = 0;
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                {
                    userId = client.Dologin(userName, password);
                }
                return Json(userId, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Registration(FormCollection collection)
        {
            string gender = "Male";
            string interestType = "Near to Office";
            int userId = 0;
            string userName = Convert.ToString(collection["Name"]);
            long mobile = Convert.ToInt64(collection["Mobile"]);
            //DateTime dob = Convert.ToDateTime(collection["Dob"]);
            DateTime dob = DateTime.Now;
            if (collection["Female"] != null)
                gender = "Female";
            if (collection["DesiredLocality"] != null)
                interestType = "DesiredLocality";
            else if (collection["NeedAssistance"] != null)
                interestType = "NeedAssistance";
            string nationality = Convert.ToString(collection["Nationality"]);
            string state = Convert.ToString(collection["State"]);
            string package = Convert.ToString(collection["Package"]);
            string location = Convert.ToString(collection["YourLocation"]);
            string company = Convert.ToString(collection["Company"]);
            string otherRequirement = Convert.ToString(collection["OtherRequirement"]);
            string emailId = Convert.ToString(collection["EmailId"]);
            string password = Convert.ToString(collection["Password"]);
            string confirmPassword = Convert.ToString(collection["ConfirmPassword"]);

            customer customer = new customer();
            customer.cust_name = userName;
            customer.cust_email = emailId;
            customer.cust_password = password;
            customer.cust_phone = mobile;
            customer.cust_gender = gender;
            customer.cust_state = state;
            customer.cust_nationality = nationality;
            customer.cust_dob = dob;
            customer.cust_age = DateTime.Now.Year - dob.Year;
            customer.cust_PrefferedLocation_Type = interestType;
            customer.cust_Location = location;
            customer.cust_Company = company;
            customer.cust_Package = package;
            customer.cust_other_requirements = otherRequirement;

            userId = client.Register(customer);
            Session["UserId"] = userId;
            string result = string.Empty;
            //result = Mail.SendMail("Priyanka", "princey_78670@yahoo.com", "abc@ak2", MailType.Registration);
            //result = SendMail("Priyanka", "yasharth.bhatt@gmail.com", "abc@ak2", MailType.Registration, package);
            result = SendMail(userName, emailId, password, MailType.Registration, package);
            return RedirectToAction("DashBoard", "Home");
            // return View();

        }

        public ActionResult RequestHandler(FormCollection collection)
        {
            foreach (string name in Request.Form)
            {
                if (name != null)
                {
                    if (!name.StartsWith("_"))
                    {
                        ccaRequest = ccaRequest + name + "=" + Request.Form[name] + "&";
                        /* Response.Write(name + "=" + Request.Form[name]);
                          Response.Write("</br>");*/
                    }
                }
            }
            int amount = 0;
            int userId = 0;
            string customerName = string.Empty;
            string customerPhone = string.Empty;
            string customerEmail = string.Empty;
            string merchantId = string.Empty;
            string ordetId = string.Empty;
            string customerState = string.Empty;
            string customerNationality = string.Empty;
            if (!string.IsNullOrEmpty(Convert.ToString(Session["UserId"])))
                userId = Convert.ToInt32(Session["UserId"]);

            customer customer = new customer();
            customer = client.GetCustomerData(userId);

            if (customer.cust_Package == "Basic")
                amount = 199;
            else if (customer.cust_Package == "Standard")
                amount = 599;
            else
                amount = 999;

            customerName = customer.cust_name;
            customerPhone = customer.cust_phone.ToString();
            customerEmail = customer.cust_email;
            customerState = customer.cust_state;
            customerNationality = customer.cust_nationality;

            string id = DateTime.Now.ToString();
            DateTime utcNow = DateTime.UtcNow;
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long ts = (long)((utcNow - epoch).TotalMilliseconds);
            merchantId = ConfigurationManager.AppSettings["merchant_id"];
            ordetId = "114";

            ////ccaRequest = ccaRequest + "tid" + "=" + "T_" + ts + "&";
            ////  ccaRequest = ccaRequest + "tid" + "=" + ts + "&";
            ////  ccaRequest = ccaRequest + "merchant_id" + "=" + ConfigurationManager.AppSettings["merchant_id"] + "&";
            //ccaRequest = ccaRequest + "order_id" + "=" + id + "&";
            //ccaRequest = ccaRequest + "amount" + "=" + amount + "&";
            //ccaRequest = ccaRequest + "currency" + "=" + "INR" + "&";
            //ccaRequest = ccaRequest + "redirect_url" + "=" + ConfigurationManager.AppSettings["redirect_url"] + "&";
            //ccaRequest = ccaRequest + "cancel_url" + "=" + ConfigurationManager.AppSettings["cancel_url"] + "&";
            ////ccaRequest = ccaRequest + "billing_name" + "=" + customerName + "&";
            ////ccaRequest = ccaRequest + "billing_tel" + "=" + customerPhone + "&";
            ////ccaRequest = ccaRequest + "billing_email" + "=" + customerEmail + "&";

            //  ccaRequest = "tid=1485431477245&merchant_id=121242&order_id=111&amount=1&currency=INR&redirect_url=http://192.168.0.89/MCPG.ASP.net.2.0.kit/ccavResponseHandler.aspx&cancel_url=http://192.168.0.96/mcpg_new/iframe/ccavResponseHandler.php&";
            ccaRequest = "tid=" + ts + "&merchant_id=" + merchantId + "&order_id=" + customer.cust_orderID + "&amount=" + "1" + "&currency=INR&redirect_url=" + ConfigurationManager.AppSettings["redirect_url"] + "&cancel_url=" + ConfigurationManager.AppSettings["cancel_url"] + "&billing_name=" + customerName + "&billing_address=Bangalore&billing_city=Bangalore&billing_state=" + customerState + "&billing_zip=000000&billing_country=India&billing_tel=" + customerPhone + "&billing_email=" + customerEmail + "&delivery_name=&delivery_address=&delivery_city=&delivery_state=&delivery_zip=&delivery_country=&delivery_tel=&merchant_param1=&merchant_param2=&merchant_param3=&merchant_param4=&merchant_param5=&promo_code=&customer_identifier=&";
            strEncRequest = ccaCrypto.Encrypt(ccaRequest, workingKey);
            ViewBag.strEncRequest = strEncRequest;
            ViewBag.strAccessCode = strAccessCode;
            return View();
        }

        public ActionResult ResponseHandler()
        {
            //string workingKey = "EA92C282280B2BF2312F0E08132717A4";//put in the 32bit alpha numeric key in the quotes provided here
            CCACrypto ccaCrypto = new CCACrypto();
            string encResponse = ccaCrypto.Decrypt(Request.Form["encResp"], workingKey);
            //NameValueCollection Params = new NameValueCollection();
            Dictionary<string, string> Params = new Dictionary<string, string>();
            string[] segments = encResponse.Split('&');
            foreach (string seg in segments)
            {
                string[] parts = seg.Split('=');
                if (parts.Length > 0)
                {
                    string Key = parts[0].Trim();
                    string Value = parts[1].Trim();
                    Params.Add(Key, Value);
                }
            }


            var paymentStatus = Params["order_status"];
            ViewBag.PaymentStatus = paymentStatus;
            int userId = 0;
            if (!string.IsNullOrEmpty(Convert.ToString(Session["UserId"])))
                userId = Convert.ToInt32(Session["UserId"]);

            client.UpdateCustomerPaymentDetails(userId, paymentStatus);

            ViewBag.segment = encResponse;
            ViewBag.segmentTest = paymentStatus;

            Session["PaymentStatus"] = paymentStatus;
            //Session["PaymentStatus"] = "Success";
            //if()
            //for (int i = 0; i < Params.Count; i++)
            //{
            //    Response.Write(Params.Keys[i] + " = " + Params[i] + "<br>");
            //}
            return RedirectToAction("DashBoard");
        }

        public ActionResult DashBoard()
        {
            //ResponseHandler();
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserId"])))
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(Convert.ToString(Session["CurrentTab"])))
            {
                Session["CurrentTab"] = "tab1";
            }
            ViewBag.CurrentTab = Session["CurrentTab"];

            int userId = 0;
            if (!string.IsNullOrEmpty(Convert.ToString(Session["UserId"])))
                userId = Convert.ToInt32(Session["UserId"]);
            //userId = 11;

            customer customer = new customer();
            customer = client.GetCustomerData(userId);
            Session["userlogin"] = customer.cust_name.Split(' ').ToArray()[0];

            ViewBag.IsPaid = customer.cust_IsPaid;
            if (customer.cust_IsPaid == false)
                return View();

            //customer_service customerService = new customer_service();
            //customerService = client.GetCustomerService(userId);

            //   ViewBag.CustomerBasicDetails = customer;


            if (Convert.ToString(Session["PaymentStatus"]) != null)
                Session["FirstPaymentStatus"] = Convert.ToString(Session["PaymentStatus"]);
            Session.Remove("PaymentStatus");


            customer_service customerService = new customer_service();
            customerService = client.GetCustomerService(24);

            ViewBag.CustomerBasicDetails = customer;

            if (customerService == null)
                return View();


           

            //-----------Cook
            if (!string.IsNullOrEmpty(customerService.cook_id))
            {
                string[] cookArr = customerService.cook_id.Split(',');
                List<cook> lstCook = new List<cook>();
                foreach (var c in cookArr)
                {
                    int cookId = Convert.ToInt32(c);
                    cook cook = new cook();
                    cook = client.GetCookData(cookId);
                    if (cook != null)
                        lstCook.Add(cook);
                }
                ViewBag.CookDetails = lstCook;
            }

            //---------------Restaurant

            if (!string.IsNullOrEmpty(customerService.restaurant_id))
            {
                string[] restArr = customerService.restaurant_id.Split(',');
                List<restaurant> lstRestaurent = new List<restaurant>();
                foreach (var r in restArr)
                {
                    int restId = Convert.ToInt32(r);
                    restaurant resturant = new restaurant();
                    resturant = client.GetRestaurantData(restId);
                    if (resturant != null)
                        lstRestaurent.Add(resturant);
                }
                ViewBag.RestDetails = lstRestaurent;
            }

            //restaurant restaurant = new restaurant();
            //restaurant = client.GetRestaurantData(restId);
            //List<restaurant> lstRest = new List<restaurant>();
            //lstRest.Add(restaurant);
            //ViewBag.RestDetails = lstRest;

            //---------------LPG

            if (!string.IsNullOrEmpty(customerService.lpg_id))
            {
                string[] lpgArr = customerService.lpg_id.Split(',');
                List<lpg> lstLpg = new List<lpg>();
                foreach (var r in lpgArr)
                {
                    int lpgId = Convert.ToInt32(r);
                    lpg lpg = new lpg();
                    lpg = client.GetLpgData(lpgId);
                    if (lpg != null)
                        lstLpg.Add(lpg);
                }
                ViewBag.LpgDetails = lstLpg;
            }


            //---------------Accomodation

            if (!string.IsNullOrEmpty(customerService.accommodation_id))
            {
                string[] accomodationArr = customerService.accommodation_id.Split(',');
                List<accommodation> lstAccomodation = new List<accommodation>();
                foreach (var r in accomodationArr)
                {
                    int accomodationId = Convert.ToInt32(r);
                    accommodation accomodation = new accommodation();
                    accomodation = client.GetAccomodationData(accomodationId);
                    if (accomodation != null)
                        lstAccomodation.Add(accomodation);
                }
                ViewBag.AccomodationDetails = lstAccomodation;
            }

            //---------------Laundry 

            if (!string.IsNullOrEmpty(customerService.laundry_id))
            {
                string[] laundryArr = customerService.laundry_id.Split(',');
                List<laundry> lstLaundry = new List<laundry>();
                foreach (var r in laundryArr)
                {
                    int laundryId = Convert.ToInt32(r);
                    laundry laundry = new laundry();
                    laundry = client.GetLaundryData(laundryId);
                    if (laundry != null)
                        lstLaundry.Add(laundry);
                }
                ViewBag.Laundry = lstLaundry;
            }

            //---------------Salon 

            if (!string.IsNullOrEmpty(customerService.salon_id))
            {
                string[] salonArr = customerService.salon_id.Split(',');
                List<salon> lstSalon = new List<salon>();
                foreach (var r in salonArr)
                {
                    int salonId = Convert.ToInt32(r);
                    salon salon = new salon();
                    salon = client.GetSalonData(salonId);
                    if (salon != null)
                        lstSalon.Add(salon);
                }
                ViewBag.Salon = lstSalon;
            }


            //---------------Internet 

            if (!string.IsNullOrEmpty(customerService.internet_id))
            {
                string[] internetArr = customerService.internet_id.Split(',');
                List<internet> lstInternet = new List<internet>();
                foreach (var r in internetArr)
                {
                    int internetId = Convert.ToInt32(r);
                    internet internet = new internet();
                    internet = client.GetInternetData(internetId);
                    if (internet != null)
                        lstInternet.Add(internet);
                }
                ViewBag.InternetDetails = lstInternet;
            }



            //---------------Market 

            if (!string.IsNullOrEmpty(customerService.market_id))
            {
                string[] marketArr = customerService.market_id.Split(',');
                List<market> lstMarket = new List<market>();
                foreach (var r in marketArr)
                {
                    int marketId = Convert.ToInt32(r);
                    market market = new market();
                    market = client.GetMarketData(marketId);
                    if (market != null)
                        lstMarket.Add(market);
                }
                ViewBag.Market = lstMarket;
            }

            //---------------Packers 

            if (!string.IsNullOrEmpty(customerService.packers_id))
            {
                string[] packerArr = customerService.packers_id.Split(',');
                List<packer> lstPackers = new List<packer>();
                foreach (var r in packerArr)
                {
                    int packerId = Convert.ToInt32(r);
                    packer packer = new packer();
                    packer = client.GetPackerData(packerId);
                    if (packer != null)
                        lstPackers.Add(packer);
                }
                ViewBag.Packers = lstPackers;
            }



            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult AboutBengaluru()
        {

            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }

        public ActionResult Tester()
        {
            return View();
        }


        //public ActionResult ViewLyubomir(string type, int id, int imgCount)
        //{
        //     type = "accommodation";
        //     id = 1;
        //     imgCount = 4;
        //    //ViewBag.Count = 6;
        //    string path = "/Images/DashBoard/" + type;
        //    string imagePath = string.Empty;
        //    List<string> lstImage = new List<string>();
        //    for (int i = 1; i <= imgCount; i++)
        //    {
        //        imagePath = path + "/" + id + "_" + i + ".jpg";
        //        lstImage.Add(@imagePath);
        //    }
        //    ViewBag.Images = lstImage;
        //    return PartialView("DashBoardAnimation");
        //}

        //public ActionResult ViewLyubomir(string type, int id, int imgCount)
        public ActionResult ViewAnimation(string type, int id, int imgCount)
        {
            //type = "accommodation";
            //id = 1;
            //imgCount = 4;
            //ViewBag.Count = 6;z
            string tab = string.Empty;
            if (type == "accommodation")
                tab = "tab1";
            else if (type == "restaurant")
                tab = "tab2";
            else if (type == "market")
                tab = "tab3";
            else if (type == "cook")
                tab = "tab4";
            else if (type == "lpg")
                tab = "tab5";
            else if (type == "internet")
                tab = "tab6";
            else if (type == "laundry")
                tab = "tab7";
            else
                tab = "tab1";

            Session["CurrentTab"] = tab;
            ViewBag.CurrentTab = Session["CurrentTab"];
            string path = "/Images/DashBoard/" + type;
            string imagePath = string.Empty;
            List<string> lstImage = new List<string>();
            for (int i = 1; i <= imgCount; i++)
            {
                imagePath = path + "/" + id + "_" + i + ".jpg";
                lstImage.Add(@imagePath);
            }
            ViewBag.Images = lstImage;
            return PartialView("DashBoardAnimation");
        }

        [HttpPost]
        public ActionResult Animation()
        {
            return RedirectToAction("DashBoard");
        }



        [HttpPost]
        public ActionResult UserQuery(FormCollection collection)
        {
            //collection["name"]
            string name = string.Empty;
            string email = string.Empty;
            string subject = string.Empty;
            string message = string.Empty;

            name = Convert.ToString(collection["name"]);
            email = Convert.ToString(collection["email"]);
            subject = Convert.ToString(collection["subject"]);
            message = Convert.ToString(collection["message"]);

            string result = string.Empty;
            //result = Mail.SendMail("Priyanka", "princey_78670@yahoo.com", "abc@ak2", MailType.Registration);
            result = SendMail(name, email, string.Empty, MailType.UserQuery, "");
            return RedirectToAction("Index");
        }

        #region Email
        enum MailType
        {
            Registration,
            UserQuery,
            UserPayment
        };
        private static string SendMail(string userName, string userMail, string userPassword, MailType type, string package)
        {
            try
            {
                var fromAddress = new MailAddress("support@gofinc.com", "GoFinc");
                const string fromPassword = "123!@#sof";
                var toAddress = new MailAddress(userMail);
                string subject = string.Empty;//"Registration - Finc";
                string body = string.Empty;

                body = createEmailBody(userName, userMail, userPassword, type, package, ref subject);
                var smtp = new SmtpClient
                {
                    Host = "smtpout.asia.secureserver.net",
                    Port = 3535,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }

                return "Sent";
            }
            catch (Exception)
            {
                return "Failed";
            }

        }
        private static string createEmailBody(string userName, string userMail, string userPassword, MailType type, string package, ref string subject)
        {
            string body = string.Empty;
            string mailPage = string.Empty;
            string userPayment = string.Empty;
            //if (package.Contains("Basic"))
            //{
            //    userPayment = "https://imjo.in/NUFXW";
            //}
            //else if (package.Contains("Standard"))
            //{
            //    userPayment = "https://imjo.in/NtB3F";
            //}
            //else
            //{
            //    userPayment = "https://imjo.in/NtB3G";
            //}

            if (type == MailType.Registration)
            {
                mailPage = @"D:\finc\Finc24\Finc24\MailTemplates\MailRegistration.html";
                mailPage = @"C:\Users\user\Videos\Finc24_30Dec\Finc24\MailTemplates\MailRegistration.html";
                mailPage = "~/MailTemplates/MailRegistration.html";

                //  string a = HttpContext.Current.Server.MapPath("~/App_Data/Example.xml");

                //   mailPage = "~/MailTemplates/MailRegistration.html";
                using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(mailPage)))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{UserName}", userName); //replacing the required things  
                body = body.Replace("{UserMail}", userMail);
                body = body.Replace("{UserPassword}", userPassword);
                subject = "Registration - Finc";
                // body = body.Replace("{UserPayment}", userPayment);
            }
            else
            {
                //  mailPage = @"/MailTemplates/MailUserQuery.html";
                // HttpContext.Current.Server.MapPath("/UploadedFiles");
                //  mailPage = @"D:\finc\Finc24\Finc24\MailTemplates\MailUserQuery.html";
                mailPage = "~/MailTemplates/MailUserQuery.html";
                using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(mailPage)))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{UserName}", userName); //replacing the required things  
                subject = "Thank you - FiNC";
            }
            return body;

        }

        #endregion

        #region Crud operations
        // GET: Home/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Home/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Home/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        #endregion

    }
}
