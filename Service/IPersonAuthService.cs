using person.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace person.Service
{
    public interface IPersonAuthService
    {
        public string CreateAuth(string name, Auth auth);
        public bool Validate(string accessToken);
    }
}
