using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using FluentNHibernate.Automapping;
//using System.Configuration;
using Biblioteka.Models.Identity;
using Microsoft.AspNet.Identity;
using Bialjam.Models.Identity;
using System.Configuration;

namespace Biblioteka.Models
{
    public class DatabaseContext
    {
        private readonly ISessionFactory sessionFactory;

        public DatabaseContext()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connectionString))
                .Mappings(m =>
                {
                    m.FluentMappings.AddFromAssemblyOf<DatabaseContext>();
                    //m.FluentMappings.AddFromAssemblyOf<Author>();
                })

                  .BuildSessionFactory();
        }
        public ISession MakeSession()
        {
            return sessionFactory.OpenSession();
        }

        public IUserStore<User, int> Users
        {
            get { return new IdentityStore(MakeSession()); }
        }

        
    }
}


