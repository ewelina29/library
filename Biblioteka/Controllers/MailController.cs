using Biblioteka.Models;
using Biblioteka.Models.Mail;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EASendMail;

namespace Biblioteka.Controllers
{
    public class MailController : AuthorizationController
    {
        SmtpClient smtpClient = new SmtpClient();
        SmtpServer oServer = new SmtpServer("smtp.gmail.com")
        {
            Port = 587,
            ConnectType = SmtpConnectType.ConnectSSLAuto,
            User = MailFactory.EmailFrom,
            Password = "" 
        };

        public ActionResult SendMails(bool lateRental = false, bool closeReturn = false)
        {
            try
            {
                if (!IsLogged()) return RedirectToAction("Login", "Account");
                using (ISession session = database.MakeSession())
                {
                    ValidateReaderForbiddenAccess(session);
                    SmtpClient smtpClient = new SmtpClient();
                    if (lateRental)
                    {
                        SendLateRentalEmails(session);
                    }
                    if (closeReturn)
                    {

                        SendCloseReturnEmails(session);
                    }

                }
                return View();
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Forbidden", "Error");
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                return RedirectToAction("Index", "Home");
            }


        }

        private void SendMail(String message, String subject, String mailTo)
        {
            SmtpMail smtpMail = new SmtpMail("TryIt")
            {
                From = MailFactory.EmailFrom,
                To = mailTo,
                Subject = subject,
                TextBody = message
            };

            smtpClient.SendMail(oServer, smtpMail);

        }

        private void SendLateRentalEmails(ISession session)
        {
            var allCurrentRentals = session.QueryOver<Rental>().Where(x => x.Deleted == false).List();
           
            foreach (var rental in allCurrentRentals)
            {
                if (IsRentalLate(rental))
                {

                    MailFactory mailFactory = new LateReturnFactory();
                    Mail lateReturn = mailFactory.CreateMail();

                    var message = lateReturn.CreateMessage(rental.Copy.Id, rental.Copy.Book.Title);
                    var subject = lateReturn.CreateSubject();
                    var mailTo = rental.Reader.User.Email;
                    SendMail(message, subject, mailTo);

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        rental.LateReturnMailSent = true;
                        session.SaveOrUpdate(rental);
                        transaction.Commit();
                    }


                }
            }

        }

        private void SendCloseReturnEmails(ISession session)
        {
            var allCurrentRentals = session.QueryOver<Rental>().Where(x => x.Deleted == false).List();


            foreach (var rental in allCurrentRentals)
            {
                if (IsRentalCloseToReturn(rental))
                {
                    MailFactory mailFactory = new CloseReturnFactory();
                    Mail closeReturn = mailFactory.CreateMail();
                    var message = closeReturn.CreateMessage(rental.Copy.Id, rental.Copy.Book.Title);
                    var subject = closeReturn.CreateSubject();
                    var mailTo = rental.Reader.User.Email;
                    SendMail(message, subject, mailTo);

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        rental.CloseReturnMailSent = true;
                        session.SaveOrUpdate(rental);
                        transaction.Commit();
                    }
                }
            }

        }
    }
}
