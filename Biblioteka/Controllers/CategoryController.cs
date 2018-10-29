using Biblioteka.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biblioteka.Controllers
{
    public class CategoryController : AuthorizationController
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
                    var categories = session.QueryOver<Category>().List();
                    ViewBag.Role = GetRole(session);

                    return View(categories);
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
        public ActionResult Create(Category category)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {

                    using (var transaction = session.BeginTransaction())
                    {
                        session.SaveOrUpdate(category);

                        transaction.Commit();
                        session.Close();

                    }
                }

                return RedirectToAction("Index");
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
                    var category = session.Get<Category>(id);
                    return View(category);
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
        public ActionResult Delete(int id, Category category)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        var categoryToDelete = session.Get<Tag>(id);
                        session.Delete(categoryToDelete);
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

        public JsonResult IsCategoryNameExist(string name)
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