using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Assignment2.Model
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.CreditCard)]
        public string CreditCardNo { get; set; }

        public string Gender { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string MobileNo { get; set; }

        public string DeliveryAddress { get; set; }

        [RegularExpression(@"^.+\.jpg$", ErrorMessage = "Invalid file format. Only .JPG files are allowed.")]
        public string Photo { get; set; }

        [RegularExpression(@"^.+$", ErrorMessage = "Invalid characters in About Me")]
        public string AboutMe { get; set; }
    }
}
