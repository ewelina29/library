using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Biblioteka.Models
{
    public class Book
    {
        public virtual int Id { get; set; }

        [Required]
        public virtual Author Author { get; set; }

        [Required]
        [System.Web.Mvc.Remote("IsTitleExist", "Book",
        ErrorMessage = "Book title already exists")]
        public virtual string Title { get; set; }
        public virtual string ISBN { get; set; }

        [DisplayName("Table of contents")]
        public virtual string TableOfContents { get; set; }
        public virtual string Description { get; set; }

        [Required]
        public virtual Category Category { get; set; }

        public virtual IList<Tag> Tags { get; set; }

        public virtual string FullName
        {
            get
            {
                return Author.Name + " " + Author.Surname;
            }
        }
        public Book()
        {
            Tags = new List<Tag>();
        }

        public class Map : ClassMap<Book>
        {
            public Map()
            {
                Table("Books");
                Id(x => x.Id).GeneratedBy.Identity();
                References(x => x.Author).Column("AuthorId");
                Map(x => x.Title).Not.Nullable().Unique();
                Map(x => x.ISBN).Not.Nullable();
                Map(x => x.TableOfContents).Not.Nullable();
                Map(x => x.Description).Not.Nullable();
                References(x => x.Category).Column("CategoryId");

                HasManyToMany(u => u.Tags)
                .Table("BookTag")
                .ParentKeyColumn("BookId")
                .ChildKeyColumn("TagId");

            }
        }

      
    }
}