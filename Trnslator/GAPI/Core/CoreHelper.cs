using System;
using System.Text;
using System.Net;
using Gapi.Json;
using System.IO;

namespace Gapi.Core
{
    internal class CoreHelper
    {
        public static IWebProxy Proxy = null;

        public static void SetProxy(IWebProxy newProxy)
        {
            CoreHelper.Proxy = newProxy;
        }

        public static WebRequest BuildWebRequest(string url)
        {
            WebRequest webRequest = WebRequest.Create(url);
            
            //webRequest.Method = "POST";
            
            if (CoreHelper.Proxy != null)
                webRequest.Proxy = CoreHelper.Proxy;
            else
            {
                IWebProxy proxy = webRequest.Proxy;
                if (proxy != null)
                    proxy.Credentials = CredentialCache.DefaultCredentials;
            }
            webRequest.ContentType = "application/x-www-form-urlencoded";            
            return webRequest;
        }

        public static WebRequest request;
        public static string  BuildWebRequest(string url,string PostString)
        {
            string strUrl = url;
            string strPost = PostString;
            
            // Create a request using a URL that can receive a post. 
            request = WebRequest.Create(strUrl);
            IWebProxy proxy = request.Proxy;
            proxy.Credentials = CredentialCache.DefaultCredentials;
            request.Proxy = proxy;
            // Set the Method property of the request to POST.
            request.Method = "POST";
           

            // Create POST data and convert it to a byte array.
            string postData = strPost;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";            
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }


        public static string PerformRequest(string url)
        {
            WebRequest request = CoreHelper.BuildWebRequest(url);
           
            using (WebResponse response = request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public static string PerformRequest(string url,CookieContainer cookie)
        {
            HttpWebRequest request =(HttpWebRequest) CoreHelper.BuildWebRequest(url);
            request.CookieContainer = cookie;
            using (WebResponse response = request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public static JsonObject ParseGoogleAjaxAPIResponse(string responseData)
        {
            // Common response validation
            // Validate 'responseStatus'
            JsonObject jsonObject = JsonObject.Parse(responseData);
            if (jsonObject == null)
                throw new GapiException("No JsonObject found in the response: " + responseData);

            string responseDetails = null;
            if ((jsonObject.ContainsKey("responseDetails")) &&
                 (jsonObject["responseDetails"] is JsonString))
                responseDetails = ((JsonString)jsonObject["responseDetails"]).Value;

            JsonHelper.ValidateJsonField(jsonObject, "responseStatus", typeof(JsonNumber));

            if (((JsonNumber)jsonObject["responseStatus"]).IntValue != 200)
            {
                if (responseDetails == null)
                    throw new GapiException("ResponseStatus: " + ((JsonNumber)jsonObject["responseStatus"]).IntValue.ToString() + ", Response data: " + responseData);
                else
                    throw new GapiException(string.Format("ResponseStatus: {0}, Reason: {1}, Response data: {2}",
                        ((JsonNumber)jsonObject["responseStatus"]).IntValue,
                        responseDetails,
                        responseData));
            }

            return jsonObject;
        }
    }
}
