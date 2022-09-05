using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBSD_CW2.Models
{
    public abstract class ParentModel
    {
        public abstract string getAllQuery { get; }
        public abstract string insertQuery { get; }
        public abstract string getOne { get; }
        public abstract string deleteOne { get; }
        public abstract string updateOne { get; }

        public abstract string filter { get; }
        public abstract string exportXML { get; }
        public abstract string exportJSON { get; }
        public abstract string exportCSV { get; }
    }
}
