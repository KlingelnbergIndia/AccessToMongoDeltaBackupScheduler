using Models.Standard.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace Models.Standard.HR.Attendance
{
    public class AccessEvent
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId _objectId { get; set; }

        public int ID { get; set; }

        public int AccessPointID { get; set; }

        public string AccessPointName { get; set; }

        public string AccessPointIPAddress { get; set; }

        public int EmployeeID { get; set; }

        public string EmployeeFirstName { get; set; }

        public string EmployeeLastName { get; set; }

        [BsonDateTimeOptions]
        public DateTime EventTime { get; set; }
    }
}
