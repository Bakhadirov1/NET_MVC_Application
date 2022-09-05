using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBSD_CW2.DAL;
using DBSD_CW2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using X.PagedList;
using System.Text;
using CsvHelper;
using System.Globalization;
using System.Xml.Serialization;
using System.Text.Json;

namespace DBSD_CW2.Controllers
{
    public class EmployeesController : Controller
    {
        private IRepository _DbManager;
        private Employee _model;

        public EmployeesController(IRepository repository)
        {
            _DbManager = repository;
            _model = new Employee();
        }
        // GET: EmployeesController

        public ActionResult Index()
        {
            ViewBag.listOfPositions = getAllPositions();
            ViewBag.listOfProjects = getAllProjects();
            var listOfEmployees = _DbManager.GetAllProcedure<Employee>(_model);
            return View(listOfEmployees);
        }
        public ActionResult Filter(EmployeeFilter empFilter)
        {
            string colSort;

            colSort = empFilter.ColAsc ? "ASC" : "DESC";
            ViewBag.ColAsc = !empFilter.ColAsc;
            empFilter.Page = empFilter.Page == 0 ? 1 : empFilter.Page;
            var listOfEmployees = _DbManager.Filter<Employee>(_model,
                new
                {
                    Name = empFilter.Name,
                    LastName = empFilter.LastName,
                    DateOfBirth = empFilter.DateOfBirth,
                    PhoneNumber = empFilter.PhoneNumber,
                    PositionId = empFilter.PositionId,
                    ProjectId = empFilter.ProjectId,
                    SortName = empFilter.SortColName,
                    Sort = colSort,
                    Page = empFilter.Page,
                },
                    out int totalCount
                );
            empFilter.Employees = new StaticPagedList<Employee>(listOfEmployees, empFilter.Page, 3, totalCount);
            ViewBag.listOfPositions = getAllPositions();
            ViewBag.listOfProjects = getAllProjects();
            return View(empFilter);
        }

        public ActionResult exportXML(EmployeeFilter empFilter)
        {
            string colSort;

            colSort = empFilter.ColAsc ? "ASC" : "DESC";
            empFilter.Page = empFilter.Page == 0 ? 1 : empFilter.Page;

            var xml = _DbManager.ExportXML(_model, new
            {
                Name = empFilter.Name,
                LastName = empFilter.LastName,
                DateOfBirth = empFilter.DateOfBirth,
                PhoneNumber = empFilter.PhoneNumber,
                PositionId = empFilter.PositionId,
                ProjectId = empFilter.ProjectId,
                SortName = empFilter.SortColName,
                Sort = colSort,
                Page = empFilter.Page,
            });

            if (string.IsNullOrEmpty(xml))
            {
                return NotFound();
            }
            else
            {
                return File(Encoding.UTF8.GetBytes(xml),
                            "application/xml",
                            $"Employees_{DateTime.Now}.xml");
            }
        }

        public ActionResult exportJSON(EmployeeFilter empFilter)
        {
            string colSort;

            colSort = empFilter.ColAsc ? "ASC" : "DESC";
            empFilter.Page = empFilter.Page == 0 ? 1 : empFilter.Page;

            var json = _DbManager.ExportJSON(_model, new
            {
                Name = empFilter.Name,
                LastName = empFilter.LastName,
                DateOfBirth = empFilter.DateOfBirth,
                PhoneNumber = empFilter.PhoneNumber,
                PositionId = empFilter.PositionId,
                ProjectId = empFilter.ProjectId,
                SortName = empFilter.SortColName,
                Sort = colSort,
                Page = empFilter.Page,
            });

            if (string.IsNullOrEmpty(json))
            {
                return NotFound();
            }
            else
            {
                return File(Encoding.UTF8.GetBytes(json),
                            "text/json",
                            $"Employees_{DateTime.Now}.json");
            }
        }

