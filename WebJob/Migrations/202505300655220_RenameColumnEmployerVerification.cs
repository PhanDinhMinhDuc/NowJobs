namespace WebJob.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameColumnEmployerVerification : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.EmployerVerifications", name: "AccountId", newName: "UserId");
            RenameIndex(table: "dbo.EmployerVerifications", name: "IX_AccountId", newName: "IX_UserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.EmployerVerifications", name: "IX_UserId", newName: "IX_AccountId");
            RenameColumn(table: "dbo.EmployerVerifications", name: "UserId", newName: "AccountId");
        }
    }
}
