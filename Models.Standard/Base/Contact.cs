using Models.Standard.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Models.Standard
{
    public class Contact
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId _objectId { get; set; }

        public int ID { get; set; }

        public Address Address { get; set; }

        [StringLength(15)]
        [DataType(DataType.PhoneNumber)]
        public string PrimaryPhone { get; set; }

        [StringLength(15)]
        [DataType(DataType.PhoneNumber)]
        public string SecondaryPhone { get; set; }

        [StringLength(15)]
        [DataType(DataType.PhoneNumber)]
        public string Fax { get; set; }
    }

}
