using food_allergen_prediction_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace food_allergen_prediction_backend.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<AllergyProfile> AllergyProfiles => Set<AllergyProfile>();
        public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<AllergyProfile>()
                .Property(a => a.Allergies)
                .HasColumnType("text[]");
        }
    }
}
