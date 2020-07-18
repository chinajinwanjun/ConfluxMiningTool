using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Latest_release_18._2._44.Data
{

    public class StudentService
    {
        private SoftwareDbContext db;
        public StudentService(SoftwareDbContext db)
        {
            this.db = db;
        }
        public dynamic GetStudent()
        {
            return db.Students.ToList();
        }
        public void Add(Student student)
        {
            db.Students.Add(student);
            db.SaveChanges();
        }
        public void Update(Student student)
        {
            var s = db.Students.FirstOrDefault(x => x.ID == student.ID);
            s.Name = student.Name;
            db.SaveChanges();
        }
        public void Delete(Student student)
        {
            var s = db.Students.FirstOrDefault(x => x.ID == student.ID);
            db.Students.Remove(s);
            db.SaveChanges();
        }
    }
}
