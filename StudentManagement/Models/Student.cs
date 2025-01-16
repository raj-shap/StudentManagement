namespace StudentManagement.Models
{
    public class Student
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Standard { get; set; }
        public string Address { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? Path { get; set; }

        //public List<Subject> Subjects { get; set; }
        public List<Marks> Marks { get; set; }
    }
}
