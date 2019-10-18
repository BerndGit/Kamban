using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamban.ViewModels.Core
{
    public class LogEntryViewModel : ILogEntry
    {
        public int Id { get; set; }

        public DateTime Time { get; set; }
        public string Source { get; set; }
        public string Board { get; set; }
        public string Column { get; set; }
        public string Row { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Note { get; set; }

        public int CardId { get; set; }
        public int RowId { get; set; }
        public int ColumnId { get; set; }
        public int BoardId { get; set; }

        public bool Automatic { get; set; } = true;
}
}
