using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Core
{
    public class SocialProfileType
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string Profile { get; set; }
    }

    public class SocialProfile
    {
        public int ID { get; set; }

        public SocialProfileType ProfileType { get; set; }

        [StringLength(20)]
        public string AccountId { get; set; }
    }
}
