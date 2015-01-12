
using Kayak.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BCF_API
{
  public class OAuthModule : KayakService
  {
    public static ServerLoginUI ServerLoginUIObj { get; set; }

    private static SynchronizationContext
      m_SyncContext;

    static OAuthModule()
    {
      m_SyncContext = SynchronizationContext.Current;
    }

    [Verb("GET")]
    [Path("/")]
    public void Execute()
    {
      int
        iPosStart = this.Request.RequestUri.IndexOf("?code=");
      if (iPosStart >= 0)
      {
        int
          iPosEnd = this.Request.RequestUri.IndexOf("&");
        ServerLoginUIObj.sAuthCode = this.Request.RequestUri.Substring(iPosStart + 6, iPosEnd - iPosStart - 6);
      }
      else
        throw new Exception("Login failed!");

      m_SyncContext.Post((x) => ServerLoginUIObj.Close(), null);
      Response.WriteLine("OK");
    }
  }
}
