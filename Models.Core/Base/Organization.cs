using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Models.Core.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Models.Core
{
    public abstract class Organization
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId _objectId { get; set; }

        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [BsonDateTimeOptions]
        [DataType(DataType.Date)]
        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; }

        [StringLength(50)]
        [DataType(DataType.Url)]
        public string Website { get; set; }

        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string PrimaryEmailAddress { get; set; }

        public byte[] Logo { get; set; }

        public List<Contact> Locations { get; set; }

    }

}


