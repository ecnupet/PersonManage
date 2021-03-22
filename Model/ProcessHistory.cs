using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace person.Model
{
    public enum ProcessHistoryStatus
    {
        Approved,
        Unapproved,
        Running,
        Warning,
        Finished,
        Cancelled
    }
    [Table("ProcessHistory")]
    public class ProcessHistory
    {
        [Key]
        [Column("HID")]
        public int HistroyID { get; set; }
        [Column("UID")]
        public int UserID { get; set; }
        [Column("Process")]
        public string Process { get; set; }
        [Column("Time")]
        public string Time { get; set; }
        [Column("Status")]
        public ProcessHistoryStatus Status { get; set; }
    }
}
