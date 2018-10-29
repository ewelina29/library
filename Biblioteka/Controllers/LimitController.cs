using Biblioteka.Models;
using Biblioteka.Service;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biblioteka.Controllers
{
    public class LimitController : AuthorizationController
    {
       /* public ActionResult Details()
        {

            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
   
                using (ISession session = database.MakeSession())
                {
                    ValidateReaderAndEmployeeForbiddenAccess(session);

                    LimitService limitService = LimitService.getInstance();
                    Limit limit = limitService.getLimit();
                    return View(limit);
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
        }*/


        public ActionResult Edit()
        {
            try
            {
                if (!IsLogged()) return RedirectToAction("Login", "Account");

                using (ISession session = database.MakeSession())
                {
                    ValidateReaderForbiddenAccess(session);

                    LimitService limitService = LimitService.getInstance();
                    Limit limit = limitService.getLimit();
                    return View(limit);
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

        [HttpPost]
        public ActionResult Edit( Limit limit)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    LimitService.getInstance().setLimit(limit.MaxDaysOfRental, limit.MaxAmountOfBooks);

                }
                return RedirectToAction("Edit", "Limit");
            }
            catch
            {
                return View();
            }
        }
    }

}