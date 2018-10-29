using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Models.Identity
{
    public class Login
    {
        [DisplayName("Username")]
        [Required]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
    }
}