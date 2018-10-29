using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Models
{
    public class Notice
    {
        public virtual int Id { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public class Map : ClassMap<Notice>
        {
            public Map()
            {
                Table("Notices");
                Id(x => x.Id).GeneratedBy.Identity();
                Map(x => x.Title).Not.Nullable();
                Map(x => x.Description).Not.Nullable();
            }
        }
    }
}