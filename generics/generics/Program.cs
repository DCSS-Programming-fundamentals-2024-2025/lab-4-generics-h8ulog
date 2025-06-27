using System;
using System.Collections.Generic;
using System.Linq;

class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
}

class Student : Person
{
    public void SubmitWork() => Console.WriteLine($"{Name} подав(ла) роботу");
    public void SayName() => Console.WriteLine($"Я студент(ка) {Name}");
}

class Teacher : Person
{
    public void GradeStudent(Student student) => Console.WriteLine($"{Name} оцінив(ла) {student.Name}");
    public void ExpelStudent(Student student) => Console.WriteLine($"{Name} виключив(ла) {student.Name}");
    public void ShowPresentStudents(List<Student> students)
    {
        Console.WriteLine($"Присутні студенти (викладач {Name}):");
        foreach (var s in students)
            Console.WriteLine($"  {s.Id}: {s.Name}");
    }
}

interface IRepository<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey>, IWriteRepository<TEntity, TKey>
    where TEntity : class, new()
    where TKey : struct
{
    void Add(TKey id, TEntity entity);
    TEntity Get(TKey id);
    IEnumerable<TEntity> GetAll();
    void Remove(TKey id);
}

interface IReadOnlyRepository<out TEntity, in TKey>
{
    TEntity Get(TKey id);
    IEnumerable<TEntity> GetAll();
}

interface IWriteRepository<in TEntity, in TKey>
{
    void Add(TKey id, TEntity entity);
    void Remove(TKey id);
}

class InMemoryRepository<TEntity, TKey> :
    IRepository<TEntity, TKey>
    where TEntity : class, new()
    where TKey : struct
{
    private Dictionary<TKey, TEntity> _data = new Dictionary<TKey, TEntity>();

    public void Add(TKey id, TEntity entity) => _data[id] = entity;
    public TEntity Get(TKey id) => _data.TryGetValue(id, out var value) ? value : null;
    public IEnumerable<TEntity> GetAll() => _data.Values;
    public void Remove(TKey id) => _data.Remove(id);
}

class Group
{
    public int Id { get; set; }
    public string Name { get; set; }

    private IRepository<Student, int> _students = new InMemoryRepository<Student, int>();

    public void AddStudent(Student s) => _students.Add(s.Id, s);
    public void RemoveStudent(int studentId) => _students.Remove(studentId);
    public IEnumerable<Student> GetAllStudents() => _students.GetAll();
    public Student FindStudent(int studentId) => _students.Get(studentId);
}

class Faculty
{
    public int Id { get; set; }
    public string Name { get; set; }

    private IRepository<Group, int> _groups = new InMemoryRepository<Group, int>();

    public void AddGroup(Group g) => _groups.Add(g.Id, g);
    public void RemoveGroup(int id) => _groups.Remove(id);
    public IEnumerable<Group> GetAllGroups() => _groups.GetAll();
    public Group GetGroup(int id) => _groups.Get(id);

    public void AddStudentToGroup(int groupId, Student s)
    {
        var group = GetGroup(groupId);
        if (group != null)
            group.AddStudent(s);
        else
            Console.WriteLine($"Група з ID {groupId} не знайдена. Студента '{s.Name}' не додано.");
    }

    public void RemoveStudentFromGroup(int groupId, int studentId)
    {
        var group = GetGroup(groupId);
        if (group != null)
            group.RemoveStudent(studentId);
        else
            Console.WriteLine($"Група з ID {groupId} не знайдена. Студента з ID '{studentId}' не видалено.");
    }

    public IEnumerable<Student> GetAllStudentsInGroup(int groupId)
    {
        var group = GetGroup(groupId);
        return group?.GetAllStudents() ?? Enumerable.Empty<Student>();
    }

