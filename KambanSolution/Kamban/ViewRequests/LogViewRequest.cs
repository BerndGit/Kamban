using Kamban.ViewModels.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ui.Wpf.Common;

namespace Kamban.ViewRequests
{
    class LogViewRequest : ViewRequest
    {
  //      public LogViewModel Log { get; set; }
        public BoxViewModel Box { get; set; }

        public LogViewRequest()
        {
        }

        public LogViewRequest(string viewId) : base(viewId)
        {
            ViewId = viewId;
        }
    }
}
