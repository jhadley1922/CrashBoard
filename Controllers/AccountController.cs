using CrashBoard.Models;
using CrashBoard.Models.ViewModels;
using Google.Authenticator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Controllers
{
    public class AccountController : Controller
    {
        private ICrashRepository repo { get; set; }

        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> um, SignInManager<IdentityUser> sim, ICrashRepository temp)
        {
            userManager = um;
            signInManager = sim;
            repo = temp;
        }

        // Get Login page, checks if user is logged in already
        [HttpGet]
        public IActionResult Login()
        {
            if (LoginCookieModel.UserId != null)
            {
                IdentityUser user = userManager.Users.FirstOrDefault(x => x.Id == LoginCookieModel.UserId);

                if (userManager.FindByIdAsync(LoginCookieModel.UserId) != null)
                {
                    if (user.TwoFactorEnabled && !LoginCookieModel.Authorized)
                    {
                        return View(new LoginModel { });
                    }
                    var am = new AdminModel
                    {
                        severity = 0,
                        pageNum = 1,
                        searchString = ""
                    };
                    return RedirectToAction("Admin", am);
                }
            }
            
            return View(new LoginModel { });
        }

        // Login post that checks if the user needs to be authorized after logging in with correct credentials
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel lm)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByNameAsync(lm.Username);

                if (user != null)
                {
                    await signInManager.SignOutAsync();

                    if ((await signInManager.PasswordSignInAsync(user, lm.Password, false, false)).Succeeded)
                    {
                        LoginCookieModel.UserId = user.Id;

                        if (user.TwoFactorEnabled)
                        {
                            return RedirectToAction("Authorize");
                        }
                        LoginCookieModel.Authorized = true;
                        var am = new AdminModel
                        {
                            severity = 0,
                            pageNum = 1,
                            searchString = ""
                        };
                        return RedirectToAction("Admin", am);
                    }
                }
            }
            ModelState.AddModelError("", "Invalid name or password.");
            return View(lm);
        }

        //Logs out user
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            LoginCookieModel.UserId = null;
            LoginCookieModel.Authorized = false;

            return View("Login");
        }
        //Gets Admin page, allows search
        public IActionResult Admin(int severity, int pageNum, string searchString)
        {
            int pageSize = 50;

            ViewBag.PageNum = pageNum;
            ViewBag.SelectedSeverity = severity;
            ViewBag.TotalPages = (int)Math.Ceiling((double)repo.Crashes.Count() / pageSize);
            ViewBag.SearchString = searchString;

            var cvm = new CrashesViewModel
            {
                Crashes = repo.Crashes
                    .Include(x => x.CITY)
                    .Include(x => x.COUNTY)
                    .Include(x => x.SEVERITY)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize)
            };

            if (severity != 0 && !String.IsNullOrEmpty(searchString))
            {
                cvm = new CrashesViewModel
                {
                    Crashes = repo.Crashes
                        .Include(x => x.CITY)
                        .Include(x => x.COUNTY)
                        .Include(x => x.SEVERITY)
                        .Where(x => x.SeverityId == severity)
                        .Where(x => x.CRASH_ID.Contains(searchString)
                               || x.CITY.CITY_NAME.Contains(searchString)
                               || x.COUNTY.COUNTY_NAME.Contains(searchString)
                               || x.MAIN_ROAD_NAME.Contains(searchString))
                        .Skip((pageNum - 1) * pageSize)
                        .Take(pageSize)
                };
                ViewBag.TotalPages = (int)Math.Ceiling((double)repo.Crashes
                        .Where(x => x.SeverityId == severity)
                        .Where(x => x.CRASH_ID.Contains(searchString)
                               || x.CITY.CITY_NAME.Contains(searchString)
                               || x.COUNTY.COUNTY_NAME.Contains(searchString)
                               || x.MAIN_ROAD_NAME.Contains(searchString))
                        .Count() / pageSize);
            }
            else if (!String.IsNullOrEmpty(searchString))
            {
                cvm = new CrashesViewModel
                {
                    Crashes = repo.Crashes
                        .Include(x => x.CITY)
                        .Include(x => x.COUNTY)
                        .Include(x => x.SEVERITY)
                        .Where(x => x.CRASH_ID.Contains(searchString)
                               || x.CITY.CITY_NAME.Contains(searchString)
                               || x.COUNTY.COUNTY_NAME.Contains(searchString)
                               || x.MAIN_ROAD_NAME.Contains(searchString))
                        .Skip((pageNum - 1) * pageSize)
                        .Take(pageSize)
                };
                ViewBag.TotalPages = (int)Math.Ceiling((double)repo.Crashes
                         .Where(x => x.CRASH_ID.Contains(searchString)
                               || x.CITY.CITY_NAME.Contains(searchString)
                               || x.COUNTY.COUNTY_NAME.Contains(searchString)
                               || x.MAIN_ROAD_NAME.Contains(searchString))
                         .Count() / pageSize);
            }
            else if (severity != 0)
            {
                cvm = new CrashesViewModel
                {
                    Crashes = repo.Crashes
                        .Include(x => x.CITY)
                        .Include(x => x.COUNTY)
                        .Include(x => x.SEVERITY)
                        .Where(x => x.SeverityId == severity)
                        .Skip((pageNum - 1) * pageSize)
                        .Take(pageSize)
                };
                ViewBag.TotalPages = (int)Math.Ceiling((double)repo.Crashes
                    .Where(x => x.SeverityId == severity)
                    .Count() / pageSize);
            }
            return View(cvm);
        }

        //Gets Create page
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Cities = repo.Cities.OrderBy(x => x.CITY_NAME).ToList();
            ViewBag.Counties = repo.Counties.OrderBy(x => x.COUNTY_NAME).ToList();
            ViewBag.Severities = repo.Severities.ToList();
            return View();
        }

        //Creates crash
        [HttpPost]
        public IActionResult Create(Crash c)
        {
            repo.CreateCrash(c);
            return View("CreateConfirm");
        }

        // Gets edit page
        [HttpGet]
        public IActionResult Edit(int crashpk)
        {
            var crash = repo.Crashes.FirstOrDefault(x => x.CRASH_PK == crashpk);
            ViewBag.Cities = repo.Cities.OrderBy(x => x.CITY_NAME).ToList();
            ViewBag.Counties = repo.Counties.OrderBy(x => x.COUNTY_NAME).ToList();
            ViewBag.Severities = repo.Severities.ToList();
            return View(crash);
        }

        // Edits a crash
        [HttpPost]
        public IActionResult Edit(Crash c)
        {
            repo.SaveCrash(c);
            return View("EditConfirm");
        }

        // Gets Delete page
        [HttpGet]
        public IActionResult Delete(int crashpk)
        {
            var crash = repo.Crashes.FirstOrDefault(x => x.CRASH_PK == crashpk);
            return View(crash);
        }

        // Deletes a crash
        [HttpPost]
        public IActionResult Delete(Crash c)
        {
            repo.DeleteCrash(c);
            return View("DeleteConfirm");
        }

        // Gets MFA authorize page
        [HttpGet]
        public IActionResult Authorize()
        {
            return View();
        }

        // Authorizes a user, returns to login page if failed
        [HttpPost]
        public IActionResult Authorize(string inputCode)
        {
            IdentityUser user = userManager.Users.FirstOrDefault(x => x.Id == LoginCookieModel.UserId);
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(TwoFactorKey(user), inputCode);
            if (!isValid)
            {
                return Redirect("Login");
            }
            LoginCookieModel.Authorized = true;
            var am = new AdminModel
            {
                severity = 0,
                pageNum = 1,
                searchString = ""
            };
            return RedirectToAction("Admin", am);
        }

        // MFA key
        private static string TwoFactorKey(IdentityUser user)
        {
            return $"myverysecretkey+{user.Email}";
        }
    }
}
