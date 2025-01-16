using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentManagement.Context;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects.Where(s=> s.IsDeleted == false).ToListAsync();
            //if(subjects.IsNullOrEmpty())
            //{
            //    ViewData["NotFound"] = "No Data Available";
            //    return NotFound();
            //}
            return View(subjects);
        }

        [HttpGet]
        public ActionResult AddSubject()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddSubject(Subject subject)
        {
            if(subject == null)
            {
                return BadRequest();
            }
            var subjectExist = _context.Subjects.Any(s => s.SubjectName.ToLower() == subject.SubjectName.ToLower().Trim());
            if (subjectExist)
            {
                ViewBag.SubjectExist = "Subject already exist...";
                return View();
            }
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            subject.SubjectName = textInfo.ToTitleCase(subject.SubjectName.ToLower()).Trim();
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Subject");
        }

        [HttpGet]
        public async Task<IActionResult> EditSubject(Guid id)
        {
            if(id == Guid.Empty || id == null)
            {
                return BadRequest();
            }
            var subject = await _context.Subjects.FindAsync(id);
            if(subject == null || subject.SubjectName.IsNullOrEmpty())
            {
                return NotFound($"No subject found with ID : {subject.Id}");
            }
            return View(subject);
        }
        [HttpPost]
        public async Task<IActionResult> EditSubject(Subject subject)
        {
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            subject.SubjectName = textInfo.ToTitleCase(subject.SubjectName.ToLower());
            
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Subject");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteSubject(Guid id)
        {
            if(id == null || id == Guid.Empty)
            {
                return BadRequest();
            }
            var subject = await _context.Subjects.FindAsync(id);
            if(subject == null || string.IsNullOrEmpty(subject.SubjectName) || subject.IsDeleted == true){
                return NotFound();
            }
            return View(subject);
        }

        [HttpPost,ActionName("DeleteSubject")]
        public async Task<IActionResult> DeleteSubjectConfirm(Guid id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            subject.IsDeleted = true;
            _context.Subjects.Update(subject);
            return RedirectToAction("Index","Subject");
        }
    }
}
