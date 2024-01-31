using Assignment2.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Assignment2.Pages
{
    public class HomeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public HomeModel(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public ApplicationUser User { get; set; } // Assuming ApplicationUser is the correct type
        public string UserPhotoUrl { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var username = httpContextAccessor.HttpContext.User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/Login"); // Redirect to login if the user is not authenticated
            }

            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                return RedirectToPage("/Login"); // Redirect to login if the user is not found
            }

            // Decrypt CreditCardNo
            user.CreditCardNo = Decrypt(user.CreditCardNo);

            User = user;

            // Fetch the image URL from the database (assuming User.Photo is the URL)
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
