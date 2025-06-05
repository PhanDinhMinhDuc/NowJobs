namespace WebJob.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateJobApplicationRelatiionship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tb_JobApplication", "Applications_ApplicationID", "dbo.tb_Application");
            DropIndex("dbo.tb_JobApplication", new[] { "Applications_ApplicationID" });
            RenameColumn(table: "dbo.tb_JobApplication", name: "Applications_ApplicationID", newName: "ApplicationID");
            AlterColumn("dbo.tb_JobApplication", "ApplicationID", c => c.Int(nullable: false));
            CreateIndex("dbo.tb_JobApplication", "ApplicationID");
            AddForeignKey("dbo.tb_JobApplication", "ApplicationID", "dbo.tb_Application", "ApplicationID", cascadeDelete: true);
            DropColumn("dbo.tb_JobApplication", "ApplicantID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_JobApplication", "ApplicantID", c => c.Int(nullable: false));
            DropForeignKey("dbo.tb_JobApplication", "ApplicationID", "dbo.tb_Application");
            DropIndex("dbo.tb_JobApplication", new[] { "ApplicationID" });
            AlterColumn("dbo.tb_JobApplication", "ApplicationID", c => c.Int());
            RenameColumn(table: "dbo.tb_JobApplication", name: "ApplicationID", newName: "Applications_ApplicationID");
            CreateIndex("dbo.tb_JobApplication", "Applications_ApplicationID");
            AddForeignKey("dbo.tb_JobApplication", "Applications_ApplicationID", "dbo.tb_Application", "ApplicationID");
        }
    }
}
