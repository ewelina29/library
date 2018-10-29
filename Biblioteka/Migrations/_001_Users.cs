using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Migrations
{
    [Migration(1)]
    public class _001_Users : Migration
    {
        public override void Up()
        {
            Create.Table("Users")
               .WithColumn("Id").AsInt32().Identity().PrimaryKey()
               .WithColumn("UserName").AsString().Unique()
               .WithColumn("PasswordHash").AsString()
               .WithColumn("Role").AsInt16()
               .WithColumn("Email").AsString().Unique()
               .WithColumn("Name").AsString()
               .WithColumn("Surname").AsString();
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}