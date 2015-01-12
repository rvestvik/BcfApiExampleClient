using Kayak;
using Kayak.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BCF_API
{
  public partial class ServerLoginUI : Form
  {
    public string sAuthCode { get; set; }

    public ServerLoginUI()
    {
      InitializeComponent();

      var server = new KayakServer();
      server.UseFramework();
      server.Start(new IPEndPoint(IPAddress.Any, 25512));

      this.FormClosing += (s, e) => server.Stop();
    }
  }
}
