using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlueRibbonsReview.Models;
using BlueRibbonsReview.ViewModels;
using Microsoft.AspNet.Identity;

namespace BlueRibbonsReview.Controllers
{
    public class ReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reviews       
        public ActionResult Index(string sortOrder)
        {
  
            ViewBag.Title = "Product Reviews";

            List<AnalyticsViewModel> campaignList = new List<AnalyticsViewModel>();

            // LINQ Query Syntax used over Method Syntax for simplicity and clarity; performance will be the same. 
            var campaignsWithReviews = from campaigns in db.Campaigns
                                                                 from reviews in db.Reviews
                                                                 where campaigns.CampaignID == reviews.CampaignId
                                                                 orderby campaigns.CampaignID
                                                                 select campaigns;

            ViewBag.NameSortParm = "name";
            ViewBag.NameDescSortParm = "name_desc";
            ViewBag.RatingSortParm = "rating";
            ViewBag.RatingDescSortParm = "rating_desc";

            switch (sortOrder)
            {  //sort reviews based on "Sort by" dropdown selection
                case "name":
                    campaignsWithReviews = campaignsWithReviews.OrderBy(r => r.Name);
                    break;
                case "name_desc":
                    campaignsWithReviews = campaignsWithReviews.OrderByDescending(r => r.Name);
                    break;
                case "rating":
                    campaignsWithReviews = from campaigns in db.Campaigns
                                           from reviews in db.Reviews
                                           where campaigns.CampaignID == reviews.CampaignId
                                           orderby reviews.ProductRating
                                           select campaigns;
                    break;
                case "rating_desc":
                    campaignsWithReviews = from campaigns in db.Campaigns
                                           from reviews in db.Reviews
                                           where campaigns.CampaignID == reviews.CampaignId
                                           orderby reviews.ProductRating descending
                                           select campaigns;
                    break;
            }

            // Creating new variable of List<Campaign>, containing only unique instances of campagins with reviews.
            var uniqueCampaignsWithReviews = new HashSet<Campaign>(campaignsWithReviews).ToList();
            foreach (var campaign in uniqueCampaignsWithReviews)
            {
                AnalyticsViewModel campaignModel = new AnalyticsViewModel(campaign);
                campaignList.Add(campaignModel);
            }

            return View(campaignList);

        }

        // GET: Relevant Reviews on the Campaigns the Seller has created.   
        public ActionResult SellerReviewIndex()
        {
            List<AnalyticsViewModel> campaignList = new List<AnalyticsViewModel>();
            string UserId = HttpContext.User.Identity.GetUserId();
            var campaigns = db.Campaigns.Where(x => x.ApplicationUser.Id.ToString() == UserId);

            foreach (var campaign in campaigns)
            {
                AnalyticsViewModel campaignModel = new AnalyticsViewModel(campaign);
                campaignList.Add(campaignModel);
            }
    
            ViewBag.Title = "Reviews of Your Products";

            return View("Index", campaignList);
        }

        // GET: Reviews the buyer has created
        // Same expression as seen on Campaign Controller
        public ActionResult ReviewIndex()
        {
            List<AnalyticsViewModel> campaignList = new List<AnalyticsViewModel>();
            string UserId = HttpContext.User.Identity.GetUserId();
            var campaigns = db.Campaigns.Where(x => x.ApplicationUser.Id.ToString() == UserId);

            foreach (var campaign in campaigns)
            {
                AnalyticsViewModel campaignModel = new AnalyticsViewModel(campaign);
                campaignList.Add(campaignModel);
            }

            ViewBag.Title = "Products You've Reviewd";

            return View("Index", campaignList);
        }

        // GET: Reviews/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // GET: Reviews/Create
        public ActionResult Create(int? id, string name)
        {
            string Campaign = "";

            if (id == null)
            {
                // Create raw html for each item in db.Campaigns and pass to ViewBag
                Campaign = "<div class=\"dropdown filterArea\">" +
                           "<input class=\"dropdown-toggle\" id=\"campaignReview\" type=\"text\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"true\" placeholder=\"Select A Campaign\">" +
                           "<ul class=\"dropdown-menu\">\"";
                foreach (var item in db.Campaigns)
                {
                    Campaign += string.Format(
                        "<li id=\"{0}\"><a><img src=\"{1}\" class=\"imgDisplay\" />" +
                        "<p class=\"imageText\" id=\"dropdownName\">{2}</p></a>" +
                        "</li>",
                        item.CampaignID, item.ImageUrL, item.Name);
                }
                Campaign += "</ul>" +
                            "<br />" +
                            "</div>";
            }
            else
            {
                // Create raw html for the item specified in parameters, send CampaignId received to controller
                Campaign = string.Format("<input type='text' value=\"{0}\" readonly", name);
                ViewBag.CampaignReviewId = id;


            }

            ViewBag.Campaign = Campaign;
            Review review = new Review();
            review.ReviewDate = DateTime.Today;

            return View(review);
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductRating,ReviewDetails,CampaignId")] Review review)
        {
            try
            {
                if (string.IsNullOrEmpty(review.ReviewDetails))
                {
                    ModelState.AddModelError("ReviewDetails", "Please write a review.");
                }
                if (review.CampaignId == null)
                {
                    ModelState.AddModelError("CampaignId", "Please select a campaign.");
                }
                if (ModelState.IsValid)
                {
                    review.ReviewID = Guid.NewGuid();
                    review.ReviewDate = DateTime.Today;

                    // Application User ID Change
                    string applicationUserId = User.Identity.GetUserId();
                    review.ApplicationUser = db.Users.Where(u => u.Id == applicationUserId).Single();
                    review.ProductRating = Int32.Parse(Request.Form["rating"]);
                    db.Reviews.Add(review);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Please try again and if the problem persists, see your System Administrator.");
            }

            // Create raw html for each item in db.Campaigns and pass to ViewBag
            string Campaign = "";
            Campaign = "<div class=\"dropdown filterArea\">" +
                       "<input class=\"dropdown-toggle\" id=\"campaignReview\" type=\"text\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"true\" placeholder=\"Select a Campaign\">" +
                       "<ul class=\"dropdown-menu\">\"";
            foreach (var item in db.Campaigns)
            {
                Campaign += string.Format(
                    "<li id=\"{0}\"><a><img src=\"{1}\" class=\"imgDisplay\" />" +
                    "<p class=\"imageText\" id=\"dropdownName\">{2}</p></a>" +
                    "</li>",
                    item.CampaignID, item.ImageUrL, item.Name);
            }
            Campaign += "</ul>" +
                        "<br />" +
                        "</div>";
            ViewBag.Campaign = Campaign;

            return View(review);
        }

        // GET: Reviews/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Edit/5
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
            Review review = db.Reviews.Find(id);
            if (TryUpdateModel(review, "", new string[] { "ReviewDate", "ProductRating", "ReviewDetails" }))
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DataException/*dex*/)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again and if the problem continues contact your System Administrator.");
                }
            }
            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
            db.SaveChanges();
            return RedirectToAction("Index");
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
