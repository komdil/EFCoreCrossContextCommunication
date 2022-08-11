using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace EFCoreCrossContextCommunication
{
    public class Student : EntityBase
    {
        Student(ILazyLoader lazyLoader) : base(lazyLoader)
        {

        }

        public Student()
        {

        }

        ReferenceCollection<Backpack> backpacks;
        public virtual ReferenceCollection<Backpack> Backpacks
        {
            get
            {
                if (backpacks == null)
                    LazyLoader?.Load(this, ref backpacks);
                return backpacks;
            }
            set
            {
                backpacks = value;
            }
        }
        public DateTime DateOfBirth { get; set; }

        public string LastName { get; set; }

        public string Name { get; set; }
    }
}