        public ActionResult exportCSV(EmployeeFilter empFilter)
        {
            string colSort;

            colSort = empFilter.ColAsc ? "ASC" : "DESC";
            empFilter.Page = empFilter.Page == 0 ? 1 : empFilter.Page;

            var csv = _DbManager.ExportCSV(_model, new
            {
                Name = empFilter.Name,
                LastName = empFilter.LastName,
                DateOfBirth = empFilter.DateOfBirth,
                PhoneNumber = empFilter.PhoneNumber,
                PositionId = empFilter.PositionId,
                ProjectId = empFilter.ProjectId,
                SortName = empFilter.SortColName,
                Sort = colSort,
                Page = empFilter.Page,
            });

            var headers = @$"EmployeeId,Name,LastName,DateOfBirth,PhoneNumber,Email,Address,City,PositionId,PositionName,ProjectId,ProjectName{System.Environment.NewLine}";

            headers += csv;

            System.Diagnostics.Debug.WriteLine(headers);

            if (string.IsNullOrEmpty(headers))
            {
                return NotFound();
            }
            else
            {
                return File(Encoding.UTF8.GetBytes(headers),
                            "text/csv",
                            $"Employees_{DateTime.Now}.csv");
            }

        }

        public ActionResult ImportXML()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportXML(IFormFile importFile)
        {
            var employees = new List<Employee>();

            if (importFile != null)
            {
                using (var stream = importFile.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    var serializer = new XmlSerializer(typeof(List<Employee>), new XmlRootAttribute("Employees"));
                    employees = (List<Employee>) serializer.Deserialize(reader);
                }
                foreach(var employee in employees)
                {
                    _DbManager.InsertOneProcedure<Employee>(_model, new
                    {
                        ProjectId = employee.ProjectId,
                        PositionId = employee.PositionId,
                        Name = employee.Name,
                        LastName = employee.LastName,
                        DateOfBirth = employee.DateOfBirth,
                        PhoneNumber = employee.PhoneNumber,
                        Email = employee.Email,
                        Address = employee.Address,
                        City = employee.City,
                        Photo = byte.MinValue
                    });
                }
                return redirToFilter();
            }
            else
            {
                ModelState.AddModelError("", "Empty file");
            }

            return View();
        }

        public ActionResult ImportJSON()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ImportJSON(IFormFile importFile)
        {
            var employees = new List<Employee>();

            if (importFile != null)
            {
                using (var stream = importFile.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    var readVal = reader.ReadToEnd();
                    employees = JsonSerializer.Deserialize<List<Employee>>(readVal);
                }
                foreach (var employee in employees)
                {
                    _DbManager.InsertOneProcedure<Employee>(_model, new
                    {
                        ProjectId = employee.ProjectId,
                        PositionId = employee.PositionId,
                        Name = employee.Name,
                        LastName = employee.LastName,
                        DateOfBirth = employee.DateOfBirth,
                        PhoneNumber = employee.PhoneNumber,
                        Email = employee.Email,
                        Address = employee.Address,
                        City = employee.City,
                        Photo = byte.MinValue
                    });

                }
                return redirToFilter();
            }
            else
            {
                ModelState.AddModelError("", "Empty file");
            }

            return View();

        }

        public ActionResult ImportCSV()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportCSV(IFormFile importFile)
        {
            var employees = new List<Employee>();
            if (importFile != null)
            {
                using (var stream = importFile.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    var serializer = new CsvReader(reader, CultureInfo.InvariantCulture);
                    employees = serializer.GetRecords<Employee>().ToList();
                };

                foreach (Employee employee in employees)
                {
                    _DbManager.InsertOneProcedure<Employee>(_model, new
                    {
                        ProjectId = employee.ProjectId,
                        PositionId = employee.PositionId,
                        Name = employee.Name,
                        LastName = employee.LastName,
                        DateOfBirth = employee.DateOfBirth,
                        PhoneNumber = employee.PhoneNumber,
                        Email = employee.Email,
                        Address = employee.Address,
                        City = employee.City,
                        Photo = byte.MinValue
                    });
                };

                return redirToFilter();
            }
            else
            {
                ModelState.AddModelError("", "Empty file");
            }

            return View();

        }


        // GET: EmployeesController/Details/5
        public ActionResult Details(int id)
        {
            var employee = _DbManager.GetOneProcedure<Employee>(_model, new { EmployeeId = id });
            return View(employee);
        }

        // GET: EmployeesController/Create
        public ActionResult Create()
        {

            ViewBag.listOfPositions = getAllPositions();
            ViewBag.listOfProjects = getAllProjects();
            return View();
        }

