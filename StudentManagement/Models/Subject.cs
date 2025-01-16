
namespace StudentManagement.Models
{
    public class Subject
    {
        public Guid Id { get; set; }
        public string SubjectName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; } 

        public List<Marks> Marks { get; set; }
    }
}
