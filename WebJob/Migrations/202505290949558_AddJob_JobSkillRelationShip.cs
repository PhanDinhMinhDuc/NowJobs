namespace WebJob.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddJob_JobSkillRelationShip : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_Job", "JobSkillID", c => c.Int());
            CreateIndex("dbo.tb_Job", "JobSkillID");
            AddForeignKey("dbo.tb_Job", "JobSkillID", "dbo.tb_JobSkill", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_Job", "JobSkillID", "dbo.tb_JobSkill");
            DropIndex("dbo.tb_Job", new[] { "JobSkillID" });
            DropColumn("dbo.tb_Job", "JobSkillID");
        }
    }
}
