namespace ASPMVCProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbContextUpdated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartProducts",
                c => new
                    {
                        CartId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CartId, t.ProductId })
                .ForeignKey("dbo.Carts", t => t.CartId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.CartId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        Price = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 200),
                        status = c.String(),
                        creationDate = c.DateTime(nullable: false),
                        Category_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 30),
                        Source = c.String(nullable: false),
                        Product_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .Index(t => t.Product_Id);
            
            CreateTable(
                "dbo.Sales",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Discount = c.Single(nullable: false),
                        Code = c.String(nullable: false, maxLength: 30),
                        startDate = c.DateTime(nullable: false),
                        endDate = c.DateTime(nullable: false),
                        Product_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .Index(t => t.Product_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sales", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Images", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Products", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.CartProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.CartProducts", "CartId", "dbo.Carts");
            DropIndex("dbo.Sales", new[] { "Product_Id" });
            DropIndex("dbo.Images", new[] { "Product_Id" });
            DropIndex("dbo.Products", new[] { "Category_Id" });
            DropIndex("dbo.CartProducts", new[] { "ProductId" });
            DropIndex("dbo.CartProducts", new[] { "CartId" });
            DropTable("dbo.Sales");
            DropTable("dbo.Images");
            DropTable("dbo.Categories");
            DropTable("dbo.Products");
            DropTable("dbo.Carts");
            DropTable("dbo.CartProducts");
        }
    }
}
