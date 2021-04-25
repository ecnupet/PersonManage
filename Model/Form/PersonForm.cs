using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace person.Model
{
    public class PersonLogonForm
    {
        /// <summary>
        /// 用户账号
        /// </summary>
        [Description("用户账号")]
        public string Name { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Description("用户密码")]
        public byte[] Password { get; set; }

    }
    public class PersonLoginForm
    {
        /// <summary>
        /// 用户账号
        /// </summary>
        [Description("用户账号")]
        public string Name { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Description("用户密码")]
        public byte[] Password { get; set; }
    }
    public class PersonInfoChangeForm
    {
        /// <summary>
        /// 用户账号
        /// </summary>
        [Description("用户账号")]
        public string Name { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Description("用户新密码")]
        public byte[] NewPassword { get; set; }
        /// <summary>
        /// 是否为管理身份
        /// </summary>
        [Description("是否为管理身份")]
        public int IsAdmin { get; set; }
    }

    public class PersonDeleteForm
    {
        /// <summary>
        /// 删除用户名
        /// </summary>
        [Description("删除用户名")]
        public string UserName { get; set; }
    }
    public class SearchResult<T>
    {
        public int Count { get; set; }
        public List<T> Records { get; set; }
    }
}
