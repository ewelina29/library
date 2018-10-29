using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Biblioteka.Models
{
    
    public class Author
    {
        public Author()
        {
            Books = new List<Book>();
        }
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Surname { get; set; }

        [DisplayName("Year of birth")]
        public virtual int YearOfBirth { get; set; }
        public virtual IList<Book> Books { get; set; }


        public class Map : ClassMap<Author>
        {
            public Map()
            {
                Table("Authors");
                Id(x => x.Id).GeneratedBy.Identity();
                Map(x => x.Name).Not.Nullable();
                Map(x => x.Surname).Not.Nullable();
                Map(x => x.YearOfBirth).Not.Nullable();

                HasMany<Book>(x => x.Books)
                    .KeyColumn("AuthorId")
                    .Cascade.All()
                    .Inverse().LazyLoad();
            }
        }

        public override string ToString()
        {
            return Name + " " + Surname + " (" + YearOfBirth + ")";
        }
    }
}