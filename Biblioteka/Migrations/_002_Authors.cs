using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Migrations
{
    [Migration(2)]
    public class _002_Authors : Migration
    {
        public override void Up()
        {
            Create.Table("Authors")
               .WithColumn("Id").AsInt32().Identity().PrimaryKey()
               .WithColumn("Name").AsString()
               .WithColumn("Surname").AsString()
               .WithColumn("YearOfBirth").AsInt16();
        }

        public override void Down()
        {
            Delete.Table("Authors");
        }
    }
}