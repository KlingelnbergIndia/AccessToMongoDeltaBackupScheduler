using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceDataExtractor.ApplicationLogGenerator
{
    public class SchedulerSerilogs : ISchedulerLogs
    {
        private Serilog.Core.Logger Serilog;
        public SchedulerSerilogs()
        {
            Serilog = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File($"D:/Attendace/{DateTime.Now.ToShortDateString()}_logs.txt")
                .CreateLogger();
        }

        public void WriteInformation(string msg)
        {
            Serilog.Information(msg);
        }
    }
}
