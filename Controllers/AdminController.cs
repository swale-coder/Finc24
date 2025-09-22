using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using Finc24.FincService;
using CCA.Util;
using Finc24.ServiceReference1;


namespace Finc24.Controllers
{
    public class AdminController : Controller
    {
        Service1Client client = new Service1Client();
        CCACrypto ccaCrypto = new CCACrypto();

        string workingKey = "EA92C282280B2BF2312F0E08132717A4";//put in the 32bit alpha numeric key in the quotes provided here 	
        string ccaRequest = "";
        public string strEncRequest = "";
        public string strAccessCode = "AVPX68EA78BD03XPDB"; // put the access code in the quotes provided here.
        public string testServerPath = @"G:\PleskVhosts\gofinc.com\Test";

        public ActionResult Payment()
        {
            return View();
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
            strEncRequest = ccaCrypto.Encrypt(ccaRequest, workingKey);
            ViewBag.strEncRequest = strEncRequest;
            ViewBag.strAccessCode = strAccessCode;
            return View();
        }

        public ActionResult CustomerList()
        {
            //List<FincService.customer> lstCustomer = client.GetCustomerList().ToList();
            //ViewBag.CustomerList = lstCustomer;
            return View();
        }

        public ActionResult CustomerDetails(int id)
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult CustomerDetails(int custid)
        //{
        //    return View();
        //}

