using Microsoft.EntityFrameworkCore.Infrastructure;

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
        public Guid? StudentId { get; set; }

        public string Name { get; set; }
    }
}
