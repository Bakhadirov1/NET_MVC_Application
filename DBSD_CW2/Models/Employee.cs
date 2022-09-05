using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DBSD_CW2.Models
{
    public class Employee : ParentModel
    {
        [HiddenInput(DisplayValue = false)]
        public override string getAllQuery { get => @"getEmployees"; }

        [HiddenInput(DisplayValue = false)]
        public override string insertQuery { get => @"insertEmployee"; }
        [HiddenInput(DisplayValue = false)]
        public override string getOne { get => @"getOneEmployee"; }
        [HiddenInput(DisplayValue = false)]
        public override string deleteOne { get => @"deleteOneEmployee"; }
        [HiddenInput(DisplayValue = false)]
        public override string updateOne { get => @"updateOneEmployee"; }
        [HiddenInput(DisplayValue = false)]
        public override string filter { get => @"filterEmployees"; }
        [HiddenInput(DisplayValue = false)]
        public override string exportXML { get => @"employeeExportXML"; }
        [HiddenInput(DisplayValue = false)]
        public override string exportJSON { get => @"employeeExportJSON"; }
        [HiddenInput(DisplayValue = false)]
        public override string exportCSV { get => @"employeeExportCSV"; }

        public int EmployeeId { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayName("Last name")]
        [Required]
        public string LastName { get; set; }
        [Required]
        [CsvHelper.Configuration.Attributes.Ignore]
        public byte[] Photo { get; set; }
        [Required]
        [DisplayName("Birth date")]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [RegularExpression(@"^[+]+[0-9]{12,}$")]
        [StringLength(13)]
        [DisplayName("Phone")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]

        public string Address { get; set; }
        [Required]

        public string City { get; set; }

        [DisplayName("Position")]
        [Required]
        public int PositionId { get; set; }
        [DisplayName("Position")]
        public string PositionName { get; set; }

        [DisplayName("ProjectId")]
        [Required]
        public int ProjectId { get; set; }
        [DisplayName("Project Name")]
        public string ProjectName { get; set; }

        public override string ToString()
        {
            return $"{this.Name}, {this.LastName}, {this.DateOfBirth}, {this.PhoneNumber}";
        }

    }
}
