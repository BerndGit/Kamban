using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamban.ViewModels.Core
{
    public class LogEntry : ILogEntry
    {
        public DateTime Time { get; set; }
        public string Source { get; set; }
        public string Board { get; set; }
        public string Cloumn { get; set; }
        public string Row { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Note { get; set; }
        public int RowId { get; set; }
        public int CloumnId { get; set; }
        public int BoardId { get; set; }
    }
}
