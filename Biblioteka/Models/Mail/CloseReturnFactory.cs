using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Biblioteka.Models.Identity;
using Biblioteka.Controllers;
using NHibernate;
using System.Diagnostics;

namespace Biblioteka.Models.Mail
{
    public class CloseReturnFactory : MailFactory
    {
        public override Mail CreateMail()
        {
            return new CloseReturnMail();
        }
    }
}