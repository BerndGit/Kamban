using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamban.ViewModels.Core
{
    public interface ILogEntry
    {
        DateTime Time { get; set; }
        String Source { get; set; }

        String Board { get; set; }
        String Cloumn { get; set; }
        String Row { get; set; }
        String Property { get; set; }
        String OldValue { get; set; }
        String NewValue { get; set; }

        String Note { get; set; }

        int RowId { get; set; }
        int CloumnId { get; set; }
        int BoardId { get; set; }
    }
}
