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

    [Route("api/[controller]/[action]")]
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
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<Object>>> PersonLogon(PersonLogonForm personLogonForm)
        {
            if (_context.PersonInfo.Where(x => x.Name == personLogonForm.Name).Select(x => x).Count() != 0)
            {
                return Exist("已存在");
            }
            _context.PersonInfo.AddRange(new PersonInfomation { Name = personLogonForm.Name, Password = personLogonForm.Password, IsBoss = personLogonForm.IsBoss });
            await _context.SaveChangesAsync();
            return Success("注册成功");
        }
        /// <summary>
        /// 用户登录（todo:等待前端对接时加入md5转换验证登录）
        /// </summary>
        /// <param name="personLogin"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<Object>>> PersonLogin(PersonLoginForm personLogin)
        {
            var result = _context.PersonInfo.Where(x => x.Name == personLogin.Name).Select(x => x);
            if (result.Count() == 0)
            {
                return NotFound("用户名不存在");
            }
            var res = result.First();
            if (PasswordCompare(res.Password,personLogin.Password))
            {
                var isBoss = result.Select(x => x.IsBoss).FirstOrDefault();
                var auth = AuthService.CreateAuth(personLogin.Name, isBoss);
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, personLogin.Name),
                    new Claim("IsBoss", isBoss.ToString()),
                    new Claim("ID", result.Select(x => x.ID).First().ToString()),
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
        /// 用户改密码
        /// </summary>
        /// <param name="personPasswordChangeForm"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<Object>>> PersonChangeSerect(PersonPasswordChangeForm personPasswordChangeForm)
        {
            var result = _context.PersonInfo.Where(x => x.Name == personPasswordChangeForm.Name).Select(x => x);
            if (result.Count() == 0)
            {
                return NotFound("用户名不存在");
            }
            else if (result.Where(x => PasswordCompare(x.Password, personPasswordChangeForm.OldPassword)).Count() != 0)
            {
                var res = result.First();
                res.Password = personPasswordChangeForm.NewPassword;
                _context.PersonInfo.Update(res);
                await _context.SaveChangesAsync();
                return Success("修改成功");
            }
            else
            {
                return Fail("修改失败");
            }
        }

        /// <summary>
        /// 用户信息更新
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<Object>>> PersonInfoUpdate(PersonInfoUpdateForm personInfo)
        {
            var name = _accessor.HttpContext.User.Identity.Name;
            if (name == personInfo.Name)
            {
                var result = _context.PersonInfo.Where(x => x.Name == name).Select(x => x).First();
                result.Sex = personInfo.Sex;
                result.Phone = personInfo.Phone;
                result.Apartment = personInfo.Apartment;
                await _context.SaveChangesAsync();
                return Success("修改用户信息成功");
            }
            else
            {
                return Fail("无权修改该用户信息");
            }
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        [HttpGet]
        public ActionResult<ResponseResultModel<PersonInfoUpdateForm>> PersonInfoSearch()
        {

            var result = _context.PersonInfo.Where(x => x.Name == _accessor.HttpContext.User.Identity.Name).First();
            var response = new PersonInfoUpdateForm
            {
                Name = result.Name,
                Apartment = result.Apartment,
                Phone = result.Phone,
                Sex = result.Sex
            };
            return Success<PersonInfoUpdateForm>(response, "获取用户信息成功");
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<Object>>> PersonLogout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Success("退出登录");
        }


        /// <summary>
        /// 人员搜索（测试功能）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonInfomation>> PersonSearch(int id)
        {
            var person = await _context.PersonInfo.FindAsync(id);
            return person;
        }
        /// <summary>
        /// test
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet()]
        public ActionResult<IEnumerable<string>> Get(string str)
        {
            ///获取Token的三种方式

            //第一种直接用JwtSecurityTokenHandler提供的read方法
            var jwtHander = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken = jwtHander.ReadJwtToken(str);

            //第二种 通过User对象获取
            var sub = User.FindFirst(d => d.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

            //第三种 通过Httpcontext上下文中直接获取
            var name = _accessor.HttpContext.User.Identity.Name;
            var Claims = _accessor.HttpContext.User.Claims;
            var claimstype = (from item in Claims where item.Type == JwtRegisteredClaimNames.Email select item.Value).ToList();

            return new string[] { JsonConvert.SerializeObject(jwtSecurityToken), sub, name, JsonConvert.SerializeObject(claimstype) };
        }
        [Authorize]
        [HttpGet]
        public ActionResult<ResponseResultModel<Object>> AuthCheck()
        {
            return Success("已授权请求");
        }


        [NonAction]
        private int SearchUserID(string name) => _context.PersonInfo.Where(x => x.Name == name).Select(x => x.ID).FirstOrDefault();
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
