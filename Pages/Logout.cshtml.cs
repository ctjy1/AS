using Assignment2.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks; // Required for Task

namespace Assignment2.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            // Clear the session data
            HttpContext.Session.Clear();

            // Sign out the user
            await _signInManager.SignOutAsync();

            // Redirect to the login page
            return RedirectToPage("Login");
        }

        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            // No action is taken, just redirect to the Index page
            return RedirectToPage("Index");
        }
    }
}
