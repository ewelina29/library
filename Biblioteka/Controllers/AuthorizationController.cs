using Biblioteka.Models;
using Biblioteka.Models.Identity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Biblioteka.Controllers
{
    public class AuthorizationController : Controller
    {
        public DatabaseContext database;
        public const string SESSION_CART = "cart";
        public const string SESSION_QUEUE = "queue";


        protected AuthorizationController()
        {
            this.database = new DatabaseContext();
        }

        protected bool IsLogged()
        {
            return this.User.Identity.IsAuthenticated;
        }

        protected void ValidateReaderForbiddenAccess(ISession session)
        {
            if (IsLogged())
            {
                var userId = Int32.Parse(this.User.Identity.GetUserId());
                var user = session.Get<User>(userId);
                if (user.Role == UserRole.READER_ROLE)
                {

                    throw new UnauthorizedAccessException();
                }
            }
            return;
        }

        protected void ValidateReaderAndEmployeeForbiddenAccess(ISession session)
        {
            if (IsLogged())
            {
                var userId = Int32.Parse(this.User.Identity.GetUserId());
                var user = session.Get<User>(userId);
                if (user.Role == UserRole.READER_ROLE || user.Role == UserRole.EMPLOYEE_ROLE)
                {

                    throw new UnauthorizedAccessException();
                }
                
            }
            return;


        }
        protected void ValidateAdminAndEmployeeForbiddenAccess(ISession session)
        {
            if (IsLogged())
            {
                var userId = Int32.Parse(this.User.Identity.GetUserId());
                var user = session.Get<User>(userId);
                if (user.Role == UserRole.ADMIN_ROLE || user.Role == UserRole.EMPLOYEE_ROLE)
                {

                    throw new UnauthorizedAccessException();
                }

            }
            return;
        }

        protected Reader GetReader(ISession session)
        {
            var userId = Int32.Parse(this.User.Identity.GetUserId());
            var user = session.Get<User>(userId);
            var reader = session.QueryOver<Reader>().Where(x => x.User == user).List()[0];
            return reader;

        }

        protected void DeleteFromSession(int copyId)
        {
            var copiesInCart = Session[SESSION_CART] as List<int>;
            copiesInCart.Remove(copyId);
            Session[SESSION_CART] = copiesInCart;
        }

        public void ClearSession()
        {
            var copiesInCart = Session[SESSION_CART] as List<int>;
            copiesInCart.Clear();
            Session[SESSION_CART] = copiesInCart;
        }

        protected bool IsRentalLate(Rental rental)
        {
            var dateTo = DateTime.Parse(rental.DateTo);
            var today = DateTime.Today;
            if (dateTo < today && !rental.LateReturnMailSent)
            {
                return true;
            }
            return false;
        }
       
        protected bool IsRentalCloseToReturn(Rental rental)
        {
            var dateTo = DateTime.Parse(rental.DateTo);
            var today = DateTime.Today;
            if (dateTo.AddDays(-3) < today && !rental.CloseReturnMailSent && !rental.LateReturnMailSent)
            {
                return true;
            }
            return false;
        }


        protected int GetRole(ISession session)
        {
            if (IsLogged())
            {
                var userId = Int32.Parse(User.Identity.GetUserId());
                var user = session.Get<User>(userId);
                return user.Role;
            }
            return 0;
        }
    }

    
}