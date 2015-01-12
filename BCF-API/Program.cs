using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BCF_API
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      var login = new ServerLoginUI();
      OAuthModule.ServerLoginUIObj = login;
      login.ShowDialog();

      var client = new WebWrapper();
      
      string AuthorizationPlain = "YjEzMmQ0ZjMtYTY4Mi00ZDg0LWI4NTktNGU3NjM0YjcyNWZh" + ":" + "MzllY2I4MmUtYTQ3Mi00ZWI3LTkzZmYtYzczZjNiMzQyMDFk";
      string AuthorizationEncoded = Convert.ToBase64String(Encoding.Default.GetBytes(AuthorizationPlain));
      string result = client.Request("https://bim--it.net/bcf/OAuth2/token", "POST", "Basic " + AuthorizationEncoded, "grant_type=authorization_code&code=" + login.sAuthCode + "\n");
      dynamic json = JObject.Parse(result);
      string token = json.access_token;

      var str = new StringBuilder();
      str.AppendLine("access_token: " + json.access_token);
      str.AppendLine("token_type: " + json.token_type);
      str.AppendLine("expires_in: " + json.expires_in);
      str.AppendLine("refresh_token: " + json.refresh_token);
      MessageBox.Show(str.ToString());

      result = client.Request("https://bim--it.net/bcf/1.0/projects", "GET", "Bearer " + token, "");
      dynamic projects = JObject.Parse("{list:" + result + "}");

      StringBuilder
        sb = new StringBuilder();
      foreach (var project in projects.list)
      {
        sb.AppendLine((string)project.name);
      }
      MessageBox.Show("GET https://bim--it.net/bcf/1.0/projects" + Environment.NewLine + Environment.NewLine + sb.ToString());
    }
  }
}
