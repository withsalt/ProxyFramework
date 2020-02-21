using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace ProxyFramework
{
    public abstract class IProxyFramework
    {
        public SessionEventArgs EventArgs { get; set; }

        public abstract Task<bool> Action(SessionEventArgs e);

        public virtual Task Request()
        {
            if (EventArgs == null)
            {
                throw new Exception("SessionEventArgs can not null.");
            }
            return Task.CompletedTask;
        }

        public virtual Task Response()
        {
            if (EventArgs == null)
            {
                throw new Exception("SessionEventArgs can not null.");
            }
            return Task.CompletedTask;
        }
    }
}
