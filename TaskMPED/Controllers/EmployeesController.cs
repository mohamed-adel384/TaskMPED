using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaskMPED.Models;
using TaskMPED.ViewModels;

namespace TaskMPED.Controllers
{
    public class EmployeesController : Controller
    {
        // initial from Db
        private readonly EmployeeDbContext _context;
        private readonly IToastNotification _toastNotification;
        private readonly List<string> _allowedExtentions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;
        public EmployeesController(EmployeeDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            var employee = await _context.Employees.ToListAsync();
            return View(employee);
        }

        public async Task<IActionResult> Create()
        {
            var ViewModel = new EmployeeFormViewModel
            {
                Cities = await _context.Cities.ToListAsync(),
                Departments = await _context.Departments.ToListAsync()
                
            };
            return View("EmployeeForm", ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Cities = await _context.Cities.ToListAsync();
                model.Departments = await _context.Departments.ToListAsync();
            }
            var ViewModel = new EmployeeFormViewModel
            {
                Cities = await _context.Cities.ToListAsync(),
                Departments = await _context.Departments.ToListAsync()
            };
            //(1)  retrive any file in form :-
            var files = Request.Form.Files;
            // if any file not attachment in form :-
            if (!files.Any())
            {
                model.Cities = await _context.Cities.ToListAsync();
                model.Departments = await _context.Departments.ToListAsync();

                ModelState.AddModelError("Image", "Please select Profile Picture");
                return View("EmployeeForm", model);
            }
            //(2) if file is exist :-
            var profilePic = files.FirstOrDefault();
            var allowedExtentions = new List<string> { ".jpg", ".png" };
            // (3) check for extintion in case not valid Raise error
            if (!allowedExtentions.Contains(Path.GetExtension(profilePic.FileName).ToLower()))
            {
                model.Cities = await _context.Cities.ToListAsync();
                model.Departments = await _context.Departments.ToListAsync();

                ModelState.AddModelError("Image", "only .png , .jpg images are allowed!");
                return View("EmployeeForm", model);
            }
            // (4) check size is allowed :-
            if (profilePic.Length > _maxAllowedPosterSize)
            {
                model.Cities = await _context.Cities.ToListAsync();
                model.Departments = await _context.Departments.ToListAsync();

                ModelState.AddModelError("Image", "Image cannot be more than 1 MB!");
                return View("EmployeeForm", model);
            }
            // (5) in case any error not exist :-
            // save Profile Picture  :-
            using var dataStream = new MemoryStream();
            await profilePic.CopyToAsync(dataStream);
            // (6) and save record in database :-
            var employees = new Employee
            {
                Name = model.Name,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                Phone = model.Phone,
                Address = model.Address,
                City_Id = model.City_Id,
                Dep_Id = model.Dep_Id,
                Image = dataStream.ToArray()     
            };
            _context.Employees.Add(employees);
            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Employee created Successfully");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            // check in URL
            if (id == null)
            {
                return BadRequest();
            }
            var emp = await _context.Employees.FindAsync(id);
            //check in DB
            if (emp == null)
            {
                return NotFound();
            }
            var employeeModel = new EmployeeFormViewModel
            {
                Id = emp.Id,
                Name = emp.Name,
                Email = emp.Email,
                DateOfBirth = emp.DateOfBirth,
                Address = emp.Address,
                Phone = emp.Phone,
                Dep_Id = emp.Dep_Id,
                City_Id = emp.City_Id,
                Image = emp.Image,
                Cities = await _context.Cities.ToListAsync(),
                Departments = await _context.Departments.ToListAsync()
            };
            return View("EmployeeForm", employeeModel);          
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeFormViewModel model)
        {
            // validation Form :-
            if (!ModelState.IsValid)
            {
                model.Cities = await _context.Cities.ToListAsync();
                model.Departments = await _context.Departments.ToListAsync();
                return View("EmployeeForm", model);
            }
            // check ID
            var emp = await _context.Employees.FindAsync(model.Id);
            if (emp == null)
            {
                return NotFound();
            }
            // check Any Image is exist or Not
            var files = Request.Form.Files;
            if (files.Any())
            {
                var profilePic = files.FirstOrDefault();
                using var dataStream = new MemoryStream();
                await profilePic.CopyToAsync(dataStream);

                model.Image = dataStream.ToArray();

                if (!_allowedExtentions.Contains(Path.GetExtension(profilePic.FileName).ToLower()))
                {
                    model.Cities = await _context.Cities.ToListAsync();
                    model.Departments = await _context.Departments.ToListAsync();

                    ModelState.AddModelError("Image", "Only .PNG , .JPG image are allowed");
                    return View("EmployeeForm", model);
                }
                // check Size :-
                if (profilePic.Length > _maxAllowedPosterSize)
                {
                    model.Cities = await _context.Cities.ToListAsync();
                    model.Departments = await _context.Departments.ToListAsync();

                    ModelState.AddModelError("Image", "profile picture cannot be more than 1MB!");
                    return View("EmployeeForm", model);
                }
                emp.Image = model.Image;
            }
            emp.Name = model.Name;
            emp.Email = model.Email;
            emp.Phone = model.Phone;
            emp.Address = model.Address;
            emp.City_Id = model.City_Id;
            emp.Dep_Id = model.Dep_Id;
            emp.DateOfBirth = model.DateOfBirth;

            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Employee Updated Successfully");
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var emp = await _context.Employees.Include(m => m.City).Include(m => m.Department).SingleOrDefaultAsync(m => m.Id == id);
            if (emp == null)
            {
                return NotFound();
            }
            return View(emp);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
            {
                return NotFound();
            }
            _context.Employees.Remove(emp);
            _context.SaveChanges();
            return Ok();
        }
    }
}
