using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;
namespace Biblioteka.Migrations
{
    [Migration(11)]
    public class _011_Rentals : Migration
    {
        public override void Up()
        {
            Create.Table("Rentals")
               .WithColumn("Id").AsInt32().Identity().PrimaryKey()
               .WithColumn("CopyId").AsInt32().ForeignKey("Copies", "Id")
               .WithColumn("ReaderId").AsInt32().ForeignKey("Readers", "Id")
               .WithColumn("DateFrom").AsString()
               .WithColumn("DateTo").AsString()
               .WithColumn("Deleted").AsBoolean()
               .WithColumn("CloseReturnMailSent").AsBoolean().WithDefaultValue(false).Nullable()
               .WithColumn("LateReturnMailSent").AsBoolean().WithDefaultValue(false).Nullable();

        }

        public override void Down()
        {
            Delete.Table("Rentals");
        }
    }
}