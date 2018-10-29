using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;


namespace Biblioteka.Models
{
    public class Fine
    {
        public virtual int Id { set; get; }
        public virtual float Amount { set; get; }
        public virtual string Description { set; get; }
        public virtual bool Deleted { set; get;}
        public virtual Reader Reader { get; set; }
        public virtual Copy Copy { get; set; }

        public class Map : ClassMap<Fine>
        {
            public Map()
            {
                Table("Fines");
                Id(x => x.Id).GeneratedBy.Identity();
                References(x => x.Reader).Column("ReaderId");
                Map(x => x.Amount).Not.Nullable();
                Map(x => x.Description).Not.Nullable();
                Map(x => x.Deleted).Not.Nullable();
                References(x => x.Copy).Column("CopyId");

            }


        }
    }
}