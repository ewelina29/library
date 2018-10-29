using FluentMigrator;

namespace Biblioteka.Migrations
{
    [Migration(10)]
    public class _010_Notices : Migration
    {
        public override void Up()
        {
            Create.Table("Notices")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey()
                .WithColumn("Title").AsString().NotNullable()
                .WithColumn("Description").AsString(1000).NotNullable();
        }
        public override void Down()
        {
            Delete.Table("Notices");
        }
    }
}