using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace BCF_API
{
  public class WebWrapper
  {
    public string Request(
      string url,
      string method, 
      string authHeader, 
      string body
    )
    {
      HttpWebRequest TokenWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
      TokenWebRequest.Method = method;
      TokenWebRequest.ContentType = "application/x-www-form-urlencoded; charset = UTF - 8";
      TokenWebRequest.Headers["Authorization"] = authHeader;
      if (method == "POST")
      {
        byte[] ByteArray = new System.Text.UTF8Encoding().GetBytes(body);
        TokenWebRequest.ContentLength = ByteArray.Length;
        using (Stream TextRequestStream = TokenWebRequest.GetRequestStream())
        {
          TextRequestStream.Write(ByteArray, 0, ByteArray.Length);
          TextRequestStream.Flush();
        }
      }

      HttpWebResponse TokenWebResponse = (HttpWebResponse)TokenWebRequest.GetResponse();
      Stream ResponseStream = TokenWebResponse.GetResponseStream();
      StreamReader ResponseStreamReader = new StreamReader(ResponseStream);
      string response = ResponseStreamReader.ReadToEnd();
      ResponseStreamReader.Close();
      ResponseStream.Close();

      return response;
    }
  }
}
