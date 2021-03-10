using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace person.Model
{
    public class PersonInfomation
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Description("用户ID")]
        public int ID { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        [Description("用户账号")]
        public string Name { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Description("用户密码")]
        public string Password { get; set; }
        /// <summary>
        /// 是否为管理身份
        /// </summary>
        [Description("是否为管理身份")]
        public int IsBoss { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [Description("性别")]
        public int Sex { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        [Description("电话")]
        public string Phone { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        [Description("部门")]
        public string Apartment { get; set; }
    }
}
