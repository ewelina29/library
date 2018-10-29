using Biblioteka.Models;
using Biblioteka.ModelViews;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biblioteka.Controllers
{
    public class FineController : AuthorizationController

    {
        // GET: Fine
        public ActionResult Index(bool calculate=false)
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    ValidateReaderForbiddenAccess(session);

                    if (calculate)
                    {
                        CalculateFines();
                    }
                    var fines = session.QueryOver<Fine>().Where(x => x.Deleted == false).List();
                    var finesData = new List<FineCopyView>();

                    foreach (var fin in fines)
                    {
                        var fine = new FineCopyView()
                        {
                            Id = fin.Id,
                            CopyId = fin.Copy.Id,
                            Title = fin.Copy.Book.Title,
                            Amount = fin.Amount,
                            Deleted = fin.Deleted,
                            Description = fin.Description

                        };
                        finesData.Add(fine);
                    }

                    return View(finesData);
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

        public ActionResult UserFines()
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    ValidateAdminAndEmployeeForbiddenAccess(session);
                    var reader = GetReader(session);

                    var fines = session.QueryOver<Fine>().Where(x => x.Reader == reader).Where(x => x.Deleted == false).List();
                    var finesData = new List<FineCopyView>();

                    foreach (var fin in fines)
                    {
                        var fine = new FineCopyView()
                        {
                            Id = fin.Id,
                            CopyId = fin.Copy.Id,
                            Title = fin.Copy.Book.Title,
                            Amount = fin.Amount,
                            Deleted = fin.Deleted,
                            Description = fin.Description

                        };
                        finesData.Add(fine);
                    }

                    return View(finesData);
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

        public void CalculateFines()
        {
            using (ISession session = database.MakeSession())
            {
                IList<Rental> currentRentals = session.QueryOver<Rental>().Where(x => x.Deleted == false).List();
                var today = DateTime.Today;

                using (ITransaction transaction = session.BeginTransaction())
                {
                    foreach (var rental in currentRentals)
                    {
                        var dateTo = DateTime.Parse(rental.DateTo);
                        if (dateTo > today) continue;

                        System.TimeSpan diff = today - dateTo;

                        var currentFine = session.QueryOver<Fine>().Where(x => x.Reader == rental.Reader).Where(x => x.Copy == rental.Copy).SingleOrDefault();
                        if (currentFine == null)
                        {
                            var newFine = new Fine()
                            {
                                Copy = rental.Copy,
                                Reader = rental.Reader,
                                Amount = (float)(diff.Days * 0.2),
                                Description = "Delay - days:" + diff.Days,
                                Deleted = false
                            };
                            session.SaveOrUpdate(newFine);
                            session.Flush();
                        }
                        else
                        {

                            currentFine.Amount = (float)(diff.Days * 0.2);
                            currentFine.Description = "Delay - days:" + diff.Days;
                            currentFine.Deleted = false;
                            session.SaveOrUpdate(currentFine);
                            session.Flush();
                        }

                    }
                    transaction.Commit();

                }


            }
        }
    }
}