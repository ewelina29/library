using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Models.Mail
{
    public class LateReturnFactory : MailFactory
    {
        public override Mail CreateMail()
        {
            return new LateReturnMail();
        }
    }
}