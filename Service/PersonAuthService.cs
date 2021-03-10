﻿using person.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace person.Service
{
    public class PersonAuthService : IPersonAuthService
    {
        public PersonContext _context { get; set; }
        public SymmetricSecurityKey Key { get; set; }
        public PersonAuthService(PersonContext personContext)
        {
            _context = personContext;
            Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("494812665@qq.com"));
        }

        public string CreateAuth(string name, int IsBoss)
        {
            var claim = new Claim[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role,IsBoss.ToString())
            };
            var token = new JwtSecurityToken(
                claims: claim,//声明的数组
                expires: DateTime.Now.AddDays(1),//当前时间加一小时，一小时后过期
                signingCredentials: new SigningCredentials(Key, SecurityAlgorithms.HmacSha256));
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return  jwtToken ;
        }
    }
}