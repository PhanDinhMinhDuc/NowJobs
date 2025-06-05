namespace WebJob.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCandidateProfile1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tb_CandidateProfile", "LevelID", "dbo.tb_Level");
            DropIndex("dbo.tb_CandidateProfile", new[] { "LevelID" });
            AddColumn("dbo.tb_CandidateProfile", "JobCategoryID", c => c.Int());
            CreateIndex("dbo.tb_CandidateProfile", "JobCategoryID");
            AddForeignKey("dbo.tb_CandidateProfile", "JobCategoryID", "dbo.tb_JobCategory", "JobCategoryID");
            DropColumn("dbo.tb_CandidateProfile", "IndustryID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_CandidateProfile", "IndustryID", c => c.Int());
            DropForeignKey("dbo.tb_CandidateProfile", "JobCategoryID", "dbo.tb_JobCategory");
            DropIndex("dbo.tb_CandidateProfile", new[] { "JobCategoryID" });
            DropColumn("dbo.tb_CandidateProfile", "JobCategoryID");
            CreateIndex("dbo.tb_CandidateProfile", "LevelID");
            AddForeignKey("dbo.tb_CandidateProfile", "LevelID", "dbo.tb_Level", "LevelID");
        }
    }
}
