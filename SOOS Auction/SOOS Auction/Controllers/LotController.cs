using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SOOS_Auction.AuctionDatabase.Models;
using SOOS_Auction.AuctionGoogleDrive;
using SOOS_Auction.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SOOS_Auction.Controllers
{
    [Authorize]
    public class LotController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager { get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); private set => _userManager = value; }

        static List<SelectListItem> Locations = new List<SelectListItem>() {
                  new SelectListItem { Text = "Минск", Value = "Минск", Selected = true },
                  new SelectListItem { Text = "Брест", Value = "Брест" },
                  new SelectListItem { Text = "Гродно", Value = "Гродно" },
                  new SelectListItem { Text = "Гомель", Value = "Гомель" },
                  new SelectListItem { Text = "Витебск", Value = "Витебск" },
                  new SelectListItem { Text = "Могилев", Value = "Могилев" },
                  new SelectListItem { Text = "Бобруйск", Value = "Бобруйск" },
                  new SelectListItem { Text = "Барановичи", Value = "Барановичи" },
                  new SelectListItem { Text = "Новополоцк", Value = "Новополоцк" },
                  new SelectListItem { Text = "Пинск", Value = "Пинск" },
                  new SelectListItem { Text = "Борисов", Value = "Борисов" },
                  new SelectListItem { Text = "Мозырь", Value = "Мозырь" },
                  new SelectListItem { Text = "Полоцк", Value = "Полоцк" },
                  new SelectListItem { Text = "Слоним", Value = "Слоним" },
                  new SelectListItem { Text = "Лида", Value = "Лида" },
                  new SelectListItem { Text = "Орша", Value = "Орша" },
                  new SelectListItem { Text = "Молодечно", Value = "Молодечно" },
                  new SelectListItem { Text = "Жлобин", Value = "Жлобин" },
                  new SelectListItem { Text = "Кобрин", Value = "Кобрин" },
                  new SelectListItem { Text = "Слуцк", Value = "Слуцк" }
                     };


        // GET: Lot/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Lot findedLot;
            AuctionContext context = new AuctionContext();

            findedLot = context.Lots.Include(p=>p.Bids).Include(p=>p.Category).Include(p=>p.LotPayment).Include(p=>p.LotReceiving).Include(p=>p.Bids).Where(p => p.LotId == id).SingleOrDefault();
            if(findedLot==null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if(findedLot.State=="pending")
            {
                if (HttpContext.User.IsInRole("admin")||HttpContext.User.IsInRole("moder"))
                {
                    return RedirectToAction("ModerLotDetails", "AuctionManager", new { id = findedLot.LotId });
                }
                else return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);

            }

            LotDetails lotDetails = LotToLotDetailsModel(findedLot);
            if (lotDetails == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            return View(lotDetails);
        }

        
        // GET: Lot/Create
        public ActionResult Create()
        {
            DeleteUserImages();
            AuctionContext auctionContext = new AuctionContext();
            List<Lot> lots = auctionContext.Lots.Include(path=>path.LotPayment).Include(p=>p.LotReceiving).ToList();
            Section section = auctionContext.Sections.First();
            SelectList sections = new SelectList(auctionContext.Sections, "SectionId", "Name", section.Name);
            ViewBag.Sections = sections;
            SelectList categories = new SelectList(auctionContext.Categories.Where(p=>p.SectionId==section.SectionId), "CategoryId", "Name");
            ViewBag.Categories = categories;
            ViewBag.Locations = Locations;
            return View();
        }

        // POST: Lot/Create
        [HttpPost]
        public ActionResult Create(LotCreate model)
        {
            AuctionContext auctionContext = new AuctionContext();
            if (!ModelState.IsValid)
            {
                DeleteUserImages();
                Section section = auctionContext.Sections.First();
                SelectList sections = new SelectList(auctionContext.Sections, "SectionId", "Name", section.Name);
                ViewBag.Sections = sections;
                SelectList categories = new SelectList(auctionContext.Categories.Where(p => p.SectionId == section.SectionId), "CategoryId", "Name");
                ViewBag.Categories = categories;
                ViewBag.Locations = Locations;
                return View(model);
            }

            Lot newLot = new Lot() { Name = model.Name, CategoryId = model.CategoryId, State = "pending", CurrentPrice = 0, MinimalPrice = model.MinimalPrice, MinimalStep = model.MinimalStep, Description = model.Description, DaysDuration = model.DaysDuration, UserId = HttpContext.User.Identity.GetUserId(), StartDate=DateTime.Now,FinishDate=DateTime.Now };
            LotReceiving newReceiving = new LotReceiving() { ByPost = model.ByPost, ByPostToAnotherCountry = model.ByPostToAnotherCountry, DeliveryInPerson = model.DeliveryInPerson, Location = model.Location, ReturnAfterBuyingIsForbidden = model.ReturnAfterBuyingIsForbidden };
            LotPayment newPayment = new LotPayment() { Cash = model.Cash, NonCash = model.NonCash, FullPrepaymentPostSending = model.FullPrepaymentPostSending, AdditionalInformation = model.AdditionalInformation };
            auctionContext.LotsReceivings.Add(newReceiving);
            auctionContext.SaveChanges();
            auctionContext.LotPayments.Add(newPayment);
            auctionContext.SaveChanges();

            newLot.LotReceiving = newReceiving;
            newLot.LotPayment = newPayment;
            auctionContext.Lots.Add(newLot);
            auctionContext.SaveChanges();
            //image uploading
            string newFolderId = FileApiMethods.CreateFolder(newLot.LotId);
            FileApiMethods.MakeFilePublic(newFolderId);
            List<string> files = GetUserImages();
            if (files.Count != 0)
            {
                foreach (var file in files)
                {
                    string mimeType = MimeMapping.GetMimeMapping(file);
                    FileApiMethods.Upload(file, file, mimeType, newFolderId);
                }

            }
            newLot.ImagesUrl = newFolderId;
            auctionContext.SaveChanges();
            List<string> lul = FileApiMethods.GetFilesIDFromFolder(newFolderId);
            //image uploading
            return RedirectToAction("About", "Home");
        }
        [HttpPost]
        public void MakeBid(string newBid, int lotId)
        {
            AuctionContext context = new AuctionContext();
            float bid;
            int lot;
            try
            {
                bid = (float)Convert.ToDouble(newBid);
                lot = Convert.ToInt32(lotId);
            }
            catch(FormatException e) { return; }
            Lot findedLot;
            findedLot = context.Lots.Where(p => p.LotId == lotId).SingleOrDefault();
            if (findedLot == null) return;
            if (findedLot.CurrentPrice + findedLot.MinimalStep > bid) return;
            Bid newbid = new Bid() { LotId = findedLot.LotId, Price = bid, BidDate = DateTime.Now, User = HttpContext.User.Identity.GetUserId() };
            context.Bids.Add(newbid);
            context.SaveChanges();
            UpdateLotPrice(lotId);
        }

        public void UpdateLotPrice(int id)
        {
            AuctionContext context = new AuctionContext();
            Lot findedLot;
            findedLot = context.Lots.Where(p => p.LotId == id).SingleOrDefault();
            if (findedLot == null) return;
            List<Bid> bids = context.Bids.Where(p => p.LotId == id).OrderByDescending(p => p.Price).ToList();
            if (bids.Count == 0) findedLot.CurrentPrice = 0;
            else
            {
                findedLot.CurrentPrice = bids[0].Price;
            }
            context.SaveChanges();

        }

        [Authorize]
        public ActionResult GetCategories(int? id)
        {
            AuctionContext auctionContext = new AuctionContext();
          List<Category> categories = auctionContext.Sections.Include(p => p.Categories).First(c => c.SectionId == id).Categories;
            return PartialView(categories);
        }

        [Authorize]
        public ActionResult GetBids(int? id)
        {
            AuctionContext auctionContext = new AuctionContext();
            List<BidDetails> bidsDetails = new List<BidDetails>();
            List<Bid> bids =  auctionContext.Bids.Include(p=>p.Lot).Where(p=>p.LotId==id).ToList();
            if (bids == null) bidsDetails = new List<BidDetails>();
            string OwnerName = default(string);
            if (bids.Count != 0)
            {
                string OwnerID = bids[0].Lot.UserId;
                ApplicationUser owner = UserManager.Users.Where(p => p.Id == OwnerID).SingleOrDefault();
                if (owner == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                OwnerName = owner.UserName;
            }
            foreach (var Bid in bids)
            {
                ApplicationUser user = UserManager.Users.Where(p => p.Id == Bid.User).SingleOrDefault();
                if (user == null) continue;
                bidsDetails.Add(new BidDetails() { bid = Bid, UserName = user.UserName, LotOwnerUserName = OwnerName, CurrentUserName = HttpContext.User.Identity.Name });
            }
            bidsDetails = bidsDetails.OrderByDescending(p => p.bid.Price).ToList();
            return PartialView(bidsDetails);
        }

        [HttpPost]
        public JsonResult ImageUpload()
        {
            string __filepath = Server.MapPath("~/uploads");
            int __maxSize = 2 * 1024 * 1024;    // максимальный размер файла 2 Мб
            // допустимые MIME-типы для файлов
            List<string> mimes = new List<string>
            {
                "image/jpeg", "image/jpg", "image/png"
            };

            var result = new Result
            {
                Files = new List<string>()
            };

            if (Request.Files.Count > 0)
            {
                foreach (string f in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[f];

                    // Выполнить проверки на допустимый размер файла и формат
                    if (file.ContentLength > __maxSize)
                    {
                        result.Error = "Размер файла не должен превышать 2 Мб";
                        break;
                    }
                    else if (mimes.FirstOrDefault(m => m == file.ContentType) == null)
                    {
                        result.Error = "Недопустимый формат файла";
                        break;
                    }

                    // Сохранить файл и вернуть URL
                    if (Directory.Exists(__filepath))
                    {
                        Guid guid = Guid.NewGuid();
                        if (GetUserImages().Count == 0)
                        {
                            file.SaveAs($@"{__filepath}\{User.Identity.GetUserId()}.{guid}.title.{file.FileName}");
                            result.Files.Add($"/uploads/{User.Identity.GetUserId()}.{guid}.title.{file.FileName}");
                        }
                        else
                        {
                            file.SaveAs($@"{__filepath}\{User.Identity.GetUserId()}.{guid}.{file.FileName}");
                            result.Files.Add($"/uploads/{User.Identity.GetUserId()}.{guid}.{file.FileName}");
                        }
                    }
                }
            }

            return Json(result);
        }


        [HttpPost]
        public JsonResult ImageDelete(string id)
        {
            string res = "no";
            id = id.Replace("/", "\\");
            string path = HttpContext.Server.MapPath("/uploads");
            string[] names = Directory.GetFiles(path);
            string finded = names.Where(p => p.Contains(id)).SingleOrDefault();
            if(finded!=default(string))
            {
                FileInfo file = new FileInfo(finded);
                file.Delete();
                res = "ok";
                return Json(res);
            }
            
            return Json(res);
        }

        public void DeleteUserImages()
        {
            string path = HttpContext.Server.MapPath("/uploads");
            string[] names = Directory.GetFiles(path);
            foreach (var name in names)
            {
                if (name.Contains(HttpContext.User.Identity.GetUserId()))
                {
                    FileInfo file = new FileInfo(name);
                    file.Delete();
                }
            }
        }

        //
        public List<string> GetUserImages()
        {
            string path = HttpContext.Server.MapPath("/uploads");
            string[] names = Directory.GetFiles(path);
            List<string> files = new List<string>();
            foreach (var name in names)
            {
                if (name.Contains(HttpContext.User.Identity.GetUserId()))
                {
                    files.Add(name);
                }
            }
            return files;
        }

        LotDetails LotToLotDetailsModel( Lot findedLot)
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
            lotDetails.PostPaymentAdditionalInformation = findedLot.LotPayment.AdditionalInformation;
            lotDetails.UserImagesID = FileApiMethods.GetFilesIDFromFolder(findedLot.ImagesUrl);
            lotDetails.Bids = findedLot.Bids;
            lotDetails.State = findedLot.State;
            return lotDetails;
        }

    }

   
    
    public class Result
    {
        public string Error { get; set; }
        public List<string> Files { get; set; }
    }
}

