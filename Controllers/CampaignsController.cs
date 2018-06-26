using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using BlueRibbonsReview.Models;
using BlueRibbonsReview.ViewModels;
using Microsoft.AspNet.Identity;

namespace BlueRibbonsReview.Controllers
{
    public class CampaignsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private object userManager;

        public ActionResult About()
        {
            ViewBag.Message = "";

            return View();
        }

        // GET: Relevant Seller Campaigns
        // Static Search For Now, Will change depending on user after SellerID and ApplicationUser Id are 
        // a single data point "db.Campaigns.Where(s => s.ID = ....)"; 

        public ActionResult SellerIndex()
        {
            List<AnalyticsViewModel> campaigns = new List<AnalyticsViewModel>();
            string applicationUserId = User.Identity.GetUserId();
            var sellerCampaigns = db.Campaigns.Where(s => s.ApplicationUser.Id == applicationUserId);
            foreach (var campaign in sellerCampaigns)
            {
                AnalyticsViewModel campaignModel = new AnalyticsViewModel(campaign);
                campaigns.Add(campaignModel);
            }

            return View(campaigns);
        }

        // GET: Campaigns
        public ViewResult Index(string sortOrder, string searchString)
        {

            ViewBag.StartDateDescSortParm = "startDate_desc";
            ViewBag.StartDateSortParm = "startDate";

            ViewBag.CloseDateSortParm = "closeDate";

            ViewBag.NameSortParm = "name";
            ViewBag.NameDescSortParm = "name_desc";

            ViewBag.SalePriceSortParm = "salePrice";
            ViewBag.SalePriceDescSortParm = "salePrice_desc";
            var campaigns = from c in db.Campaigns.Include(c => c.ApplicationUser)
                            select c;

            campaigns = campaigns.Where(c => c.OpenCampaign == true && DateTime.Today < c.ExpireDate || c.ExpireDate == null);

            if (!String.IsNullOrEmpty(searchString))
            {
                campaigns = campaigns.Where(c => c.Name.Contains(searchString)
                    || c.Description.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "startDate_desc":
                    campaigns = campaigns.OrderByDescending(c => c.StartDate);
                    break;
                case "startDate":
                    campaigns = campaigns.OrderBy(c => c.StartDate);
                    break;
                case "closeDate":
                    campaigns = campaigns.OrderBy(c => c.CloseDate);
                    break;
                case "name":
                    campaigns = campaigns.OrderBy(c => c.Name);
                    break;
                case "name_desc":
                    campaigns = campaigns.OrderByDescending(c => c.Name);
                    break;
                case "salePrice":
                    campaigns = campaigns.OrderBy(c => c.SalePrice);
                    break;
                case "salePrice_desc":
                    campaigns = campaigns.OrderByDescending(c => c.SalePrice);
                    break;

                default:
                    campaigns = campaigns.OrderByDescending(c => c.StartDate);
                    break;
            }

            return View(campaigns.ToList());
        }



        // GET: Campaigns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            return View(campaign);
        }


        // GET: Campaigns/Create
        public ActionResult Create()
        {
            return View();
        }


        public ActionResult GetFakeDetails()
        {
            return PartialView("_Details", new Campaign());
        }

        // POST: Campaigns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CampaignID,ASIN,Name,ImageUrL,Description,RetailPrice,SalePrice,StartDate,CloseDate,ExpireDate,VendorsPurchaseInstructions")] Campaign campaign)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    campaign.VendorsPurchaseURL = String.Format("https://www.amazon.com/dp/{0}", campaign.ASIN);
                    campaign.OpenCampaign = false;

