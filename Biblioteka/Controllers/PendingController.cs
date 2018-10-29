using Biblioteka.Models;
using Biblioteka.Models.Identity;
using Biblioteka.ModelViews;
using Microsoft.AspNet.Identity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers.Entities;

namespace Biblioteka.Controllers
{
    public class PendingController : AuthorizationController
    {
        // GET: Pending
        public ActionResult Index()
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    ValidateReaderForbiddenAccess(session);
                    var pendings = session.QueryOver<Pending>().List();
                    var pendingsData = new List<CopiesStatusesModelView>();
                    foreach (var pen in pendings)
                    {
                        var reservation = new CopiesStatusesModelView()
                        {
                            Id = pen.Id,
                            CopyId = pen.Copy.Id,
                            Reader = pen.Reader.ToString(),
                            Title = pen.Copy.Book.Title,
                            BookId = pen.Copy.Book.Id,
                            DateFrom = pen.DateFrom,
                            DateTo = pen.DateTo

                        };
                        pendingsData.Add(reservation);

                    }
                    return View(pendingsData);
                }

            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Forbidden", "Error");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public ActionResult UserPendings()
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    ValidateAdminAndEmployeeForbiddenAccess(session);
                    var reader = GetReader(session);
                    var pendings = session.QueryOver<Pending>().Where(x => x.Reader == reader).List();
                    var pendingsData = new List<CopiesStatusesModelView>();
                    foreach (var pen in pendings)
                    {
                        var pending = new CopiesStatusesModelView()
                        {
                            Id = pen.Id,
                            CopyId = pen.Copy.Id,
                            Reader = pen.Reader.ToString(),
                            Title = pen.Copy.Book.Title,
                            BookId = pen.Copy.Book.Id,
                            DateFrom = pen.DateFrom,
                            DateTo = pen.DateTo

                        };
                        pendingsData.Add(pending);

                    }

                    return View(pendingsData);
                }
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Forbidden", "Error");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    var pending = session.Get<Pending>(id);

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.Delete(pending);
                        transaction.Commit();
                    }

                    var userId = Int32.Parse(this.User.Identity.GetUserId());
                    var user = session.Get<Models.Identity.User>(userId);
                    if (user.Role == UserRole.READER_ROLE)
                    {
                        return RedirectToAction("UserPendings", "Pending");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Pending");
                    }

                }
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Forbidden", "Error");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}