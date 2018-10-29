using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Migrations
{
    [Migration(4)]
    public class _004_Categories : Migration
    {
        public override void Up()
        {
            Create.Table("Categories")
               .WithColumn("Id").AsInt32().Identity().PrimaryKey()
               .WithColumn("Name").AsString().Unique();
        }

        public override void Down()
        {
            Delete.Table("Categories");
        }
    }
}