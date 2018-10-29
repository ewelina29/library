using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Biblioteka.Models
{
    public class Limit
    {
        public virtual int Id { get; set; }

        [DisplayName("Maximum days of rentals")]
        public virtual int MaxDaysOfRental { get; set;}

        [DisplayName("Maximum books, which user has in reservations and rentals")]
        public virtual int MaxAmountOfBooks { get; set; }
        
        public class Map : ClassMap<Limit>
        {
            public Map()
            {
                Table("Limits");
                Id(x => x.Id).GeneratedBy.Identity();
                Map(x => x.MaxDaysOfRental).Not.Nullable();
                Map(x => x.MaxAmountOfBooks).Not.Nullable();
            }
        }
    }
}