using Biblioteka.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Biblioteka.Models.Identity;
using Microsoft.AspNet.Identity;
using Biblioteka.ModelViews;
using NHibernate.Criterion;
using System.Net;
using System.IO;

namespace Biblioteka.Controllers
{
    public class BookController : AuthorizationController
    {

        // GET: Book
        public ActionResult Index(string titleText, string authorText, string categoryText, FormCollection form)
        {
            using (ISession session = database.MakeSession())
            {
                ViewBag.Role = GetRole(session);

                Category catAlias = null;
                Author authorAlias = null;

                IList<Book> books;
                //@TODO add searching tags
                List<int> tagsIds = new List<int>();
                if (form["tags"] != null)
                {
                    foreach (string tag in form["tags"].Split(','))
                    {
                        tagsIds.Add(Convert.ToInt32(tag));
                    }
                    books = session.CreateCriteria<Book>()
                    .CreateCriteria("Tags")
                    .Add(Restrictions.In("id", tagsIds))
                    .List<Book>();

                    books = books.Distinct().ToList();
                }

                else
                {
                    books = session.QueryOver<Book>()
                                   .JoinAlias(x => x.Category, () => catAlias)
                                   .JoinAlias(x => x.Author, () => authorAlias)
                                   .Where(Restrictions.Like("Title", "%" + titleText + "%"))
                                   .Where(Restrictions.Like("catAlias.Name", "%" + categoryText + "%"))
                                   .Where(Restrictions.Like("authorAlias.Surname", "%" + authorText + "%"))
                                   .List();

                }


                //It's for display all needed fields. It's not worked without BookIndexView
                List<BookIndexView> data = new List<BookIndexView>();
                BookIndexView bv;
                foreach (var book in books)
                {
                    bv = new BookIndexView();
                    bv.Id = book.Id;
                    bv.Title = book.Title;
                    bv.Author = book.Author.ToString();
                    bv.Category = book.Category.Name;

                    data.Add(bv);
                }
                ViewBag.Tagslist = GetTags(null);


                return View(data);
            }

        }

        private MultiSelectList GetTags(string[] selectedValues)
        {
            using (ISession session = database.MakeSession())
            {
                var Tags = session.QueryOver<Tag>().List();

                int i = 0;


                return new MultiSelectList(Tags, "Id", "Name", selectedValues);
            }

        }

        public ActionResult Create()
        {
            try
            {
                if (!IsLogged()) return RedirectToAction("Login", "Account");
                List<SelectListItem> authorItems = new List<SelectListItem>();
                List<SelectListItem> categoryItems = new List<SelectListItem>();

                using (ISession session = database.MakeSession())
                {
                    ValidateReaderForbiddenAccess(session);
                    var authors = session.QueryOver<Author>().List();

                    foreach (Author author in authors)
                    {
                        authorItems.Add(new SelectListItem { Text = author.Name + " " + author.Surname, Value = author.Id.ToString() });
                    }

                    var categories = session.QueryOver<Category>().List();

                    foreach (Category category in categories)
                    {
                        categoryItems.Add(new SelectListItem { Text = category.Name, Value = category.Id.ToString() });
                    }

                    ViewBag.Role = GetRole(session);

                }

                ViewBag.Tagslist = GetTags(null);

                ViewBag.AuthorType = authorItems;
                ViewBag.CategoryType = categoryItems;

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
        public ActionResult Create(Book book, int AuthorType, int CategoryType, FormCollection form, HttpPostedFileBase file)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        ViewBag.AuthorType = null;
                        ViewBag.Role = GetRole(session);

                        //@TODO we must check if this author exist
                        book.Author = session.Get<Author>(AuthorType);
                        book.Category = session.Get<Category>(CategoryType);

                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            book.TableOfContents = fileName;
                        }
                        else
                        {
                            book.TableOfContents = "";
                        }

                        var tags = form["Tags"];
                        if (tags != null)
                        {
                            foreach (string tag in tags.Split(','))
                            {
                                book.Tags.Add(session.Get<Tag>(Convert.ToInt32(tag)));
                            }
                        }

                        session.Save(book);
                        // when we add to many to many relationship 
                        session.Flush();

                        transaction.Commit();

                    }
                }

