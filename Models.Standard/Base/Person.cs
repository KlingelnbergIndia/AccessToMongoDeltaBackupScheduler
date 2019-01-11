using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;
using Models.Standard.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Models.Standard 
{
    public enum Gender
    {
        Male = 1,
        Female = 2
    }

    public class Person
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId _objectId { get; set; }

        public int ID { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public Gender Gender { get; set; } = Gender.Male;

        [StringLength(10)]
        public string Prefix { get; set; } = "Mr.";

        [BsonDateTimeOptions]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [BsonDateTimeOptions]
        public DateTime BirthDate { get; set; }

        public Address HomeAddress { get; set; } = new Address();
        public Address WorkAddress { get; set; } = new Address()
        {
            Unit = "Office No. 501",
            Building = "OM Chambers, T29/31",
            Street = "Bhosari Telco Road",
            Landmark = "Near Sharayu Toyota Showroom",
            Locality = "MIDC Bhosari",
            City = "Pune",
            State = "Maharashtra",
            Country = "India",
            ZipCode = "411026",
            MapLink = "https://goo.gl/maps/1WJqS32rMw12"
        };

        [StringLength(15)]
        public string MobilePhone { get; set; } = "+91 9999999999";

        [StringLength(15)]
        public string WorkPhone { get; set; } = "+91 9999999999";

        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = "xyz@Klingelnberg.com";

        public byte[] Photo { get; set; } = null;

        //public List<SocialProfile> SocialConnections { get; set; }
    }

}
