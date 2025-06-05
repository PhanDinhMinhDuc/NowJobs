namespace WebJob.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeVerificationStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmployerVerifications", "Status", c => c.Int(nullable: false));
            DropColumn("dbo.EmployerVerifications", "IsVerified");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmployerVerifications", "IsVerified", c => c.Boolean(nullable: false));
            DropColumn("dbo.EmployerVerifications", "Status");
        }
    }
}
