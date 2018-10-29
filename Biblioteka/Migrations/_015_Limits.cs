using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.Migrations
{
    [Migration(15)]
    public class _015_Limits : Migration
    {

        public override void Up()
        {
            Create.Table("Limits")
               .WithColumn("Id").AsInt32().Identity().PrimaryKey()
               .WithColumn("MaxDaysOfRental").AsInt16()
               .WithColumn("MaxAmountOfBooks").AsInt16();


            Insert.IntoTable("Limits").Row(new { MaxDaysOfRental = 30, MaxAmountOfBooks = 5 });
        }

        public override void Down()
        {
            Delete.Table("Limits");
        }
   
    }
}