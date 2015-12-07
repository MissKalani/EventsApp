namespace DataModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOneTimeUseToInviteLinks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InviteLinks", "OneTimeUse", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InviteLinks", "OneTimeUse");
        }
    }
}
