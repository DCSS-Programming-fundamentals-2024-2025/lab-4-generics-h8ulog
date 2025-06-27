using System;
using System.Collections.Generic;
using System.Linq;
using generics.Interfaces;
using generics.InMemoryRepository;

namespace generics.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Student : Person
    {
        public void SubmitWork()
        {
            Console.WriteLine($"{Name} submits work.");
        }

        public void SayName()
        {
            Console.WriteLine($"My name is {Name}.");
        }
    }

    public class Teacher : Person
    {
        public void GradeStudent(Student student)
        {
            Console.WriteLine($"{Name} grades {student.Name}.");
        }

        public void ExpelStudent(Student student)
        {
            Console.WriteLine($"{Name} expels {student.Name}.");
        }

        public void ShowPresentStudents(IEnumerable<Student> students)
        {
            Console.WriteLine($"{Name} checks present students:");
            foreach (var student in students)
            {
                Console.WriteLine($"- {student.Name}");
            }
        }
    }

    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private IRepository<Student, int> _students = new InMemoryRepository<Student, int>();

        public void AddStudent(Student s)
        {
            _students.Add(s.Id, s);
        }

        public void RemoveStudent(int studentId)
        {
            _students.Remove(studentId);
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return _students.GetAll();
        }

        public Student FindStudent(int studentId)
        {
            return _students.Get(studentId);
        }
    }

    public class Faculty
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private IRepository<Group, int> _groups = new InMemoryRepository<Group, int>();

        public void AddGroup(Group g)
        {
            _groups.Add(g.Id, g);
        }

        public void RemoveGroup(int id)
        {
            _groups.Remove(id);
        }

        public IEnumerable<Group> GetAllGroups()
        {
            return _groups.GetAll();
        }

        public Group GetGroup(int id)
        {
            return _groups.Get(id);
        }

        public void AddStudentToGroup(int groupId, Student s)
        {
            var group = GetGroup(groupId);
            if (group != null)
            {
                group.AddStudent(s);
            }
        }

        public void RemoveStudentFromGroup(int groupId, int studentId)
        {
            var group = GetGroup(groupId);
            if (group != null)
            {
                group.RemoveStudent(studentId);
            }
        }

        public IEnumerable<Student> GetAllStudentsInGroup(int groupId)
        {
            var group = GetGroup(groupId);
            return group?.GetAllStudents() ?? Enumerable.Empty<Student>();
        }

        public Student FindStudentInGroup(int groupId, int studentId)
        {
            return group?.FindStudent(studentId);
        }
    }
}