    public Student FindStudentInGroup(int groupId, int studentId)
    {
        var group = GetGroup(groupId);
        return group?.FindStudent(studentId);
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("--- Демонстрація моделі 'Факультет → Група → Студент' та Generics ---");

        Faculty fpm = new Faculty { Id = 1, Name = "Факультет Прикладного Математики" };

        Console.WriteLine("\nДодаємо групи до факультету ФПМ:");
        fpm.AddGroup(new Group { Id = 41, Name = "КП-41" });
        fpm.AddGroup(new Group { Id = 42, Name = "КП-42" });
        fpm.AddGroup(new Group { Id = 43, Name = "КП-43" });
        Console.WriteLine($"Групи ФПМ: {string.Join(", ", fpm.GetAllGroups().Select(g => g.Name))}");

        Student s1 = new Student { Id = 1, Name = "Олег Іваненко" };
        Student s2 = new Student { Id = 2, Name = "Ірина Петренко" };
        Student s3 = new Student { Id = 3, Name = "Андрій Сидоренко" };
        Student s4 = new Student { Id = 4, Name = "Олена Коваль" };

        Console.WriteLine("\nДодаємо студентів до груп:");
        fpm.AddStudentToGroup(41, s1);
        fpm.AddStudentToGroup(42, s2);
        fpm.AddStudentToGroup(41, s3);
        fpm.AddStudentToGroup(99, s4);

        Console.WriteLine("\nСтуденти групи КП-41:");
        foreach (var student in fpm.GetAllStudentsInGroup(41))
            Console.WriteLine($"  {student.Id}: {student.Name}");

        Console.WriteLine("\nСтуденти групи КП-42:");
        foreach (var student in fpm.GetAllStudentsInGroup(42))
            Console.WriteLine($"  {student.Id}: {student.Name}");

        Console.WriteLine("\nСтуденти групи КП-43 (має бути порожньо):");
        foreach (var student in fpm.GetAllStudentsInGroup(43))
            Console.WriteLine($"  {student.Id}: {student.Name}");

        Console.WriteLine("\n--- Етап 3: Перевірка обмежень типів (WHERE) ---");
        IRepository<Student, int> studentRepoCheck = new InMemoryRepository<Student, int>();
        Console.WriteLine("IRepository<Student, int> компілюється успішно (Student є class, new()).");

        // IRepository<int, int> intRepoCheck = new InMemoryRepository<int, int>();
        Console.WriteLine("IRepository<int, int> викликала б помилку компіляції, оскільки 'int' не задовольняє обмеження 'class, new()'.");


        Console.WriteLine("\n--- Етап 4: Перевірка коваріантності (IReadOnlyRepository<out TEntity, ...>) ---");
        var studentReadRepo = new InMemoryRepository<Student, int>();
        studentReadRepo.Add(5, new Student { Id = 5, Name = "Марко Дмитренко" });
        studentReadRepo.Add(6, new Student { Id = 6, Name = "Вікторія Захарчук" });

        IReadOnlyRepository<Person, int> readPersonRepo = studentReadRepo;

        Person p_retrieved = readPersonRepo.Get(5);
        Console.WriteLine($"Коваріантність працює: отримано {p_retrieved?.Name} (як Person) з репозиторію студентів.");
        Console.WriteLine($"Всі особи з репозиторію студентів (через IReadOnlyRepository<Person, int>):");
        foreach (var person in readPersonRepo.GetAll())
        {
            Console.WriteLine($"  {person.Id}: {person.Name}");
        }

        Console.WriteLine("\n--- Етап 5: Перевірка контраваріантності (IWriteRepository<in TEntity, ...>) ---");
        // Застосовано явне приведення для демонстрації контраваріантності
        IWriteRepository<Student, int> studentWriteRepo = new InMemoryRepository<Student, int>();
        IWriteRepository<Person, int> personWriter = (IWriteRepository<Person, int>)studentWriteRepo;

        personWriter.Add(7, new Person { Id = 7, Name = "Софія Руденко" });
        personWriter.Add(8, new Student { Id = 8, Name = "Максим Крулько" });

        var actualStudentRepo = (InMemoryRepository<Student, int>)personWriter;
        var addedPerson = actualStudentRepo.Get(7);
        var addedStudent = actualStudentRepo.Get(8);
        Console.WriteLine($"Контраваріантність працює: додано Person ({addedPerson?.Name}) та Student ({addedStudent?.Name}) через IWriteRepository<Person, int>.");

        personWriter.Remove(7);
        Console.WriteLine($"Видалено об'єкт з ID 7. Залишились студенти:");
        foreach (var student in actualStudentRepo.GetAll())
        {
            Console.WriteLine($"  {student.Id}: {student.Name}");
        }


        Console.WriteLine("\n--- Демонстрація методів Student та Teacher ---");
        s1.SubmitWork();
        s2.SayName();

        Teacher t1 = new Teacher { Id = 101, Name = "Професор Коваленко" };
        t1.GradeStudent(s1);
        t1.ExpelStudent(s3);

        Console.WriteLine($"\nВикладач {t1.Name} показує присутніх студентів у КП-41:");
        List<Student> currentStudentsIn41 = fpm.GetAllStudentsInGroup(41).ToList();
        t1.ShowPresentStudents(currentStudentsIn41);
    }
}
