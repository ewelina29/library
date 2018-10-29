using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Models.Mail
{
    public abstract class MailFactory
    {
        public static readonly String EmailFrom = "library.mvc.bg@gmail.com";

        public abstract Mail CreateMail();


    }
}