namespace ASPMVCProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixDisplayName : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Images", "Source", c => c.String(nullable: false, maxLength: 700));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Images", "Source", c => c.String(nullable: false));
        }
    }
}
