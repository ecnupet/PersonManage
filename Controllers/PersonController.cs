using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public PersonController(PersonContext context, IPersonAuthService authService, IHttpContextAccessor http)
        {
            _context = context;
            AuthService = authService;
            _accessor = http;
        }
        /// <summary>
        /// 注册接口（todo：等待前端对接时加入md5转换）
        /// </summary>
        /// <param name="personInformation"></param>
        /// <returns>
        /// 
        /// </returns>
        [Route("user/logon")]
        [HttpPost]
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
        [Route("user/login")]
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<Object>>> PersonLogin(PersonLoginForm personLogin)
        {
            var result = _context.PersonInfo.Where(x => x.UserName == personLogin.Name).Select(x => x);
            if (result.Count() == 0)
            {
                return NotFound("用户名不存在");
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
        [HttpGet]
        [Route("user/userinfo")]
        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        public  ActionResult<ResponseResultModel<PersonInfoResponse>> PersonInfoGet()
        {
            var user = _accessor.HttpContext.User.Identity.Name;
            var isAdmin = _accessor.HttpContext.User.Claims.Where(c => c.Type == "IsAdmin").First().Value;
            return Success(new PersonInfoResponse { Name = user, IsAdmin = int.Parse(isAdmin) }, "查询成功");
        }
        /// <summary>
        /// 用户改密码
        /// </summary>
        /// <param name="personPasswordChangeForm"></param>
        /// <returns></returns>
        [Route("admin/userinfo")]
        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<Object>>> PersonChangeSerect(PersonPasswordChangeForm personPasswordChangeForm)
        {
            var isAdmin = _accessor.HttpContext.User.Claims.Where(c => c.Type == "isAdmin").First().Value;
            if (!Convert.ToBoolean(isAdmin))
            {
                return Fail("无权更改");
            }
            var result = _context.PersonInfo.Where(x => x.UserName == personPasswordChangeForm.Name).Select(x => x);
            if (result.Count() == 0)
            {
                return NotFound("用户名不存在");
            }
            else
            {
                var res = result.First();
                res.Password = personPasswordChangeForm.NewPassword;
                _context.PersonInfo.Update(res);
                await _context.SaveChangesAsync();
                return Success("修改成功");
            }
        }
        /// <summary>
        /// 管理员删除用户
        /// </summary>
        /// <param name="personDeleteForm"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("admin/user")]
        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        public async Task<ActionResult<ResponseResultModel<Object>>> DeleteUser(PersonDeleteForm personDeleteForm)
        {
            var isAdmin = _accessor.HttpContext.User.Claims.Where(c => c.Type == "IsAdmin").First().Value;
            if (!Convert.ToBoolean(isAdmin))
            {
                return Fail("无权更改");
            }
            var result = _context.PersonInfo.Where(x => x.UserName == personDeleteForm.UserName).Select(x => x);
            if (result.Count() == 0)
            {
                return NotFound("用户名不存在");
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
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        [HttpPost]
        [Route("user/logout")]
        public async Task<ActionResult<ResponseResultModel<Object>>> PersonLogout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Success("退出登录");
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
