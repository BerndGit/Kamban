using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;


namespace Kamban.Repository.Models
{
    public class LogEntry 
    {
        [BsonId] public int Id { get; set; }

        public DateTime Time { get; set; }
        public String Source { get; set; }

        public String Board { get; set; }
        public String Cloumn { get; set; }
        public String Row { get; set; }
        public String Property { get; set; }
        public String OldValue { get; set; }
        public String NewValue { get; set; }

        public String Note { get; set; }

        public int RowId { get; set; }
        public int CloumnId { get; set; }
        public int BoardId { get; set; }
        public int CardId { get; set; }

        public bool Automatic { get; set; }
    }
}
