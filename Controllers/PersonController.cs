using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using person.Model;
using person.Response;
using person.Service;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static person.Response.ResponseResult;
namespace person.Controllers
{

    [Route("api/pm")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        public PersonContext _context { get; }
        public IPersonAuthService AuthService { get; set; }
        public IHttpContextAccessor _accessor { get; set; }
        public ILogger<PersonController> _logger { get; set; }
        public PersonController(PersonContext context, IPersonAuthService authService, IHttpContextAccessor http, ILogger<PersonController> logger)
        {
            _context = context;
            AuthService = authService;
            _accessor = http;
            _logger = logger;
        }
        /// <summary>
        /// 注册接口（todo：等待前端对接时加入md5转换）
        /// </summary>
        /// <param name="personInformation"></param>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost("user/logon")]
        public async Task<ActionResult<ResponseResultModel<Object>>> PersonLogon(PersonLogonForm personLogonForm)
        {
            if (_context.PersonInfo.Where(x => x.UserName == personLogonForm.Name).Select(x => x).Count() != 0)
            {
                return Exist("已存在");
            }
            _context.PersonInfo.AddRange(new PersonInfomation { UserName = personLogonForm.Name, Password = personLogonForm.Password });
            await _context.SaveChangesAsync();
            return Success("注册成功");
        }
        /// <summary>
        /// 用户登录（todo:等待前端对接时加入md5转换验证登录）
        /// </summary>
        /// <param name="personLogin"></param>
        /// <returns></returns>
        [HttpPost("user/login")]
        public async Task<ActionResult<ResponseResultModel<Object>>> PersonLogin(PersonLoginForm personLogin)
        {
            var result = _context.PersonInfo.Where(x => x.UserName == personLogin.Name).Select(x => x);
            if (result.Count() == 0)
            {
                return ResponseResult.NotFound("用户名不存在");
            }
            var res = result.First();
            if (PasswordCompare(res.Password,personLogin.Password))
            {
                var isAdmin = result.Select(x => x.IsAdmin).FirstOrDefault();
                var auth = AuthService.CreateAuth(personLogin.Name, isAdmin);
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, personLogin.Name),
                    new Claim("isAdmin", isAdmin.ToString()),
                    new Claim("Name", result.Select(x => x.UserName).First().ToString()),
                    new Claim("accesss_token", auth)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authticationProperties = new AuthenticationProperties();
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authticationProperties);
                return Success("登录成功");
            }
            else
            {
                return Fail("登录失败");
            }
        }
        /// <summary>
        /// 用户信息查询
        /// </summary>
        /// <param name="personPasswordChangeForm"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        [HttpGet("user/userinfo")]
        public  ActionResult<ResponseResultModel<PersonInfoResponse>> PersonInfoGet()
        {
            var user = _accessor.HttpContext.User.Identity.Name;
            var isAdmin = _accessor.HttpContext.User.Claims.Where(c => c.Type == "isAdmin").First().Value;
            return Success(new PersonInfoResponse { Name = user, IsAdmin = int.Parse(isAdmin) }, "查询成功");
        }
        /// <summary>
        /// 管理员修改用户密码和权限
        /// </summary>
        /// <param name="personInfoChangeForm"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        [HttpPost("admin/infochange")]
        public async Task<ActionResult<ResponseResultModel<Object>>> PersonInfoChange(PersonInfoChangeForm personInfoChangeForm)
        {
            var isAdmin = _accessor.HttpContext.User.Claims.Where(c => c.Type == "isAdmin").First().Value;
            if (!Convert.ToBoolean(isAdmin))
            {
                return Fail("无权更改");
            }
            var result = _context.PersonInfo.Where(x => x.UserName == personInfoChangeForm.Name).Select(x => x);
            if (result.Count() == 0)
            {
                return ResponseResult.NotFound("用户名不存在");
            }
            else
            {
                var res = result.First();
                res.Password = personInfoChangeForm.NewPassword;
                res.IsAdmin = personInfoChangeForm.IsAdmin;
                _context.PersonInfo.Update(res);
                await _context.SaveChangesAsync();
                return Success("修改成功");
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        [HttpGet("admin/userlist")]
        public  ActionResult<ResponseResultModel<SearchResult<PersonInfomation>>> UserSearchAsync(int page, int pageSize, string keyWord)
        {
            var isAdmin = _accessor.HttpContext.User.Claims.Where(c => c.Type == "isAdmin").First().Value;
            if (!Convert.ToBoolean(isAdmin))
            {
                return ResponseResult.Unauthorized(default(SearchResult<PersonInfomation>),"无权查询");
            }
            var key = keyWord ?? "";
            var number = _context.PersonInfo.Where(x => x.UserName.Contains(key)).Count();
            var personInfomations = _context.PersonInfo.Where(x => x.UserName.Contains(key)).Select(x => x).Take(page * pageSize).Skip((page - 1) * pageSize).ToList();
            var res = new SearchResult<PersonInfomation> { Records = personInfomations, Count = number };
            return Success(res, "查询成功");
        }

        /// <summary>
        /// 管理员删除用户
        /// </summary>
        /// <param name="personDeleteForm"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        [HttpDelete("admin/user")]
        public async Task<ActionResult<ResponseResultModel<Object>>> DeleteUser(PersonDeleteForm personDeleteForm)
        {
            var isAdmin = _accessor.HttpContext.User.Claims.Where(c => c.Type == "isAdmin").First().Value;
            if (!Convert.ToBoolean(isAdmin))
            {
                return Fail("无权更改");
            }
            var result = _context.PersonInfo.Where(x => x.UserName == personDeleteForm.UserName).Select(x => x);
            if (result.Count() == 0)
            {
                return ResponseResult.NotFound("用户名不存在");
            }
            else
            {
                var res = result.First();
                _context.PersonInfo.Remove(res);
                await _context.SaveChangesAsync();
                return Success("修改成功");
            }
        }
        /// <summary>
        /// http鉴权
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        [HttpGet("auth/check")]
        public ActionResult<ResponseResultModel<AuthCheckResponse>> AuthCheck()
        {
            string accessToken = null;
            var a = _accessor.HttpContext.User.Claims.Where(c => c.Type == "accesss_token").First();
            var name = _accessor.HttpContext.User.Identity.Name;
            var isBoss = _accessor.HttpContext.User.Claims.Where(c => c.Type == "isAdmin").First();
            var id = _accessor.HttpContext.User.Claims.Where(c => c.Type == "Name").First();
            Console.WriteLine(a.ToString());
            accessToken = a.Value;
            var res = AuthService.Validate(accessToken);
            return Success(new AuthCheckResponse { Message = res, ID = id.Value, Name = name, IsAdmin = isBoss.Value},"权限已验证");
        }


        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        [HttpPost("user/logout")]
        public async Task<ActionResult<ResponseResultModel<Object>>> PersonLogout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Success("退出登录");
        }

        [HttpGet("user/test")]
        public  ActionResult<ResponseResultModel<Object>> Test()
        {
            return Success("注册成功");
        }

        [NonAction]
        private bool PasswordCompare(byte[] fromDB, byte[] fromWeb)
        {
            if(fromWeb.Count()> fromDB.Count())
            {
                return false;
            }
            
            for (var i = 0; i < fromWeb.Count();i++)
            {
                if(fromWeb[i] != fromDB[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
