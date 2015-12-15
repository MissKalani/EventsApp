namespace DataModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeDeleteForInviteLinks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InviteLinks", "EventId", "dbo.Events");
            AddForeignKey("dbo.InviteLinks", "EventId", "dbo.Events", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InviteLinks", "EventId", "dbo.Events");
            AddForeignKey("dbo.InviteLinks", "EventId", "dbo.Events", "Id");
        }
    }
}
