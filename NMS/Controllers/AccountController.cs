using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using NMS.Models;
using NMS.Tools;
using System.Net;
using System.Data.Entity;
using System.IO;
using NMS.Data;
using NMS.Core.Resources;

namespace NMS.Controllers
{

    public class AccountController : Controller
    {
        /// <summary>
        /// test 13-02-Night
        /// //
        /// </summary>
        /// 
       
        /*******/
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();
        private NaalaxAuthEntities2 dbx = new NaalaxAuthEntities2();
        private ApplicationDbContext dbcx = new ApplicationDbContext();
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }
    

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(ManageMessageId? message)
       {
            //  if (Request.IsAuthenticated)
            //  {
            //   Session["Culture"] = "Ar";






            Session["Layout"] = "~/Views/Shared/_Layout.cshtml";
                Session["Role"] = null;

                ViewBag.Lang = new SelectList(

                        new List<SelectListItem>
                        {
                    new SelectListItem { Text = CommonRes.Arabic , Value = "Ar" },
                    new SelectListItem { Text = CommonRes.English , Value = "En" },

                        }, "Value", "Text");

                ViewBag.StatusMessage =
                      message == ManageMessageId.Error ? "Invalid Username or Password" : "";
                return View();

          //  }
            //else{

            //    Session["Layout"] = "~/Views/Shared/_Layout.cshtml";

            //    return RedirectToAction("AccessDenied", "Account");
            //}
          
         
        }   
        
        //
        public ActionResult DesignH2()
        {

            return View();
        }
        [HttpGet]
        public ActionResult AccessDenied()
        {

            return View();
        }
        public ActionResult Exception()
        {

            return View();
        }
        //
        public ActionResult Index()
        {
            var items = dbx.AspNetUsers;
            return View(items.ToList());
        }
        //
        //public ActionResult Edit(string Id)
        //{
        //    if (Id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var userid = dbx.AspNetUsers.Find(Id); 
        //    if (userid == null)
        //    {
        //        return HttpNotFound();
        //    }
            
        //    ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", userid.WareHouse_ID);
       

          

        //    return View(userid);
        //}
        ////
        //public ActionResult Edit(AspNetUser asp)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        AspNetUser tem = dbx.AspNetUsers.Where(i => i.UserName == asp.UserName).SingleOrDefault();
                
        //        db.Entry(tem).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
          
        //    ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", asp.WareHouse_ID);
          

        //    return View(asp);
        //}
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    Session[SessionIndex.CurrentUser] = user;
                    Session["Name"] = user.UserName;
                    Session["Branch_ID"] = user.Branch_ID;
                    Session["WareHouse_ID"] = user.WareHouse_ID;
                    //  string local="AccessDenied";
                    Session[SessionIndex.CurrentUserName] = user.UserName;
                    Session[SessionIndex.CurrentUserId] = user.Id;
                       AspNetUser user2 = dbx.AspNetUsers.SingleOrDefault(o => o.UserName == model.UserName);
                       if (user2.AspNetRoles.Any())
                       {
                       
                        Session["Role"] = user2.AspNetRoles.FirstOrDefault().Name;
                       // return RedirectToLocal(returnUrl);
                    }
                       
                
                    //
                    Session["Culture"] = model.langage;// = "En";
                    Session["Layout"] = "~/Views/Shared/_Layout.cshtml";//-En
                    await SignInAsync(user, model.RememberMe);

