using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace person.Model
{
    public enum Auth
    {
        Normal,
        Admin,
        SuperAdmin
    }
    [Table("user_info")]
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
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Description("用户密码")]
        public byte[] Password { get; set; }
        /// <summary>
        /// 是否为管理身份
        /// </summary>
        [Description("是否为管理身份")]
        public Auth Authorization { get; set; }

    }
}
