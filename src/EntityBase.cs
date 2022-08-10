using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCoreCrossContextCommunication
{
    public class EntityBase
    {
        protected EntityBase(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }

        public EntityBase()
        {

        }

        protected ILazyLoader LazyLoader { get; }

        public Guid Id { get; set; }
    }
}
