using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Migrations
{
    [Migration(8)]
    public class _008_Readers : Migration
    {
        public override void Up()
        {
            Create.Table("Readers")
               .WithColumn("Id").AsInt32().Identity().PrimaryKey()
               .WithColumn("Pesel").AsString()
               .WithColumn("Telephone").AsString()
               .WithColumn("RegistrationDate").AsString()
               .WithColumn("UserId").AsInt32();
        }

        public override void Down()
        {
            Delete.Table("Readers");
        }
    }
}