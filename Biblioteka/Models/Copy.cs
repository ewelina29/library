using Biblioteka.Enums;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Models
{
    public class Copy
    {
        public virtual int Id { get; set; }
        public virtual Book Book { get; set; }
        public virtual Status Status { get; set; } 

        public class Map : ClassMap<Copy>
        {
            public Map()
            {
                Table("Copies");
                Id(x => x.Id).GeneratedBy.Identity();
                References(x => x.Book).Column("BookId");
                Map(x => x.Status).Not.Nullable();

            }
        }
    }
}