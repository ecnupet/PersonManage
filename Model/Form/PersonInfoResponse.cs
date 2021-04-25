using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace person.Model
{
    public class PersonInfoResponse
    {
        public string Name { get; set; }
        public int Authorization { get; set; }
    }
    public class AuthCheckResponse
    {
        public string Authorization { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public bool Message { get; set; }
    }
}
