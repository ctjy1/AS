using System.ComponentModel.DataAnnotations;

namespace Assignment2.ViewModels
{
    public class Membership
    {

        [Required]
        [EmailAddress]
        [Key]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.CreditCard)]
        public string CreditCardNo { get; set; }

        public string Gender { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[689]\d{7}$", ErrorMessage = "Invalid Singapore phone number.")]
        public string MobileNo { get; set; }
        public string DeliveryAddress { get; set; }

        [RegularExpression(@"^.+\.jpg$", ErrorMessage = "Invalid file format. Only .JPG files are allowed.")]
        public string Photo { get; set; }

        public string AboutMe { get; set; }

        /*public string PreviousPasswordHash1 { get; set; }
        public string PreviousPasswordHash2 { get; set; }
        public DateTime LastPasswordChangeDate { get; set; }*/

    }


}

