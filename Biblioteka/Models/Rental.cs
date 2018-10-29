using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;


namespace Biblioteka.Models
{
    public class Rental
    {
        public virtual int Id { set; get; }
        public virtual Copy Copy { set; get; }
        public virtual Reader Reader { set; get; }
        public virtual string DateFrom { set; get; }
        public virtual string DateTo { set; get; }
        public virtual bool Deleted { set; get; }
        public virtual bool CloseReturnMailSent { set; get; }
        public virtual bool LateReturnMailSent { set; get; }


        public class Map : ClassMap<Rental>
        {
            public Map()
            {
                Table("Rentals");
                Id(x => x.Id).GeneratedBy.Identity();
                References(x => x.Copy).Column("CopyId");
                References(x => x.Reader).Column("ReaderId");
                Map(x => x.DateFrom).Not.Nullable();
                Map(x => x.DateTo).Not.Nullable();
                Map(x => x.Deleted).Not.Nullable();
                Map(x => x.CloseReturnMailSent).Nullable();
                Map(x => x.LateReturnMailSent).Nullable();


            }
        }
    }
}