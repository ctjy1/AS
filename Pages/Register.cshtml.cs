using Assignment2.Model;
using Assignment2.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Assignment2.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AuthDbContext authDbContext;

        [BindProperty]
        public Membership RModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AuthDbContext authDbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.authDbContext = authDbContext;
        }

        public void OnGet()
        {
        }

        public JsonResult OnPostCheckPasswordPolicy(string password)
        {
            var passwordValidationResult = CheckPasswordPolicy(password);

            if (passwordValidationResult.Succeeded)
            {
                return new JsonResult(new { success = true });
            }
            else
            {
                var errorMessage = string.Join(", ", passwordValidationResult.Errors.Select(error => error.Description));
                return new JsonResult(new { success = false, errorMessage });
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Check if the password meets the policy
                var passwordPolicyResult = CheckPasswordPolicy(RModel.Password);

                if (!passwordPolicyResult.Succeeded)
                {
                    foreach (var error in passwordPolicyResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return Page();
                }

                var user = new ApplicationUser()
                {
                    UserName = RModel.Email,
                    FullName = RModel.FullName,
                    Email = RModel.Email,
                    CreditCardNo = Encrypt(RModel.CreditCardNo),
                    Gender = RModel.Gender,
                    MobileNo = RModel.MobileNo,
                    DeliveryAddress = RModel.DeliveryAddress,
                    Photo = RModel.Photo,
                    AboutMe = WebUtility.HtmlEncode(RModel.AboutMe)
                };

                var result = await userManager.CreateAsync(user, RModel.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return Page();
        }

        private IdentityResult CheckPasswordPolicy(string password)
        {
            var passwordPolicy = new PasswordOptions
            {
                RequiredLength = 12,
                RequireUppercase = true,
                RequireLowercase = true,
                RequireDigit = true,
                RequireNonAlphanumeric = true
            };

            var passwordValidationResult = new PasswordValidator<ApplicationUser>().ValidateAsync(userManager, null, password).Result;

            if (passwordValidationResult.Succeeded)
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed(passwordValidationResult.Errors.ToArray());
            }
        }

        private string Encrypt(string input)
        {
            string key = "0123456789abcdef";
            string iv = "0123456789abcdef";

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (System.IO.MemoryStream msEncrypt = new System.IO.MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }
}
