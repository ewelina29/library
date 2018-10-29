using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;
namespace Biblioteka.Migrations
{
    [Migration(14)]
    public class _014_Fines : Migration
    {
        public override void Up()
        {
            Create.Table("Fines")
               .WithColumn("Id").AsInt32().Identity().PrimaryKey()
               .WithColumn("ReaderId").AsInt32().ForeignKey("Readers", "Id")
               .WithColumn("Amount").AsFloat()
               .WithColumn("Description").AsString()
               .WithColumn("Deleted").AsBoolean()
               .WithColumn("CopyId").AsInt32().ForeignKey("Copies", "Id");

        }

        public override void Down()
        {
            Delete.Table("Fines");
        }
    }
}