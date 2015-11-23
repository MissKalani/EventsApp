namespace DataModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInviteSeen : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invites", "Seen", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invites", "Seen");
        }
    }
}
