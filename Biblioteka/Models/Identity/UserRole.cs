using FluentNHibernate.Mapping;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Models.Identity
{
    public class UserRole
    {

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<User> Users { get; set; }

        public const int ADMIN_ROLE = 1;
        public const int EMPLOYEE_ROLE = 2;
        public const int READER_ROLE = 3;
    }


       

    
}