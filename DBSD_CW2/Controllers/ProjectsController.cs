using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBSD_CW2.DAL;
using DBSD_CW2.Models;

namespace DBSD_CW2.Controllers
{
    public class ProjectsController : Controller
    {
        private IRepository _DbManager;
        private Project _model;

        public ProjectsController(IRepository repository)
        {
            _DbManager = repository;
            _model = new Project();
        }
        // GET: ProjectController
        public ActionResult Index()
        {
            var listOfProjects = _DbManager.GetAllProcedure<Project>(_model);
            return View(listOfProjects);
        }

        // GET: ProjectController/Create
        public ActionResult Create()
        {
            ViewBag.ErrorMessage = "";
            return View();
        }

        // POST: ProjectController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Project project)
        {
            try
            {
                _DbManager.InsertOneProcedure<Project>(project, new { projectName = project.ProjectName });
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.ErrorMessage = "Probably ProjectName is too short, at least 3 chars needed";
                return View();
            }
        }

        // GET: ProjectController/Edit/5
        public ActionResult Edit(int id)
        {
            return View(_DbManager.GetOneProcedure<Project>(_model, new { ProjectId = id }));
        }

        // POST: ProjectController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Project project)
        {
            try
            {
                ViewBag.ErrorMessage = "";
                _DbManager.UpdateOneProcedure<Project>(project, new {
                    ProjectName = project.ProjectName,
                    ProjectId = project.ProjectId
                });
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.ErrorMessage = "Probably ProjectName is too short, at least 3 chars needed";
                return View();
            }
        }

        // GET: ProjectController/Delete/5
        public ActionResult Delete(int id)
        {
            _DbManager.DeleteOneProcedure<Project>(_model, new { ProjectId = id });
            return RedirectToAction(nameof(Index));
        }
    }
}
