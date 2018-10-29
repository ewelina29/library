using FluentNHibernate.Mapping;
using Microsoft.AspNet.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Models.Identity
{
    public class User : IUser<int>
    {
        public virtual int Id { get; protected set; }

        [Required]
        [DisplayName("Username")]
        [System.Web.Mvc.Remote("IsUserNameExist", "Account",
                ErrorMessage = "Username already exists")]
        public virtual string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{8,15}", ErrorMessage = "The field Password must contain 8-15 characters and must include at least one upper case letter, one lower case letter, and one numeric digit.")]
        public virtual string PasswordHash { get; set; }

        public virtual int Role { get; set; }

        [Required]
        [System.Web.Mvc.Remote("IsEmailExist", "Account",
        ErrorMessage = "Email already exists")]
        [DataType(DataType.EmailAddress)]
        public virtual string Email { get; set; }

        [Required]
        public virtual string Name { get; set; }

        [Required]
        public virtual string Surname { get; set; }


        public class Map : ClassMap<User>
        {
            public Map()
            {
                Table("Users");
                Id(x => x.Id).GeneratedBy.Identity();
                Map(x => x.UserName).Not.Nullable().Unique();
                Map(x => x.PasswordHash).Not.Nullable();
                Map(x => x.Role).Not.Nullable().Not.Nullable();
                Map(x => x.Email).Not.Nullable().Unique();
                Map(x => x.Name).Not.Nullable();
                Map(x => x.Surname).Not.Nullable();

            }
        }

        public virtual string FormattedRole
        {
            get
            {
                if (Role == UserRole.ADMIN_ROLE)
                {
                    return "ADMIN";
                }
                else if (Role == UserRole.EMPLOYEE_ROLE)
                {
                    return "EMPLOYEE";
                }
                else return "READER";

            }
        }


    }
}