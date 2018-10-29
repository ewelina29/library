using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Migrations
{
    [Migration(5)]
    public class _005_Tags : Migration
    {
        public override void Up()
        {
            Create.Table("Tags")
               .WithColumn("Id").AsInt32().Identity().PrimaryKey()
               .WithColumn("Name").AsString().Unique();
        }

        public override void Down()
        {
            Delete.Table("Tags");

        }
    }
}