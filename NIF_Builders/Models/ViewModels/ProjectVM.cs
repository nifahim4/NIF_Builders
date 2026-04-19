using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NIF_Builders.Models.ViewModels
{
    public class ProjectVM
    {
        public int ProjectID { get; set; }

        [Display(Name = "Project Name"), Required]
        public string ProjectName { get; set; }

        [Display(Name = "Start Date"), Required, Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime StartDate { get; set; }

        [Display(Name = "Estimate End Date"), Required, Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime EstimateEndDate { get; set; }

        [Display(Name = "Budget")]
        public int EstimatedDays { get; set; }

        [Display(Name = "Project Document Path")]
        public string ProjectDocuments { get; set; }

        [Display(Name = "Upload Document")]
        public HttpPostedFileBase ProjectDocumentFile { get; set; }

        [Display(Name = "Work in Progress")]
        public bool WorkInProgress { get; set; }

        // This list will hold the selected Equipment IDs from your dynamic dropdowns
        public List<int> EquipmentList { get; set; } = new List<int>();
    }
}