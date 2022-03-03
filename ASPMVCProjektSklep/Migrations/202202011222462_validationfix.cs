namespace ASPMVCProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class validationfix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Adresses", "city", c => c.String(nullable: false));
            AlterColumn("dbo.Adresses", "street", c => c.String(nullable: false));
            AlterColumn("dbo.Adresses", "building", c => c.String(nullable: false));
            AlterColumn("dbo.Adresses", "apartment", c => c.String(nullable: false));
            AlterColumn("dbo.Adresses", "postalCode", c => c.String(nullable: false, maxLength: 10));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Adresses", "postalCode", c => c.String());
            AlterColumn("dbo.Adresses", "apartment", c => c.String());
            AlterColumn("dbo.Adresses", "building", c => c.String());
            AlterColumn("dbo.Adresses", "street", c => c.String());
            AlterColumn("dbo.Adresses", "city", c => c.String());
        }
    }
}
