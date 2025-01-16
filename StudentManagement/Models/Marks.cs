namespace StudentManagement.Models
{
    public class Marks
    {
        public Guid Id { get; set; }
        public Guid SubjectId { get; set; }
        public virtual Subject Subject { get; set; }
        public Guid StudentId { get; set; }
        public virtual Student Student { get; set; }
        public int? Mark { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

    }
}
