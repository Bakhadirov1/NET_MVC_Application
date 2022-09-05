using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBSD_CW2.Models;

namespace DBSD_CW2.DAL
{
    public interface IRepository
    {
        public IEnumerable<T> GetAll<T>(ParentModel model) where T: ParentModel;
        public IEnumerable<T> GetAllProcedure<T>(ParentModel model) where T : ParentModel;
        public int InsertOneProcedure<T>(ParentModel model, object values) where T : ParentModel;
        public T GetOneProcedure<T>(ParentModel model, object values) where T : ParentModel;
        public int DeleteOneProcedure<T>(ParentModel model, object values) where T : ParentModel;
        public int UpdateOneProcedure<T>(ParentModel model, object values) where T : ParentModel;

        public IEnumerable<T> Filter<T>(ParentModel model, object values, out int totalCount) where T : ParentModel;
        public string ExportXML(ParentModel model, object values);
        public string ExportJSON(ParentModel model, object values);
        public string ExportCSV(ParentModel model, object values);
        public int InsertMany<T>(ParentModel model, object[] values) where T : ParentModel;

    }
}
