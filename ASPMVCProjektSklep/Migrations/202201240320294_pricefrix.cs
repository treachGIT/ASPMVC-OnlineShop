namespace ASPMVCProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pricefrix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OrderProducts", "Price", c => c.Single(nullable: false));
            AlterColumn("dbo.Products", "Price", c => c.Single(nullable: false));
            DropColumn("dbo.Sales", "Code");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sales", "Code", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.Products", "Price", c => c.Int(nullable: false));
            AlterColumn("dbo.OrderProducts", "Price", c => c.Int(nullable: false));
        }
    }
}
