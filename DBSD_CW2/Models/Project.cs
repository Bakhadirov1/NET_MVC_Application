using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DBSD_CW2.Models
{
    public class Project : ParentModel
    {
        [HiddenInput(DisplayValue = false)]
        public override string getAllQuery { get => @"getProjects"; }
        [HiddenInput(DisplayValue = false)]
        public override string insertQuery { get => @"insertProject"; }
        [HiddenInput(DisplayValue = false)]
        public override string getOne { get => @"getProject"; }
        [HiddenInput(DisplayValue = false)]
        public override string deleteOne { get => @"deleteProject"; }
        [HiddenInput(DisplayValue = false)]
        public override string updateOne { get => @"updateProject"; }

        public int ProjectId { get; set; }
        [Required]
        public string ProjectName { get; set; }

        public override string filter { get => ""; }

        public override string exportXML { get => ""; }

        public override string exportJSON { get => ""; }

        public override string exportCSV { get => ""; }
    }
}
