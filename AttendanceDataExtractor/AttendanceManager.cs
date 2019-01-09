using AttendanceDataExtractor.ApplicationLogGenerator;
using Models.Core.Employment;
using Models.Core.HR.Attendance;
using Models.Core.Operationals;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AttendanceDataExtractor
{
    public class AttendanceManager
    {
        private static ISchedulerLogs _applicationLogs;
        static public List<Employee> Employees { get; set; }

        static public List<Department> Departments { get; set; }

        static public List<AccessPoint> AccessPoints { get; set; }

        static public List<MonthlyAccessLog> MonthlyAccessLogs { get; set; }

        static public List<AccessEvent> AccessEvents
        {
            get
            {
                List<AccessEvent> list = new List<AccessEvent>();
                foreach (var v in MonthlyAccessLogs)
                {
                    foreach (var e in v.AccessEvents)
                    {
                        list.Add(e);
                    }
                }
                return list;
            }
        }

        internal static void InitLogger(ISchedulerLogs schedulerLogs)
        {
            _applicationLogs = schedulerLogs;
        }

        #region MongoDB Specific

        static MongoClient _client = null;

        static IMongoDatabase _database = null;
        static IMongoCollection<Employee> _employeesCollection = null;
        static IMongoCollection<Department> _departmentsCollection = null;
        static IMongoCollection<AccessPoint> _accessPointsCollection = null;
        static IMongoCollection<AccessEvent> _accessEventsCollection = null;
        static IMongoCollection<User> _usersCollection = null;

        internal static void LoadToMongoDB()
        {
            try
            {
                _applicationLogs.WriteInformation("----LoadToMongoDB -> Process started-------");

                _client = new MongoClient("mongodb://localhost:27017");

                _database = _client.GetDatabase("AttendanceDB");
                _accessEventsCollection = _database.GetCollection<AccessEvent>("AccessEvents");
                _accessPointsCollection = _database.GetCollection<AccessPoint>("AccessPoints");

                _database = _client.GetDatabase("EmployeeDB");
                _employeesCollection = _database.GetCollection<Employee>("Employees");

                _database = _client.GetDatabase("OperationalsDB");
                _departmentsCollection = _database.GetCollection<Department>("Departments");

                _database = _client.GetDatabase("AuthDB");
                _usersCollection = _database.GetCollection<User>("Users");

                _applicationLogs.WriteInformation("Mongo Database loaded");
                _applicationLogs.WriteInformation($"Departments count:{Departments?.Count()}");
                foreach (var v in Departments)
                {
                    AddDepartment(v);
                }

                _applicationLogs.WriteInformation($"Employees count:{Employees?.Count()}");
                foreach (var v in Employees)
                {
                    AddEmployee(v);
                }

                _applicationLogs.WriteInformation($"AccessPoints count:{AccessPoints?.Count()}");
                foreach (var v in AccessPoints)
                {
                    AddAccessPoint(v);
                }

                _applicationLogs.WriteInformation($"MonthlyAccessLogs count:{MonthlyAccessLogs?.Count()}");
                foreach (var v in MonthlyAccessLogs)
                {
                    foreach (var e in v.AccessEvents)
                    {
                        AddAccessLog(e);
                    }
                }
                _applicationLogs.WriteInformation($"----LoadToMongoDB -> Process completed----");
            }
            catch (Exception ex)
            {
                _applicationLogs.WriteInformation($"Error:{ex.Message} \n {ex.StackTrace}\n {ex.StackTrace}");
            }
        }

        static public IEnumerable<Department> GetAllDepartments()
        {
            try
            {
                return _departmentsCollection.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        static public IEnumerable<Employee> GetAllEmployees()
        {
            try
            {
                return _employeesCollection.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        static public IEnumerable<AccessPoint> GetAllAccessPoints()
        {
            try
            {
                return _accessPointsCollection.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        static public IEnumerable<AccessEvent> GetAllAccessEvents()
        {
            try
            {
                return _accessEventsCollection.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        static void AddDepartment(Department item)
        {
            try
            {
                var existing = GetAllDepartments();
                foreach (var d in existing)
                {
                    if (d.ID == item.ID)
                    {
                        return;
                    }
                }
                _departmentsCollection.InsertOne(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static public void AddEmployee(Employee item)
        {
            string formatedBDate = item.BirthDate.ToString("dd-mm-yyyy");
            try
            {
                var existing = GetAllEmployees();
                foreach (var d in existing)
                {
                    if (d.ID == item.ID)
                    {
                        return;
                    }
                }

                _employeesCollection.InsertOne(item);
                _usersCollection.InsertOne(new User
                {
                    ID = item.ID,
                    Email = item.Email,
                    UserName = $"{item.FirstName}.{item.LastName}",
                    PasswordHash = ToSha256(formatedBDate)
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static public void AddAccessPoint(AccessPoint item)
        {
            try
            {
                var existing = GetAllAccessPoints();
                foreach (var d in existing)
                {
                    if (d.ID == item.ID)
                    {
                        return;
                    }
                }
                _accessPointsCollection.InsertOne(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static public void AddAccessLog(AccessEvent item)
        {
            try
            {
                var Utc_EventTime = item.EventTime.ToUniversalTime();

                var existingRecord = GetAllAccessEvents()
                    .Any(x =>
                        x.EmployeeID == item.EmployeeID &&
                        x.AccessPointID == item.AccessPointID &&
                        x.AccessPointName == item.AccessPointName &&
                        x.EventTime == Utc_EventTime
                    );
                if (!existingRecord)
                {
                    _accessEventsCollection.InsertOne(item);
                    return;
                }
                /*
                var existing = GetAllAccessEvents();
                foreach (var d in existing)
                {
                    //if(String.IsNullOrEmpty(item.EmployeeFirstName))
                    //{
                    //    return;
                    //}
                    if(d.EmployeeID == 0)
                    {
                        continue;
                    }
                    if (d.AccessPointID == item.AccessPointID &&
                        d.AccessPointName == item.AccessPointName &&
                        d.EmployeeID == item.EmployeeID &&
                        d.EmployeeFirstName == item.EmployeeFirstName &&
                        d.EmployeeLastName == item.EmployeeLastName &&
                        d.EventTime == item.EventTime)
                    {
                        return;
                    }
                }*/
                //_accessEventsCollection.InsertOne(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static AccessPoint AccessPointById(int v)
        {
            foreach (var d in AccessPoints)
            {
                if (d.ID == v)
                {
                    return d;
                }
            }
            return null;
        }

        internal static Department DepartmentById(int v)
        {
            foreach (var d in Departments)
            {
                if (d.ID == v)
                {
                    return d;
                }
            }
            return null;
        }

        internal static Employee EmployeeById(int v)
        {
            foreach (var d in Employees)
            {
                if (d.ID == v)
                {
                    return d;
                }
            }
            return null;
        }

        //internal static void LoadUsersToDB()
        //{
        //    var client = new MongoClient("mongodb://localhost:27017");
        //    var database = client.GetDatabase("AuthDB");
        //    var collection = database.GetCollection<User>("Users");

        //    foreach (var v in Employees)
        //    {
        //        var user = new User()
        //        {
        //            ID = v.ID,
        //            UserName = v.FirstName + "." + v.LastName,
        //            Email = v.FirstName + "." + v.LastName + "@Klingelnberg.com",
        //            PasswordHash = ToSha256(v.FirstName + "$xyz987")
        //        };
        //        collection.InsertOne(user);
        //    }
        //}

        internal static string ToSha256(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.ASCII.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }

        }


        #endregion
    }
}
