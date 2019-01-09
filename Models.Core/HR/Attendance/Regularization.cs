using Models.Core.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace Models.Core.HR.Attendance
{
    public enum RegularizationType
    {
        OutstationDuty,
        CustomerVisit,
        VendorVisit,
        Conference,
        Exhibition,
        WorkFromOtherLocation,
        OfficialWorkOutside,
    }

    public enum RegularizationStatus
    {
        Applied,
        Approved,
        Rejected
    }

    public class Regularization
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId _objectId { get; set; }

        public int ID { get; set; }

        [BsonDateTimeOptions]
        public DateTime StartDate { get; set; }

        [BsonDateTimeOptions]
        public DateTime EndDate { get; set; }

        public int EmployeeID { get; set; }

        public RegularizationType LeaveType { get; set; }

        public RegularizationStatus LeaveStatus { get; set; }

        public string Description { get; set; }
    }
}
