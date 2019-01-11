using System.Collections.Generic;

namespace Models.Standard
{

    public class MyCompany : Organization
    {
        public List<int> Employees { get; set; }

        public List<int> Departments { get; set; }

    }
}

