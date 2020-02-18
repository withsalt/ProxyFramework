using ProxyFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace Demo.QXClub
{
    public class QXClubProxy : IProxyFramework
    {
        public override bool Rule(SessionEventArgs e)
        {
            return true;
        }

        public override Task Response()
        {
            base.Response();

            return Task.CompletedTask;
        }
    }
}
