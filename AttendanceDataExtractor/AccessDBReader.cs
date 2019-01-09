using AttendanceDataExtractor.ApplicationLogGenerator;
using Models.Core.Employment;
using Models.Core.HR.Attendance;
using Models.Core.Operationals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace AttendanceDataExtractor
{
    public static class AccessDBReader
    {
        static private OleDbConnection _accessDBconnection = new OleDbConnection()
        {
            ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:/Attendace/eTimeTrackLite1.mdb; Persist Security Info = False;"
        };

        static private bool _readDB = true;
        static private int _lastMonthsCount = 2;
        private static ISchedulerLogs _applicationLogs;

        internal static void InitLogger(ISchedulerLogs schedulerLogs)
        {
            _applicationLogs = schedulerLogs;
        }

        static public void Read()
        {
            try
            {
                CopyDataBaseFile();

                _accessDBconnection.Open();

                ReadDepartments();
                ReadEmployees();
                ReadAccessDevices();

                AttendanceManager.MonthlyAccessLogs = new List<MonthlyAccessLog>();

                if (_readDB)
                {
                    List<Tuple<int, int>> months = GetLastMonths(_lastMonthsCount);
                    foreach (var t in months)
                    {
                        ReadAccessEventLogsForMonth(t.Item1, t.Item2);
                    }
                }

                _accessDBconnection.Close();
            }
            catch (Exception ex)
            {
                _applicationLogs.WriteInformation($"Error:{ex.Message} \n {ex.StackTrace}\n {ex.StackTrace}");
            }
        }

        private static void CopyDataBaseFile()
        {
            var sourcePath = @"D:/essl/eTimeTrackLite1.mdb";
            var targetPath = @"D:/Attendace/eTimeTrackLite1.mdb";
            if (!System.IO.Directory.Exists(@"D:/Attendace/"))
            {
                System.IO.Directory.CreateDirectory(@"D:/Attendace/");
            }
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourcePath, targetPath, true);
            _applicationLogs.WriteInformation($"Master db file copied to location: {targetPath}");
        }

        //Get last monthsCount months
        private static List<Tuple<int, int>> GetLastMonths(int monthsCount)
        {
            List<Tuple<int, int>> months = new List<Tuple<int, int>>();

            var currMonth = DateTime.Now.Month;
            var currYear = DateTime.Now.Year;

            var curr = new DateTime(currYear, currMonth, 1 /* First day of this month */);

            for (int i = 0; i < monthsCount; i++)
            {
                var dt = curr.AddMonths(-i); //Get first day of the last month
                months.Add(new Tuple<int, int>(dt.Month, dt.Year));
            }
            return months;
        }

        static private void ReadAccessDevices()
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = _accessDBconnection;
            command.CommandText = "select DeviceId,DeviceFName,IpAddress from Devices";

            OleDbDataAdapter da = new OleDbDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);

            DataColumn DeviceFName = dt.Columns["DeviceFName"];
            DataColumn DeviceId = dt.Columns["DeviceId"];
            DataColumn IpAddress = dt.Columns["IpAddress"];

            List<AccessPoint> accessPoints = new List<AccessPoint>();

            foreach (DataRow row in dt.Rows)
            {
                AccessPoint d = new AccessPoint();
                d.ID = int.Parse(row[DeviceId].ToString());
                d.Name = row[DeviceFName].ToString();
                if (d.Name.Contains("Manual") || d.Name.Contains("Test Device"))
                {
                    continue;
                }
                d.IpAddress = row[IpAddress].ToString();
                accessPoints.Add(d);
            }

            AttendanceManager.AccessPoints = accessPoints;
        }

        static private void ReadDepartments()
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = _accessDBconnection;
            command.CommandText = "select DepartmentId,DepartmentFName from Departments";

            OleDbDataAdapter da = new OleDbDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);

            DataColumn DepartmentFName = dt.Columns["DepartmentFName"];
            DataColumn DepartmentId = dt.Columns["DepartmentId"];

            List<Department> departments = new List<Department>();

            foreach (DataRow row in dt.Rows)
            {
                Department d = new Department();
                d.ID = int.Parse(row[DepartmentId].ToString());
                d.Name = row[DepartmentFName].ToString();
                departments.Add(d);
            }

            AttendanceManager.Departments = departments;
        }

        static public void ReadEmployees()
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = _accessDBconnection;
            command.CommandText = "select EmployeeName,EmployeeId,EmployeeCode,Gender,DepartmentId from Employees";

            OleDbDataAdapter da = new OleDbDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);

            DataColumn EmployeeId = dt.Columns["EmployeeId"];
            DataColumn EmployeeName = dt.Columns["EmployeeName"];
            DataColumn EmployeeCode = dt.Columns["EmployeeCode"];
            DataColumn Gender = dt.Columns["Gender"];
            DataColumn DepartmentId = dt.Columns["DepartmentId"];

            List<Employee> employees = new List<Employee>();

            foreach (DataRow row in dt.Rows)
            {
                Employee emp = new Employee();

                var code = row[EmployeeCode].ToString();
                if (ContainsAlphabeticals(code))
                {
                    continue;
                }

                emp.ID = int.Parse(code);
                emp.DepartmentId = int.Parse(row[DepartmentId].ToString());

                var name = row[EmployeeName].ToString();
                var tokens = name.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                emp.FirstName = tokens[0];
                emp.LastName = name.Substring(tokens[0].Length).Trim();

                var gender = row[Gender].ToString();
                emp.Gender = gender == "Male" ? Models.Core.Gender.Male : Models.Core.Gender.Female;

                employees.Add(emp);
            }

            AttendanceManager.Employees = employees;
        }

        private static bool ContainsAlphabeticals(string str)
        {
            foreach (char c in str)
            {
                if (char.IsLetter(c) || c == '_')
                {
                    return true;
                }
            }
            return false;
        }

        public static bool TableExists(string table)
        {
            return _accessDBconnection.GetSchema("Tables", new string[4] { null, null, table, "TABLE" }).Rows.Count > 0;
        }

        static public void ReadAccessEventLogsForMonth(int month, int year)
        {
            var tableName = "DeviceLogs_" + month.ToString() + "_" + year.ToString();
            if(!TableExists(tableName))
            {
                return;
            }

            OleDbCommand command = new OleDbCommand();
            command.Connection = _accessDBconnection;
            command.CommandText = "select DeviceId,UserId,LogDate,DeviceLogId from " + tableName + " where LogDate >= DateAdd('d', -2, Date())";

            OleDbDataAdapter da = new OleDbDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);

            DataColumn UserId = dt.Columns["UserId"];
            DataColumn DeviceId = dt.Columns["DeviceId"];
            DataColumn LogDate = dt.Columns["LogDate"];
            DataColumn DeviceLogId = dt.Columns["DeviceLogId"];

            List<AccessEvent> accessEvents = new List<AccessEvent>();

            foreach (DataRow row in dt.Rows)
            {
                AccessEvent d = new AccessEvent();
                var useId = int.Parse(row[UserId].ToString());
                var employee = AttendanceManager.EmployeeById(useId);
                if (employee == null)
                {
                    continue;
                }
                d.EmployeeID = employee.ID;
                d.EmployeeFirstName = employee.FirstName;
                d.EmployeeLastName = employee.LastName;

                var accessPoint = AttendanceManager.AccessPointById(int.Parse(row[DeviceId].ToString()));
                d.AccessPointID = accessPoint.ID;
                d.AccessPointName = accessPoint.Name;
                d.AccessPointIPAddress = accessPoint.IpAddress;

                var dtString = row[LogDate].ToString().Trim();
                var tokens = dtString.Split(new char[] { ' ', '-', ':' });
                var ye = int.Parse(tokens[2]);
                var mo = int.Parse(tokens[1]);
                var dy = int.Parse(tokens[0]);
                var h = int.Parse(tokens[3]);
                var m = int.Parse(tokens[4]);
                var s = int.Parse(tokens[5]);
                d.EventTime = new DateTime(ye, mo, dy, h, m, s);

                var deviceLogId = row[DeviceLogId].ToString();

                var y = ye.ToString().Substring(2);

                var idStr = d.AccessPointID.ToString() + y + mo.ToString() + deviceLogId;
                var logId = int.Parse(idStr);
                d.ID = logId;

                accessEvents.Add(d);
            }

            var monthlyLog = new MonthlyAccessLog();
            monthlyLog.Month = new DateTime(year, month, 1);
            monthlyLog.AccessEvents = accessEvents;

            AttendanceManager.MonthlyAccessLogs.Add(monthlyLog);
        }

    }
}