                    //
                    return RedirectToAction("Index", "Home");
                }
                else
                {


                    ModelState.AddModelError("", "Invalid username or password.");
                    return RedirectToAction("Login", new { Message = ManageMessageId.Error });

                }
            }

            // If we got this far, something failed, redisplay form

            ModelState.AddModelError("", "Invalid username or password.");
            return RedirectToAction("Login", new { Message = ManageMessageId.Error });


        }
        //

        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Branch_ID = new SelectList(db.Branches.Where(i => i.Status == "Active"), "Branch_ID", "Branch_Name_AR");
            ViewBag.Warhouse_ID = new SelectList(db.WareHouses.Where(i => i.Status == "Active"), "Warehouse_ID", "Name");

            ViewBag.Roles = new SelectList(dbcx.Roles.ToList(), "Name", "Name");
            return View("Register");
        }
        //
        //EngRegister
       
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {

           //if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName, Branch_ID=model.Branch_ID ,WareHouse_ID=model.Warhouse_ID};
                
                var result = await UserManager.CreateAsync(user, model.Password);
                   Session["Branch_ID"] = user.Branch_ID;
                //var bran = new Branch();
             //  var aspbranch = dbx..Where(i => i.== model.Branch_ID).SingleOrDefault();

           //    db.Entry(aspbranch).State = EntityState.Modified;
           //   db.SaveChanges();
                if (result.Succeeded)
                {
                    //
                    await this.UserManager.AddToRolesAsync(user.Id, model.Roles);
                    //
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            ViewBag.Branch_ID = new SelectList(db.Branches.Where(i => i.Status == "Active"), "Branch_ID", "Branch_Name_AR",model.Branch_ID);
            ViewBag.Warhouse_ID = new SelectList(db.WareHouses.Where(i => i.Status == "Active"), "Warehouse_ID", "Name");

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        //
        [HttpPost]
     
        //checked if Eng Applied/
     

        //checked UserLoginInfo Name  //
        public JsonResult ChekUsername(string Username)
        {
            try
            {
                ApplicationDbContext dbx = new ApplicationDbContext();
                var user = dbx.Users.SingleOrDefault(i => i.UserName == Username);//await UserManager.FindByNameAsync(Username);
                if (user != null)
                {

                    return Json("", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string Msg = "NotExist";

                    return Json(Msg, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
              var  msg=ex.Message;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("login", new { Message = message });
        }

        //
        // GET: /Account/Manage
        //public ActionResult Manage(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
        //        : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
        //        : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : message == ManageMessageId.Error ? "An error has occurred."
        //        : "";
        //    ViewBag.HasLocalPassword = HasPassword();
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    return View();
        //}

        //
       
      
        // POST: /Account/Manage
       
        //public async Task<ActionResult> Manage(ManageUserViewModel model)
        //{
        //    bool hasPassword = HasPassword();
        //    ViewBag.HasLocalPassword = hasPassword;
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    if (hasPassword)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
        //            }
        //            else
        //            {
        //                AddErrors(result);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // User does not have a password so remove any validation errors caused by a missing OldPassword field
        //        ModelState state = ModelState["OldPassword"];
        //        if (state != null)
        //        {
        //            state.Errors.Clear();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
        //            }
        //            else
        //            {
        //                AddErrors(result);
        //            }
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        //
        // GET: /Account/ExternalLoginCallback
        //  [AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var user = await UserManager.FindAsync(loginInfo.Login);
        //    if (user != null)
        //    {
        //        await SignInAsync(user, isPersistent: false);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    else
        //    {
        //        // If the user does not have an account, then prompt the user to create an account
        //        ViewBag.ReturnUrl = returnUrl;
        //        ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
        //    }
        //}

        //
        // POST: /Account/LinkLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LinkLogin(string provider)
        //{
        //    // Request a redirect to the external login provider to link a login for the current user
        //    return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        //}

        //
        // GET: /Account/LinkLoginCallback
        //public async Task<ActionResult> LinkLoginCallback()
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //    }
        //    var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("Manage");
        //    }
        //    return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //}

        //
        // POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }  



        //        var user = new ApplicationUser() { UserName = model.UserName };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInAsync(user, isPersistent: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        //
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }





        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //   IdentityResult result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
                ApplicationDbContext context = new ApplicationDbContext();
                UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
                String userId = model.UserName;//"<YourLogicAssignsRequestedUserId>";
                String newPassword = model.NewPassword; //"<PasswordAsTypedByUser>";
                String hashedNewPassword = UserManager.PasswordHasher.HashPassword(newPassword);
                ApplicationUser cUser = await store.FindByNameAsync(userId);
                await store.SetPasswordHashAsync(cUser, hashedNewPassword);
                await store.UpdateAsync(cUser);

                if (store.UpdateAsync(cUser) != null)
                {
                    return RedirectToAction("Login", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return RedirectToAction("Login", new { Message = ManageMessageId.Error });

                }


                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
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
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);


            Session.Abandon();
            return RedirectToAction("Login", "Account");
            // return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //[ChildActionOnly]
        //public ActionResult RemoveAccountList()
        //{
        //    var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
        //    ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
        //    return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
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

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
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
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
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