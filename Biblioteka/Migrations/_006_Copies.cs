using FluentMigrator;


namespace Biblioteka.Migrations
{
    [Migration(6)]
    public class _006_Copies : Migration
    {
        public override void Up()
        {
            Create.Table("Copies")
               .WithColumn("Id").AsInt32().Identity().PrimaryKey()
               .WithColumn("BookId").AsInt32().ForeignKey("Books", "Id")
               .WithColumn("Status").AsString();

        }

        public override void Down()
        {
            Delete.Table("Copies");
        }
    }
}