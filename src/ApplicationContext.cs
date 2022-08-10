using Microsoft.EntityFrameworkCore;

namespace EFCoreCrossContextCommunication
{
    public class ApplicationContext : DbContext
    {
        ApplicationSession ApplicationSession { get; }
        internal ApplicationContext(ApplicationSession applicationSession)
        {
            ApplicationSession = applicationSession;
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

        internal void Reload(EntityBase entityToUpdate)
        {
            var entry = ChangeTracker.Entries<EntityBase>().FirstOrDefault(s => s.Entity.Id == entityToUpdate.Id);
            if (entry != null)
            {
                entry.Reload();
            }
        }

        internal IEnumerable<EntityBase> GetTrackedEntities()
        {
            return ChangeTracker.Entries<EntityBase>().Select(s => s.Entity);
        }

        public override int SaveChanges()
        {
            var modifiedEntities = ChangeTracker.Entries<EntityBase>().Where(s => s.State == EntityState.Modified).Select(s => s.Entity).ToList();
            var res = base.SaveChanges();
            ApplicationSession.UpdateContexts(this, modifiedEntities);
            return res;
        }

        public override void Dispose()
        {
            base.Dispose();
            ApplicationSession.RemoveContext(this);
        }
    }
}
