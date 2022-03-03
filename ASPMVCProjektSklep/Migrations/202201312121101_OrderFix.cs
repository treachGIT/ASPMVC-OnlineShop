namespace ASPMVCProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderFix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Adresses", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Adresses", "User_Id");
            AddForeignKey("dbo.Adresses", "User_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Adresses", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Adresses", new[] { "User_Id" });
            DropColumn("dbo.Adresses", "User_Id");
        }
    }
}
