using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;


namespace Kamban.Repository.Models
{
    public class LogEntry 
    {
        public int Id { get; set; }

        public DateTime Time { get; set; }
        public String Topic { get; set; }

        public String Board { get; set; }
        public String Column { get; set; }
        public String Row { get; set; }
        public String Property { get; set; }
        public String OldValue { get; set; }
        public String NewValue { get; set; }

        public String Note { get; set; }

        public int RowId { get; set; }
        public int ColumnId { get; set; }
        public int BoardId { get; set; }
        public int CardId { get; set; }

        public bool Automatic { get; set; }
    }
}
