using Biblioteka.Models;
using Biblioteka.Models.Identity;
using FizzWare.NBuilder;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biblioteka.Controllers
{
    public class HomeController : Controller
    {

        DatabaseContext database = new DatabaseContext();

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Notice");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public UserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<UserManager>(); }
        }


        private IList<Tag> createTags()
        {
            return Builder<Tag>.CreateListOfSize(10)
              .All()
              .With(c => c.Name = Faker.Name.First())
              .Build();

        }

        private IList<Category> createCategories()
        {
            return Builder<Category>.CreateListOfSize(10)
                .All()
                .With(c => c.Name = Faker.Name.First())
                .Build();

        }

        private IList<Author> createAuthors()
        {

            var yearGenerator = new RandomGenerator();

            return Builder<Author>.CreateListOfSize(10)
            .All()
            .With(c => c.Name = Faker.Name.First())
            .With(c => c.Surname = Faker.Name.Last())
            .With(c => c.YearOfBirth = yearGenerator.Next(1950, 2000))
            .Build();
        }

        private IList<Notice> createNotices()
        {
            return Builder<Notice>.CreateListOfSize(5)
            .All()
            .With(c => c.Title = Faker.Name.First())
            .With(c => c.Description = Faker.Lorem.Sentence())
            .Build();
        }


        private User createUser(string UserName, int Role)
        {
            return new User()
            {
                UserName = UserName,
                Email = Faker.Internet.Email(),
                Name = Faker.Name.First(),
                Surname = Faker.Name.Last(),
                Role = Role
            };
        }

        private IList<Book> createBooks(IList<Author> authors, IList<Category> categories)
        {
            return Builder<Book>.CreateListOfSize(5)
      .All()
      .With(c => c.Title = Faker.Company.Name())
      .With(c => c.ISBN = Convert.ToString(Faker.RandomNumber.Next(10000)))
      .With(c => c.Author = Pick<Author>.RandomItemFrom(authors))
      .With(c => c.Description = Faker.Lorem.Paragraph())
      .With(c => c.Category = Pick<Category>.RandomItemFrom(categories))

      .Build();
        }

        public string Seed()
        {

            var user = createUser("user", UserRole.READER_ROLE);
            var reader = new Reader()
            {
                Pesel = "12345678912",
                Telephone = Faker.Phone.Number(),
                User = user,
                RegistrationDate = DateTime.Now.ToString()
            };

            var admin = createUser("admin", UserRole.ADMIN_ROLE);

            var employee = createUser("employee", UserRole.EMPLOYEE_ROLE);


            using (ISession session = database.MakeSession())
            {


                using (var transaction = session.BeginTransaction())
                {

                    UserManager.Create(user, "Haslo12345");
                    session.Save(reader);
                    UserManager.Create(admin, "Haslo12345");
                    UserManager.Create(employee, "Haslo12345");


                    foreach (var tag in createTags())
                    {
                        session.Save(tag);
                    }
                    transaction.Commit();


                }

                var categories = createCategories();

                using (var transaction = session.BeginTransaction())
                {

                    foreach (var category in categories)
                    {
                        session.Save(category);
                    }
                    transaction.Commit();

                }

                var authors = createAuthors();

                using (var transaction = session.BeginTransaction())
                {

                    foreach (var author in authors)
                    {
                        session.Save(author);
                    }
                    transaction.Commit();

                }

                var notices= createNotices();

                using (var transaction = session.BeginTransaction())
                {

                    foreach (var notice in notices)
                    {
                        session.Save(notice);
                    }
                    transaction.Commit();

                }


                using (var transaction = session.BeginTransaction())
                {

                    foreach (var book in createBooks(authors, categories))
                    {
                        session.Save(book);
                    }
                    transaction.Commit();

                }

                session.Close();
            }

            return "Gotowe, seedy załadowane.";
        }
    }
}