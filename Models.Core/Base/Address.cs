using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using MongoDB.Bson;
using Models.Core.Helpers;

namespace Models.Core
{
    public class Address
    {
        public int ID { get; set; }

        [StringLength(25)]
        public string Unit { get; set; }

        [StringLength(25)]
        public string Building { get; set; }

        [StringLength(25)]
        public string Street { get; set; }

        [StringLength(50)]
        public string Locality { get; set; }

        [StringLength(25)]
        public string Landmark { get; set; }

        [StringLength(25)]
        public string City { get; set; }

        [StringLength(25)]
        public string State { get; set; }

        [StringLength(20)]
        public string Country { get; set; }

        [StringLength(15)]
        public string ZipCode { get; set; }

        [DataType(DataType.Url)]
        public string MapLink { get; set; }
    }
}
