namespace WebJob.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteCandidateLevel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tb_CandidateProfile", "LevelID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_CandidateProfile", "LevelID", c => c.Int());
        }
    }
}
