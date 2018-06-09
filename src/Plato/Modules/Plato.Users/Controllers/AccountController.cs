﻿using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Plato.Users.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Plato.Data.Abstractions.Schemas;
using Plato.Models.Users;

namespace Plato.Users.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISchemaBuilder _schemaBuilder;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManage,
            IHttpContextAccessor httpContextAccessor, ISchemaBuilder schemaBuilder)
        {
            _userManager = userManager;
            _signInManager = signInManage;
            _httpContextAccessor = httpContextAccessor;
            _schemaBuilder = schemaBuilder;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {


            //for (var i = 0; i < 500; i++)
            //{
            //    var password = "pAs5word#" + i;
            //    var result = await _userManager.CreateAsync(new User()
            //    {
            //        UserName = "Username" + i,
            //        Email = "email" + i + "@address.com"

            //    }, password);
            //}


            //var users = new SchemaTable()
            //{
            //    ViewName = "Users",
            //    Columns = new List<SchemaColumn>()
            //    {
            //        new SchemaColumn()
            //        {
            //            PrimaryKey = true,
            //            ViewName = "Id",
            //            DbType = DbType.Int32
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "UserName",
            //            Length = "255",
            //            DbType = DbType.String
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "NormalizedUserName",
            //            Length = "255",
            //            DbType = DbType.String
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "Email",
            //            Length = "255",
            //            DbType = DbType.String
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "NormalizedEmail",
            //            Length = "255",
            //            DbType = DbType.String
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "EmailConfirmed",
            //            DbType = DbType.Boolean
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "DisplayName",
            //            Length = "255",
            //            DbType = DbType.String
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "SamAccountName",
            //            Length = "255",
            //            DbType = DbType.String
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "PasswordHash",
            //            Length = "255",
            //            DbType = DbType.String
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "SecurityStamp",
            //            Length = "255",
            //            DbType = DbType.String
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "PhoneNumber",
            //            Length = "255",
            //            DbType = DbType.String
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "PhoneNumberConfirmed",
            //            DbType = DbType.Boolean
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "TwoFactorEnabled",
            //            DbType = DbType.Boolean
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "LockoutEnd",
            //            Nullable = true,
            //            DbType = DbType.DateTime2
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "LockoutEnabled",
            //            DbType = DbType.Boolean
            //        },
            //        new SchemaColumn()
            //        {
            //            ViewName = "AccessFailedCount",
            //            DbType = DbType.Int32
            //        },
            //    }
            //};

            //using (var builder = _schemaBuilder)
            //{

            //    // create tables and default procedures
            //    builder
            //        .Configure(options =>
            //        {
            //            options.ModuleName = "Plato.Users";
            //            options.Version = "1.0.0";
            //        })
            //        .CreateProcedure(new SchemaProcedure("SelectUsersPaged", StoredProcedureType.SelectPaged)
            //            .ForTable(users)
            //            .WithParameters(new List<SchemaColumn>()
            //            {
            //                new SchemaColumn()
            //                {
            //                    ViewName = "Id",
            //                    DbType = DbType.Int32
            //                },
            //                new SchemaColumn()
            //                {
            //                    ViewName = "UserName",
            //                    DbType = DbType.String,
            //                    Length = "255"
            //                },
            //                new SchemaColumn()
            //                {
            //                    ViewName = "Email",
            //                    DbType = DbType.String,
            //                    Length = "255"
            //                }
            //            }));


            //    var result = await builder.ApplySchemaAsync();


            //}

            var user = _httpContextAccessor.HttpContext.User;
            var claims = user.Claims;

            var model = new LoginViewModel();
            model.Email = "";
            model.UserName = "";
            model.Password = "";

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {


         
             // ----------

             ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName, 
                    model.Password, 
                    model.RememberMe,
                    lockoutOnFailure: false);
                if (result.Succeeded)
                {

                    //var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    //identity.AddClaim(new Claim(ClaimTypes.ViewName, model.UserName));

                    ////context.Authenticate | Challenge | SignInAsync("scheme"); // Calls 2.0 auth stack

                    //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    //    new ClaimsPrincipal(identity));
                    

                    //_logger.LogInformation(1, "User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    //return RedirectToAction(nameof(LoginWith2fa), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });


                    ModelState.AddModelError(string.Empty, "Account Required Two Factor Authentication.");
                    return View(model);

                }
                if (result.IsLockedOut)
                {
                    //_logger.LogWarning(2, "User account locked out.");
                    //return View("Lockout");

                    ModelState.AddModelError(string.Empty, "Account Locked out.");
                    return View(model);

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {

   
            var model = new RegisterViewModel
            {
                Email = "admin@admin.com",
                UserName = "admin@Adm1in.com",
                Password = "admin@Adm1in.com"
            };

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model, 
            string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    //_logger.LogInformation(3, "User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("~/");
            }
        }




    }
}
