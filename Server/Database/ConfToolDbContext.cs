using Microsoft.EntityFrameworkCore;

namespace ConfTool.Server.Database
{
    public class ConfToolDbContext : DbContext
    {
        public DbSet<Conference> Conferences { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<ContributionSpeaker> ContributionSpeakers { get; set; }
        public ConfToolDbContext(DbContextOptions<ConfToolDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ContributionSpeaker>().HasKey(sc => new { sc.ContributionId, sc.SpeakerId });

            modelBuilder.Entity<ContributionSpeaker>()
                        .HasOne(sc => sc.Contribution)
                        .WithMany(s => s.ContributionSpeakers)
                        .HasForeignKey(sc => sc.ContributionId)
                        .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ContributionSpeaker>()
                        .HasOne(sc => sc.Speaker)
                        .WithMany(s => s.ContributionSpeakers)
                        .HasForeignKey(sc => sc.SpeakerId)
                        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
