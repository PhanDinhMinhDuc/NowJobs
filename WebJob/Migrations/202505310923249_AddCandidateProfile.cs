namespace WebJob.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCandidateProfile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_CandidateProfile",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        CVPath = c.String(),
                        ExperienceID = c.Int(),
                        PositionID = c.Int(),
                        LevelID = c.Int(),
                        LocationID = c.Int(),
                        IndustryID = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.tb_Experience", t => t.ExperienceID)
                .ForeignKey("dbo.tb_Level", t => t.LevelID)
                .ForeignKey("dbo.tb_Location", t => t.LocationID)
                .ForeignKey("dbo.tb_Position", t => t.PositionID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ExperienceID)
                .Index(t => t.PositionID)
                .Index(t => t.LevelID)
                .Index(t => t.LocationID);
            
            CreateTable(
                "dbo.tb_CandidateProfileSkill",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CandidateProfileID = c.Int(nullable: false),
                        SkillID = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.tb_CandidateProfile", t => t.CandidateProfileID, cascadeDelete: true)
                .ForeignKey("dbo.tb_JobSkill", t => t.SkillID, cascadeDelete: true)
                .Index(t => t.CandidateProfileID)
                .Index(t => t.SkillID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_CandidateProfile", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.tb_CandidateProfile", "PositionID", "dbo.tb_Position");
            DropForeignKey("dbo.tb_CandidateProfile", "LocationID", "dbo.tb_Location");
            DropForeignKey("dbo.tb_CandidateProfile", "LevelID", "dbo.tb_Level");
            DropForeignKey("dbo.tb_CandidateProfile", "ExperienceID", "dbo.tb_Experience");
            DropForeignKey("dbo.tb_CandidateProfileSkill", "SkillID", "dbo.tb_JobSkill");
            DropForeignKey("dbo.tb_CandidateProfileSkill", "CandidateProfileID", "dbo.tb_CandidateProfile");
            DropIndex("dbo.tb_CandidateProfileSkill", new[] { "SkillID" });
            DropIndex("dbo.tb_CandidateProfileSkill", new[] { "CandidateProfileID" });
            DropIndex("dbo.tb_CandidateProfile", new[] { "LocationID" });
            DropIndex("dbo.tb_CandidateProfile", new[] { "LevelID" });
            DropIndex("dbo.tb_CandidateProfile", new[] { "PositionID" });
            DropIndex("dbo.tb_CandidateProfile", new[] { "ExperienceID" });
            DropIndex("dbo.tb_CandidateProfile", new[] { "UserId" });
            DropTable("dbo.tb_CandidateProfileSkill");
            DropTable("dbo.tb_CandidateProfile");
        }
    }
}
