using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SOOS_Auction.AuctionDatabase.Models;
using SOOS_Auction.AuctionGoogleDrive;
using SOOS_Auction.Models;
using SOOS_Auction.Models.ImagesRequests;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SOOS_Auction.Controllers
{
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

        public ActionResult Index()
        {
            AuctionContext auctionContext = new AuctionContext();
            List<Lot> lots = new List<Lot>();
            lots = auctionContext.Lots.Include(path=>path.Bids).Where(p=>p.State=="started").OrderBy(p => p.FinishDate).ToList();
            List<LotPreviewDetails> lotPreviewDetails = LotsToLotsPreview(lots);
            ViewBag.Title = "Все текущие лоты:";
            return View(lotPreviewDetails);
        }

        [HttpGet]
        public ActionResult Search(string searchText)
        {
            AuctionContext context = new AuctionContext();
            string[] splittedText = searchText.Split(' ');
            List<Lot> lots = new List<Lot>();
            List<Lot> databaseLots = context.Lots.Include(path=>path.Bids).Where(p => p.State == "started").ToList();
            foreach (var lot in databaseLots)
            {
                foreach (var item in splittedText)
                {
                    if (lot.Name.Contains(item)) { lots.Add(lot); break; }
                }
            }
            List<LotPreviewDetails> details = LotsToLotsPreview(lots);
            ViewBag.Title = "Результаты поиска по запросу: " + searchText;
            return View("Index", details);
        }

        [HttpGet]
        public ActionResult BySection(int sectionId)
        {
            AuctionContext auctionContext = new AuctionContext();
            Section section = auctionContext.Sections.Where(p => p.SectionId == sectionId).SingleOrDefault();
            if (section == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            List<Lot> lots = auctionContext.Lots.Include(path => path.Bids).Include(p => p.Category).Where(p => p.Category.SectionId == section.SectionId&&p.State=="started").ToList();
            List<LotPreviewDetails> details = LotsToLotsPreview(lots);
            ViewBag.Title = "Результаты поиска по разделу " + section.Name;
            return View("Index", details);
        }

        [HttpGet]
        public ActionResult ByCategory(int categoryId)
        {
            AuctionContext auctionContext = new AuctionContext();
            Category category = auctionContext.Categories.Where(p => p.CategoryId == categoryId).SingleOrDefault();
            if (category == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            List<Lot> lots = auctionContext.Lots.Include(path => path.Bids).Include(p => p.Category).Where(p => p.Category.CategoryId == category.CategoryId && p.State == "started").ToList();
            List<LotPreviewDetails> details = LotsToLotsPreview(lots);
            ViewBag.Title = "Результаты поиска по категории " + category.Name;
            return View("Index", details);
        }
        List<LotPreviewDetails> LotsToLotsPreview(List<Lot> lots)
        {
            List<LotPreviewDetails> lotPreviewDetails = new List<LotPreviewDetails>();
            foreach (var lot in lots)
            {
                LotPreviewDetails detail = new LotPreviewDetails();
                detail.LotId = lot.LotId;
                detail.CurrentPrice = lot.CurrentPrice;
                detail.BidsCount = lot.Bids.Count();
                detail.FinishDate = lot.FinishDate.ToString("D");
                detail.Location = lot.Location;
                detail.Name = lot.Name;
                if (lot.State == "started") { detail.State = "Торги начались"; }
                else if (lot.State == "pending") { detail.State = "Ожидает подтверждения"; }
                else if (lot.State == "finished") { detail.State = "Торги завершены"; }
                else if (lot.State == "rejected") { detail.State = "Отклонен"; }
                if (lot.isPaymentBySite)
                {
                    detail.Payment = "C помощью сайта";
                }
                else detail.Payment = "По договоренности";
                List<string> images = FileApiMethods.GetFilesIDFromFolder(lot.ImagesUrl);
                if (images.Count == 0) detail.ImageUrl = "1R65ppqtbBGs3CJMKRDL8Mb5cA1WpA1-y";
                else
                {
                    detail.ImageUrl = images.Last();
                }
                lotPreviewDetails.Add(detail);
            }
            return lotPreviewDetails;
        }

        // GET: Lot/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Lot findedLot;
            AuctionContext context = new AuctionContext();

            findedLot = context.Lots.Include(p=>p.Category).Include(p=>p.Bids).Include(p=>p.Category).Include(p=>p.LotPayment).Include(p=>p.LotReceiving).Include(p=>p.Bids).Where(p => p.LotId == id).SingleOrDefault();
            if(findedLot==null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if(findedLot.State=="pending"||findedLot.State=="rejected")
            {
                if (HttpContext.User.IsInRole("admin")||HttpContext.User.IsInRole("moder"))
                {
                    return RedirectToAction("ModerLotDetails", "AuctionManager", new { id = findedLot.LotId });
                }
                else return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);

            }

            LotDetails lotDetails = LotToLotDetailsModel(findedLot);
            if (lotDetails == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ViewBag.Title = lotDetails.Name;
            return View(lotDetails);
        }


        [Authorize]
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

        [Authorize]
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

            Lot newLot = new Lot() { Name = model.Name, CategoryId = model.CategoryId, State = "pending", CurrentPrice = 0, MinimalPrice = model.MinimalPrice, MinimalStep = model.MinimalStep, Description = model.Description, DaysDuration = model.DaysDuration, UserId = HttpContext.User.Identity.GetUserId(), isPaymentBySite=model.isPaymentBySite, StartDate=DateTime.Now,FinishDate=DateTime.Now, Location=model.Location };
            LotReceiving newReceiving = new LotReceiving() { ByPost = model.ByPost, ByPostToAnotherCountry = model.ByPostToAnotherCountry, DeliveryInPerson = model.DeliveryInPerson, Location = model.Location, ReturnAfterBuyingIsForbidden = model.ReturnAfterBuyingIsForbidden, AdditionalInformation = model.AdditionalInformation };

            LotPayment newPayment;
            if(newLot.isPaymentBySite)
                newPayment = new LotPayment() { Cash =false, NonCash = false, FullPrepaymentPostSending = false };
            else
            {
                newPayment = new LotPayment() { Cash = model.Cash, NonCash = model.NonCash, FullPrepaymentPostSending = model.FullPrepaymentPostSending };
            }
            auctionContext.LotsReceivings.Add(newReceiving);
            auctionContext.SaveChanges();
            auctionContext.LotPayments.Add(newPayment);
            auctionContext.SaveChanges();

            newLot.LotReceiving = newReceiving;
            newLot.LotPayment = newPayment;
            auctionContext.Lots.Add(newLot);
            auctionContext.SaveChanges();
            //
            string newFolderId = FileApiMethods.CreateFolder(newLot.LotId);
            FileApiMethods.MakeFilePublic(newFolderId);
            newLot.ImagesUrl = newFolderId;
            auctionContext.SaveChanges();
            var outer = Task.Factory.StartNew(() =>      
            {
                UploadImages(newFolderId);
                
            });
            return RedirectToAction("Index", "Lot");
            //image uploading
            
            
            //image uploading
        }

        [Authorize]
        public void UploadImages(string folderId)
        {
            List<string> files = GetUserImages();
            if (files.Count != 0)
            {
                foreach (var file in files)
                {
                    string mimeType = MimeMapping.GetMimeMapping(file);
                    FileApiMethods.Upload(file, file, mimeType, folderId);
                }

            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult MakeBid(string newBid, int lotId)
        {
            AuctionContext context = new AuctionContext();
            NewBidResult newBidResult = new NewBidResult();
            if (!HttpContext.User.Identity.IsAuthenticated) { newBidResult.bidErrors.Add("authError"); return Json(newBidResult); }

            double newBidPrice;
            try
            {
                newBidPrice = (float)Convert.ToDouble(newBid);
            }
            catch(FormatException e) { newBidResult.bidErrors.Add("Введите корректную цену ставки!"); return Json(newBidResult); }
            List<Bid> bids = context.Bids.Where(p => p.LotId == lotId).OrderByDescending(p => p.Price).ToList();
            if (bids.Count != 0)
            {
                string bidUserID = bids.First().User;
                if (HttpContext.User.Identity.GetUserId() == bidUserID)
                {
                    newBidResult.bidErrors.Add("Вы не можете сделать ставку, т. к. предыдущая ставка этого лота принадлежит вам!"); return Json(newBidResult);
                }
            }
            Lot findedLot;
            findedLot = context.Lots.Include(path=>path.Bids).Where(p => p.LotId == lotId).SingleOrDefault();
            if (findedLot == null) {
                newBidResult.bidErrors.Add("Лот не существует либо удален. Обновите страницу."); return Json(newBidResult);
            };
            if (findedLot.State!="started") { newBidResult.bidErrors.Add("Вы не можете сделать ставку на лот, торги на который не идут!"); return Json(newBidResult); }

                if (findedLot.CurrentPrice + findedLot.MinimalStep > newBidPrice)
            {
                newBidResult.bidErrors.Add("Ставка должна быть больше либо равна суммы текущей цены и минимального шага!"); return Json(newBidResult);
            }
            if(findedLot.CurrentPrice==0 && newBidPrice < findedLot.MinimalPrice)
            {
                newBidResult.bidErrors.Add("Ставка должна быть больше или равна минимальной цене лота!"); return Json(newBidResult);
            }
            string currentUser = HttpContext.User.Identity.GetUserId();
            ApplicationUser winner = UserManager.Users.Where(p => p.Id == currentUser).SingleOrDefault();
            if (winner == null)
            {
                newBidResult.bidErrors.Add("Потенциальный победитель не существует или удален!"); return Json(newBidResult);
            }
            if(findedLot.isPaymentBySite)
            {
                int userBidsCount = default(int);

                    userBidsCount = findedLot.Bids.Where(p => p.User == winner.Id).Count();
                if (userBidsCount != 0)
                {
                    double maxPrice = findedLot.Bids.Where(p => p.User == winner.Id).Select(p => p.Price).Max();
                    if (winner.Balance - winner.BusyBalance < newBidPrice - maxPrice)
                    {
                        newBidResult.bidErrors.Add("У вас недостаточно денег на балансе для этой ставки!"); return Json(newBidResult);
                    }
                    winner.BusyBalance += (newBidPrice - maxPrice);
                    UserManager.Update(winner);

                }
                else
                {
                    if (winner.Balance < newBidPrice)
                    {
                        newBidResult.bidErrors.Add("У вас недостаточно денег на балансе для этой ставки!"); return Json(newBidResult);
                    }
                    winner.BusyBalance += newBidPrice;
                    UserManager.Update(winner);

                }
            }
            Bid newbid = new Bid() { LotId = findedLot.LotId, Price = newBidPrice, BidDate = DateTime.Now, User = HttpContext.User.Identity.GetUserId() };
            context.Bids.Add(newbid);
            
            context.SaveChanges();
            UpdateLotPrice(lotId);
            context = new AuctionContext();
            findedLot = context.Lots.Where(p => p.LotId == lotId).SingleOrDefault();
            if (findedLot == null)
            {
                newBidResult.bidErrors.Add("Лот не существует либо удален. Обновите страницу."); return Json(newBidResult);
            }
            newBidResult.WinnerId = findedLot.WinnerId;
            
            newBidResult.WinnerName = winner.UserName;
            newBidResult.isSuccess = true;
            return Json(newBidResult);
        }

        [HttpPost]
        public JsonResult UpdateLotData(int lotId)
        {
            AuctionContext context = new AuctionContext();
            LotData lotData = new LotData();
            Lot findedLot;
            findedLot = context.Lots.Where(p => p.LotId == lotId).SingleOrDefault();
            if (findedLot == null)
            {
                lotData.isSuccess = false; return Json(lotData);
            };
            if (findedLot.CurrentPrice != 0)
            {
                lotData.newLotPrice = findedLot.CurrentPrice.ToString("F");
                lotData.newPlaceHolder = (findedLot.CurrentPrice + findedLot.MinimalStep).ToString("F");
                lotData.WinnerId = findedLot.WinnerId;
                ApplicationUser winner = UserManager.Users.Where(p => p.Id == lotData.WinnerId).SingleOrDefault();
                if (winner == null)
                {
                    lotData.isSuccess = false; return Json(lotData);
                }
                lotData.WinnerName = winner.UserName;
            }
            else
            {
                lotData.newLotPrice = findedLot.CurrentPrice.ToString("F");
                lotData.newPlaceHolder = findedLot.MinimalPrice.ToString("F");
            }
            lotData.isSuccess = true;
            return Json(lotData);
        }

        [HttpPost]
        public JsonResult FinishLot(int lotId)
        {
            AuctionContext context = new AuctionContext();
            Lot findedLot;
            findedLot = context.Lots.Where(p => p.LotId == lotId).SingleOrDefault();
            if (findedLot == null)
            {
               return Json("no");
            };
            if (findedLot.State == "started")
            {
                findedLot.State = "finished";
                if (findedLot.isPaymentBySite)
                {
                    List<Bid> allLotBids = findedLot.Bids.OrderByDescending(p=>p.Price).ToList();
                    if (allLotBids.Count != 0)
                    {
                        ApplicationUser lotOwner = UserManager.FindById(findedLot.UserId);
                        lotOwner.Balance += (allLotBids[0].Price * 0.97);
                        ApplicationUser winner = UserManager.FindById(allLotBids[0].User);
                        winner.BusyBalance -= allLotBids[0].Price;
                        winner.Balance -= allLotBids[0].Price;
                        UserManager.Update(lotOwner);
                        UserManager.Update(winner);
                    }
                }
                context.SaveChanges();
                return Json("ok");
            }
            else if(findedLot.State=="finished") { return Json("finished"); }
                return Json("no");
        }

        [HttpGet]
        public JsonResult UpdateWinnerInfo (int lotId)
        {
            WinnerInfo info = new WinnerInfo();
            AuctionContext context = new AuctionContext();
           Lot findedLot = context.Lots.Where(p => p.LotId == lotId).SingleOrDefault();
            if (findedLot == null)
            {
                info.isSuccess = false; return Json(info);
            }
            info.WinnerId = findedLot.WinnerId;
            ApplicationUser winner = UserManager.Users.Where(p => p.Id == info.WinnerId).SingleOrDefault();
            if (winner == null)
            {
                info.isSuccess = false;  return Json(info);
            }
            info.WinnerName = winner.UserName;
            info.isSuccess = true;
            return Json(info);
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
                findedLot.WinnerId = bids[0].User;
            }
            context.SaveChanges();

        }


        public ActionResult GetCategories(int? id)
        {
            AuctionContext auctionContext = new AuctionContext();
          List<Category> categories = auctionContext.Sections.Include(p => p.Categories).First(c => c.SectionId == id).Categories;
            return PartialView(categories);
        }

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
                bidsDetails.Add(new BidDetails() { bid = Bid, UserName = user.UserName, UserId=user.Id,LotOwnerUserName = OwnerName, CurrentUserName = HttpContext.User.Identity.Name, LotState = Bid.Lot.State });
                
            }
            bidsDetails = bidsDetails.OrderByDescending(p => p.bid.Price).ToList();
            return PartialView(bidsDetails);
        }

        [HttpGet]
        public ActionResult ManageGetLots(string userId, string state)
        {
            AuctionContext auctionContext = new AuctionContext();
            ApplicationUser user = UserManager.FindById(userId);
            if (user == null) return PartialView(new List<LotPreviewDetails>());
            List<Lot> lots;
            List<LotPreviewDetails> details = new List<LotPreviewDetails>();
            if (state == "userStarted")
            {
                lots = auctionContext.Lots.Include(path => path.Bids).Where(p => p.UserId == user.Id && p.State == "started").OrderBy(p => p.FinishDate).ToList();
                foreach (var lot in lots)
                {
                    LotPreviewDetails detail = new LotPreviewDetails();
                    detail.LotId = lot.LotId;
                    detail.CurrentPrice = lot.CurrentPrice;
                    detail.BidsCount = lot.Bids.Count();
                    detail.FinishDate = lot.FinishDate.ToString("D");
                    detail.Location = lot.Location;
                    detail.Name = lot.Name;
                    if (lot.State == "started") { detail.State = "Торги начались"; }
                    else if (lot.State == "pending") { detail.State = "Ожидает подтверждения"; }
                    else if (lot.State =="finished") { detail.State = "Торги завершены"; }
                    else if (lot.State =="rejected") { detail.State = "Отклонен"; }
                    if (lot.isPaymentBySite)
                    {
                        detail.Payment = "C помощью сайта";
                    }
                    else detail.Payment = "По договоренности";
                    List<string> images = FileApiMethods.GetFilesIDFromFolder(lot.ImagesUrl);
                    if (images.Count == 0) detail.ImageUrl = "1R65ppqtbBGs3CJMKRDL8Mb5cA1WpA1-y";
                    else
                    {
                        detail.ImageUrl = images.First();
                    }
                    details.Add(detail);
                }
            }
            else if (state == "userPending")
            {
                lots = auctionContext.Lots.Include(path => path.Bids).Where(p => p.UserId == user.Id && p.State == "pending").OrderBy(p => p.FinishDate).ToList();
                foreach (var lot in lots)
                {
                    LotPreviewDetails detail = new LotPreviewDetails();
                    detail.LotId = lot.LotId;
                    detail.CurrentPrice = lot.CurrentPrice;
                    detail.BidsCount = lot.Bids.Count();
                    detail.FinishDate = lot.FinishDate.ToString("D");
                    detail.Location = lot.Location;
                    detail.Name = lot.Name;
                    if (lot.State == "started") { detail.State = "Торги начались"; }
                    else if (lot.State == "pending") { detail.State = "Ожидает подтверждения"; }
                    else if (lot.State == "finished") { detail.State = "Торги завершены"; }
                    else if (lot.State == "rejected") { detail.State = "Отклонен"; }
                    if (lot.isPaymentBySite)
                    {
                        detail.Payment = "C помощью сайта";
                    }
                    else detail.Payment = "По договоренности";
                    List<string> images = FileApiMethods.GetFilesIDFromFolder(lot.ImagesUrl);
                    if (images.Count == 0) detail.ImageUrl = "1R65ppqtbBGs3CJMKRDL8Mb5cA1WpA1-y";
                    else
                    {
                        detail.ImageUrl = images.First();
                    }
                    details.Add(detail);
                }
            }
            else if (state == "userFinished")
            {
                lots = auctionContext.Lots.Include(path => path.Bids).Where(p => p.UserId == user.Id && p.State == "finished").OrderBy(p => p.FinishDate).ToList();
                foreach (var lot in lots)
                {
                    LotPreviewDetails detail = new LotPreviewDetails();
                    detail.LotId = lot.LotId;
                    detail.CurrentPrice = lot.CurrentPrice;
                    detail.BidsCount = lot.Bids.Count();
                    detail.FinishDate = lot.FinishDate.ToString("D");
                    detail.Location = lot.Location;
                    detail.Name = lot.Name;
                    if (lot.State == "started") { detail.State = "Торги начались"; }
                    else if (lot.State == "pending") { detail.State = "Ожидает подтверждения"; }
                    else if (lot.State == "finished") { detail.State = "Торги завершены"; }
                    else if (lot.State == "rejected") { detail.State = "Отклонен"; }
                    if (lot.isPaymentBySite)
                    {
                        detail.Payment = "C помощью сайта";
                    }
                    else detail.Payment = "По договоренности";
                    List<string> images = FileApiMethods.GetFilesIDFromFolder(lot.ImagesUrl);
                    if (images.Count == 0) detail.ImageUrl = "1R65ppqtbBGs3CJMKRDL8Mb5cA1WpA1-y";
                    else
                    {
                        detail.ImageUrl = images.First();
                    }
                    details.Add(detail);
                }
            }
            else if (state == "userParticipating")
            {
                lots = auctionContext.Lots.Include(path => path.Bids).Where(p => p.UserId != user.Id && p.State == "started").OrderBy(p => p.FinishDate).ToList();
                List<Lot> participatingLots = new List<Lot>();
                foreach (var lot in lots)
                {
                    List<Bid> bids = lot.Bids.ToList();
                    int count = bids.Where(p => p.User == user.Id).Count();
                    if (count != 0) participatingLots.Add(lot);
                }
                foreach (var lot in participatingLots)
                {
                    LotPreviewDetails detail = new LotPreviewDetails();
                    detail.LotId = lot.LotId;
                    detail.CurrentPrice = lot.CurrentPrice;
                    detail.BidsCount = lot.Bids.Count();
                    detail.FinishDate = lot.FinishDate.ToString("D");
                    detail.Location = lot.Location;
                    detail.Name = lot.Name;
                    if (lot.State == "started") { detail.State = "Торги начались"; }
                    else if (lot.State == "pending") { detail.State = "Ожидает подтверждения"; }
                    else if (lot.State == "finished") { detail.State = "Торги завершены"; }
                    else if (lot.State == "rejected") { detail.State = "Отклонен"; }
                    if (lot.isPaymentBySite)
                    {
                        detail.Payment = "C помощью сайта";
                    }
                    else detail.Payment = "По договоренности";
                    List<string> images = FileApiMethods.GetFilesIDFromFolder(lot.ImagesUrl);
                    if (images.Count == 0) detail.ImageUrl = "1R65ppqtbBGs3CJMKRDL8Mb5cA1WpA1-y";
                    else
                    {
                        detail.ImageUrl = images.First();
                    }
                    details.Add(detail);
                }
            }
            else if (state == "userWon")
            {
                lots = auctionContext.Lots.Include(path => path.Bids).Where(p => p.WinnerId == user.Id && p.State == "finished").OrderBy(p => p.FinishDate).ToList();
                foreach (var lot in lots)
                {
                    LotPreviewDetails detail = new LotPreviewDetails();
                    detail.LotId = lot.LotId;
                    detail.CurrentPrice = lot.CurrentPrice;
                    detail.BidsCount = lot.Bids.Count();
                    detail.FinishDate = lot.FinishDate.ToString("D");
                    detail.Location = lot.Location;
                    detail.Name = lot.Name;
                    if (lot.State == "started") { detail.State = "Торги начались"; }
                    else if (lot.State == "pending") { detail.State = "Ожидает подтверждения"; }
                    else if (lot.State == "finished") { detail.State = "Торги завершены"; }
                    else if (lot.State == "rejected") { detail.State = "Отклонен"; }
                    if (lot.isPaymentBySite)
                    {
                        detail.Payment = "C помощью сайта";
                    }
                    else detail.Payment = "По договоренности";
                    List<string> images = FileApiMethods.GetFilesIDFromFolder(lot.ImagesUrl);
                    if (images.Count == 0) detail.ImageUrl = "1R65ppqtbBGs3CJMKRDL8Mb5cA1WpA1-y";
                    else
                    {
                        detail.ImageUrl = images.First();
                    }
                    details.Add(detail);
                }
            }
            else if (state == "user")
            {
                lots = auctionContext.Lots.Include(path => path.Bids).Where(p => p.UserId == user.Id).OrderBy(p => p.FinishDate).ToList();
                foreach (var lot in lots)
                {
                    LotPreviewDetails detail = new LotPreviewDetails();
                    detail.LotId = lot.LotId;
                    detail.CurrentPrice = lot.CurrentPrice;
                    detail.BidsCount = lot.Bids.Count();
                    detail.FinishDate = lot.FinishDate.ToString("D");
                    detail.Location = lot.Location;
                    detail.Name = lot.Name;
                    if (lot.State == "started") { detail.State = "Торги начались"; }
                    else if (lot.State == "pending") { detail.State = "Ожидает подтверждения"; }
                    else if (lot.State == "finished") { detail.State = "Торги завершены"; }
                    else if (lot.State == "rejected") { detail.State = "Отклонен"; }
                    if (lot.isPaymentBySite)
                    {
                        detail.Payment = "C помощью сайта";
                    }
                    else detail.Payment = "По договоренности";
                    List<string> images = FileApiMethods.GetFilesIDFromFolder(lot.ImagesUrl);
                    if (images.Count == 0) detail.ImageUrl = "1R65ppqtbBGs3CJMKRDL8Mb5cA1WpA1-y";
                    else
                    {
                        detail.ImageUrl = images.First();
                    }
                    details.Add(detail);
                }
            }
            return PartialView(details);
        }

        [HttpPost]
        [Authorize]
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
        [Authorize]
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

        [HttpPost]
        [Authorize]
        public JsonResult BidDelete(string id)
        {
            string res = "no";
            int BidId = default(int);
            try
            {
                BidId = Convert.ToInt32(id);
            }
            catch(Exception e) { return Json(res); }
            AuctionContext auctionContext = new AuctionContext();
            Bid bid = auctionContext.Bids.Include(p => p.Lot).Where(p => p.BidId == BidId).SingleOrDefault();
            if (bid == null) { return Json(res); }
            string lotOwnerId = bid.Lot.UserId;
            ApplicationUser lotOwner = UserManager.Users.Where(p => p.Id == lotOwnerId).SingleOrDefault();
            if (lotOwner == null) return Json(res);
            if (lotOwner.UserName == HttpContext.User.Identity.Name&&bid.Lot.State=="started")
            {
                if (bid.Lot.isPaymentBySite)
                {
                    ApplicationUser bidder = UserManager.FindById(bid.User);
                    Lot lot = auctionContext.Lots.Include(p => p.Bids).Where(p => p.LotId == bid.LotId).SingleOrDefault();
                    if (lot == null) return Json("no");
                    double bidPrice = bid.Price;
                    int bidsCount = lot.Bids.Where(p => p.User== bid.User).Count();
                    if (bidsCount == 1)
                    {
                        bidder.BusyBalance -= bid.Price;
                        UserManager.Update(bidder);
                    }
                    else
                    {
                        double nextMaxPrice = lot.Bids.Where(p => p.Price != bid.Price && p.User==bidder.Id).Select(p => p.Price).Max();
                        bidder.BusyBalance -= bid.Price;
                        bidder.BusyBalance += nextMaxPrice;
                        UserManager.Update(bidder);
                    }
                }
                auctionContext.Bids.Remove(bid);
                auctionContext.SaveChanges();
                UpdateLotPrice(bid.LotId);
                res = "ok";
                return Json(res);
            }

            return Json(res);
        }

        [Authorize]
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

        [Authorize]
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
            lotDetails.CategoryId = currentCategory.CategoryId;
            lotDetails.SectionId = currentCategory.Section.SectionId;
            lotDetails.Location = findedLot.LotReceiving.Location;
            lotDetails.ByPost = findedLot.LotReceiving.ByPost;
            lotDetails.DeliveryInPerson = findedLot.LotReceiving.DeliveryInPerson;
            lotDetails.ByPostToAnotherCountry = findedLot.LotReceiving.ByPostToAnotherCountry;
            lotDetails.ReturnAfterBuyingIsForbidden = findedLot.LotReceiving.ReturnAfterBuyingIsForbidden;
            lotDetails.Cash = findedLot.LotPayment.Cash;
            lotDetails.NonCash = findedLot.LotPayment.NonCash;
            lotDetails.FullPrepaymentPostSending = findedLot.LotPayment.FullPrepaymentPostSending;
            lotDetails.PostPaymentAdditionalInformation = findedLot.LotReceiving.AdditionalInformation;
            lotDetails.UserImagesID = FileApiMethods.GetFilesIDFromFolder(findedLot.ImagesUrl);
            if (lotDetails.UserImagesID == null) return null;
            lotDetails.Bids = findedLot.Bids;
            lotDetails.State = findedLot.State;
            lotDetails.WinnerId = findedLot.WinnerId;
            lotDetails.isPaymentBySite = findedLot.isPaymentBySite;
            lotDetails.Location = findedLot.Location;
            ApplicationUser winner = UserManager.Users.Where(p => p.Id == findedLot.WinnerId).SingleOrDefault();
            if (winner != null)
            {
                lotDetails.WinnerName = winner.UserName;
            }
            return lotDetails;
        }

    }

}

