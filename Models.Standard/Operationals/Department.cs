using Models.Standard.Employment;
using Models.Standard.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Models.Standard.Operationals
{
    public class Department
    {
        [JsonConverter(typeof(ObjectIdConverter))]
        [BsonId]
        public ObjectId _objectId { get; set; }

        public int ID { get; set; }

        public int ParentDepartmentID { get; set; }

        public string Name { get; set; }

        public List<int> Employees { get; set; } = new List<int>();

        public List<int> SubDepartments { get; set; } = new List<int>();

        public int DepartmentHeadEmployeeId { get; set; } = -1;

    }
}
