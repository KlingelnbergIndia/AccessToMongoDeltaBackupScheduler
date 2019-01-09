using AttendanceDataExtractor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceDataExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var seriLog = new ApplicationLogGenerator.SchedulerSerilogs();
            AccessDBReader.InitLogger(seriLog);
            AttendanceManager.InitLogger(seriLog);

            AccessDBReader.Read();
            AttendanceManager.LoadToMongoDB();

            Console.ReadLine();
        }
    }
}
