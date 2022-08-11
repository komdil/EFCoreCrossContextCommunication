namespace EFCoreCrossContextCommunication.Test
{
    public class CrossContextCommunicationTests
    {
        ApplicationSession applicationSession;
        Guid studentId;
        Guid backpackId;
        [SetUp]
        public void SetUp()
        {
            studentId = Guid.NewGuid();
            backpackId = Guid.NewGuid();
            applicationSession = new ApplicationSession();
            using (var context = applicationSession.CreateContext())
            {
                var student = new Student()
                {
                    DateOfBirth = new DateTime(1999, 11, 29),
                    Id = studentId,
                    Name = "Dilshod",
                    LastName = "Komilov"
                };

                var backpack = new Backpack()
                {
                    Id = backpackId,
                    Name = "Cool backpack",
                    Student = student
                };

                context.Add(backpack);
                context.SaveChanges();
            }
        }

        [Test]
        public void CrossContextCommunication_UpdatingPrimitivePropertiesTest()
        {
            using var context1 = applicationSession.CreateContext();
            using var context2 = applicationSession.CreateContext();

            var studentInContext1 = context1.Set<Student>().FirstOrDefault(s => s.Id == studentId);
            var studentInContext2 = context2.Set<Student>().FirstOrDefault(s => s.Id == studentId);

            studentInContext1.Name = "Mirolim";
            context1.SaveChanges();

            Assert.That(studentInContext2.Name, Is.EqualTo("Mirolim"));
        }

        [Test]
        public void CrossContextCommunication_UpdatingReferenceToNullPropertiesTest()
        {
            using var context1 = applicationSession.CreateContext();
            using var context2 = applicationSession.CreateContext();

            var backpackInContext1 = context1.Set<Backpack>().FirstOrDefault(s => s.Id == backpackId);
            var backpackInContext2 = context2.Set<Backpack>().FirstOrDefault(s => s.Id == backpackId);

            Assert.That(backpackInContext1.Student.Id, Is.EqualTo(backpackInContext2.Student.Id));

            backpackInContext1.Student = null;
            context1.SaveChanges();

            Assert.That(backpackInContext2.Student, Is.Null);
        }

        [Test]
        public void CrossContextCommunication_UpdatingReferencePropertiesTest()
        {
            using var context1 = applicationSession.CreateContext();
            using var context2 = applicationSession.CreateContext();

            var backpackInContext1 = context1.Set<Backpack>().FirstOrDefault(s => s.Id == backpackId);
            var backpackInContext2 = context2.Set<Backpack>().FirstOrDefault(s => s.Id == backpackId);

            Assert.That(backpackInContext1.Student.Id, Is.EqualTo(backpackInContext2.Student.Id));

            var newStudentInContext1 = new Student() { Id = Guid.NewGuid(), Name = "NewStudent", LastName = "New" };
            context1.Add(newStudentInContext1);
            backpackInContext1.Student = newStudentInContext1;
            context1.SaveChanges();

            Assert.That(backpackInContext2.Student.Id, Is.EqualTo(newStudentInContext1.Id));
        }

        [Test]
        public void CrossContextCommunication_UpdatingReferenceCollectionPropertiesTest()
        {
            using var context1 = applicationSession.CreateContext();
            using var context2 = applicationSession.CreateContext();

            var studentInContext1 = context1.Set<Student>().FirstOrDefault(s => s.Id == studentId);
            var studentInContext2 = context2.Set<Student>().FirstOrDefault(s => s.Id == studentId);

            _ = studentInContext1.Backpacks;
            _ = studentInContext2.Backpacks;

            var newBackpack = new Backpack() { Id = Guid.NewGuid(), Name = "Second cool", Student = studentInContext1 };
            context1.Add(newBackpack);
            context1.SaveChanges();

            Assert.That(studentInContext2.Backpacks.Any(s => s.Id == newBackpack.Id), Is.True);
        }
    }
}