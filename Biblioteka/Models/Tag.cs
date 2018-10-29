using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Biblioteka.Models
{
    public class Tag
    {
        public virtual int Id { get; set; }

        [Required]
        [System.Web.Mvc.Remote("IsTagNameExist", "Tag", ErrorMessage = "Tag name already exists")]
        public virtual string Name { get; set; }

        public virtual IList<Book> Books { get; set; }

        public Tag()
        {
            Books = new List<Book>();
        }

        public class Map : ClassMap<Tag>
        {
            public Map()
            {
                Table("Tags");
                Id(x => x.Id).GeneratedBy.Identity();
                Map(x => x.Name).Not.Nullable().Unique();
            

               HasManyToMany(u => u.Books)
              .Table("BookTag")
              .ParentKeyColumn("TagId")
              .ChildKeyColumn("BookId");
        }
        }
    }
}