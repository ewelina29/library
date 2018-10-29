using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Biblioteka.Models
{
    public class Category
    {
        public virtual int Id { get; set; }

        [Required]
        [System.Web.Mvc.Remote("IsCategoryNameExist", "Category",
        ErrorMessage = "Category name already exists")]
        public virtual string Name { get; set; }

        public class Map : ClassMap<Category>
        {
            public Map()
            {
                Table("Categories");
                Id(x => x.Id).GeneratedBy.Identity();
                Map(x => x.Name).Not.Nullable().Unique();
            }

        }
    }

}