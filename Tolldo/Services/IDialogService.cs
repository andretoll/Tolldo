using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tolldo.Services
{
    /// <summary>
    /// Dialog service that handles message input and output.
    /// </summary>
    public interface IDialogService
    {
        event EventHandler MessageChanged;

        void SetMessage(string message);

        string GetMessage();
    }
}