        // POST: EmployeesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee emp, IFormFile Photo)
        {
            try
            {
                using (var memorySteam = new MemoryStream())
                {
                    Photo.CopyTo(memorySteam);
                    if (memorySteam.Length > 0)
                    {
                        emp.Photo = memorySteam.ToArray();
                    }
                }
                _DbManager.InsertOneProcedure<Employee>(_model,
                   new
                   {
                       ProjectId = emp.ProjectId,
                       PositionId = emp.PositionId,
                       Name = emp.Name,
                       LastName = emp.LastName,
                       Photo = emp.Photo,
                       DateOfBirth = emp.DateOfBirth,
                       PhoneNumber = emp.PhoneNumber,
                       Email = emp.Email,
                       Address = emp.Address,
                       City = emp.City
                   });
                return redirToFilter();
            }
            catch
            {
                return View();
            }
        }

        // GET: EmployeesController/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.listOfPositions = getAllPositions();
            ViewBag.listOfProjects = getAllProjects();
            return View(_DbManager.GetOneProcedure<Employee>(_model, new { EmployeeId = id }));
        }

        // POST: EmployeesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee emp, IFormFile Photo)
        {
            try
            {
                if (Photo?.Length > 0)
                {
                    using (var memorySteam = new MemoryStream())
                    {
                        Photo.CopyTo(memorySteam);
                        if (memorySteam.Length > 0)
                        {
                            emp.Photo = memorySteam.ToArray();
                        }
                    }
                }
                else
                {
                    var empLocal = _DbManager.GetOneProcedure<Employee>(_model, new { EmployeeId = emp.EmployeeId });
                    emp.Photo = empLocal.Photo;
                }

                _DbManager.UpdateOneProcedure<Employee>(_model,
                   new
                   {
                       EmployeeId = emp.EmployeeId,
                       ProjectId = emp.ProjectId,
                       PositionId = emp.PositionId,
                       Name = emp.Name,
                       LastName = emp.LastName,
                       Photo = emp.Photo,
                       DateOfBirth = emp.DateOfBirth,
                       PhoneNumber = emp.PhoneNumber,
                       Email = emp.Email,
                       Address = emp.Address,
                       City = emp.City
                   });
                return redirToFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return redirToFilter();
            }
        }

        // GET: EmployeesController/Delete/5
        public ActionResult Delete(int id)
        {
            _DbManager.DeleteOneProcedure<Employee>(_model, new { EmployeeId = id });
            return redirToFilter();
        }

        // POST: EmployeesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return redirToFilter();
            }
            catch
            {
                return View();
            }
        }

        public FileResult sendPhoto(int id)
        {
            var emp = _DbManager.GetOneProcedure<Employee>(_model, new { EmployeeId = id });
            if (emp != null && emp.Photo?.Length > 0)
            {
                return File(emp.Photo, "image/jpeg", $"{emp.LastName}.jpg");
            }
            return null;
        }

        private List<SelectListItem> getAllPositions()
        {
            List<SelectListItem> listOfPositions = new List<SelectListItem>();
            var allPositions = _DbManager.GetAllProcedure<Position>(new Position());
            listOfPositions.Add(new SelectListItem()
            {
                Value = "",
                Text = "--Select--"
            });
            foreach (Position pos in allPositions.ToList())
            {
                var item = new SelectListItem()
                {
                    Value = pos.PositionId.ToString(),
                    Text = pos.PositionName
                };
                listOfPositions.Add(item);
            }

            return listOfPositions;
        }

        private List<SelectListItem> getAllProjects()
        {
            List<SelectListItem> listOfProjects = new List<SelectListItem>();

            var allProjects = _DbManager.GetAllProcedure<Project>(new Project());

            listOfProjects.Add(new SelectListItem()
            {
                Value = "",
                Text = "--Select--"
            });


            foreach (Project proj in allProjects.ToList())
            {
                var item = new SelectListItem()
                {
                    Value = proj.ProjectId.ToString(),
                    Text = $"{proj.ProjectId} - {proj.ProjectName}"
                };
                listOfProjects.Add(item);
            }

            return listOfProjects;
        }

        private ActionResult redirToFilter()
        {
            return RedirectToAction("Filter", new
            {
                Name = "",
                LastName = "",
                PhoneNumber = "",
                DateOfBirth = "",
                PositionId = "",
                ProjectId = ""
            });
        }
    }
}
