using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using WebJob.Models.EF;
using WebJob.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext()
        : base("DefaultConnection", throwIfV1Schema: false)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyImage> CompanyImages { get; set; }
    public DbSet<Salary> Salaries { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Level> Levels { get; set; }
    public DbSet<JobCategory> JobCategories { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobSkill> JobSkills { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<EmailSubscription> EmailSubscriptions { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<CategoryProduct> categoryProducts { get; set; }
    public DbSet<Product> products { get; set; }
    public DbSet<Order> orders { get; set; }
    public DbSet<OrderDetail> orderDetails { get; set; }
    public DbSet<ProductImage> productImages { get; set; }
    public DbSet<ThongKe> thongKes { get; set; }
    public DbSet<Message> message { get; set; }
    public DbSet<Conversation> conversations { get; set; }
    public DbSet<EmployerVerification> employerVerifications { get; set; }
    public DbSet<Job_JobSkill> JobJobSkills { get; set; }
    public DbSet<EmployerVerificationProduct> EmployerVerificationProducts { get; set; }
    public DbSet<CandidateProfile> CandidateProfiles { get; set; }
    public DbSet<CandidateProfileSkill> CandidateProfileSkills { get; set; }
    public static ApplicationDbContext Create()
    {
        return new ApplicationDbContext();
    }
}