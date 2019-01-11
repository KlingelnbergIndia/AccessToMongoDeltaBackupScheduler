using Models.Standard.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Models.Standard.HR.Attendance
{
    public class LeaveRecord
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId _objectId { get; set; }

        public int EmployeeId { get; set; }

        public int CarriedForwardBalance { get; set; }

        public List<Leave> ApprovedEarnedLeaves { get; set; }

        public List<Leave> ApprovedSickLeaves { get; set; }

        public List<Leave> PendingAppliedEarnedLeaves { get; set; }

    }
}
