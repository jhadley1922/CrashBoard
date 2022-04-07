using CrashBoard.Models;
using CrashBoard.Models.ViewModels;
using Google.Authenticator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Controllers
{
    public class TwoFactorAuthenticationController : Controller
    {
        private ICrashRepository repo { get; set; }
        private UserManager<IdentityUser> userManager;

        public TwoFactorAuthenticationController(ICrashRepository temp, UserManager<IdentityUser> um)
        {
            repo = temp;
            userManager = um;
        }

        //Gets enable MFA page
        [HttpGet]
        public ActionResult Enable()
        {
            var user = userManager.Users.FirstOrDefault(x => x.Id == LoginCookieModel.UserId);
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            var setupInfo =
                twoFactor.GenerateSetupCode("myapp", user.Email, TwoFactorKey(user), false, 3);
            ViewBag.SetupCode = setupInfo.ManualEntryKey;
            ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            return View();
        }

        // MFA key
        private static string TwoFactorKey(IdentityUser user)
        {
            return $"myverysecretkey+{user.Email}";
        }

        //Enables MFA for a user
        [HttpPost]
        public ActionResult Enable(string inputCode)
        {
            IdentityUser user = userManager.Users.FirstOrDefault(x => x.Id == LoginCookieModel.UserId);
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(TwoFactorKey(user), inputCode);
            if (!isValid)
            {
                return Redirect("/twofactorauthentication/enable");
            }

            user.TwoFactorEnabled = true;
            
            return Redirect("/");
        }
    }
}
