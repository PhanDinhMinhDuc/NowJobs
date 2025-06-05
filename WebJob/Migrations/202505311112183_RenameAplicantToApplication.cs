namespace WebJob.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameAplicantToApplication : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tb_JobApplication", "ApplicantID", "dbo.tb_Applicant");
            DropForeignKey("dbo.tb_Applicant", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.tb_Applicant", new[] { "UserId" });
            DropIndex("dbo.tb_JobApplication", new[] { "ApplicantID" });
            CreateTable(
                "dbo.tb_Application",
                c => new
                    {
                        ApplicationID = c.Int(nullable: false, identity: true),
                        FullName = c.String(maxLength: 255),
                        Email = c.String(maxLength: 255),
                        PhoneNumber = c.String(maxLength: 50),
                        CVFilePath = c.String(),
                        CoverLetter = c.String(),
                        ViewStatus = c.Int(nullable: false),
                        ApplicationStatus = c.Int(nullable: false),
                        FeebBack = c.String(maxLength: 1000),
                        IsActive = c.Boolean(nullable: false),
                        CandidateProfileID = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApplicationID)
                .ForeignKey("dbo.tb_CandidateProfile", t => t.CandidateProfileID)
                .Index(t => t.CandidateProfileID);
            
            AddColumn("dbo.tb_JobApplication", "Applications_ApplicationID", c => c.Int());
            CreateIndex("dbo.tb_JobApplication", "Applications_ApplicationID");
            AddForeignKey("dbo.tb_JobApplication", "Applications_ApplicationID", "dbo.tb_Application", "ApplicationID");
            DropTable("dbo.tb_Applicant");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.tb_Applicant",
                c => new
                    {
                        ApplicantID = c.Int(nullable: false, identity: true),
                        FullName = c.String(maxLength: 255),
                        Email = c.String(maxLength: 255),
                        PhoneNumber = c.String(maxLength: 50),
                        CVFilePath = c.String(),
                        CoverLetter = c.String(),
                        ViewStatus = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        FeebBack = c.String(maxLength: 1000),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApplicantID);
            
            DropForeignKey("dbo.tb_JobApplication", "Applications_ApplicationID", "dbo.tb_Application");
            DropForeignKey("dbo.tb_Application", "CandidateProfileID", "dbo.tb_CandidateProfile");
            DropIndex("dbo.tb_JobApplication", new[] { "Applications_ApplicationID" });
            DropIndex("dbo.tb_Application", new[] { "CandidateProfileID" });
            DropColumn("dbo.tb_JobApplication", "Applications_ApplicationID");
            DropTable("dbo.tb_Application");
            CreateIndex("dbo.tb_JobApplication", "ApplicantID");
            CreateIndex("dbo.tb_Applicant", "UserId");
            AddForeignKey("dbo.tb_Applicant", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.tb_JobApplication", "ApplicantID", "dbo.tb_Applicant", "ApplicantID", cascadeDelete: true);
        }
    }
}
