using Biblioteka.Enums;
using Biblioteka.Models;
using Biblioteka.ModelViews;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using EASendMail;
using Biblioteka.Service;

namespace Biblioteka.Controllers
{
    public class RentalController : AuthorizationController
    {
        // GET: Rental
        public ActionResult Index(bool deleted = false)
        {
            try
            {
                var smtp = new EASendMail.SmtpClient();

                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    ValidateReaderForbiddenAccess(session);
                    IList<Rental> rentals;
                    bool sendLateEmails = false;
                    bool sendCloseToReturnEmails = false;


                    if (deleted)
                    {
                        rentals = session.QueryOver<Rental>().Where(Restrictions.Eq("Deleted", true))
                            .OrderBy(x => x.DateFrom).Desc().List();
                        ViewBag.Info = "Past user rentals";
                        ViewBag.showReturnBtn = false;

                    }
                    else
                    {
                        rentals = session.QueryOver<Rental>().Where(Restrictions.Eq("Deleted", false)).
                             OrderBy(x => x.DateFrom).Desc().List();
                        ViewBag.Info = "Current user rentals";
                        ViewBag.showReturnBtn = true;


                    }
                    var rentalsData = new List<CopiesStatusesModelView>();


                    foreach (var ren in rentals)
                    {
                        var rental = new CopiesStatusesModelView()
                        {
                            Id = ren.Id,
                            CopyId = ren.Copy.Id,
                            Reader = ren.Reader.ToString(),
                            Title = ren.Copy.Book.Title,
                            BookId = ren.Copy.Book.Id,
                            DateFrom = ren.DateFrom,
                            DateTo = ren.DateTo

                        };
                        //sprawdzenie czy minal termin nieodebranych ksiazek

                        if (!deleted)
                        {
                            if (IsRentalLate(ren))
                            {
                                sendLateEmails = true;
                            }
                            else if(IsRentalCloseToReturn(ren))
                            {
                                sendCloseToReturnEmails = true;
                            }
                        }
                        rentalsData.Add(rental);

                    }

                    if (sendLateEmails)
                    {
                        ViewBag.ShowLateRentalButton = true;
                    }
                    if(sendCloseToReturnEmails)
                    {
                        ViewBag.ShowCloseToReturnRentalButton = true;
                    }

                    return View(rentalsData);
                }

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

       

        public ActionResult UserRentals()
        {
            try
            { 
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    ValidateAdminAndEmployeeForbiddenAccess(session);
                    var reader = GetReader(session);

                    var rentals = session.QueryOver<Rental>().Where(x => x.Reader == reader).Where(x => x.Deleted == false).List();
                    var rentalsData = new List<CopiesStatusesModelView>();

                    foreach (var ren in rentals)
                    {
                        var rental = new CopiesStatusesModelView()
                        {                   

                        Id = ren.Id,
                            CopyId = ren.Copy.Id,
                            Reader = ren.Reader.ToString(),
                            Title = ren.Copy.Book.Title,
                            BookId = ren.Copy.Book.Id,
                            DateFrom = ren.DateFrom,
                            DateTo = ren.DateTo

                        }; 
                        rentalsData.Add(rental);      
                    }

                    return View(rentalsData);
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

        public ActionResult History()
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    ValidateAdminAndEmployeeForbiddenAccess(session);
                    var reader = GetReader(session);
                    var rentals = session.QueryOver<Rental>().Where(x => x.Reader == reader).OrderBy(x => x.DateFrom).Desc().Where(x => x.Deleted == true).List();
                    var historyData = new List<CopiesStatusesModelView>();
                    foreach (var ren in rentals)
                    {
                        var rental = new CopiesStatusesModelView()
                        {
                            Id = ren.Id,
                            CopyId = ren.Copy.Id,
                            Reader = ren.Reader.ToString(),
                            Title = ren.Copy.Book.Title,
                            BookId = ren.Copy.Book.Id,
                            DateFrom = ren.DateFrom,
                            DateTo = ren.DateTo

                        };
                        historyData.Add(rental);

                    }

                    return View(historyData);
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
        public ActionResult Add(int reservationId)
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    ValidateReaderForbiddenAccess(session);
                    var reservation = session.Get<Reservation>(reservationId);
                    var copy = reservation.Copy;
                    LimitService limitService = LimitService.getInstance();
                    var limit = limitService.getLimit().MaxDaysOfRental;
                    var rental = new Rental()
                    {
                        Copy = copy,
                        Reader = reservation.Reader,
                        DateFrom = DateTime.Now.ToString("dd.MM.yyyy"),
                        DateTo = DateTime.Now.AddDays(limit).ToString("dd.MM.yyyy"),
                        Deleted = false,
                        CloseReturnMailSent = false,
                        LateReturnMailSent = false

                    };
                    copy.Status = Enums.Status.RENTED;

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.SaveOrUpdate(rental);
                        session.Delete(reservation);
                        session.SaveOrUpdate(copy);
                        session.Flush();
                        transaction.Commit();
                    }
                    return RedirectToAction("Index", "Reservation");
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
        public ActionResult Return(int rentalId)
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    var rental = session.Get<Rental>(rentalId);
                    var copyId = rental.Copy.Id;
                    var copy = session.Get<Copy>(copyId);
                    copy.Status = Enums.Status.IN_STOCK;
                    rental.Deleted = true;
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.SaveOrUpdate(rental);
                        session.SaveOrUpdate(copy);
                        session.Flush();
                        transaction.Commit();
                    }

                    //PENDINGS
                    var pending = session.QueryOver<Pending>().Where(x => x.Copy == copy).
                        OrderBy(x => x.DateFrom).Asc().Take(1).SingleOrDefault();


                    if (pending != null)
                    {
                        var limit = 30; //@todo
                        var newReservation = new Reservation()
                        {
                            Copy = copy,

                            Reader = pending.Reader,
                            DateFrom = DateTime.Now.ToString("dd.MM.yyyy"),
                            DateTo = DateTime.Now.AddDays(limit).ToString("dd.MM.yyyy")

                        };
                        copy.Status = Enums.Status.RENTED;



                        using (ITransaction transaction = session.BeginTransaction())
                        {
                            session.SaveOrUpdate(newReservation);
                            session.Delete(pending);
                            session.SaveOrUpdate(copy);
                            session.Flush();
                            transaction.Commit();
                        }

                    }

                    return RedirectToAction("Index", "Rental");
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
