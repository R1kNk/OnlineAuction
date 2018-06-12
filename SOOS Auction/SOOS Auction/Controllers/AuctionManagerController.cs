using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using SOOS_Auction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SOOS_Auction.Controllers
{
    [Authorize(Roles ="admin,moder")]
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

        public ActionResult Index()
        {
            return View();
        }

        // GET: AuctionManager
        public ActionResult Users()
        {
            string UserID = HttpContext.User.Identity.GetUserId();
            List<ApplicationUser> users = UserManager.Users.Where(p => p.Id != UserID).ToList();
            //users[0].UserName = "123lul";
            return View(users);
        }

        // GET: AuctionManager/Details/mail?=...
        public ActionResult UserDetails(string mail)
        {
            ApplicationUser appUser = new ApplicationUser();
            if(mail==null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            appUser = UserManager.FindByEmail(mail);
            
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
        public ActionResult UserEdit(string mail)
        {
            ApplicationUser appUser = new ApplicationUser();
            if (mail == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            appUser = UserManager.FindByEmail(mail);

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
        public ActionResult UserDelete(string mail)
        {
            if (mail == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = UserManager.FindByEmail(mail);

            if (user == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

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

    }
}
