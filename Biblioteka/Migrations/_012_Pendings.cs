using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;
namespace Biblioteka.Migrations
{
    [Migration(12)]
    public class _012_Pendings : Migration
    {
        public override void Up()
        {
            Create.Table("Pendings")
               .WithColumn("Id").AsInt32().Identity().PrimaryKey()
               .WithColumn("CopyId").AsInt32().ForeignKey("Copies", "Id")
               .WithColumn("ReaderId").AsInt32().ForeignKey("Readers", "Id")
               .WithColumn("DateFrom").AsString()
               .WithColumn("DateTo").AsString();
        }

        public override void Down()
        {
            Delete.Table("Pendings");
        }
    }
}