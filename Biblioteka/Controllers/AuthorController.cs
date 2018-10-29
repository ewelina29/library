using Biblioteka.Models;
using Biblioteka.Models.Identity;
using Microsoft.AspNet.Identity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biblioteka.Controllers
{
    public class AuthorController : AuthorizationController
    {
        public ActionResult Index()
        {
            using (ISession session = database.MakeSession())
            {
                var authors = session.QueryOver<Author>().List();
                ViewBag.Role = GetRole(session);

                return View(authors);
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
                    
                    return View("Create");
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
        public ActionResult Create(Author author)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        session.SaveOrUpdate(author);

                        transaction.Commit();
                        session.Close();

                    }
                }

                return Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception e)
            {
                return View(author);
            }

        }

        public ActionResult Details(int id)
        {
            using (ISession session = database.MakeSession())
            {
                var author = session.Get<Author>(id);
                return View(author);
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    if (!IsLogged()) return RedirectToAction("Login", "Account");

                    ValidateReaderForbiddenAccess(session);


                    var author = session.Get<Author>(id);
                    return View(author);
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
        public ActionResult Edit(int id, Author author)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    var authorData = session.Get<Author>(id);

                    authorData.Name = author.Name;
                    authorData.Surname = author.Surname;
                    authorData.YearOfBirth = author.YearOfBirth;

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.Save(authorData);
                        transaction.Commit();
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }



        public ActionResult Delete(int id)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    if (!IsLogged()) return RedirectToAction("Login", "Account");

                    ValidateReaderForbiddenAccess(session);


                    var author = session.Get<Author>(id);
                    return View(author);
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
        public ActionResult Delete(int id, Author author)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        var authorToDelete = session.Get<Author>(id);
                        session.Delete(authorToDelete);
                        transaction.Commit();
                        session.Close();
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception )
            {
                return View();
            }
        }
    }
}