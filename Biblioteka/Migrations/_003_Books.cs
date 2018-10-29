using FluentMigrator;


namespace Biblioteka.Migrations
{
    [Migration(3)]
    public class _003_Books : Migration
    {
        public override void Up()
        {
            Create.Table("Books")
               .WithColumn("Id").AsInt32().Identity().PrimaryKey()
               .WithColumn("AuthorId").AsInt32().ForeignKey("Authors", "Id")
               .WithColumn("Title").AsString().Unique()
               .WithColumn("ISBN").AsString()
               .WithColumn("TableOfContents").AsString()
               .WithColumn("Description").AsString()
               .WithColumn("CategoryId").AsString();
            
        }

        public override void Down()
        {
            Delete.Table("Books");
        }
    }
}