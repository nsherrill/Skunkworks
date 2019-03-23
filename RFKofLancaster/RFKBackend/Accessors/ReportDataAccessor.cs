using RFKBackend.Shared;
using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Accessors
{
    public interface IReportDataAccessor
    {
        ReportDataCollection ExecuteReport(ReportType reportType, int year, string optionalParams = null);
    }

    public class ReportDataAccessor : BaseSqlAccessor, IReportDataAccessor
    {
        Dictionary<ReportType, string> cachedReportSprocs = new Dictionary<ReportType, string>();
        public ReportDataAccessor()
        {
            cachedReportSprocs.Add(ReportType.StaffSheet, "[dbo].[GetStaffSheetByYear]");
        }

        public ReportDataCollection ExecuteReport(ReportType reportType, int year, string optionalParams = null)
        {
            if (!cachedReportSprocs.ContainsKey(reportType))
                return null;

            string sqlString = $@" exec {cachedReportSprocs[reportType]} {year} {optionalParams}";
            List<ReportData> allData = new List<ReportData>();

            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlString;

                    var rdr = cmd.ExecuteReader();
                    bool isFirst = true;
                    while (isFirst || rdr.NextResult())
                    {
                        isFirst = false;
                        string[] headers = null;
                        List<ReportDataRow> currDataSet = new List<ReportDataRow>();
                        while (rdr.Read())
                        {
                            if (headers == null)
                            {
                                headers = base.GetHeaders(rdr);
                            }

                            currDataSet.Add(new ReportDataRow()
                            {
                                Headers = headers,
                                Values = base.GetValues(rdr),
                            });
                        }

                        allData.Add(new ReportData()
                        {
                            Headers = headers,
                            Data = currDataSet.ToArray(),
                        });
                    }
                }
            }

            return new ReportDataCollection()
            {
                Year = year,
                Data = allData.ToArray(),
            };
        }
    }
}
