namespace WebJob.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmployerVerificationProductRelationShip : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_EmployerVerificationProduct",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployerVerificationId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PurchaseDate = c.DateTime(nullable: false),
                        TransactionId = c.String(),
                        PaymentMethod = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmployerVerifications", t => t.EmployerVerificationId, cascadeDelete: true)
                .ForeignKey("dbo.tb_Product", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.EmployerVerificationId)
                .Index(t => t.ProductId);
            
            AddColumn("dbo.EmployerVerifications", "PostRemain", c => c.Int(nullable: false));
            AddColumn("dbo.EmployerVerifications", "ValidityDays", c => c.Int(nullable: false));
            AddColumn("dbo.EmployerVerifications", "ExpiryDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_EmployerVerificationProduct", "ProductId", "dbo.tb_Product");
            DropForeignKey("dbo.tb_EmployerVerificationProduct", "EmployerVerificationId", "dbo.EmployerVerifications");
            DropIndex("dbo.tb_EmployerVerificationProduct", new[] { "ProductId" });
            DropIndex("dbo.tb_EmployerVerificationProduct", new[] { "EmployerVerificationId" });
            DropColumn("dbo.EmployerVerifications", "ExpiryDate");
            DropColumn("dbo.EmployerVerifications", "ValidityDays");
            DropColumn("dbo.EmployerVerifications", "PostRemain");
            DropTable("dbo.tb_EmployerVerificationProduct");
        }
    }
}
