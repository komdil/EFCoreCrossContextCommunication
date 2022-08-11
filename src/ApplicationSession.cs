using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EFCoreCrossContextCommunication
{
    public class ApplicationSession
    {
        readonly List<ApplicationContext> Contexts = new();

        public ApplicationContext CreateContext()
        {
            var newContext = new ApplicationContext(this);
            Contexts.Add(newContext);
            return newContext;
        }

        public void UpdateContexts(ApplicationContext savingContext, IEnumerable<EntityBase> updatedEntities)
        {
            var contextsToUpdate = Contexts.Where(s => s != savingContext);
            foreach (var context in contextsToUpdate)
            {
                var entitiesToUpdate = GetEntitiesToUpdate(context, updatedEntities);

                foreach (var entityToUpdate in entitiesToUpdate.ToList())
                {
                    context.Reload(entityToUpdate);
                }
            }
        }

        IEnumerable<EntityBase> GetEntitiesToUpdate(ApplicationContext contextToUpdate, IEnumerable<EntityBase> updatedEntities)
        {
            return contextToUpdate.GetTrackedEntities().Where(s => updatedEntities.Any(c => c.Id == s.Id));
        }

        internal void RemoveContext(ApplicationContext applicationContext)
        {
            Contexts.Remove(applicationContext);
        }
    }
}
