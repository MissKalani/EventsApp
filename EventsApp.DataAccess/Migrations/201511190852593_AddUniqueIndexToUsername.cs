namespace DataModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniqueIndexToUsername : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.AspNetUsers", "Username", true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUsers", "Username");
        }
    }
}
