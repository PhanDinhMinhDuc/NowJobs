namespace WebJob.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateJobDatabase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_CandidateProfile", "AvatarPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_CandidateProfile", "AvatarPath");
        }
    }
}
