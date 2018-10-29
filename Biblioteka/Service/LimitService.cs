using Biblioteka.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Service
{
    public class LimitService
    {
        private static LimitService instance = null;
        public DatabaseContext database;

        private Limit Limit;


        private LimitService()
        {
            this.database = new DatabaseContext();
            using (ISession session = database.MakeSession())
            {
                Limit = session.Get<Limit>(1);
            }
        }

        public static LimitService getInstance()
        {

            if (instance == null)
            {
                instance = new LimitService();
            }
            return instance;
        }

        public void setLimit(int MaxDaysOfRental, int MaxAmountOfBooks)
        {

            using (ISession session = database.MakeSession())
            {

                var limit = this.Limit;

                limit.MaxDaysOfRental = MaxDaysOfRental;
                limit.MaxAmountOfBooks = MaxAmountOfBooks;

                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(limit);
                    transaction.Commit();
                }
            }
        }


        public Limit getLimit()
        {
            return Limit;
        }
    }
}