using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Biblioteka.Models.Mail
{
    public class LateReturnMail : Mail
    {
        public override String CreateMessage(int copyId, string bookTitle)
        {
            return "The return time on the book: " + bookTitle + " (signature: " + copyId + ") has expired. " ;
        }

        public override String CreateSubject()
        {
            return "Library Notification - Late return";
        }
    }
}