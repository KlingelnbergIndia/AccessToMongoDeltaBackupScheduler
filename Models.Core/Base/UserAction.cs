using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Framework
{
    public class UserAction
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string ActionName { get; set; }

        public int UserId { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [BsonDateTimeOptions]
        public DateTime Date { get; set; }
    }

}
