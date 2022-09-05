using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using DBSD_CW2.Models;

namespace DBSD_CW2.DAL
{
    public class DbManager: IRepository
    {
        private string _connStr;

        public DbManager(string connStr)
        {
            _connStr = connStr;
        }
        public IEnumerable<T> GetAll<T>(ParentModel model) where T: ParentModel
        {
            using(var conn = new SqlConnection(_connStr))
            {
                return conn.Query<T>(model.getAllQuery);
            }
        }

        public IEnumerable<T> GetAllProcedure<T>(ParentModel model) where T: ParentModel
        {
            using(var conn = new SqlConnection(_connStr))
            {
                return conn.Query<T>(model.getAllQuery, null, commandType: CommandType.StoredProcedure);
            }
        }

        public int InsertOneProcedure<T>(ParentModel model, Object values) where T: ParentModel
        {
            using(var conn = new SqlConnection(_connStr))
            {
                return conn.QueryFirstOrDefault<int>(
                    model.insertQuery,
                    values,
                    commandType: CommandType.StoredProcedure
                    );
            }
        }

        public T GetOneProcedure<T>(ParentModel model, Object values) where T: ParentModel
        {
            using(var conn = new SqlConnection(_connStr))
            {
                return conn.QueryFirstOrDefault<T>(
                    model.getOne,
                    values,
                    commandType: CommandType.StoredProcedure
                    );
            }
        }

        public int DeleteOneProcedure<T>(ParentModel model, object values) where T : ParentModel
        {
            using (var conn = new SqlConnection(_connStr))
            {
                return conn.QueryFirstOrDefault<int>(
                    model.deleteOne,
                    values,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public int UpdateOneProcedure<T>(ParentModel model, object values) where T : ParentModel
        {
            using (var conn = new SqlConnection(_connStr))
            {
                return conn.QueryFirstOrDefault<int>(model.updateOne,
                    values,
                    commandType: CommandType.StoredProcedure);
            }

        }

        public IEnumerable<T> Filter<T>(ParentModel model, object values, out int totalCount) where T : ParentModel
        {
            using (var conn = new SqlConnection(_connStr))
            {
                var dPars = new DynamicParameters(values);
                dPars.Add("@totalCount",
                    dbType: DbType.Int32,
                    direction: ParameterDirection.Output);
                conn.InfoMessage += connection_InfoMessage;
                var res = conn.Query<T>(model.filter, dPars, commandType: CommandType.StoredProcedure);
                totalCount = dPars.Get<int>("@totalCount");

                return res;
            }
        }
        private static void connection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            // this gets the print statements (maybe the error statements?)
            var outputFromStoredProcedure = e.Message;
        }

        public string ExportXML(ParentModel model, object values)
        {
            using(var conn = new SqlConnection(_connStr))
            {
                var dPar = new DynamicParameters(values);
                dPar.Add("@xml", dbType: DbType.Xml, direction: ParameterDirection.Output);
                conn.Execute(model.exportXML,
                    dPar,
                    commandType: CommandType.StoredProcedure);
                return dPar.Get<string>("@xml");
            }
        }

        public string ExportJSON(ParentModel model, object values)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                var dPar = new DynamicParameters(values);
                dPar.Add("@JSON", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);
                conn.Execute(model.exportJSON,
                    dPar,
                    commandType: CommandType.StoredProcedure);
                return dPar.Get<string>("@JSON");
            }
        }

        public string ExportCSV(ParentModel model, object values)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                var dPar = new DynamicParameters(values);
                dPar.Add("@csv", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);
                conn.Execute(model.exportCSV,
                    dPar,
                    commandType: CommandType.StoredProcedure);
                return dPar.Get<string>("@csv");
            }
        }

        public int InsertMany<T>(ParentModel model, object[] values) where T : ParentModel
        {
            using (var conn = new SqlConnection(_connStr))
            {
                foreach (var emp in values)
                {
                    conn.QueryFirstOrDefault<int>(
                    model.insertQuery,
                    emp,
                    commandType: CommandType.StoredProcedure
                    );
                }
            }

            return 1;
                
        }
    }
}
