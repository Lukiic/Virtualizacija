using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.EventArguments
{
    public class EventArgsWithMessage : EventArgs
    {
        public string EventMessage { get; }

        public EventArgsWithMessage(string eventMessage)
        {
            EventMessage = eventMessage;
        }
    }
}
