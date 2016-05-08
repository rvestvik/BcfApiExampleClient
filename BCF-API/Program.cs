using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace BCF_API
{
    static class Program
    {
        public const bool USE_DYNAMIC_CLIENT_REGISTRATION = true;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Please fill inn:
            string url = "https://bim--it.net/bcf";
            string clientId;
            string clientSecret;

            var client = new WebWrapper();


            // Get server authentication endpoints
            dynamic authResult = JObject.Parse(client.Request(url + "/auth", "GET", string.Empty, string.Empty));
            // Get user-client grant authorization endpoint (Webpage that is displayed to the user)
            var authEndpoint = (string) authResult.oauth2_auth_url;
            // Get endpoint for dynamic client registrations
            var clientRegistrationEndpoint = (string) authResult.oauth2_dynamic_client_reg_url;
            // Get the endpoint where token should be POSTed to
            var tokenEndpoint = (string) authResult.oauth2_token_url;


            if (USE_DYNAMIC_CLIENT_REGISTRATION)
            {
                var clientInformation = new
                {
                    client_name = "BCF API Example Client",
                    client_description = "Simple example client",
                    client_url = "https://github.com/rvestvik/BcfApiExampleClient",
                    redirect_url = "http://localhost:25512" // IP adress on which the local Kayak Server is listening
                };
                // Register an OAuth2 client
                dynamic clientCreationResult = JObject.Parse(client.Request(clientRegistrationEndpoint, "POST", string.Empty, JsonConvert.SerializeObject(clientInformation)));
                clientId = (string) clientCreationResult.client_id;
                clientSecret = (string) clientCreationResult.client_secret;
            }
            else
            {
                clientId = "YOUR_CLIENT_ID";
                clientSecret = "YOU_CLIENT_SECRET";
            }


            var login = new ServerLoginUI($"{authEndpoint}?response_type=code&client_id={clientId}&state={Guid.NewGuid()}");
            OAuthModule.ServerLoginUIObj = login;
            login.ShowDialog();

            string AuthorizationPlain = clientId + ":" + clientSecret;
            string AuthorizationEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(AuthorizationPlain));
            string result = client.Request(tokenEndpoint, "POST", "Basic " + AuthorizationEncoded, "grant_type=authorization_code&code=" + login.sAuthCode);
            dynamic json = JObject.Parse(result);
            string token = json.access_token;

            var str = new StringBuilder();
            str.AppendLine("access_token: " + json.access_token);
            str.AppendLine("token_type: " + json.token_type);
            str.AppendLine("expires_in: " + json.expires_in);
            str.AppendLine("refresh_token: " + json.refresh_token);
            MessageBox.Show(str.ToString());

            result = client.Request(url + "/1.0/projects", "GET", "Bearer " + token, "");
            dynamic projects = JObject.Parse("{list:" + result + "}");

            StringBuilder
                sb = new StringBuilder();
            foreach (var project in projects.list)
            {
                sb.AppendLine((string) project.name);
            }
            MessageBox.Show("GET https://bim--it.net/bcf/1.0/projects" + Environment.NewLine + Environment.NewLine + sb.ToString());
        }
    }
}
