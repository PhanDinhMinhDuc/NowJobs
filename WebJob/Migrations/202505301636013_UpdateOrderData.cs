namespace WebJob.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateOrderData : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tb_Order", "Address");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_Order", "Address", c => c.String(nullable: false));
        }
    }
}
