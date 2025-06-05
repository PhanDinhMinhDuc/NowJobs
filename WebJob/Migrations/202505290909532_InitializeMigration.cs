namespace WebJob.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitializeMigration : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.ApplicantID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.tb_JobApplication",
                c => new
                    {
                        JobApplicationID = c.Int(nullable: false, identity: true),
                        JobID = c.Int(nullable: false),
                        ApplicantID = c.Int(nullable: false),
                        CoverLetter = c.String(),
                        Note = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobApplicationID)
                .ForeignKey("dbo.tb_Applicant", t => t.ApplicantID, cascadeDelete: true)
                .ForeignKey("dbo.tb_Job", t => t.JobID, cascadeDelete: true)
                .Index(t => t.JobID)
                .Index(t => t.ApplicantID);
            
            CreateTable(
                "dbo.tb_Job",
                c => new
                    {
                        JobID = c.Int(nullable: false, identity: true),
                        JobTitle = c.String(nullable: false, maxLength: 255),
                        CompanyID = c.Int(),
                        SalaryID = c.Int(),
                        PositionID = c.Int(),
                        ExperienceID = c.Int(),
                        LocationID = c.Int(),
                        IndustryID = c.Int(),
                        LevelID = c.Int(),
                        JobCategoryID = c.Int(),
                        ViewCount = c.Int(nullable: false),
                        Alias = c.String(),
                        JobDescription = c.String(),
                        JobBenefits = c.String(),
                        JobRequirements = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsNow = c.Boolean(nullable: false),
                        UserId = c.String(),
                        EndDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        Category_Id = c.Int(),
                    })
                .PrimaryKey(t => t.JobID)
                .ForeignKey("dbo.tb_Company", t => t.CompanyID)
                .ForeignKey("dbo.tb_Experience", t => t.ExperienceID)
                .ForeignKey("dbo.tb_JobCategory", t => t.JobCategoryID)
                .ForeignKey("dbo.tb_Level", t => t.LevelID)
                .ForeignKey("dbo.tb_Location", t => t.LocationID)
                .ForeignKey("dbo.tb_Position", t => t.PositionID)
                .ForeignKey("dbo.tb_Salary", t => t.SalaryID)
                .ForeignKey("dbo.tb_Category", t => t.Category_Id)
                .Index(t => t.CompanyID)
                .Index(t => t.SalaryID)
                .Index(t => t.PositionID)
                .Index(t => t.ExperienceID)
                .Index(t => t.LocationID)
                .Index(t => t.LevelID)
                .Index(t => t.JobCategoryID)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "dbo.tb_Company",
                c => new
                    {
                        CompanyID = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(maxLength: 255),
                        CompanyDescription = c.String(),
                        CompanyEmail = c.String(),
                        CompanyAddress = c.String(),
                        Alias = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CompanyID);
            
            CreateTable(
                "dbo.tb_CompanyImage",
                c => new
                    {
                        CompanyImageID = c.Int(nullable: false, identity: true),
                        CompanyID = c.Int(nullable: false),
                        JobID = c.Int(nullable: false),
                        ImageURL = c.String(maxLength: 255),
                        IsDefault = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CompanyImageID)
                .ForeignKey("dbo.tb_Company", t => t.CompanyID, cascadeDelete: true)
                .ForeignKey("dbo.tb_Job", t => t.JobID, cascadeDelete: true)
                .Index(t => t.CompanyID)
                .Index(t => t.JobID);
            
            CreateTable(
                "dbo.tb_Experience",
                c => new
                    {
                        ExperienceID = c.Int(nullable: false, identity: true),
                        ExperienceLevel = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.ExperienceID);
            
            CreateTable(
                "dbo.tb_JobCategory",
                c => new
                    {
                        JobCategoryID = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 255),
                        Alias = c.String(maxLength: 255),
                        Icon = c.String(maxLength: 255),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobCategoryID);
            
            CreateTable(
                "dbo.tb_Level",
                c => new
                    {
                        LevelID = c.Int(nullable: false, identity: true),
                        LevelName = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.LevelID);
            
            CreateTable(
                "dbo.tb_Location",
                c => new
                    {
                        LocationID = c.Int(nullable: false, identity: true),
                        LocationName = c.String(nullable: false, maxLength: 255),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.LocationID);
            
            CreateTable(
                "dbo.tb_Position",
                c => new
                    {
                        PositionID = c.Int(nullable: false, identity: true),
                        PositionName = c.String(maxLength: 255),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PositionID);
            
            CreateTable(
                "dbo.tb_Salary",
                c => new
                    {
                        SalaryID = c.Int(nullable: false, identity: true),
                        SalaryRange = c.String(maxLength: 50),
                        SalaryMin = c.Int(nullable: false),
                        SalaryMax = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SalaryID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(),
                        Phone = c.String(),
                        IsVerified = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.tb_Article",
                c => new
                    {
                        ArticleID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        Alias = c.String(maxLength: 255),
                        Content = c.String(),
                        ImageURL = c.String(maxLength: 255),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArticleID);
            
            CreateTable(
                "dbo.tb_Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 150),
                        CategoryType = c.Int(nullable: false),
                        Alias = c.String(),
                        Description = c.String(maxLength: 250),
                        Position = c.Int(nullable: false),
                        SeoTitle = c.String(),
                        SeoDescription = c.String(),
                        SeoKeywords = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tb_CategoryProduct",
                c => new
                    {
                        CateProId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 250),
                        Alias = c.String(),
                        Description = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CateProId);
            
            CreateTable(
                "dbo.tb_Product",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 500),
                        Description = c.String(),
                        Alias = c.String(),
                        ProductCode = c.String(),
                        CateProId = c.Int(nullable: false),
                        Images = c.String(maxLength: 300),
                        Detail = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OriginalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PriceSale = c.Decimal(precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.tb_CategoryProduct", t => t.CateProId, cascadeDelete: true)
                .Index(t => t.CateProId);
            
            CreateTable(
                "dbo.tb_OrderDetail",
                c => new
                    {
                        OrderDetailID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderDetailID)
                .ForeignKey("dbo.tb_Order", t => t.OrderID, cascadeDelete: true)
                .ForeignKey("dbo.tb_Product", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.OrderID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.tb_Order",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        CustomerName = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        Address = c.String(nullable: false),
                        Email = c.String(),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TypePayment = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        Status = c.Int(nullable: false),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.OrderID);
            
            CreateTable(
                "dbo.tb_ProductImage",
                c => new
                    {
                        ProductImgID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        Image = c.String(),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ProductImgID)
                .ForeignKey("dbo.tb_Product", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.tb_Contact",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                        Website = c.String(maxLength: 150),
                        Email = c.String(nullable: false, maxLength: 150),
                        Message = c.String(nullable: false),
                        IsRead = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Conversations",
                c => new
                    {
                        ConversationID = c.Int(nullable: false, identity: true),
                        UserID = c.String(nullable: false),
                        AdminID = c.String(),
                        LastMessageTime = c.DateTime(nullable: false),
                        IsArchived = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ConversationID);
            
            CreateTable(
                "dbo.Message",
                c => new
                    {
                        MessageID = c.Int(nullable: false, identity: true),
                        ConversationID = c.Int(nullable: false),
                        SenderID = c.String(nullable: false),
                        Content = c.String(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MessageID)
                .ForeignKey("dbo.Conversations", t => t.ConversationID, cascadeDelete: true)
                .Index(t => t.ConversationID);
            
            CreateTable(
                "dbo.tb_EmailSubscription",
                c => new
                    {
                        SubscriptionID = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 255),
                        JobCategoryID = c.Int(),
                        LocationID = c.Int(),
                        PositionID = c.Int(),
                        SalaryRange = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SubscriptionID)
                .ForeignKey("dbo.tb_JobCategory", t => t.JobCategoryID)
                .ForeignKey("dbo.tb_Location", t => t.LocationID)
                .ForeignKey("dbo.tb_Position", t => t.PositionID)
                .Index(t => t.JobCategoryID)
                .Index(t => t.LocationID)
                .Index(t => t.PositionID);
            
            CreateTable(
                "dbo.EmployerVerifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(),
                        CompanyName = c.String(nullable: false),
                        PhoneNumber = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        BusinessLicenseNumber = c.String(nullable: false),
                        VerificationDocumentPath = c.String(),
                        AccountId = c.String(maxLength: 128),
                        IsVerified = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AccountId)
                .ForeignKey("dbo.tb_Company", t => t.CompanyId)
                .Index(t => t.CompanyId)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.tb_JobSkill",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        JobSkillName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tb_News",
                c => new
                    {
                        NewsID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 500),
                        Alias = c.String(),
                        Tag = c.String(),
                        Meta = c.String(),
                        Content = c.String(),
                        ImageURL = c.String(maxLength: 300),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.NewsID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.tb_Setting",
                c => new
                    {
                        SettingID = c.Int(nullable: false, identity: true),
                        SettingKey = c.String(maxLength: 255),
                        SettingValue = c.String(maxLength: 255),
                        Description = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.SettingID);
            
            CreateTable(
                "dbo.tb_ThongKe",
                c => new
                    {
                        ThongKeId = c.Int(nullable: false, identity: true),
                        ThoiGian = c.DateTime(nullable: false),
                        SoTruyCap = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ThongKeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.EmployerVerifications", "CompanyId", "dbo.tb_Company");
            DropForeignKey("dbo.EmployerVerifications", "AccountId", "dbo.AspNetUsers");
            DropForeignKey("dbo.tb_EmailSubscription", "PositionID", "dbo.tb_Position");
            DropForeignKey("dbo.tb_EmailSubscription", "LocationID", "dbo.tb_Location");
            DropForeignKey("dbo.tb_EmailSubscription", "JobCategoryID", "dbo.tb_JobCategory");
            DropForeignKey("dbo.Message", "ConversationID", "dbo.Conversations");
            DropForeignKey("dbo.tb_ProductImage", "ProductID", "dbo.tb_Product");
            DropForeignKey("dbo.tb_OrderDetail", "ProductID", "dbo.tb_Product");
            DropForeignKey("dbo.tb_OrderDetail", "OrderID", "dbo.tb_Order");
            DropForeignKey("dbo.tb_Product", "CateProId", "dbo.tb_CategoryProduct");
            DropForeignKey("dbo.tb_Job", "Category_Id", "dbo.tb_Category");
            DropForeignKey("dbo.tb_Applicant", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.tb_Job", "SalaryID", "dbo.tb_Salary");
            DropForeignKey("dbo.tb_Job", "PositionID", "dbo.tb_Position");
            DropForeignKey("dbo.tb_Job", "LocationID", "dbo.tb_Location");
            DropForeignKey("dbo.tb_Job", "LevelID", "dbo.tb_Level");
            DropForeignKey("dbo.tb_Job", "JobCategoryID", "dbo.tb_JobCategory");
            DropForeignKey("dbo.tb_JobApplication", "JobID", "dbo.tb_Job");
            DropForeignKey("dbo.tb_Job", "ExperienceID", "dbo.tb_Experience");
            DropForeignKey("dbo.tb_Job", "CompanyID", "dbo.tb_Company");
            DropForeignKey("dbo.tb_CompanyImage", "JobID", "dbo.tb_Job");
            DropForeignKey("dbo.tb_CompanyImage", "CompanyID", "dbo.tb_Company");
            DropForeignKey("dbo.tb_JobApplication", "ApplicantID", "dbo.tb_Applicant");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.EmployerVerifications", new[] { "AccountId" });
            DropIndex("dbo.EmployerVerifications", new[] { "CompanyId" });
            DropIndex("dbo.tb_EmailSubscription", new[] { "PositionID" });
            DropIndex("dbo.tb_EmailSubscription", new[] { "LocationID" });
            DropIndex("dbo.tb_EmailSubscription", new[] { "JobCategoryID" });
            DropIndex("dbo.Message", new[] { "ConversationID" });
            DropIndex("dbo.tb_ProductImage", new[] { "ProductID" });
            DropIndex("dbo.tb_OrderDetail", new[] { "ProductID" });
            DropIndex("dbo.tb_OrderDetail", new[] { "OrderID" });
            DropIndex("dbo.tb_Product", new[] { "CateProId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.tb_CompanyImage", new[] { "JobID" });
            DropIndex("dbo.tb_CompanyImage", new[] { "CompanyID" });
            DropIndex("dbo.tb_Job", new[] { "Category_Id" });
            DropIndex("dbo.tb_Job", new[] { "JobCategoryID" });
            DropIndex("dbo.tb_Job", new[] { "LevelID" });
            DropIndex("dbo.tb_Job", new[] { "LocationID" });
            DropIndex("dbo.tb_Job", new[] { "ExperienceID" });
            DropIndex("dbo.tb_Job", new[] { "PositionID" });
            DropIndex("dbo.tb_Job", new[] { "SalaryID" });
            DropIndex("dbo.tb_Job", new[] { "CompanyID" });
            DropIndex("dbo.tb_JobApplication", new[] { "ApplicantID" });
            DropIndex("dbo.tb_JobApplication", new[] { "JobID" });
            DropIndex("dbo.tb_Applicant", new[] { "UserId" });
            DropTable("dbo.tb_ThongKe");
            DropTable("dbo.tb_Setting");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.tb_News");
            DropTable("dbo.tb_JobSkill");
            DropTable("dbo.EmployerVerifications");
            DropTable("dbo.tb_EmailSubscription");
            DropTable("dbo.Message");
            DropTable("dbo.Conversations");
            DropTable("dbo.tb_Contact");
            DropTable("dbo.tb_ProductImage");
            DropTable("dbo.tb_Order");
            DropTable("dbo.tb_OrderDetail");
            DropTable("dbo.tb_Product");
            DropTable("dbo.tb_CategoryProduct");
            DropTable("dbo.tb_Category");
            DropTable("dbo.tb_Article");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.tb_Salary");
            DropTable("dbo.tb_Position");
            DropTable("dbo.tb_Location");
            DropTable("dbo.tb_Level");
            DropTable("dbo.tb_JobCategory");
            DropTable("dbo.tb_Experience");
            DropTable("dbo.tb_CompanyImage");
            DropTable("dbo.tb_Company");
            DropTable("dbo.tb_Job");
            DropTable("dbo.tb_JobApplication");
            DropTable("dbo.tb_Applicant");
        }
    }
}
