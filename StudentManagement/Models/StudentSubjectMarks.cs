namespace StudentManagement.Models
{
    public class StudentSubjectMarks
    {
        public Guid StudentId { get; set; }
        public string Name {  get; set; }
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int Mark { get; set; }
    }
}
