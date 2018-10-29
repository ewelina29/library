using Biblioteka.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Biblioteka.Controllers
{
    public class NoticeController : AuthorizationController
    {
        public ActionResult Index()
        {
            using (ISession session = database.MakeSession())
            {

                        var notices = session.QueryOver<Notice>().List();
                        return View(notices.Reverse().Take(5));
            }
         }
    
        public ActionResult IndexAdmin()
        {
            if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
            using (ISession session = database.MakeSession())
            {
                ValidateReaderForbiddenAccess(session);
                var notices = session.QueryOver<Notice>().List();
                return View(notices);
            }
        }



        public ActionResult Create()
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    if (!IsLogged()) return RedirectToAction("Login", "Account");
                    ValidateReaderForbiddenAccess(session);
                }
                return View();
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Forbidden", "Error");
            }
            catch (Exception)
            {
                return RedirectToAction("IndexAdmin");
            }
        }

        [HttpPost]
        public ActionResult Create(Notice notice)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    if (ModelState.IsValid)
                    {
                        using (var transaction = session.BeginTransaction())
                        {
                            session.SaveOrUpdate(notice);
                            session.Flush();
                            transaction.Commit();
                            session.Close();

                        }
                        return RedirectToAction("IndexAdmin");

                    }
                    else
                    {
                        return View(notice);
                    }
                }
            }
            catch (Exception)
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                if (!IsLogged()) return RedirectToAction("Login", "Account");
                using (ISession session = database.MakeSession())
                {
                    ValidateReaderForbiddenAccess(session);
                    var notice = session.Get<Notice>(id);
                    return View(notice);
                }
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Forbidden", "Error");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Delete(int id, Notice notice)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        var noticeToDelete = session.Get<Notice>(id);
                        session.Delete(noticeToDelete);
                        transaction.Commit();
                        session.Close();
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                if (!IsLogged()) return RedirectToAction("Login", "Account");
                using (ISession session = database.MakeSession())
                {
                    ValidateReaderForbiddenAccess(session);
                    var notice = session.Get<Notice>(id);
                    return View(notice);
                }
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Forbidden", "Error");
            }
            catch (Exception)
            {
                return RedirectToAction("IndexAdmin");
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, Notice notice)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        var noticeToEdit = session.Get<Notice>(id);
                        noticeToEdit.Title = notice.Title;
                        noticeToEdit.Description = notice.Description;
                        transaction.Commit();
                        session.Close();
                    }
                }
                return RedirectToAction("IndexAdmin");
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return View();
            }
        }
    }
}