using Microsoft.EntityFrameworkCore;

namespace EFCoreCrossContextCommunication
{
    public class ApplicationContext : DbContext
    {
        internal ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=CrossContextCommunication;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var student = modelBuilder.Entity<Student>();
            student.HasKey(s => s.Id);
            student.HasMany(s => s.Backpacks).WithOne(s => s.Student).HasForeignKey(s => s.StudentId);

            var backpack = modelBuilder.Entity<Backpack>();
            backpack.HasKey(s => s.Id);
            backpack.HasOne(s => s.Student).WithMany(s => s.Backpacks).HasForeignKey(s => s.StudentId);
        }
    }
}
