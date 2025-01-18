using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentManagement.Models
{
    public class MarksViewModel
    {
        public Guid StudentId { get; set; }
        public string? StudentName { get; set; }
        public Guid SubjectId { get; set; }
        public string? SubjectName {  get; set; }
        [Range(0,100,ErrorMessage ="Marks must be between 0 and 100")]
        public int? Mark { get; set; }
        public SelectList? Subjects {  get; set; }
        public SelectList? Students { get; set; }
        public SelectList? Marks { get; set; }
        //public List<SelectListItem> Subjects {  get; set; }
    }
}
