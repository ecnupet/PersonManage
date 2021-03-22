using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace person.Model
{
    [Table("UserCollectTable")]
    public class UserCollectTable
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column("HID")]
        public int HistoryID { get; set; }
        [Column("UID")]
        public int UserID { get; set; }
        [Column("Process")]
        public string Process { get; set; }

    }
}
