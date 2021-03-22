using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using person.Response;
public class MyAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler
         DefaultHandler = new AuthorizationMiddlewareResultHandler();

    public async Task HandleAsync(
        RequestDelegate requestDelegate,
        HttpContext httpContext,
        AuthorizationPolicy authorizationPolicy,
        PolicyAuthorizationResult policyAuthorizationResult)
    {
        // if the authorization was forbidden and the resource had specific requirements,
        // provide a custom response.
        if (Show401UnauthorizedResult(policyAuthorizationResult))
        {
            await httpContext.Response.WriteAsJsonAsync(ResponseResult.Fail("无权限"));
            // Return a 404 to make it appear as if the resource does not exist.
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        // Fallback to the default implementation.
        await DefaultHandler.HandleAsync(requestDelegate, httpContext, authorizationPolicy,
                               policyAuthorizationResult);
    }

    bool Show401UnauthorizedResult(PolicyAuthorizationResult policyAuthorizationResult)
    {
        
        return policyAuthorizationResult.Forbidden &&
            policyAuthorizationResult.AuthorizationFailure.FailedRequirements.OfType<
                                                           Show401Requirement>().Any();
    }
}

public class Show401Requirement : IAuthorizationRequirement { }