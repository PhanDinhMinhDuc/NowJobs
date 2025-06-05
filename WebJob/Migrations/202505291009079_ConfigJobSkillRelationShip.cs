namespace WebJob.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConfigJobSkillRelationShip : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tb_Job", "JobSkillID", "dbo.tb_JobSkill");
            DropIndex("dbo.tb_Job", new[] { "JobSkillID" });
            CreateTable(
                "dbo.tb_Job_JobSkill",
                c => new
                    {
                        JobId = c.Int(nullable: false),
                        JobSkillId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.JobId, t.JobSkillId })
                .ForeignKey("dbo.tb_Job", t => t.JobId, cascadeDelete: true)
                .ForeignKey("dbo.tb_JobSkill", t => t.JobSkillId, cascadeDelete: true)
                .Index(t => t.JobId)
                .Index(t => t.JobSkillId);
            
            DropColumn("dbo.tb_Job", "JobSkillID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_Job", "JobSkillID", c => c.Int());
            DropForeignKey("dbo.tb_Job_JobSkill", "JobSkillId", "dbo.tb_JobSkill");
            DropForeignKey("dbo.tb_Job_JobSkill", "JobId", "dbo.tb_Job");
            DropIndex("dbo.tb_Job_JobSkill", new[] { "JobSkillId" });
            DropIndex("dbo.tb_Job_JobSkill", new[] { "JobId" });
            DropTable("dbo.tb_Job_JobSkill");
            CreateIndex("dbo.tb_Job", "JobSkillID");
            AddForeignKey("dbo.tb_Job", "JobSkillID", "dbo.tb_JobSkill", "Id");
        }
    }
}
