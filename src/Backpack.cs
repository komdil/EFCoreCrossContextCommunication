using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace EFCoreCrossContextCommunication
{
    public class Backpack : EntityBase
    {
        Backpack(ILazyLoader lazyLoader) : base(lazyLoader) { }

        public Backpack()
        {

        }

        Student student;
        public virtual Student Student
        {
            get
            {
                if (student == null)
                    LazyLoader.Load(this, ref student);
                return student;
            }
            set
            {
                student = value;
            }
        }
        internal Guid? StudentId { get; set; }

        public string Name { get; set; }
    }
}
