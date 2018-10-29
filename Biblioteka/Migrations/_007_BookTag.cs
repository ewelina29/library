using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Migrations
{
    [Migration(7)]
    public class _007_BookTag :Migration
    {
        public override void Up()
        {
            Create.Table("BookTag")
               .WithColumn("Id").AsInt32().Identity().PrimaryKey()
               .WithColumn("BookId").AsInt32().ForeignKey("Books", "Id")
               .WithColumn("TagId").AsInt32().ForeignKey("Tags", "Id");
             

        }

        public override void Down()
        {
            Delete.Table("BookTag");
        }
    }
}
