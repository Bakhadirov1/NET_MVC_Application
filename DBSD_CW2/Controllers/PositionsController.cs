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
    public class PositionsController : Controller
    {
        private IRepository _DbManager;
        private Position _model;

        public PositionsController(IRepository repository)
        {
            _DbManager = repository;
            _model = new Position();
        }
        // GET: PositionsController
        public ActionResult Index()
        {
            var listOfPositions = _DbManager.GetAllProcedure<Position>(_model);
            return View(listOfPositions);
        }

        // GET: PositionsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PositionsController/Create
        public ActionResult Create()
        {
            ViewBag.ErrorMessage = "";
            return View();
        }

        // POST: PositionsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Position position)
        {
            try
            {
                _DbManager.InsertOneProcedure<Position>(_model, new { PositionName = position.PositionName });
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.ErrorMessage = "Probably PositionName is too short, at least 3 chars needed";
                return View();
            }
        }

        // GET: PositionsController/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.ErrorMessage = "";
            return View(_DbManager.GetOneProcedure<Position>(_model, new { PositionId = id }));
        }

        // POST: PositionsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Position position)
        {
            try
            {
                _DbManager.UpdateOneProcedure<Position>(_model, new
                {
                    PositionName = position.PositionName,
                    PositionId = position.PositionId
                });
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.ErrorMessage = "Probably PositionName is too short, at least 3 chars needed";
                return View();
            }
        }

        // GET: PositionsController/Delete/5
        public ActionResult Delete(int id)
        {
            _DbManager.DeleteOneProcedure<Position>(_model, new { PositionId = id });
            return RedirectToAction(nameof(Index));
        }
    }
}
