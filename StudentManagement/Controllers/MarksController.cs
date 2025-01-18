using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Context;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
    public class MarksController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MarksController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            //var marksList = _context.Students.FromSqlRaw<Student>("GetStudentSubjectMarks").ToList();
            var marksList = await _context.StudentSubjectMarks.FromSqlRaw<StudentSubjectMarks>("GetStudentSubjectMarks").ToListAsync();
            Console.WriteLine($"\n\n\n\n{marksList}\n\n\n\n");
            return View(marksList);

            {
                //var students = await _context.Students.Where(s => !s.IsDeleted).Include(s => s.Marks).ThenInclude(m => m.Subject).ToListAsync();

                //var subjects = await _context.Subjects.Where(s => !s.IsDeleted).ToListAsync();
                //ViewBag.Subjects = subjects;

                //return View(students);
            }
        }
        [HttpGet]
        //public ActionResult AddMarks(Guid id)
        public ActionResult AddMarks()
        {
            //if (id == null || id == Guid.Empty)
            //{
            //    return BadRequest("Invalid StudentId");
            //}
            //var student = _context.Students.Find(id);
            var student = _context.Students.Where(s => !s.IsDeleted).ToList();
            if (student == null || !student.Any())
            {
                return NotFound("No Student Found");
            }
            //var subjects = _context.Subjects.Where(s => !s.IsDeleted).ToList();
            //var studentMarks = _context.Marks.Where(m => m.StudentId == id && !m.IsDeleted).ToList();
            //var subjectsWithNoMarks = subjects.Where(s => !studentMarks.Any(m => m.SubjectId == s.Id && m.Mark != null)).ToList();
            var model = new MarksViewModel
            {
                //StudentId = id,
                //Subjects = new SelectList(subjectsWithNoMarks, "Id", "SubjectName")
                Students = new SelectList(student, "Id", "Name"),
                Subjects = null
            };
            return View(model);
        }
        public async Task<IActionResult> GetSubjectByStudent(Guid studentId)
        {
            if (studentId == null || studentId == Guid.Empty)
            {
                return BadRequest("Invalid student id");
            }
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);
            if (student == null)
            {
                return NotFound($"Student with ID : {studentId} Not Found");
            }
            var subjects = await _context.Subjects.Where(s => !s.IsDeleted).ToListAsync();
            if (subjects == null)
            {
                return NotFound("No subjects found");
            }
            var subjectsWithoutMarks = await _context.Subjects
                .Where(s => !s.IsDeleted &&
                    !_context.Marks.Any(m => m.SubjectId == s.Id && m.StudentId == studentId && !m.IsDeleted))
                .ToListAsync();
            if (subjectsWithoutMarks == null)
            {
                return NotFound("Marks already added");
            }

            return Json(subjectsWithoutMarks);
        }

        [HttpPost]
        public async Task<IActionResult> AddMarks(MarksViewModel marksViewModel)
        {
            var markExist = await _context.Marks.Where(m => m.StudentId == marksViewModel.StudentId && m.SubjectId == marksViewModel.SubjectId).FirstOrDefaultAsync();

            if (markExist != null)
            {
                markExist.Mark = marksViewModel.Mark;
                markExist.IsDeleted = false;
                _context.Marks.Update(markExist);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("Index", "Marks");
            }

            var marks = new Marks
            {
                StudentId = marksViewModel.StudentId,
                SubjectId = marksViewModel.SubjectId,
                Mark = marksViewModel.Mark,
                IsDeleted = false
            };
            await _context.Marks.AddAsync(marks);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Marks");
        }

        [HttpGet]
        public ActionResult EditMarks(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest("Invalid StudentId");
            }
            var student = _context.Students.Select(s => !s.IsDeleted);
            if (student == null)
            {
                return NotFound();
            }
            var mark = _context.Marks.Where(s => s.StudentId == id);
            var subjects = _context.Subjects.Where(s => !s.IsDeleted).ToList();
            var studentMarks = _context.Marks.Where(m => m.StudentId == id).ToList();
            var subjectsWithMarks = subjects.Where(s => studentMarks.Any(m => m.SubjectId == s.Id && m.Mark != null && !m.IsDeleted)).ToList();
            var model = new MarksViewModel
            {
                StudentId = id,
                Subjects = new SelectList(subjectsWithMarks, "Id", "SubjectName")
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditMarks(MarksViewModel marksViewModel)
        {
            var mark = _context.Marks.Where(s => (s.StudentId == marksViewModel.StudentId && s.SubjectId == marksViewModel.SubjectId) && !s.IsDeleted).FirstOrDefault();
            mark.Mark = marksViewModel.Mark;
            _context.Marks.Update(mark);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Marks");
        }

        [HttpGet]
        public ActionResult DeleteMarks(Guid id)
        {
            var student = _context.Students.Find(id);
            var subjects = _context.Subjects.Where(s => !s.IsDeleted).ToList();
            var studentMarks = _context.Marks.Where(m => m.StudentId == id).ToList();
            var subjectsWithMarks = subjects.Where(s => studentMarks.Any(m => m.SubjectId == s.Id && m.Mark != null)).ToList();
            var model = new MarksViewModel
            {
                StudentId = id,
                StudentName = student.Name,
                Subjects = new SelectList(subjectsWithMarks, "Id", "SubjectName"),
                Marks = new SelectList(studentMarks, "Id", "Mark")
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMarks(Guid Studentid, Guid SubjectId)
        {
            var mark = _context.Marks.Where(s => s.StudentId == Studentid && s.SubjectId == SubjectId).FirstOrDefault();
            if (mark != null)
            {
                mark.IsDeleted = true;
                mark.DeletedAt = DateTimeOffset.UtcNow;
                _context.Marks.Update(mark);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Marks");
            }
            return View();
        }

        [HttpGet]
        public ActionResult AddMarksBySubject(Guid studentId, Guid subjectId)
        {
            if ((studentId == null || studentId == Guid.Empty) || (subjectId == null || subjectId == Guid.Empty))
            {
                return BadRequest();
            }
            var subject = _context.Subjects.Where(s => s.Id == subjectId).ToList();
            var mark = new MarksViewModel
            {
                StudentId = studentId,
                SubjectId = subjectId,
                Subjects = new SelectList(subject, "Id", "SubjectName")
            };

            return View(mark);
        }
        [HttpPost]
        public async Task<IActionResult> AddMarksBySubject(MarksViewModel marksViewModel)
        {
            var mark = new Marks
            {
                IsDeleted = false,
                StudentId = marksViewModel.StudentId,
                SubjectId = marksViewModel.SubjectId,
                Mark = marksViewModel.Mark
            };
            return RedirectToAction("Index", "Marks");
        }

        [HttpGet]
        public ActionResult EditMarksBySubj(Guid SubjectId, Guid StudentId)
        {
            if ((StudentId == null || StudentId == Guid.Empty) || (SubjectId == null || SubjectId == Guid.Empty))
            {
                return BadRequest();
            }

            var subject = _context.Subjects.Where(s => s.Id == SubjectId && !s.IsDeleted).FirstOrDefault();
            if (subject == null)
            {
                return NotFound("No Subject Found");
            }
            var student = _context.Students.Where(s => s.Id == StudentId && !s.IsDeleted).FirstOrDefault();
            if (student == null)
            {
                return NotFound("Student Not Found");
            }
            var mark = _context.Marks.Where(m => m.StudentId == StudentId && m.SubjectId == SubjectId && !m.IsDeleted).FirstOrDefault();
            var markView = new MarksViewModel
            {
                StudentId = student.Id,
                StudentName = student.Name,
                SubjectId = subject.Id,
                SubjectName = subject.SubjectName,
                Mark = mark.Mark
            };

            return View(markView);
        }
        [HttpPost]
        public async Task<IActionResult> EditMarksBySubj(MarksViewModel marksViewModel)
        {
            var mark = await _context.Marks.Where(m => m.StudentId == marksViewModel.StudentId && m.SubjectId == marksViewModel.SubjectId && !m.IsDeleted).FirstOrDefaultAsync();
            if (mark == null)
            {
                return NotFound($"Marks with Not Found of Student : {marksViewModel.StudentName} for Subject : {marksViewModel.SubjectName}");
            }
            mark.Mark = marksViewModel.Mark;
            _context.Marks.Update(mark);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Marks");
        }

        [HttpGet]
        public ActionResult DeleteMarksBySubj(Guid SubjectId, Guid StudentId)
        {
            if ((StudentId == null || StudentId == Guid.Empty) || (SubjectId == null || SubjectId == Guid.Empty))
            {
                return BadRequest();
            }

            var subject = _context.Subjects.Where(s => s.Id == SubjectId && !s.IsDeleted).FirstOrDefault();
            if (subject == null)
            {
                return NotFound("No Subject Found");
            }
            var student = _context.Students.Where(s => s.Id == StudentId && !s.IsDeleted).FirstOrDefault();
            if (student == null)
            {
                return NotFound("Student Not Found");
            }
            var mark = _context.Marks.Where(m => m.StudentId == StudentId && m.SubjectId == SubjectId && !m.IsDeleted).FirstOrDefault();
            var markView = new MarksViewModel
            {
                StudentId = student.Id,
                StudentName = student.Name,
                SubjectId = subject.Id,
                SubjectName = subject.SubjectName,
                Mark = mark.Mark
            };

            return View(markView);
        }

        [HttpPost, ActionName("DeleteMarksBySubj")]
        public async Task<IActionResult> ConfimDeleteMarksBySubj(Guid SubjectId, Guid StudentId)
        {
            var mark = await _context.Marks.Where(m => m.StudentId == StudentId && m.SubjectId == SubjectId && !m.IsDeleted).FirstOrDefaultAsync();
            if (mark == null)
            {
                return NotFound($"Marks with Not Found of StudentId : {StudentId} for SubjectId : {SubjectId}");
            }
            mark.IsDeleted = true;
            _context.Marks.Update(mark);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Marks");
        }
    }
}
