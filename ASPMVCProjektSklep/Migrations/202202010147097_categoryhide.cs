namespace ASPMVCProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class categoryhide : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "status");
        }
    }
}
