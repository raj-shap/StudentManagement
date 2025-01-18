using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Context;
using StudentManagement.Helpers;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students.Where(s => s.IsDeleted == false) .ToListAsync();
            return View(students);
        }
        [HttpGet]
        public ActionResult AddStudent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(StudentViewModel studentViewModel)
        {
            if(studentViewModel == null)
            {
                return BadRequest("Data cannot be null.");
            }

            if(studentViewModel.Photo == null || studentViewModel.Photo.Length == 0)
            {
                ViewBag.NofileSelected = "No File Selected";
                return View();
            }
            var PhotoPath = await ImageInput.ImageInputHelper(studentViewModel.Photo);
            studentViewModel.Path = PhotoPath;

            {
                //var fileName = Path.GetFileName(studentViewModel.Photo.FileName);
                //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                //var directoryPath = Path.GetDirectoryName(filePath);
                //if (!Directory.Exists(directoryPath))
                //{
                //    Directory.CreateDirectory(directoryPath);
                //}

                //using (var stream = new FileStream(filePath, FileMode.Create))
                //{
                //    await studentViewModel.Photo.CopyToAsync(stream);
                //}
                //var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                //var extension = Path.GetExtension(studentViewModel.Photo.FileName).ToLower();
                //if (!allowedExtensions.Contains(extension))
                //{
                //    return Content("Invalid file type. Only image file are allowed");
                //}
            }
            var student = new Student
            {
                Name = studentViewModel.Name,
                Email = studentViewModel.Email,
                Standard = studentViewModel.Standard,
                Address = studentViewModel.Address,
                Path = studentViewModel.Path,
                IsDeleted = false
            };

            student.IsDeleted = false;
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Student");
        }
        //[HttpPost]
        //public async Task<IActionResult> AddStudent(Student student)
        //{
        //    if(student == null)
        //    {
        //        return BadRequest("Data cannot be null.");
        //    }

        //    if(student.Path == null || student.Path.Length == 0)
        //    {
        //        return Content("No File Selected");
        //    }
        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","images",student.)

        //    student.IsDeleted = false;
        //    await _context.Students.AddAsync(student);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("Index", "Student");
        //}

        [HttpGet]
        public ActionResult UpdateStudent(Guid id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            var student = _context.Students.Find(id);
            if(student == null)
            {
                return NotFound();
            }
            var viewStudent = new StudentViewModel
            {
                Name = student.Name,
                Email = student.Email,
                Standard = student.Standard,
                Address = student.Address,
                Path = student.Path
            };
            return View(viewStudent);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStudent(Guid id, StudentViewModel studentViewModel)
        {
            if(studentViewModel == null)
            {
                return BadRequest("Data can not be empty.");
            }
            if(studentViewModel.Photo == null || studentViewModel.Photo.Length == 0)
            {
                ViewBag.NofileSelected = "No File Selected";
                return View();
            }
            var PhotoPath= await ImageInput.ImageInputHelper(studentViewModel.Photo);
            studentViewModel.Path = PhotoPath;


            var student = await _context.Students.FindAsync(id);
            student.Name = studentViewModel.Name;
            student.Email = studentViewModel.Email;
            student.Standard = studentViewModel.Standard;
            student.Address = studentViewModel.Address;
            student.Path = studentViewModel.Path;
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Student");
        }

        [HttpGet]
        public ActionResult GetStudentById(Guid id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            var student = _context.Students.Where(s=> s.Id == id).SingleOrDefault();
            if(student == null)
            {
                return NotFound();
            }
            return View(student);
        }


        [HttpGet]
        public ActionResult DeleteStudent(Guid id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            var student = _context.Students.Find(id);
            if(student == null)
            {
                return NotFound();
            }
            return View(student);
        }
        [HttpPost,ActionName("DeleteStudent")]
        public async Task<IActionResult> DeleteStudentConfirm(Guid id)
        {
            var student = await _context.Students.FindAsync(id);
            student.IsDeleted = true;
            student.DeletedAt = DateTimeOffset.UtcNow;
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Student");
        }
    }
}

