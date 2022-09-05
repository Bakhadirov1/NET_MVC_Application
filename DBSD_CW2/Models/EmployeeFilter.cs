using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DBSD_CW2.Models
{
    public class EmployeeFilter
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public int PositionId { get; set; }
        public int ProjectId { get; set; }
        public string SortColName { get; set; }
        public bool ColAsc { get; set; }
        public int Page { get; set; }
        public int PageSize { get => 3; }
        public IPagedList<Employee> Employees { get; set; }

        public override string ToString()
        {
            return @$" {this.Name}, {this.LastName}, {this.DateOfBirth}, {this.PhoneNumber}, {this.PositionId}, {this.ProjectId},
                    {this.SortColName}, {this.ColAsc}, {this.Page}, {this.PageSize},
                    {this.Employees.PageSize}, {this.Employees.PageCount}";
        }
    }
}
