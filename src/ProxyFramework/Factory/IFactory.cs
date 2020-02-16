using ProxyFramework.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;

namespace ProxyFramework.Factory
{
    interface IFactory
    {
        Task Excute(SessionEventArgs content, MethodType method);
    }
}
