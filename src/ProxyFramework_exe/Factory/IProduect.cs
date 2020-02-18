using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace ProxyFramework.Factory
{
    interface IProduect
    {
        Task DoRequest(SessionEventArgs content);

        Task DoResponse(SessionEventArgs content);
    }
}
