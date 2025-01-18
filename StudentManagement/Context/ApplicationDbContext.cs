using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;

namespace StudentManagement.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Marks> Marks { get; set; }
        public DbSet<StudentSubjectMarks> StudentSubjectMarks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentSubjectMarks>().HasNoKey();
        }
    }
}