                //Save pdf is added
                string directory = "~/App_Data/TableContents/";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(Server.MapPath(directory));

                }
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    Debug.WriteLine(Path.Combine(directory, fileName));

                    file.SaveAs(Server.MapPath(directory + fileName));
                }

                //It's not return View(), because we want to clear saved data in form
                return RedirectToAction("Create", "Book");
            }
            catch (Exception)
            {
                return RedirectToAction("Create", "Book");

            }

        }

        public ActionResult Details(int id)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    var book = session.Get<Book>(id);

                    ViewBag.Author = book.Author.ToString();
                    ViewBag.Category = book.Category.Name;


                    string tags = "";
                    foreach (var tag in book.Tags)
                    {
                        if (tags == "") tags = tags + tag.Name;
                        else tags = tags + ", " + tag.Name;
                    }
                    ViewBag.Tags = tags;
                    ViewBag.Role = GetRole(session);

                    var assignedCopies = session.QueryOver<Copy>().Where(x => x.Book == book).List();
                    ViewBag.Ids = assignedCopies;
                    ViewBag.Added = WasBookAdded(id);

                    var availableCopies = session.QueryOver<Copy>().Where(x => x.Book == book).Where(x => x.Status == Enums.Status.IN_STOCK).List();
                    ViewBag.CopiesCount = assignedCopies.Count();
                    return View(book);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index");

            }


        }

        [HttpPost]
        public ActionResult Details(int id, string cart, string queue, string download)
        {
            try
            {
                if (!String.IsNullOrEmpty(cart))
                {
                    // if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                    return RedirectToAction("AddToCart", new { copyId = cart, bookId = id });
                }
                else if (!String.IsNullOrEmpty(queue))
                {
                    // if (!IsLogged()) { return RedirectToAction("Login", "Account"); }
                    return RedirectToAction("AddToQueue", new { copyId = queue, bookId = id });
                }
                else if (!String.IsNullOrEmpty(download))
                {

                    return RedirectToAction("DownloadTableOfContent", new { bookId = id });
                }
                else
                {
                    return View();
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult DownloadTableOfContent(int bookId)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    var book = session.Get<Book>(bookId);
                    var namePdf = book.TableOfContents;

                    string pdfPath = Server.MapPath(string.Format("~/App_Data/TableContents/{0}", namePdf));
                    Debug.WriteLine(pdfPath);
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}", namePdf));
                    Response.TransmitFile(pdfPath);
                    Response.End();
                }
            }
            catch (Exception)
            {

            }


            return RedirectToAction("Details", new { id = bookId });

        }

        public ActionResult AssignCopies(int id)
        {
            try
            {
                if (!IsLogged()) return RedirectToAction("Login", "Account");
                using (ISession session = database.MakeSession())
                {
                    ValidateReaderForbiddenAccess(session);

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        var book = session.Get<Book>(id);
                        //LinkedList<int> Ids = new LinkedList<int>();

                        var currentCopies = session.QueryOver<Copy>().Where(x => x.Book == book).List();
                        ViewBag.Ids = currentCopies;
                        ViewBag.Author = book.Author.ToString();

                        return View(book);
                    }
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
        public ActionResult AssignCopies(int id, int numberOfCopies)

        {

            try
            {
                using (ISession session = database.MakeSession())
                {
                    Book book = session.Get<Book>(id);
                    //LinkedList<int> Ids = new LinkedList<int>();

                    var currentCopies = session.QueryOver<Copy>().Where(x => x.Book == book).List();

                    ViewBag.Ids = currentCopies;



                    using (ITransaction transaction = session.BeginTransaction())
                    {

                        Copy copy;
                        int number = numberOfCopies;
                        for (int i = 0; i < number; i++)
                        {
                            copy = new Copy()
                            {
                                Book = book,
                                Status = Enums.Status.IN_STOCK
                            };
                            session.Save(copy);
                            currentCopies.Add(copy);

                        }
                        session.Flush();

                        transaction.Commit();

                    }
                    ViewBag.Ids = currentCopies;
                    return View(book);

                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return RedirectToAction("Index");
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
                    var book = session.Get<Book>(id);
                    return View(book);
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
        public ActionResult Delete(int id, Book book)
        {
            try
            {
                using (ISession session = database.MakeSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        var bookToDelete = session.Get<Book>(id);
                        session.Delete(bookToDelete);
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

        [HttpGet]
        public ActionResult AddToCart(int copyId, int bookId)
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }

                if (Session[SESSION_CART] != null)
                {
                    var booksInCart = Session[SESSION_CART] as List<int>;
                    if (!WasBookAdded(bookId, booksInCart))
                    {
                        booksInCart.Add(copyId);

                    }
                    else
                    {
                        Debug.Write("KOPIA DODANA");
                    }
                }
                else
                {
                    List<int> BooksInCart = new List<int>
                    {
                        copyId
                    };
                    Session[SESSION_CART] = BooksInCart;

                }

                //mozna dodac jakiegos pop-upa DODANO DO KOSZYKA
                return RedirectToAction("Details", new { id = bookId, added = true });

            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        private bool WasCopyAdded(int copyId, List<int> ids)
        {
            foreach (var id in ids)
            {
                if (id == copyId)
                {
                    return true;
                }
            }
            return false;
        }

        private bool WasBookAdded(int bookId)
        {
            var copiesInCart = Session[SESSION_CART] as List<int>;
            return WasBookAdded(bookId, copiesInCart);
        }

        private bool WasBookAdded(int bookId, List<int> copiesInCart)
        {
            using (ISession session = database.MakeSession())
            {
                if (copiesInCart != null)
                {
                    var bookToAdd = session.Get<Book>(bookId);
                    foreach (var copyId in copiesInCart)
                    {
                        var copy = session.Get<Copy>(copyId);
                        var book = copy.Book;
                        if (book == bookToAdd)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        [HttpGet]
        public ActionResult AddToQueue(int copyId, int bookId)
        {
            try
            {
                if (!IsLogged()) { return RedirectToAction("Login", "Account"); }

                using (ISession session = database.MakeSession())
                {

                    var copy = session.Get<Copy>(copyId);
                    var reader = GetReader(session);
                    var timeLimit = 3; //@TODO zmienic na pobieranie z limitow~!!!!

                    var checkPending = session.QueryOver<Pending>().Where(Restrictions.Eq("Copy.Id", copyId)).List();


                    var pending = new Pending()
                    {
                        Copy = copy,
                        Reader = reader,
                        DateFrom = DateTime.Today.ToString("dd.MM.yyyy"),
                        DateTo = DateTime.Today.AddDays(timeLimit).ToString("dd.MM.yyyy")
                    };

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.SaveOrUpdate(pending);
                        session.Flush();
                        transaction.Commit();
                    }

                }
                return RedirectToAction("Details", new { id = bookId, added = true });

            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        public JsonResult IsTitleExist(string title)
        {
            using (ISession session = database.MakeSession())
            {
                bool exists = session.QueryOver<Book>().Where(x => x.Title == title).RowCount() > 0;
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