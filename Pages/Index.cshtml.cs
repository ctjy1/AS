using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Assignment2.Model;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Assignment2.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ApplicationUser AppUser { get; set; }
        public string UserPhotoUrl { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> OnGetAsync()
        {

            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/Login");
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            user.CreditCardNo = Decrypt(user.CreditCardNo);
            AppUser = user;
            UserPhotoUrl = user.Photo;

            return Page();
        }

        private string Decrypt(string input)
        {
            // Replace these keys with the same keys used for encryption
            string key = "0123456789abcdef";
            string iv = "0123456789abcdef";

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (System.IO.MemoryStream msDecrypt = new System.IO.MemoryStream(Convert.FromBase64String(input)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}