using Biblioteka.Models.Identity;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Models
{
    public class Reader
    {
        public virtual int Id { get; set; }
        public virtual string Pesel { get; set; }
        public virtual string Telephone { get; set; }
        public virtual string RegistrationDate { get; set; }
        public virtual  User User { get; set; }

        public class Map : ClassMap<Reader>
        {
            public Map()
            {
                Table("Readers");
                Id(x => x.Id).GeneratedBy.Identity();
                Map(x => x.Pesel).Not.Nullable();
                Map(x => x.Telephone).Not.Nullable();
                Map(x => x.RegistrationDate).Not.Nullable();
                References(x => x.User).Column("UserId").Not.Nullable().Cascade.All();

            }
        }


        public override string ToString()
        {
            return User.UserName + " (" + User.Email+ ")";
        }
    }
}