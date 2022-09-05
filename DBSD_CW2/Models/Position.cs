using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DBSD_CW2.Models
{
    public class Position : ParentModel
    {
        [HiddenInput(DisplayValue = false)]
        public override string getAllQuery { get => @"getPositions"; }
        [HiddenInput(DisplayValue = false)]
        public override string insertQuery { get => @"insertPosition"; }
        [HiddenInput(DisplayValue = false)]
        public override string getOne { get => @"getPosition"; }
        [HiddenInput(DisplayValue = false)]
        public override string deleteOne { get => @"deletePosition"; }
        [HiddenInput(DisplayValue = false)]
        public override string updateOne { get => @"updatePosition"; }

        public int PositionId { get; set; }
        [Required]
        public string PositionName { get; set; }

        public override string filter { get => ""; }

        public override string exportXML { get => ""; }

        public override string exportJSON { get => ""; }

        public override string exportCSV { get => ""; }
    }
}
