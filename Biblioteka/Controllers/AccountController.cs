using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Biblioteka.Models.Identity;
using System.Web;
using System.Web.Mvc;
using NHibernate;
using Biblioteka.Models;
using System;
using System.Diagnostics;
using Biblioteka.Controllers;
using System.Collections.Generic;
using Biblioteka.ModelViews;

namespace NHibernateAspNetIdentityExample.Controllers
{
    public class AccountController : AuthorizationController
    {

        public ActionResult Index()
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    ValidateReaderAndEmployeeForbiddenAccess(session);

                    var users = session.QueryOver<User>().List();
                    return View(users);


                }
            }
            catch (UnauthorizedAccessException)
            {

                return RedirectToAction("Forbidden", "Error");


            }
            catch (Exception)
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult ReadersIndex()
        {
            try
            {
                if (!IsLogged()) return RedirectToAction("Login");

                using (ISession session = database.MakeSession())
                {
                    ValidateReaderForbiddenAccess(session);

                    var readers = session.QueryOver<Reader>().List();
                    var readersData = new List<RegisterModelView>();

                    foreach(var reader in readers)
                    {
                        var userId = reader.User.Id;
                        var user = session.Get<User>(reader.User.Id);
                        var readerData = new RegisterModelView(user, reader);
                        readersData.Add(readerData);
                    }
                    return View(readersData);


                }
            }
            catch (UnauthorizedAccessException)
            {

                return RedirectToAction("Forbidden", "Error");


            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                return RedirectToAction("Login");
            }

        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = SignInManager.PasswordSignIn(model.UserName, model.Password, false, false);
                    if (result == SignInStatus.Success)
                    {
                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    }
                }
                return View(model);
            }

            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Register()
        {
            try
            {
                if (IsLogged())
                {
                    throw new UnauthorizedAccessException();
                }

                return View();
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
        public ActionResult Register(RegisterModelView model)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    if (ModelState.IsValid)
                    {
                        var user = new User()
                        {
                            UserName = model.UserName,
                            Email = model.Email,
                            Name = model.Name,
                            Surname = model.Surname,
                            Role = UserRole.READER_ROLE
                        };
                        var result = UserManager.Create(user, model.Password);
                        var reader = new Reader()
                        {
                            Pesel = model.Pesel,
                            Telephone = model.Telephone,
                            User = user,
                            RegistrationDate = DateTime.Now.ToString("dd.MM.yyyy")
                        };

                        if (result.Succeeded)
                        {
                            using (ITransaction transaction = session.BeginTransaction())
                            {
                                SignInManager.SignIn(user, false, false);
                                session.SaveOrUpdate(reader);
                                transaction.Commit();
                            }
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error);
                            }
                        }
                    }
                    return View(model);
                }
            }
            catch (Exception e)
            {
                return View();

            }

        }

        public ActionResult AddEmployee()
        {
            try
            {
                if (!IsLogged()) return RedirectToAction("Login", "Account");
                using (ISession session = database.MakeSession())
                {
                    ValidateReaderAndEmployeeForbiddenAccess(session);

                }
                return View();
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Forbidden", "Error");
            }
        }

        [HttpPost]
        public ActionResult AddEmployee(User model)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    if (ModelState.IsValid)
                    {

                        var user = new User()
                        {
                            UserName = model.UserName,
                            Email = model.Email,
                            Name = model.Name,
                            Surname = model.Surname,
                            Role = UserRole.EMPLOYEE_ROLE
                        };

                        var result = UserManager.Create(user, model.PasswordHash);
                        if (result.Succeeded)
                        {

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error);
                                Debug.WriteLine(error);
                            }
                        }

                    }
                    return View(model);

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    if (!IsLogged()) return RedirectToAction("Login");

                    //var userId = Int32.Parse(this.User.Identity.GetUserId());
                    var user = session.Get<User>(id);
                    if (user.Role == UserRole.READER_ROLE)
                    {
                        var reader = session.QueryOver<Reader>().Where(x => x.User == user).List<Reader>()[0];
                        var userData = new RegisterModelView(user, reader);
                        return View("EditReader", userData);

                    }
                    else
                    {
                        var userData = new RegisterModelView(user);
                        return View(userData);
                    }

                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");

            }

        }

        [HttpPost]
        public ActionResult Edit(int id, RegisterModelView model)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    //var userId = Int32.Parse(this.User.Identity.GetUserId());
                    var userData = session.Get<User>(id);
                    userData.UserName = model.UserName;
                    userData.Email = model.Email;

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.SaveOrUpdate(userData);
                        if (userData.Role == UserRole.READER_ROLE)
                        {
                            var readerData = session.QueryOver<Reader>().Where(x => x.User == userData).List<Reader>()[0];
                            readerData.Telephone = model.Telephone;
                            session.SaveOrUpdate(readerData);
                            session.Flush();

                            transaction.Commit();
                            return View("EditReader", model);
                        }
                        else
                        {
                            transaction.Commit();
                            return View(model);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                return View("Index", "Home");
            }

        }

        
        [HttpGet]
        public ActionResult LogOff()
        {
            try
            {
                SignInManager.SignOut();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    if (!IsLogged()) return RedirectToAction("Login");
                    //var userId = Int32.Parse(this.User.Identity.GetUserId());
                    var userData = session.Get<User>(id);
                    return View(userData);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Delete(int id, User user)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    //var userId = Int32.Parse(this.User.Identity.GetUserId());
                    var userToDelete = session.Get<User>(id);
                    SignInManager.SignOut();

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.Delete(userToDelete);
                        transaction.Commit();
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception exception)
            {
                return View();
            }
        }

        public ActionResult Cart()
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                using (ISession session = database.MakeSession()){

                    var booksInCart = new List<BookCartModelView>();
                    if (Session[SESSION_CART] != null)
                    {
                        var copiesIds = Session[SESSION_CART] as List<int>;
                        foreach(var id in copiesIds)
                        {
                            var copy = session.Get<Copy>(id);
                            var book = new BookCartModelView
                            {
                                CopyId = copy.Id,
                                Title = copy.Book.Title,
                                Author = copy.Book.Author.ToString()
                            };

                            booksInCart.Add(book);
                        }
                    }
                    return View(booksInCart);

            }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public SignInManager SignInManager
        {
            get { return HttpContext.GetOwinContext().Get<SignInManager>(); }
        }
        public UserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<UserManager>(); }
        }

        public JsonResult IsUserNameExist(string username)
        {
            bool exists = false;

            using (ISession session = database.MakeSession())
            {
                var userCounter = session.QueryOver<User>().Where(x => x.UserName == username).List<User>().Count;
                if (userCounter > 0)
                {
                    var existedId = session.QueryOver<User>().Where(x => x.UserName == username).List<User>()[0].Id;

                    exists = true;
                    var currentId = this.User.Identity.GetUserId();
                    if (currentId != null)
                    {
                        int currId = Int32.Parse(currentId);

                        if (currId > 0 && currId == existedId)
                        {
                            exists = false;
                        }
                    }

                }

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

        public JsonResult IsEmailExist(string email)
        {
            bool exists = false;

            using (ISession session = database.MakeSession())
            {
                var userCounter = session.QueryOver<User>().Where(x => x.Email == email).List<User>().Count;
                if (userCounter > 0)
                {
                    var existedId = session.QueryOver<User>().Where(x => x.Email == email).List<User>()[0].Id;

                    exists = true;
                    var currentId = this.User.Identity.GetUserId();
                    if (currentId != null)
                    {
                        int currId = Int32.Parse(currentId);

                        if (currId > 0 && currId == existedId)
                        {
                            exists = false;
                        }
                    }

                }

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