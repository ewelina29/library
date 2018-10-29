using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Models.Mail
{
    public class CloseReturnMail : Mail
    {

        public override string CreateMessage(int copyId, string bookTitle)
        {
            return "The return time on the book: " + bookTitle + " (id: " + copyId + ") will expire in 3 days. ";
        }

        public override string CreateSubject()
        {
            return "Library Notification - Close return";
        }
    }
}