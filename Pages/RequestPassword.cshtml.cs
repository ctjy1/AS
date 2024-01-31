using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assignment2.Model;
using Microsoft.AspNetCore.Mvc;
using Assignment2.ViewModels;
using Assignment2.Models;

public class RequestPasswordModel : PageModel
{
    private readonly AuthDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public RequestPasswordModel(AuthDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    [BindProperty]
    public PasswordResetRequestViewModel PasswordResetRequest { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = _dbContext.Users.FirstOrDefault(u => u.Email == PasswordResetRequest.Email);
        if (user != null)
        {
            var token = GeneratePasswordResetToken();
            // Save the token in the database
            var passwordResetToken = new PasswordResetToken
            {
                Token = token,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddHours(24) // Token valid for 24 hours
            };
            _dbContext.PasswordResetTokens.Add(passwordResetToken);
            await _dbContext.SaveChangesAsync();

            SendPasswordResetEmail(user.Email, token);
        }

        return RedirectToPage("/ResetRequestConfirmation");
    }

    private string GeneratePasswordResetToken()
    {
        // Generate a secure token. This is a basic implementation.
        return Guid.NewGuid().ToString();
    }

    private void SendPasswordResetEmail(string email, string token)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_configuration["SmtpSettings:SenderName"], _configuration["SmtpSettings:SenderEmail"]));
        emailMessage.To.Add(MailboxAddress.Parse(email));
        emailMessage.Subject = "Password Reset Request";
        emailMessage.Body = new TextPart("html")
        {
            Text = $"Please reset your password by clicking on this link: <a href='https://yourwebsite.com/resetpassword?token={token}'>Reset Password</a>"
        };

        using var client = new SmtpClient();
        client.Connect(_configuration["SmtpSettings:Server"], int.Parse(_configuration["SmtpSettings:Port"]), true);
        client.Authenticate(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
        client.Send(emailMessage);
        client.Disconnect(true);
    }

}