namespace ASPMVCProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Sales", "Discount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sales", "Discount", c => c.Single(nullable: false));
        }
    }
}
