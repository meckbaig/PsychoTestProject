using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoTestControlPanel
{
    interface IMainView
    {
        bool operationAllowed { get; set; }
        bool atLeastOneSuccessfull { get; set; }
        bool isEncodeChecked { get; }
    }
}
