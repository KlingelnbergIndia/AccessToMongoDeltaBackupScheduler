using System;
using System.Collections.Generic;
using Models.Core.Authentication;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Core.Employment
{
    public class Employee : Person
    {
        public string Title { get; set; } = "";

        [BsonDateTimeOptions]
        public DateTime JoiningDate { get; set; }

        public List<string> Roles { get; set; } = new List<string>();

        public int DepartmentId { get; set; }

        //public List<int> OtherAssociatedDepartments { get; set; } = new List<int>();

        public int SubDivisionId { get; set; }

        public DateTime LastUpdatedOn { get; set; } = DateTime.Now;

        public int ReportingSuperiorID { get; set; } = -1;

        public List<int> Reportees { get; set; } = new List<int>();

        [BsonDateTimeOptions]
        public DateTime LeavingDate { get; set; }

        public string ProvidentFundNumber { get; set; } = "'MH/PUN/305790/XXX";
        public string ProvidentFundUANNumber { get; set; } = "000000000000";
        public string PANNumber { get; set; } = "ABCDEFGHIJK";
        public string AadharNumber { get; set; } = "XXXX YYYY ZZZZ";

        public List<string> CompanyCreditCards { get; set; } = new List<string>();

        public bool HasRole(string role)
        {
            return Roles.Contains(role);
        }

        public bool IsInActiveService()
        {
            if(LeavingDate.Year == 1 && LeavingDate.Month == 1 && LeavingDate.Day == 1)
            {
                return true;
            }
            if(LeavingDate < DateTime.Now)
            {
                return false;
            }
            return true;
        }

        public bool IsReportee(int id)
        {
            return Reportees.Contains(id);
        }
    }
}
