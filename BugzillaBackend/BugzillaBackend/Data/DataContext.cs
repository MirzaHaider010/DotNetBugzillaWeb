using BugzillaBackend.Models;

namespace BugzillaBackend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .UseSqlServer("Server=.\\SQLExpress;Database=bugzilla;Trusted_Connection=true;TrustServerCertificate=True;");
        }

        //public DbSet<User> Users => Set<User>();
        //public DbSet<Project> Projects => Set<Project>();
        //public DbSet<Bug> Bugs { get; set; }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Bug> Bugs { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Manager)
                .WithMany()
                .HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Developers)
                .WithMany(u => u.Projects)
                .UsingEntity(j => j.ToTable("ProjectDevelopers"));

            modelBuilder.Entity<Project>()
                .HasMany(p => p.QAs)
                .WithMany()
                .UsingEntity(j => j.ToTable("ProjectQAs"));

            modelBuilder.Entity<Bug>()
                .HasOne(b => b.Creator)
                .WithMany(u => u.CreatedBugs)
                .HasForeignKey(b => b.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bug>()
                .HasOne(b => b.Developer)
                .WithMany(u => u.AssignedBugs)
                .HasForeignKey(b => b.DeveloperId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bug>()
                .HasOne(b => b.Project)
                .WithMany(p => p.Bugs)
                .HasForeignKey(b => b.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