                    // Application User ID Change
                    string applicationUserId = User.Identity.GetUserId();
                    campaign.ApplicationUser = db.Users.Single(u => u.Id == applicationUserId);
                    db.Campaigns.Add(campaign);
                    db.SaveChanges();

                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem perists, see your system administrator.");
            }
            return View(campaign);
        }

        public ActionResult Sent()
        {
            return View();
        }

        public ActionResult Landing()
        {
            return View();
        }

        // GET: Campaigns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id); 
            if (campaign == null)
            {
                return HttpNotFound();
            }
            return View(campaign);
        }
        // GET: Campaigns/AdminView/EditAdmin
        public ActionResult EditAdmin(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            return View(campaign);
        }



        // POST: Campaigns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var campaignToUpdate = db.Campaigns.Find(id);
            if (TryUpdateModel(campaignToUpdate, "",
                new string[] { "ASIN", "Name", "OpenCampaign", "ImageURL",
                "Description", "RetailPrice", "SalePrice", "StartDate", "CloseDate",
                "ExpireDate", "VendorsPurchaseInstructions",
                "VendorsPurchaseURL", "SellerID"}))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("AdminView");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(campaignToUpdate);
        }

        // GET: Campaigns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            return View(campaign);
        }

        // POST: Campaigns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Campaign campaign = db.Campaigns.Find(id);
            db.Campaigns.Remove(campaign);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AdminView(string sortOrder)
        {
            ViewBag.UserNameSortParm = sortOrder == "userName" ? "userName_desc" : "userName";
            ViewBag.SellerEmailSortParm = sortOrder == "sellerEmail" ? "sellerEmail_desc" : "sellerEmail";
            ViewBag.OpenCampaignSortParm = String.IsNullOrEmpty(sortOrder) ? "openCampaign_desc" : "";
            ViewBag.StartDateSortParm = sortOrder == "startDate" ? "startDate_desc" : "startDate";
            ViewBag.CloseDateSortParm = sortOrder == "closeDate" ? "closeDate_desc" : "closeDate";
            ViewBag.ExpireDateSortParm = sortOrder == "expireDate" ? "expireDate_desc" : "expireDate";


            var campaigns = from c in db.Campaigns
                            select c;
            switch (sortOrder)
            {
                case "userName":
                    campaigns = campaigns.OrderBy(c => c.ApplicationUser.UserName);
                    break;
                case "userName_desc":
                    campaigns = campaigns.OrderByDescending(c => c.ApplicationUser.UserName);
                    break;
                case "sellerEmail":
                    campaigns = campaigns.OrderBy(c => c.ApplicationUser.Email);
                    break;
                case "sellerEmail_desc":
                    campaigns = campaigns.OrderByDescending(c => c.ApplicationUser.Email);
                    break;
                case "openCampaign_desc":
                    campaigns = campaigns.OrderByDescending(c => c.OpenCampaign);
                    break;
                 case "startDate":
                    campaigns = campaigns.OrderBy(c => c.StartDate);
                    break;
                case "startDate_desc":
                    campaigns = campaigns.OrderByDescending(c => c.StartDate);
                    break;
                case "closeDate":
                    campaigns = campaigns.OrderBy(c => c.CloseDate);
                    break;
                case "closeDate_desc":
                    campaigns = campaigns.OrderByDescending(c => c.CloseDate);
                    break;
                case "expireDate":
                    campaigns = campaigns.OrderBy(c => c.ExpireDate);
                    break;
                case "expireDate_desc":
                    campaigns = campaigns.OrderByDescending(c => c.ExpireDate);
                    break;
                default:
                    campaigns = campaigns.OrderBy(c => c.OpenCampaign);
                    break;
            }
            return View(campaigns.ToList());
        }

        public ActionResult GetLandingPartial()
        {
            var datePlusSeven = DateTime.Today.AddDays(7);
            var expiringCampaigns = from c in db.Campaigns.Include(c => c.ApplicationUser)
                                    select c;

            expiringCampaigns = expiringCampaigns.Where(c => DateTime.Today < c.ExpireDate && datePlusSeven > c.ExpireDate && c.ExpireDate != null);

            return PartialView("_LandingPartial", expiringCampaigns.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
