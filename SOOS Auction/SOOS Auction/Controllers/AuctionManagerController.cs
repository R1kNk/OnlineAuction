using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using SOOS_Auction.AuctionDatabase.Models;
using SOOS_Auction.AuctionGoogleDrive;
using SOOS_Auction.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SOOS_Auction.Controllers
{
    [Authorize(Roles ="admin, moder")]
    public class AuctionManagerController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private UserStore<ApplicationUser> _userStore;
        private RoleManager<IdentityRole> _roleManager;

        public AuctionManagerController()
        {
        }
        public AuctionManagerController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager { get=>_signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); private set=>_signInManager = value; }
        public ApplicationUserManager UserManager { get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); private set =>_userManager = value; }
        public UserStore<ApplicationUser> UserStore { get => _userStore ?? new UserStore<ApplicationUser>(new ApplicationDbContext()); set => _userStore = value; }
        public RoleManager<IdentityRole> RoleManager { get => _roleManager ?? new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext())); set => _roleManager = value; }

        // GET: AuctionManager
        public ActionResult Users()
        {
            string UserID = HttpContext.User.Identity.GetUserId();
            List<ApplicationUser> users = UserManager.Users.Where(p => p.Id != UserID).ToList();
            //users[0].UserName = "123lul";
            return View(users);
        }

        // GET: AuctionManager/Details/mail?=...
        public ActionResult UserDetails(string userId)
        {
            ApplicationUser appUser = new ApplicationUser();
            if(userId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            appUser = UserManager.FindById(userId);
            
            if (appUser == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            List<IdentityUserRole> roles = appUser.Roles.ToList();
            List<string> roleNames = new List<string>();
            foreach (var role in roles)
            {
                roleNames.Add(RoleManager.FindById(role.RoleId).Name);
            }            
            UserViewModel user = new UserViewModel() { Email = appUser.Email, UserName = appUser.UserName, Roles = roleNames, Gender = appUser.Gender, TelephoneNumber = appUser.TelephoneNumber };
            return View(user);
        }


        // GET: AuctionManager/Edit/mail?=...
        [Authorize(Roles ="admin")]
        public ActionResult UserEdit(string userId)
        {
            ApplicationUser appUser = new ApplicationUser();
            if (userId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            appUser = UserManager.FindById(userId);

            if (appUser == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            List<IdentityUserRole> roles = appUser.Roles.ToList();
            List<string> roleNames = new List<string>();
            UserEditModel user = new UserEditModel() { Email = appUser.Email, UserName = appUser.UserName, Gender = appUser.Gender, TelephoneNumber = appUser.TelephoneNumber };
            foreach (var role in roles)
            {
                string RoleName = roleManager.FindById(role.RoleId).Name;
                roleNames.Add(RoleName);
                if (RoleName == "moder") user.ModerRole = true;
                if (RoleName == "admin") user.AdminRole = true;

            }
            user.Roles = roleNames;
            List<SelectListItem> genders = new List<SelectListItem>();
            if(user.Gender=="мужской")
            genders.Add(new SelectListItem { Text = "Мужской", Value = "мужской", Selected=true});
            else genders.Add(new SelectListItem { Text = "Мужской", Value = "мужской"});
  
            if (user.Gender=="женский")
            genders.Add(new SelectListItem { Text = "Женский", Value = "женский", Selected=true});
            else genders.Add(new SelectListItem {  Text = "Женский", Value = "женский"});

            ViewData["genders"] = genders  as IEnumerable<SelectListItem>;
            return View(user);
        }

        // POST: AuctionManager/Edit/mail?=...
        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult UserEdit(UserEditModel user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            ApplicationUser findedUser = UserManager.FindByEmail(user.Email);
            findedUser.UserName = user.UserName;
            findedUser.TelephoneNumber = user.TelephoneNumber;
            findedUser.Gender = user.Gender;
            IdentityRole admin = RoleManager.FindByName("admin");
            IdentityRole moder = RoleManager.FindByName("moder");

            if (user.AdminRole == true && !findedUser.Roles.Any(p=>p.RoleId==admin.Id))
            {
                UserManager.AddToRole(findedUser.Id, "admin");
            }
             if (user.ModerRole == true && !findedUser.Roles.Any(p => p.RoleId == moder.Id))
            {
                UserManager.AddToRole(findedUser.Id, "moder");
            }
             if (user.AdminRole == false && findedUser.Roles.Any(p => p.RoleId == admin.Id))
            {
                UserManager.RemoveFromRole(findedUser.Id, "admin");
            }
            else if (user.ModerRole == false && findedUser.Roles.Any(p => p.RoleId == moder.Id))
            {
                UserManager.RemoveFromRole(findedUser.Id, "moder");
            }
            UserManager.Update(findedUser);
            return RedirectToAction("Users");
        }

        // GET: AuctionManager/Delete/5
        public ActionResult UserDelete(string userId)
        {
            AuctionContext context = new AuctionContext();
            if (userId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = UserManager.FindById(userId);
            
            if (user == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            //lots of user delete
            List<Lot> userLots = context.Lots.Where(p => p.UserId == user.Id).ToList();
            foreach (var lot in userLots) { context.Lots.Remove(lot); }
            List<Bid> userbids = context.Bids.Where(p => p.User == user.Id).ToList();
            foreach (var bid in userbids)
            { context.Bids.Remove(bid);}


            var result =  UserManager.Delete(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Users");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
        }

        [Authorize(Roles = "admin, moder")]
        public ActionResult Lots(string lotsType)
        {
            AuctionContext auctionContext = new AuctionContext();
            List<Lot> lots;
            if (lotsType == "all")
            {
                lots = auctionContext.Lots.Include(p => p.Category).Include(p => p.Bids).Where(p=>p.State=="pending").ToList();
                List<Lot> started = auctionContext.Lots.Include(p => p.Category).Include(p => p.Bids).Where(p => p.State == "started").ToList();
                List<Lot> finished = auctionContext.Lots.Include(p => p.Category).Include(p => p.Bids).Where(p => p.State == "finished").ToList();
                List<Lot> rejected = auctionContext.Lots.Include(p => p.Category).Include(p => p.Bids).Where(p => p.State == "rejected").ToList();
                lots.AddRange(started);
                lots.AddRange(finished);
                lots.AddRange(rejected);
                return View(lots);
            }
            if (lotsType == "started")
            {
                lots = auctionContext.Lots.Include(p => p.Category).Include(p => p.Bids).Where(p=>p.State=="started").ToList();
                return View(lots);
            }
            if (lotsType == "pending")
            {
                lots = auctionContext.Lots.Include(p => p.Category).Include(p => p.Bids).Where(p => p.State == "pending").ToList();
                return View(lots);
            }
            if (lotsType == "finished")
            {
                lots = auctionContext.Lots.Include(p => p.Category).Include(p => p.Bids).Where(p => p.State == "finished").ToList();
                return View(lots);
            }
            if (lotsType == "rejected")
            {
                lots = auctionContext.Lots.Include(p => p.Category).Include(p => p.Bids).Where(p => p.State == "rejected").ToList();
                return View(lots);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);    
        }

        [Authorize(Roles = "admin, moder")]
        public ActionResult AcceptLot(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Lot findedLot;
            AuctionContext context = new AuctionContext();

            findedLot = context.Lots.Where(p => p.LotId == id).SingleOrDefault();
            if (findedLot == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if (findedLot.State == "pending")
            {
                findedLot.State = "started";
                findedLot.StartDate = DateTime.Now;
                DateTime FinishDate = DateTime.Now;
                FinishDate = FinishDate.AddDays(findedLot.DaysDuration);
                findedLot.FinishDate = FinishDate;
                context.SaveChanges();
                return RedirectToAction("Lots", new { lotsType = "all" });

            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "admin, moder")]
        public ActionResult RejectLot(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Lot findedLot;
            AuctionContext context = new AuctionContext();

            findedLot = context.Lots.Where(p => p.LotId == id).SingleOrDefault();
            if (findedLot == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if (findedLot.State == "pending")
            {
                findedLot.State = "rejected";
                context.SaveChanges();
                return RedirectToAction("Lots", new { lotsType = "all" });
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "admin, moder")]
        public ActionResult DeleteLot(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Lot findedLot;
            AuctionContext context = new AuctionContext();

            findedLot = context.Lots.Include(path=>path.Bids).Where(p => p.LotId == id).SingleOrDefault();
            if (findedLot == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if (findedLot.isPaymentBySite)
            {
                if (findedLot.Bids.Count != 0)
                {
                    List<string> participatedUsersId = findedLot.Bids.Select(p => p.User).Distinct().ToList();
                    foreach (var userId in participatedUsersId)
                    {
                        double MaxPrice = findedLot.Bids.Where(p => p.User == userId).Select(p => p.Price).Max();
                        ApplicationUser bidder = UserManager.FindById(userId);
                        bidder.BusyBalance -= MaxPrice;
                        UserManager.Update(bidder);
                    }
                }
            }
            context.Lots.Remove(findedLot);
            context.SaveChanges();
            return RedirectToAction("Lots", new { lotsType = "all" });
          
        }

        public ActionResult ModerLotDetails(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Lot findedLot;
            AuctionContext context = new AuctionContext();

            findedLot = context.Lots.Include(p=>p.Category).Include(p => p.LotPayment).Include(p => p.LotReceiving).Include(p => p.Bids).Where(p => p.LotId == id).SingleOrDefault();
            if (findedLot == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            LotDetails details = LotToLotDetailsModel(findedLot);
            if (details == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            return View(details);
        }

        public ActionResult Reviews(string reviewsType)
        {
            AuctionContext auctionContext = new AuctionContext();
            List<UserReview> reviews = new List<UserReview>(); ;
            if (reviewsType == "all")
            {
                List<UserReview> pending = auctionContext.Reviews.Where(p => p.State == "pending").ToList();
                List<UserReview> checkedRev = auctionContext.Reviews.Where(p => p.State == "checked").ToList();
                List<UserReview> rejected = auctionContext.Reviews.Where(p => p.State == "rejected").ToList();
                reviews.AddRange(pending);
                reviews.AddRange(checkedRev);
                reviews.AddRange(rejected);
                return View(reviews);
            }
            
            if (reviewsType == "pending")
            {
                reviews = auctionContext.Reviews.Where(p => p.State == "pending").ToList();
                return View(reviews);
            }
            if (reviewsType == "checked")
            {
                reviews = auctionContext.Reviews.Where(p => p.State == "checked").ToList();
                return View(reviews);
            }
            if (reviewsType == "rejected")
            {
                reviews = auctionContext.Reviews.Where(p => p.State == "rejected").ToList();
                return View(reviews);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "admin, moder")]
        public ActionResult AcceptReview(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            UserReview findedReview;
            AuctionContext context = new AuctionContext();

            findedReview = context.Reviews.Where(p => p.UserReviewId == id).SingleOrDefault();
            if (findedReview == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if (findedReview.State == "pending")
            {
                findedReview.State = "checked";
                context.SaveChanges();
                ApplicationUser user = UserManager.FindById(findedReview.UserId);
                if (user != null)
                {
                    if (findedReview.isPositive) user.PositiveReview += 1;
                    else user.NegativeReview += 1;
                    UserManager.Update(user);
                }
                return RedirectToAction("Reviews", new { reviewsType = "all" });

            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "admin, moder")]
        public ActionResult RejectReview(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            UserReview findedReview;
            AuctionContext context = new AuctionContext();

            findedReview = context.Reviews.Where(p => p.UserReviewId == id).SingleOrDefault();
            if (findedReview == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if (findedReview.State == "pending")
            {
                findedReview.State = "rejected";
                context.SaveChanges();
                return RedirectToAction("Reviews", new { reviewsType = "all" });

            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "admin, moder")]
        public ActionResult DeleteReview(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            UserReview findedReview;
            AuctionContext context = new AuctionContext();

            findedReview = context.Reviews.Where(p => p.UserReviewId == id).SingleOrDefault();
            if (findedReview == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if (findedReview.State != "pending")
            {
                ApplicationUser user = UserManager.FindById(findedReview.UserId);
                if (user != null)
                {
                    if (findedReview.isPositive) user.PositiveReview -= 1;
                    else user.NegativeReview -= 1;
                    UserManager.Update(user);
                }
                context.Entry(findedReview).State = EntityState.Deleted;
                context.SaveChanges();
                return RedirectToAction("Reviews", new { reviewsType = "all" });

            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        }

        LotDetails LotToLotDetailsModel(Lot findedLot) 
        {
            if (findedLot.LotPayment == null || findedLot.LotReceiving == null) return null;
            AuctionContext context = new AuctionContext();

            LotDetails lotDetails = new LotDetails();
            lotDetails.UserId = findedLot.UserId;
            ApplicationUser user;
            user = UserManager.Users.Where(p => p.Id == findedLot.UserId).SingleOrDefault();
            if (user == null) return null;
            lotDetails.UserName = user.UserName;
            lotDetails.UserId = user.Id;
            lotDetails.UserPositiveReviews = user.PositiveReview;
            lotDetails.UserNegativeReviews = user.NegativeReview;
            lotDetails.UserLocation = user.UserLocation;
            lotDetails.UserAvatarUrl = user.AvatarUrl;

            lotDetails.LotId = findedLot.LotId;
            lotDetails.Name = findedLot.Name;
            lotDetails.MinimalPrice = findedLot.MinimalPrice;
            lotDetails.MinimalStep = findedLot.MinimalStep;
            lotDetails.CurrentPrice = findedLot.CurrentPrice;
            lotDetails.Description = findedLot.Description;
            lotDetails.DaysDuration = findedLot.DaysDuration;
            lotDetails.StartDate = findedLot.StartDate;
            lotDetails.FinishDate = findedLot.FinishDate;
            Category currentCategory = context.Categories.Include(p => p.Section).Where(p => p.CategoryId == findedLot.CategoryId).Single();
            lotDetails.SectionName = currentCategory.Section.Name;
            lotDetails.CategoryName = currentCategory.Name;
            lotDetails.Location = findedLot.LotReceiving.Location;
            lotDetails.ByPost = findedLot.LotReceiving.ByPost;
            lotDetails.DeliveryInPerson = findedLot.LotReceiving.DeliveryInPerson;
            lotDetails.ByPostToAnotherCountry = findedLot.LotReceiving.ByPostToAnotherCountry;
            lotDetails.ReturnAfterBuyingIsForbidden = findedLot.LotReceiving.ReturnAfterBuyingIsForbidden;
            lotDetails.Cash = findedLot.LotPayment.Cash;
            lotDetails.NonCash = findedLot.LotPayment.NonCash;
            lotDetails.FullPrepaymentPostSending = findedLot.LotPayment.FullPrepaymentPostSending;
            lotDetails.UserImagesID = FileApiMethods.GetFilesIDFromFolder(findedLot.ImagesUrl);
            if (lotDetails.UserImagesID == null) return null;
            lotDetails.Bids = findedLot.Bids;
            lotDetails.State = findedLot.State;
            lotDetails.WinnerId = findedLot.WinnerId;
            ApplicationUser winner = UserManager.Users.Where(p => p.Id == findedLot.WinnerId).SingleOrDefault();
            if (winner != null)
            {
                lotDetails.WinnerName = winner.UserName;
            }
            return lotDetails;
        }
    }
}
