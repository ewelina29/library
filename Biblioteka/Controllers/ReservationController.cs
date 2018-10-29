using Biblioteka.Models;
using Biblioteka.ModelViews;
using Biblioteka.Service;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biblioteka.Controllers
{
    public class ReservationController : AuthorizationController
    {
        // GET: Reservation
        public ActionResult Index()
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    ValidateReaderForbiddenAccess(session);
                    var reservations = session.QueryOver<Reservation>().List();
                    var reservationsData = new List<CopiesStatusesModelView>();
                    foreach (var res in reservations)
                    {
                        var reservation = new CopiesStatusesModelView()
                        {
                            Id = res.Id,
                            CopyId = res.Copy.Id,
                            Reader = res.Reader.ToString(),
                            DateFrom = res.DateFrom,
                            Title = res.Copy.Book.Title,
                            BookId = res.Copy.Book.Id,
                            DateTo = res.DateTo

                        };
                        reservationsData.Add(reservation);

                    }
                    return View(reservationsData);
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

        public ActionResult UserReservations()
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    ValidateAdminAndEmployeeForbiddenAccess(session);
                    var reader = GetReader(session);
                    var reservations = session.QueryOver<Reservation>().Where(x => x.Reader == reader).List();
                    var reservationsData = new List<CopiesStatusesModelView>();
                    foreach (var res in reservations)
                    {
                        var reservation = new CopiesStatusesModelView()
                        {
                            Id = res.Id,
                            CopyId = res.Copy.Id,
                            Reader = res.Reader.ToString(),
                            Title = res.Copy.Book.Title,
                            BookId = res.Copy.Book.Id,
                            DateFrom = res.DateFrom,
                            DateTo = res.DateTo

                        };
                        reservationsData.Add(reservation);

                    }

                    return View(reservationsData);
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
        public ActionResult Reserve(int copyId)
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    ValidateAdminAndEmployeeForbiddenAccess(session);
                    LimitService limitService = LimitService.getInstance();
                    var reader = GetReader(session);
                    var userRentals = session.QueryOver<Rental>().Where(x => x.Reader == reader).Where(x => x.Deleted == true).RowCount();
                    var userReservations = session.QueryOver<Reservation>().Where(x => x.Reader == reader).RowCount();

                    if (userRentals +  userReservations +1 > limitService.getLimit().MaxAmountOfBooks)
                    {
                        return RedirectToAction("Cart", "Account");
                    }
                    var copy = session.Get<Copy>(copyId);
                    copy.Status = Enums.Status.RENTED;
                    var timeLimit = limitService.getLimit().MaxDaysOfRental;
                    var reservation = new Reservation()
                    {
                        Copy = copy,
                        Reader = reader,
                        DateFrom = DateTime.Today.ToString("dd.MM.yyyy"),
                        DateTo = DateTime.Today.AddDays(timeLimit).ToString("dd.MM.yyyy")

                    };

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.SaveOrUpdate(reservation);
                        session.SaveOrUpdate(copy);
                        session.Flush();
                        transaction.Commit();
                    }
                    DeleteFromSession(copyId);


                    return RedirectToAction("Cart", "Account");
                }
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Forbidden", "Error");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult ReserveAll()
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    ValidateAdminAndEmployeeForbiddenAccess(session);
                    if (Session[SESSION_CART] == null) { return RedirectToAction("Cart", "Account"); }

                    LimitService limitService = LimitService.getInstance();
                    var reader = GetReader(session);
                    var userRentals = session.QueryOver<Rental>().Where(x => x.Reader == reader).Where(x => x.Deleted == true).RowCount();
                    var userReservations = session.QueryOver<Reservation>().Where(x => x.Reader == reader).RowCount();


                    var sessionList = (List<int>)Session[SESSION_CART];
                    if (userRentals + userReservations + sessionList.Count > limitService.getLimit().MaxAmountOfBooks)
                    {
                        return RedirectToAction("Cart", "Account");
                    }
              
                    var timeLimit = limitService.getLimit().MaxDaysOfRental;
                    //@TODO czy to bedzie inny limit?

                    var reservations = new List<Reservation>();
                    var copies = new List<Copy>();
                    foreach (var id in Session[SESSION_CART] as List<int>)
                    {
                        var copy = session.Get<Copy>(id);
                        copy.Status = Enums.Status.RENTED;
                        copies.Add(copy);
                        var reservation = new Reservation()
                        {
                            Copy = copy,
                            Reader = reader,
                            DateFrom = DateTime.Today.ToString("dd.MM.yyyy"),
                            DateTo = DateTime.Today.AddDays(timeLimit).ToString("dd.MM.yyyy")

                        };
                        reservations.Add(reservation);
                    }
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        foreach (var reservation in reservations)
                        {
                            session.SaveOrUpdate(reservation);
                            session.Flush();
                        }

                        foreach (var copy in copies)
                        {
                            session.SaveOrUpdate(copy);
                            session.Flush();
                        }
                        transaction.Commit();
                    }

                    ClearSession();

                    return RedirectToAction("Cart", "Account");
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
        public ActionResult DeleteFromCart(int copyId)
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    ValidateAdminAndEmployeeForbiddenAccess(session);
                    DeleteFromSession(copyId);

                    return RedirectToAction("Cart", "Account");
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
        public ActionResult DeleteAllFromCart()
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    ValidateAdminAndEmployeeForbiddenAccess(session);
                    ClearSession();

                    return RedirectToAction("Cart", "Account");
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
        public ActionResult DeleteReservation(int id)
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession())
                {
                    var reservation = session.Get<Reservation>(id);
                    var copy = session.Get<Copy>(reservation.Copy.Id);
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        copy.Status = Enums.Status.IN_STOCK;
                        session.Delete(reservation);
                        transaction.Commit();
                    }

                    return RedirectToAction("UserReservations");
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
