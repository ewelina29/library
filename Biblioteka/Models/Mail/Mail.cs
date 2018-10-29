using Biblioteka.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Models.Mail
{
    public abstract class Mail
    {

        public abstract string CreateMessage(int copyId, String bookTitle);

        public abstract string CreateSubject();



    }
}