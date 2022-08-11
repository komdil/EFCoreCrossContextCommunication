using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

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
                var navigationEntitiesThatNeedsToBeLoaded = entry.Navigations.Where(s => s.IsLoaded).ToList();
                entry.State = EntityState.Detached;
                foreach (var navigation in navigationEntitiesThatNeedsToBeLoaded)
                {
                    navigation.CurrentValue = null;
                    navigation.IsLoaded = false;
                }
                entry.Reload();
                foreach (var navigation in navigationEntitiesThatNeedsToBeLoaded)
                {
                    navigation.Load();
                }

                foreach (var source in entry.Collections)
                {
                    if (source.CurrentValue != null)
                    {
                        foreach (var item in source.CurrentValue)
                            source.EntityEntry.Context.Entry(item).State = EntityState.Detached;
                        source.CurrentValue = null;
                    }
                    source.IsLoaded = false;
                    source.Load();
                }
            }
        }

        internal IEnumerable<EntityBase> GetTrackedEntities()
        {
            return ChangeTracker.Entries<EntityBase>().Select(s => s.Entity);
        }

        public override int SaveChanges()
        {
            var modifiedEntities = new List<EntityBase>();
            foreach (var entity in ChangeTracker.Entries<EntityBase>())
            {
                if (entity.State == EntityState.Modified)
                {
                    modifiedEntities.Add(entity.Entity);
                }
                else if (entity.State == EntityState.Added || entity.State == EntityState.Deleted)
                {
                    foreach (var item in entity.References)
                    {
                        if (item.TargetEntry.State == EntityState.Unchanged && item.TargetEntry.Entity is EntityBase entityBaseTargetEntity)
                        {
                            modifiedEntities.Add(entityBaseTargetEntity);
                        }
                    }
                }
            }
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
