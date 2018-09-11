using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SOOS_Auction.AuctionDatabase.Models;
using SOOS_Auction.AuctionGoogleDrive;
using SOOS_Auction.Models;

namespace SOOS_Auction.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
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

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
         
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager{ get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); private set =>_signInManager = value;  }

        public ApplicationUserManager UserManager { get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); private set =>_userManager = value; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public JsonResult IsLogged()
        {
            if (HttpContext.User.Identity.IsAuthenticated) return Json("ok", JsonRequestBehavior.AllowGet);
            return Json("no", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public JsonResult UpdateBalance()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            return Json("Свободный баланс: "+user.UnBusyBalance+" бел. руб.", JsonRequestBehavior.AllowGet);
           
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index","Lot");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Locations = Locations;
            return View();
        }

        [HttpPost]
        [Authorize]
        public JsonResult UploadAvatar()
        {
            string __filepath = Server.MapPath("~/uploads");
            int __maxSize = 2 * 1024 * 1024;    // максимальный размер файла 2 Мб
            // допустимые MIME-типы для файлов
            List<string> mimes = new List<string>
            {
                "image/jpeg", "image/jpg", "image/png"
            };

            ChangeAvatarModel result = new ChangeAvatarModel();

            if (Request.Files.Count > 0)
            {
                foreach (string f in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[f];

                    // Выполнить проверки на допустимый размер файла и формат
                    if (file.ContentLength > __maxSize)
                    {
                        result.Error = "Размер файла не должен превышать 2 Мб";
                        result.IsSuccess = false;
                        return Json(result);
                    }
                    else if (mimes.FirstOrDefault(m => m == file.ContentType) == null)
                    {
                        result.Error = "Недопустимый формат файла";
                        result.IsSuccess = false;
                        return Json(result);
                    }

                    // Сохранить файл и вернуть URL
                    if (Directory.Exists(__filepath))
                    {
                        Guid guid = Guid.NewGuid();
                            file.SaveAs($@"{__filepath}\{User.Identity.GetUserId()}.{guid}.title.avatar");
                        string path = $@"{__filepath}\{User.Identity.GetUserId()}.{guid}.title.avatar";
                        string mimeType = MimeMapping.GetMimeMapping(path);
                       string urlId = FileApiMethods.Upload(path, path, mimeType);
                        FileApiMethods.MakeFilePublic(urlId);
                        result.AvatarUrl = urlId;
                        result.IsSuccess = true;
                        ApplicationUser current = UserManager.FindById(HttpContext.User.Identity.GetUserId());
                        current.AvatarUrl = result.AvatarUrl;
                        UserManager.Update(current);
                        return Json(result);
                    }
                }
            }

            return Json(result);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, TelephoneNumber = model.TelephoneNumber, Gender = model.Gender, UserLocation = model.UserLocation, PositiveReview = 0, NegativeReview = 0, Balance=0.0, BusyBalance=0.0 };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "user");
                    user.AvatarUrl = "1R65ppqtbBGs3CJMKRDL8Mb5cA1WpA1-y";
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            ViewBag.Locations = Locations;
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddBalance(string userId, string addBalance)
        {
            AddBalanceModel newAddBalanceResult = new AddBalanceModel();
            if (!HttpContext.User.Identity.IsAuthenticated) { newAddBalanceResult.Error = "authError"; return Json(newAddBalanceResult); }

            double newAddBalance;
            try
            {
                newAddBalance = (float)Convert.ToDouble(addBalance);
            }
            catch (FormatException e) { newAddBalanceResult.Error = "Введите корректную величину пополнения!"; return Json(newAddBalanceResult); }
            if (newAddBalance <= 0) { newAddBalanceResult.Error = "Величина пополнения должна быть больше нуля!"; return Json(newAddBalanceResult); }
            ApplicationUser thisUser = UserManager.FindById(HttpContext.User.Identity.GetUserId());
            thisUser.Balance += newAddBalance;
            UserManager.Update(thisUser);
            newAddBalanceResult.isSuccess = true;
            newAddBalanceResult.Balance = Math.Round(thisUser.Balance,2).ToString() + " бел. руб.";
            newAddBalanceResult.BusyBalance = Math.Round(thisUser.BusyBalance,2).ToString() + " бел. руб.";
            newAddBalanceResult.FreeBalance = thisUser.UnBusyBalance.ToString() + " бел. руб.";
            return Json(newAddBalanceResult);
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddReview(string userId, string newReview, bool isPositive)
        {
            AddReviewModel addReviewResponse = new AddReviewModel();
            addReviewResponse.IsSuccess = false;
            if (!HttpContext.User.Identity.IsAuthenticated) { addReviewResponse.Error = "authError"; addReviewResponse.IsSuccess = false; return Json(addReviewResponse); }
            if (newReview == null||newReview=="") { addReviewResponse.Error = "Поле отзыва должно быть заполнено"; addReviewResponse.IsSuccess = false; return Json(addReviewResponse); }
            UserReview review = new UserReview();
            review.date = DateTime.Now;
            if (isPositive) { review.isNegative = false; review.isPositive = true; }
            else { review.isNegative = true; review.isPositive = false; }
            review.State = "pending";
            review.Text = newReview;
            ApplicationUser reviewedUser = UserManager.FindById(userId);
            if(reviewedUser==null) { addReviewResponse.Error = "Такого пользователя не существует!"; addReviewResponse.IsSuccess = false; return Json(addReviewResponse); }
            review.UserId = reviewedUser.Id;
            review.UserIdFrom = HttpContext.User.Identity.GetUserId();
            review.Review = "";
            AuctionContext context = new AuctionContext();
            List<UserReview> userReviews = context.Reviews.ToList();
            context.Entry(review).State = EntityState.Added;
            context.SaveChanges();
            addReviewResponse.IsSuccess = true;
            return Json(addReviewResponse);
        }

        public ActionResult ViewProfile(string userId)
        {
            ApplicationUser profileUser = UserManager.Users.Where(p => p.Id == userId).SingleOrDefault();
            if (profileUser == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if (profileUser.Id == HttpContext.User.Identity.GetUserId()) return RedirectToAction("Index", "Manage");
            var model = new IndexViewModel
            {
               
                UserName = profileUser.UserName,
                Gender = profileUser.Gender,
                AvatarUrl = profileUser.AvatarUrl,
                UserLocation = profileUser.UserLocation,
                TelephoneNumber = profileUser.TelephoneNumber,
                PositiveReview = profileUser.PositiveReview,
                NegativeReview = profileUser.NegativeReview,
                Email = profileUser.Email,
                UserId = profileUser.Id
            };
            return View(model);
        }

        

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}