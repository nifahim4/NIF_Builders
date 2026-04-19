using NIF_Builders.Models;
using NIF_Builders.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NIF_Builders.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        // GET: Projects
        private readonly ConstructionDbContext db = new ConstructionDbContext();

        // GET: Projects
        [AllowAnonymous]
        public ActionResult Index(int page = 1)
        {
            var projects = db.Projects
                             .Include(x => x.ProjectEquipments.Select(b => b.Equipment))
                             .OrderByDescending(x => x.ProjectID)
                             .ToList();
            //return View(db.Trainees.OrderBy(x=>x.TraineeId).ToPagedList(page,5));
            //return View(projects);
            return View(db.Projects.OrderByDescending(x => x.ProjectID).ToPagedList(page,3));
        }
        public ActionResult AddNewEquipment(int? id)
        {
            ViewBag.equipments = new SelectList(db.Equipments.ToList(), "EquipmentID", "EquipmentName", (id != null) ? id.ToString() : "");
            return PartialView("_addNewEquipment");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProjectVM projectVM, int[] equipmentId)
        {
            if (ModelState.IsValid)
            {
                Project project = new Project()
                {
                    ProjectName = projectVM.ProjectName,
                    StartDate = projectVM.StartDate,
                    EstimateEndDate = projectVM.EstimateEndDate,
                    EstimatedDays = projectVM.EstimatedDays,
                    WorkInProgress = projectVM.WorkInProgress
                };

                // File/Image Process
                HttpPostedFileBase file = projectVM.ProjectDocumentFile;
                if (file != null)
                {
                    string fileName = Path.Combine("/Images/", DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileName));
                    file.SaveAs(Server.MapPath(fileName));
                    project.ProjectDocuments = fileName;
                }

                if (equipmentId != null)
                {
                    foreach (var item in equipmentId)
                    {
                        ProjectEquipment projectEquipment = new ProjectEquipment()
                        {
                            Project = project,
                            ProjectID = project.ProjectID,
                            EquipmentID = item
                        };
                        db.ProjectEquipments.Add(projectEquipment);
                    }
                }

                db.SaveChanges();
                return PartialView("_success");
            }
            return PartialView("_error");
        }

        public ActionResult Edit(int? id)
        {
            Project project = db.Projects.First(x => x.ProjectID == id);
            var projectEquipments = db.ProjectEquipments.Where(x => x.ProjectID == id).ToList();

            ProjectVM projectVM = new ProjectVM()
            {
                ProjectID = project.ProjectID,
                ProjectName = project.ProjectName,
                StartDate = project.StartDate,
                EstimateEndDate = project.EstimateEndDate,
                EstimatedDays = project.EstimatedDays,
                ProjectDocuments = project.ProjectDocuments,
                WorkInProgress = project.WorkInProgress,
                EquipmentList = new List<int>() 
            };

            if (projectEquipments.Count() > 0)
            {
                foreach (var item in projectEquipments)
                {
                    projectVM.EquipmentList.Add(item.EquipmentID);
                }
            }
            return View(projectVM);
        }

        [HttpPost]
        public ActionResult Edit(ProjectVM projectVM, int[] equipmentId)
        {
            if (ModelState.IsValid)
            {
                Project project = new Project()
                {
                    ProjectID = projectVM.ProjectID,
                    ProjectName = projectVM.ProjectName,
                    StartDate = projectVM.StartDate,
                    EstimateEndDate = projectVM.EstimateEndDate,
                    EstimatedDays = projectVM.EstimatedDays,
                    WorkInProgress = projectVM.WorkInProgress
                };

                HttpPostedFileBase file = projectVM.ProjectDocumentFile;
                if (file != null)
                {
                    string fileName = Path.Combine("/Images/", DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileName));
                    file.SaveAs(Server.MapPath(fileName));
                    project.ProjectDocuments = fileName;
                }
                else
                {
                    project.ProjectDocuments = projectVM.ProjectDocuments;
                }

                var equipmentEntry = db.ProjectEquipments.Where(x => x.ProjectID == project.ProjectID).ToList();

                foreach (var eq in equipmentEntry)
                {
                    db.ProjectEquipments.Remove(eq);
                }

                if (equipmentId != null)
                {
                    foreach (var item in equipmentId)
                    {
                        ProjectEquipment projectEquipment = new ProjectEquipment()
                        {
                            ProjectID = project.ProjectID,
                            EquipmentID = item
                        };
                        db.ProjectEquipments.Add(projectEquipment);
                    }
                }

                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("_success");
            }
            return PartialView("_error");
        }

        public ActionResult Delete(int? id)
        {
            Project project = db.Projects.First(x => x.ProjectID == id);
            var projectEquipments = db.ProjectEquipments.Where(x => x.ProjectID == id).ToList();

            ProjectVM projectVM = new ProjectVM()
            {
                ProjectID = project.ProjectID,
                ProjectName = project.ProjectName,
                StartDate = project.StartDate,
                EstimateEndDate = project.EstimateEndDate,
                EstimatedDays = project.EstimatedDays,
                ProjectDocuments = project.ProjectDocuments,
                WorkInProgress = project.WorkInProgress,
                EquipmentList = new List<int>()
            };

            if (projectEquipments.Count() > 0)
            {
                foreach (var item in projectEquipments)
                {
                    projectVM.EquipmentList.Add(item.EquipmentID);
                }
            }
            return View(projectVM);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Project project = db.Projects.Find(id);

            if (project == null)
            {
                return HttpNotFound();
            }

            var equipmentEntry = db.ProjectEquipments.Where(x => x.ProjectID == project.ProjectID).ToList();
            db.ProjectEquipments.RemoveRange(equipmentEntry);

            db.Entry(project).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}