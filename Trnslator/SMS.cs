using System;
using System.Collections.Generic;
using System.Text;
using Gapi.Language;
using System.Net;
using System.IO;

namespace Translator
{
    public  class TextToSpeech
    {
        public static bool TextToSpeechByGoole(string Text, Language Lang)
        {



            string strUrl = "http://translate.google.com/translate_tts?";
            string strPost = "q=%Text%&tl=%Lang%";
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create(strUrl);
            
            request.Proxy = new WebProxy();
            request.Proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            // Set the Method property of the request to POST.
            request.Method = "POST";
            strPost = strPost.Replace("%Lang%", LanguageHelper.GetLanguageString(Lang));
            strPost = strPost.Replace("%Text%", (Text));
            

            // Create POST data and convert it to a byte array.
            string postData = strPost;
            byte[] byteArray = Encoding.Default.GetBytes(postData);
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
return true;
            

        }
    }
}
