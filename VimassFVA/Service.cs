using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace VimassFVA
{
    public class Service
    {        
        public static String SendWebrequest_POST_Method(string json,string url)
        {
            string result = "";
            try
            {                               
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    result = client.UploadString(url, "POST", json);
                    Debug.WriteLine("request: " + json);
                    Debug.WriteLine("response: " + result);
                }                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return result;
        }
    }
}
