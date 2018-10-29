using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;
namespace Biblioteka.Migrations
{
    [Migration(13)]
    public class _013_Resevations : Migration
    {
        public override void Up()
        {
            Create.Table("Reservations")
               .WithColumn("Id").AsInt32().Identity().PrimaryKey()
               .WithColumn("CopyId").AsInt32().ForeignKey("Copies", "Id")
               .WithColumn("ReaderId").AsInt32().ForeignKey("Readers", "Id")
               .WithColumn("DateFrom").AsString()
               .WithColumn("DateTo").AsString();
        }

        public override void Down()
        {
            Delete.Table("Reservations");
        }
    }
}