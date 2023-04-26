// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using WebApp.Models.Identity;

namespace WebApp.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public RegistrationInputModel Input { get; set; }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser();

                await _userManager.SetUserNameAsync(user: user, userName: Input.Username);
                await _userManager.SetEmailAsync(user: user, email: Input.Email);
                _logger.LogDebug($"User named {Input.Username} created.");
                IdentityResult resultUserManager = await _userManager.CreateAsync(user: user, password: Input.Password);

                if (resultUserManager.Succeeded)
                {
                    Microsoft.AspNetCore.Identity.SignInResult resultSignInManager = await _signInManager.PasswordSignInAsync(user: user,
                        password: Input.Password,
                        isPersistent: false,
                        lockoutOnFailure: false);

                    if (resultSignInManager.Succeeded)
                    {
                        return RedirectToRoute("default");
                    }
                }

                resultUserManager.Errors.ToList().ForEach(error => ModelState.AddModelError(error.Code, error.Description));
            }

            return Page();
        }
    }
}