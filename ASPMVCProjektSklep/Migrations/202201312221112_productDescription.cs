namespace ASPMVCProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productDescription : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "Description", c => c.String(nullable: false, maxLength: 2000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "Description", c => c.String(nullable: false, maxLength: 200));
        }
    }
}