        public ActionResult Cook()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Cook(FormCollection collection, HttpPostedFileBase file)
        {
            int cookId = 0;
            int imgCount = 0;
            if (file != null)
                imgCount = Request.Files.Count;
            string name = Convert.ToString(collection["Name"]);
            int age = Convert.ToInt32(collection["Age"]);
            string phone = Convert.ToString(collection["Phone"]);
            string idProof = Convert.ToString(collection["Id"]);
            string address = Convert.ToString(collection["Address"]);
            string price = Convert.ToString(collection["Price"]);
            string speciality = Convert.ToString(collection["Speciality"]);
            string areas = Convert.ToString(collection["Areas"]);
            int locationId = Convert.ToInt32(collection["Location"]);


            cookId = client.AddToCook(name, age, phone, idProof, address, price, speciality, areas, locationId, imgCount);

            if (file != null)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 1; i <= files.Count; i++)
                {
                    HttpPostedFileBase myFile = files[i - 1];
                    string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Images/Dashboard/cook"), cookId + "_" + i + ".jpg");
                    // file is uploaded
                    myFile.SaveAs(path);

                }
            }
            return View();
        }

        public ActionResult LPG()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LPG(FormCollection collection, HttpPostedFileBase file)
        {
            int lpgId = 0;
            int imgCount = 0;
            if (file != null)
                imgCount = Request.Files.Count;
            string name = Convert.ToString(collection["Name"]);
            string owner = Convert.ToString(collection["Owner"]);
            string type = Convert.ToString(collection["Type"]);
            string cost = Convert.ToString(collection["Cost"]);
            string address = Convert.ToString(collection["Address"]);
            string phone = Convert.ToString(collection["Phone"]);
            int locationId = Convert.ToInt32(collection["Location"]);


            lpgId = client.AddToLPG(name, owner, type, cost, address, phone, locationId, imgCount);

            if (file != null)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 1; i <= files.Count; i++)
                {
                    HttpPostedFileBase myFile = files[i - 1];
                    string path = System.IO.Path.Combine(
                                     Server.MapPath("~/Images/Dashboard/lpg"), lpgId + "_" + i + ".jpg");
                    //  string path = "~/Images/Dashboard/lpg/" + lpgId + "_" + i + ".jpg";
                    //    string path = System.IO.Path.Combine(   testServerPath + "/Images/Dashboard/lpg", lpgId + "_" + i + ".jpg");
                    // file is uploaded
                    // file is uploaded
                    myFile.SaveAs(path);

                }
            }
            return View();
        }

        public ActionResult Market()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Market(FormCollection collection, HttpPostedFileBase file)
        {
            int marketId = 0;
            int imgCount = 0;
            if (file != null)
                imgCount = Request.Files.Count;
            string name = Convert.ToString(collection["Name"]);
            string type = Convert.ToString(collection["Type"]);
            string bargain = Convert.ToString(collection["Bargain"]);
            string address = Convert.ToString(collection["Address"]);
            string speciality = Convert.ToString(collection["Speciality"]);
            int locationId = Convert.ToInt32(collection["Location"]);

            marketId = client.AddToMarket(name, type, bargain, address, speciality, locationId, imgCount);

            if (file != null)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 1; i <= files.Count; i++)
                {
                    HttpPostedFileBase myFile = files[i - 1];
                    string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Images/Dashboard/market"), marketId + "_" + i + ".jpg");
                    // file is uploaded
                    myFile.SaveAs(path);

                }
            }
            return View();
        }

        public ActionResult Restaurant()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Restaurant(FormCollection collection, HttpPostedFileBase file)
        {
            int restId = 0;
            int imgCount = 0;
            if (file != null)
                imgCount = Request.Files.Count;
            string name = Convert.ToString(collection["Name"]);
            string budget = Convert.ToString(collection["Budget"]);
            string phone = Convert.ToString(collection["Phone"]);
            string address = Convert.ToString(collection["Address"]);
            string speciality = Convert.ToString(collection["Speciality"]);
            int locationId = Convert.ToInt32(collection["Location"]);

            restId = client.AddToRestaurant(name, budget, phone, address, speciality, locationId, imgCount);

            if (file != null)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 1; i <= files.Count; i++)
                {
                    HttpPostedFileBase myFile = files[i - 1];
                    string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Images/Dashboard/restaurant"), restId + "_" + i + ".jpg");
                    // file is uploaded
                    myFile.SaveAs(path);

                }
            }
            return View();
        }

        public ActionResult Packers()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Packers(FormCollection collection, HttpPostedFileBase file)
        {
            int packId = 0;
            int imgCount = 0;
            if (file != null)
                imgCount = Request.Files.Count;
            string name = Convert.ToString(collection["Name"]);
            string phone = Convert.ToString(collection["Phone"]);
            string rate = Convert.ToString(collection["Rate"]);
            string description = Convert.ToString(collection["Description"]);
            int locationId = Convert.ToInt32(collection["Location"]);

            packId = client.AddToPackers(name, phone, rate, description, locationId, imgCount);

            if (file != null)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 1; i <= files.Count; i++)
                {
                    HttpPostedFileBase myFile = files[i - 1];
                    string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Images/Dashboard/packers"), packId + "_" + i + ".jpg");
                    // file is uploaded
                    myFile.SaveAs(path);

                }
            }
            return View();
        }

        public ActionResult Laundry()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Laundry(FormCollection collection, HttpPostedFileBase file)
        {
            int laundryId = 0;
            int imgCount = 0;
            if (file != null)
                imgCount = Request.Files.Count;
            string name = Convert.ToString(collection["Name"]);
            string phone = Convert.ToString(collection["Phone"]);
            string rate = Convert.ToString(collection["Rate"]);
            string pick = Convert.ToString(collection["Pick"]);
            string address = Convert.ToString(collection["Address"]);
            string description = Convert.ToString(collection["Description"]);
            int locationId = Convert.ToInt32(collection["Location"]);

            laundryId = client.AddToLaundry(name, phone, rate, pick, address, description, locationId, imgCount);

            if (file != null)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 1; i <= files.Count; i++)
                {
                    HttpPostedFileBase myFile = files[i - 1];
                    string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Images/Dashboard/laundry"), laundryId + "_" + i + ".jpg");
                    // file is uploaded
                    myFile.SaveAs(path);

                }
            }
            return View();
        }


        public ActionResult Salon()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Salon(FormCollection collection, HttpPostedFileBase file)
        {
            int salonId = 0;
            int imgCount = 0;
            if (file != null)
                imgCount = Request.Files.Count;
            string name = Convert.ToString(collection["Name"]);
            string type = Convert.ToString(collection["Type"]);
            string services = Convert.ToString(collection["Services"]);
            string phone = Convert.ToString(collection["Phone"]);
            string address = Convert.ToString(collection["Address"]);
            int locationId = Convert.ToInt32(collection["Location"]);

            salonId = client.AddToSalon(name, type, services, phone, address, locationId, imgCount);

            if (file != null)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 1; i <= files.Count; i++)
                {
                    HttpPostedFileBase myFile = files[i - 1];
                    string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Images/Dashboard/salon"), salonId + "_" + i + ".jpg");
                    // file is uploaded
                    myFile.SaveAs(path);

                }
            }
            return View();
        }


        public ActionResult Internet()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Internet(FormCollection collection, HttpPostedFileBase file)
        {
            int internetId = 0;
            int imgCount = 0;
            if (file != null)
                imgCount = Request.Files.Count;
            string name = Convert.ToString(collection["Name"]);
            string type = Convert.ToString(collection["Type"]);
            string price = Convert.ToString(collection["Price"]);
            string phone = Convert.ToString(collection["Phone"]);
            string address = Convert.ToString(collection["Address"]);
            int locationId = Convert.ToInt32(collection["Location"]);

            internetId = client.AddToInternet(name, type, price, phone, address, locationId, imgCount);

            if (file != null)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 1; i <= files.Count; i++)
                {
                    HttpPostedFileBase myFile = files[i - 1];
                    string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Images/Dashboard/internet"), internetId + "_" + i + ".jpg");
                    // file is uploaded
                    myFile.SaveAs(path);

                }
            }
            return View();
        }



        public ActionResult Accommodation()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Accommodation(FormCollection collection, HttpPostedFileBase file)
        {
            int accoId = 0;
            int imgCount = 0;
            if (file != null)
                imgCount = Request.Files.Count;
            string type = Convert.ToString(collection["Type"]);
            string name = Convert.ToString(collection["Name"]);
            string desc = Convert.ToString(collection["Desc"]);
            string owner = Convert.ToString(collection["Owner"]);
            string address = Convert.ToString(collection["Address"]);
            string phone = Convert.ToString(collection["Phone"]);
            string rent = Convert.ToString(collection["Rent"]);
            string sharing = Convert.ToString(collection["Sharing"]);
            int locationId = Convert.ToInt32(collection["Location"]);

            accoId = client.AddToAccommodation(type, name, desc, owner, address, phone, rent, sharing, locationId, imgCount);

            if (file != null)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 1; i <= files.Count; i++)
                {
                    HttpPostedFileBase myFile = files[i - 1];
                    string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Images/Dashboard/accommodation"), accoId + "_" + i + ".jpg");
                    // file is uploaded
                    myFile.SaveAs(path);

                }
            }
            return View();
        }

        #region Crud operation
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
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

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Edit/5
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

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Delete/5
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
