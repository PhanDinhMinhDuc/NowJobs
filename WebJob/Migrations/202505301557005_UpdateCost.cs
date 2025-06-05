namespace WebJob.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_Product", "Cost", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.tb_Product", "OriginalPrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_Product", "OriginalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.tb_Product", "Cost");
        }
    }
}
