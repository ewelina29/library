using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Models.Identity
{
    public class RegisterModelView
    {
        public RegisterModelView(User user)
        {
            this.UserName = user.UserName;
            this.Name = user.Name;
            this.Surname = user.Surname;
            this.Email = user.Email;
            this.Password = user.PasswordHash;
        }
        public RegisterModelView(User user, Reader reader)
        {
            this.UserName = user.UserName;
            this.Name = user.Name;
            this.Surname = user.Surname;
            this.Email = user.Email;
            this.Password = user.PasswordHash;
            this.Pesel = reader.Pesel;
            this.Telephone = reader.Telephone;

        }

        public RegisterModelView() { }
        [Required]
        [System.Web.Mvc.Remote("IsUserNameExist", "Account",
                ErrorMessage = "Username already exists")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{8,15}", ErrorMessage = "The field Password must contain 8-15 characters and must include at least one upper case letter, one lower case letter, and one numeric digit.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [System.Web.Mvc.Remote("IsEmailExist", "Account",
               ErrorMessage = "Email already exists")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [RegularExpression(@"([0-9]{11})", ErrorMessage = "The field Pesel must contain 11 digits.")]
        public string Pesel { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessage = "The field Telephone must contain 9 to 11 digits.")]
        [StringLength(11, MinimumLength = 9, ErrorMessage = "The field Telephone must contain digits.")]
        public string Telephone { get; set; }


    }
}