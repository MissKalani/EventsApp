namespace DataModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedInvites : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Events", name: "AppUserId", newName: "OwnerId");
            RenameIndex(table: "dbo.Events", name: "IX_AppUserId", newName: "IX_OwnerId");
            CreateTable(
                "dbo.InviteLinks",
                c => new
                    {
                        LinkGUID = c.String(nullable: false, maxLength: 128),
                        EventId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LinkGUID)
                .ForeignKey("dbo.Events", t => t.EventId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.Invites",
                c => new
                    {
                        EventId = c.Int(nullable: false),
                        AppUserId = c.String(nullable: false, maxLength: 128),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EventId, t.AppUserId })
                .ForeignKey("dbo.AspNetUsers", t => t.AppUserId)
                .ForeignKey("dbo.Events", t => t.EventId)
                .Index(t => t.EventId)
                .Index(t => t.AppUserId);
            
            AddColumn("dbo.Events", "Visibility", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invites", "EventId", "dbo.Events");
            DropForeignKey("dbo.Invites", "AppUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.InviteLinks", "EventId", "dbo.Events");
            DropIndex("dbo.Invites", new[] { "AppUserId" });
            DropIndex("dbo.Invites", new[] { "EventId" });
            DropIndex("dbo.InviteLinks", new[] { "EventId" });
            DropColumn("dbo.Events", "Visibility");
            DropTable("dbo.Invites");
            DropTable("dbo.InviteLinks");
            RenameIndex(table: "dbo.Events", name: "IX_OwnerId", newName: "IX_AppUserId");
            RenameColumn(table: "dbo.Events", name: "OwnerId", newName: "AppUserId");
        }
    }
}
