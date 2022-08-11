using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace EFCoreCrossContextCommunication
{
    public abstract class EntityBase
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
