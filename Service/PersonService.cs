using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using static person.PersonInformationGet;

namespace person.Service
{
    public class PersonInfomationGet : PersonInformationGetBase
    {
        public IPersonAuthService personAuthService { get; set; }
        public PersonInfomationGet(IPersonAuthService personAuth)
        {
            personAuthService = personAuth;
        }
        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        public override Task<HelloReply> Check(HelloRequest request, ServerCallContext context)
        {
            string accessToken = null;
            var a = context.GetHttpContext().User.Claims.Where(c => c.Type == "accesss_token").First();
            var name = context.GetHttpContext().User.Identity.Name;
            var isBoss = context.GetHttpContext().User.Claims.Where(c => c.Type == "IsBoss").First();
            var id = context.GetHttpContext().User.Claims.Where(c => c.Type == "ID").First();
            Console.WriteLine(a.ToString());
            accessToken = a.Value;
            var res = personAuthService.Validate(accessToken);
            return Task.FromResult(new HelloReply { Message = res, Name = name, Id = id.Value, Isboos = isBoss.Value});
        }
    }
}
