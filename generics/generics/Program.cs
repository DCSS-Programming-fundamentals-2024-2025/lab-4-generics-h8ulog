using System;
using System.Collections.Generic;
using System.Linq;
using generics.Models;
using generics.Interfaces;
using generics.InMemoryRepository;

namespace generics
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("--- Лабораторна робота №4 – Generics ---");

            Console.WriteLine("\n--- Перевірка Етапу 2: Побудова базової моделі факультету і груп ---");

            Faculty fpm = new Faculty { Id = 1, Name = "ФПМ КПІ" };

            Group kp41 = new Group { Id = 41, Name = "КП-41" };
            Group kp42 = new Group { Id = 42, Name = "КП-42" };
            Group kp43 = new Group { Id = 43, Name = "КП-43" };

            fpm.AddGroup(kp41);
            fpm.AddGroup(kp42);
            fpm.AddGroup(kp43);

            Student s1 = new Student { Id = 101, Name = "Іван Петров" };
            Student s2 = new Student { Id = 102, Name = "Марія Сидорова" };
            Student s3 = new Student { Id = 103, Name = "Олег Коваленко" };
            Student s4 = new Student { Id = 104, Name = "Анна Мельник" };

            fpm.AddStudentToGroup(41, s1);
            fpm.AddStudentToGroup(41, s2);
            fpm.AddStudentToGroup(42, s3);
            fpm.AddStudentToGroup(43, s4);

            Console.WriteLine($"Факультет: {fpm.Name}");
            foreach (var group in fpm.GetAllGroups())
            {
                Console.WriteLine($"  Група: {group.Name}");
                foreach (var student in fpm.GetAllStudentsInGroup(group.Id))
                {
                    Console.WriteLine($"    - Студент: {student.Name} (ID: {student.Id})");
                    student.SayName();
                    student.SubmitWork();
                }
            }

            Console.WriteLine("\n--- Перевірка Етапу 3: Додавання обмежень типів (where) ---");
            Console.WriteLine("Перевірка компіляції:");

            Console.WriteLine("IRepository<Student,int> - успішно компілюється (використовується в Group і Faculty).");

            // IRepository<int, int> invalidRepo = new InMemoryRepository<int, int>();
            Console.WriteLine("IRepository<int,int> - має викликати помилку компіляції (int не є class, new()).");
            Console.WriteLine("Рядок закоментовано, щоб дозволити компіляцію всього проекту. Розкоментуйте для перевірки.");

            Console.WriteLine("\n--- Демонстрація методів Teacher ---");
            Teacher t1 = new Teacher { Id = 201, Name = "Професор Сміт" };
            t1.GradeStudent(s1);
            t1.ExpelStudent(s3);
            t1.ShowPresentStudents(fpm.GetAllStudentsInGroup(41));

            Console.WriteLine("\n--- Перевірка Етапу 4*: Read-only інтерфейс (коваріантність) ---");

            var tempStudentRepo = new InMemoryRepository<Student, int>();
            tempStudentRepo.Add(1, new Student { Id = 1, Name = "Коваріантний Студент 1" });
            tempStudentRepo.Add(2, new Student { Id = 2, Name = "Коваріантний Студент 2" });

            IReadOnlyRepository<Student, int> studReadOnlyRepo = tempStudentRepo;

            Console.WriteLine("Студенти з IReadOnlyRepository<Student, int>:");
            foreach (var student in studReadOnlyRepo.GetAll())
            {
                Console.WriteLine($"- {student.Name}");
            }

            IReadOnlyRepository<Person, int> persReadOnlyRepo = studReadOnlyRepo;
            Console.WriteLine("IReadOnlyRepository<Person,int>  persReadOnlyRepo = studReadOnlyRepo; - Успішно компілюється.");
            Console.WriteLine("Об'єкти з IReadOnlyRepository<Person, int> (де фактично Student):");
            foreach (var person in persReadOnlyRepo.GetAll())
            {
                Console.WriteLine($"- {person.Name} (як Person)");
            }

            Console.WriteLine("\n--- Перевірка Етапу 5*: Write-only інтерфейс (контраваріантність) ---");

            IWriteRepository<Person, int> persWrite = new InMemoryRepository<Person, int>();
            persWrite.Add(1001, new Student { Id = 1001, Name = "Контраваріантний Студент 1" });
            persWrite.Add(1002, new Person { Id = 1002, Name = "Просто Person" });

            Console.WriteLine("\nДодаємо Student до IWriteRepository<Person, int>:");
            persWrite.Add(105, new Student { Id = 105, Name = "Новий студент через Writer<Person>" });
            
            IReadOnlyRepository<Person, int> persRead = (IReadOnlyRepository<Person, int>)persWrite;
            Console.WriteLine("Перевіряємо, що доданий студент є:");
            var addedStudent = persRead.Get(105);
            if (addedStudent != null)
            {
                Console.WriteLine($"Знайдено: {addedStudent.Name}");
            }

            Console.WriteLine("Завдання 5: Перевірка контраваріантності в C# є більш тонкою.");
            Console.WriteLine("Рядок `IWriteRepository<Student,int> studWrite = persWrite;` (де persWrite - IWriteRepository<Person,int>) не компілюється безпосередньо.");
            Console.WriteLine("Контраваріантність дозволяє використовувати більш узагальнений тип інтерфейсу, коли очікується більш специфічний тип.");
        }
    }
}
