using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceDataExtractor.ApplicationLogGenerator
{
    public interface ISchedulerLogs
    {
        void WriteInformation(string msg);
    }
}
