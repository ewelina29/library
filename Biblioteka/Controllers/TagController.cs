using Biblioteka.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biblioteka.Controllers
{
    public class TagController : AuthorizationController
    {
        // GET: Tag
        public ActionResult Index()
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    if (!IsLogged()) return RedirectToAction("Login", "Account");
                    ValidateReaderForbiddenAccess(session);
                    var tags = session.QueryOver<Tag>().List();
                    ViewBag.Role = GetRole(session);
                    return View(tags);
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
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Create(Tag tag)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    if (ModelState.IsValid)
                    {
                        using (var transaction = session.BeginTransaction())
                        {
                            session.SaveOrUpdate(tag);

                            transaction.Commit();
                            session.Close();

                        }
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        return View(tag);
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
                    var tag = session.Get<Tag>(id);
                    return View(tag);
                }
            }catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Forbidden", "Error");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Delete(int id, Tag tag)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        var tagToDelete = session.Get<Tag>(id);
                        session.Delete(tagToDelete);
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

        public JsonResult IsTagNameExist(string name)
        {
            using (ISession session = database.MakeSession())
            {
                bool exists = session.QueryOver<Tag>().Where(x => x.Name == name).RowCount() > 0;
                if (exists)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}
