using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Assignment2.Model;
using Assignment2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Assignment2.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LModel { get; set; }

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthDbContext _dbContext;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuthDbContext dbContext, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var identityResult = await _signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, true);

            if (identityResult.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(LModel.Email);
                if (user != null)
                {
                    await CreateSecuredSession(LModel.Email);
                    await LogAuditAsync(user.Id, $"User {user.Email} logged in successfully.");
                    return RedirectToPage("/Index");
                }
            }
            else if (identityResult.IsLockedOut)
            {
                _logger.LogWarning($"Account locked for user {LModel.Email}");
                ModelState.AddModelError(string.Empty, "Account locked due to multiple failed login attempts. Please try again in a minute.");
                await LogAuditAsync(LModel.Email, $"User {LModel.Email} account locked due to failed login attempts.", true);
            }
            else
            {
                _logger.LogWarning($"Invalid login attempt for user {LModel.Email}");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                await LogAuditAsync(LModel.Email, $"Failed login attempt for user {LModel.Email}.", true);
            }

            return Page();
        }

        private async Task CreateSecuredSession(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user != null)
            {
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Gender", user.Gender);
                HttpContext.Session.SetString("PhoneNumber", user.MobileNo);
                HttpContext.Session.SetString("DeliveryAddress", user.DeliveryAddress);
                HttpContext.Session.SetString("AboutMe", user.AboutMe);
                HttpContext.Session.SetString("CreditCard", user.CreditCardNo);
            }
            else
            {
                _logger.LogWarning("User not found during secured session creation.");
            }
        }

        private async Task LogAuditAsync(string emailOrUserId, string action, bool isEmail = false)
        {
            string userId = isEmail ? (await _userManager.FindByEmailAsync(emailOrUserId))?.Id : emailOrUserId;

            if (!string.IsNullOrEmpty(userId) || isEmail)
            {
                var auditLog = new AuditLog
                {
                    UserId = userId ?? "Unknown",
                    Action = action,
                    Timestamp = DateTime.UtcNow,
                };

                _dbContext.AuditLogs.Add(auditLog);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning($"Audit log not created for action '{action}' as UserId is null or empty.");
            }
        }
        public bool ValidateCaptcha()
        {
            string Response = Request.Form["g-recaptcha-response"];
            bool valid = false;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LdYTmIpAAAAALOYOtkw3X7EBo84dLRyNTDUKOLQ&response=" + Response);
            try
            {
                using (WebResponse wResponse = request.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        var data = JsonSerializer.Deserialize<Recaptcha>(jsonResponse);

                        valid = Convert.ToBoolean(data.Success);
                    }
                }
                return valid;
            }
            catch (WebException ex)
            {
                throw ex;
            }

        }

    }
}
