// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Domain.Entities;
using Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebApp.Models.Identity;

namespace WebApp.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IDBService _dBService;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IDBService dBService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _dBService = dBService;
        }

        [BindProperty]
        public RegistrationInputModel Input { get; set; }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new();

                await _userManager.SetUserNameAsync(user: user, userName: Input.Username);
                await _userManager.SetEmailAsync(user: user, email: Input.Email);

                _logger.LogDebug($"User named {Input.Username} created.");
                IdentityResult resultUserManager = await _userManager.CreateAsync(user: user, password: Input.Password);
                
                if (resultUserManager.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "member");

                    Author author = new()
                    {
                        Id = user.Id,
                        Username = user.UserName
                    };

                    await _dBService.AuthorRepository.AddAsync(author);

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