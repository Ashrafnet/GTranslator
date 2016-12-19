using System;
using System.Text;
using System.Net;
using System.Web;
using System.IO;

using Gapi.Core;
using Gapi.Json;

namespace Gapi.Language
{
    public static class Translator
    {
        const string LanguageTranslateUrl =  "http://ajax.googleapis.com/ajax/services/language/translate?";
        const string LanguageTranslatePostString = "v={0}&q={1}&langpair={2}|{3}";
        const string LanguageDetectUrl = "http://ajax.googleapis.com/ajax/services/language/detect?";
        const string LanguageDetectPostString = "v={0}&q={1}";
        const string LanguageApiVersion = "1.0";

        public static string Translate(string phrase, Language sourceLanguage, Language targetLanguage)
        {
            return Translate(phrase, ref sourceLanguage, targetLanguage);
        }

        public static string Translate(string phrase, Language targetLanguage)
        {
            Language sourceLanguage = Language.Unknown;
            return Translate(phrase, ref sourceLanguage, targetLanguage);
        }

        public static string Translate(string phrase, Language targetLanguage, out Language detectedSourceLanguage)
        {
            detectedSourceLanguage = Language.Unknown;
            return Translate(phrase, ref detectedSourceLanguage, targetLanguage);
        }

        private static string Translate(string input, ref Language sourceLanguage, Language targetLanguage)
        {
            string results = "";
            if ((input == null) || (input == ""))
                return "";
            string NewLine = "!!!!" ;
            input = input.Replace(Environment.NewLine, NewLine);
                //  phrase= phrase.Replace(Environment.NewLine, "<br>");
                string url = string.Format(LanguageTranslatePostString, LanguageApiVersion,
                    HttpUtility.UrlEncode(input, Encoding.UTF8),
                    LanguageHelper.GetLanguageString(sourceLanguage),
                    LanguageHelper.GetLanguageString(targetLanguage));

                //string responseData = CoreHelper.PerformRequest(url);
                string responseData = CoreHelper.BuildWebRequest(LanguageTranslateUrl, url);

                JsonObject jsonObject = CoreHelper.ParseGoogleAjaxAPIResponse(responseData);

                // Translation response validation
                // Get 'responseData'
                if (jsonObject.ContainsKey("responseData") == false)
                    throw new GapiException("Invalid response - no responseData: " + responseData);

                if (!(jsonObject["responseData"] is JsonObject))
                    throw new GapiException("Invalid response - responseData is not JsonObject: " + responseData);

                // Get 'translatedText'
                JsonObject responseContent = (JsonObject)jsonObject["responseData"];
                if (responseContent.ContainsKey("translatedText") == false)
                    throw new GapiException("Invalid response - no translatedText: " + responseData);

                if (!(responseContent["translatedText"] is JsonString))
                    throw new GapiException("Invalid response - translatedText is not JsonString: " + responseData);

                string translatedPhrase = ((JsonString)responseContent["translatedText"]).Value;

                // If there's a detected language - return it
                if ((responseContent.ContainsKey("detectedSourceLanguage") == true) &&
                     (responseContent["detectedSourceLanguage"] is JsonString))
                {
                    JsonString detectedSourceLanguage = (JsonString)responseContent["detectedSourceLanguage"];
                    sourceLanguage = LanguageHelper.GetLanguage(detectedSourceLanguage.Value);
                }

                results += HttpUtility.HtmlDecode(translatedPhrase) + Environment.NewLine;

                results = results.Replace(NewLine, Environment.NewLine);
            return results;
        }

        public static Language Detect(string phrase)
        {
            try
            {
                if (phrase.Trim() == "") return Language.English;
                bool isReliable;
                double confidence;

                return Detect(phrase, out isReliable, out confidence);
            }
            catch
            {
                return Language.Arabic;
            }
        }

        public static Language Detect(string phrase, out bool isReliable, out double confidence)
        {
            if ((phrase == null) || (phrase == ""))
                throw new GapiException("No phrase to detect from");

            string url = string.Format(LanguageDetectPostString , 
                LanguageApiVersion,
                HttpUtility.UrlEncode(phrase));

            string responseData = CoreHelper.PerformRequest(LanguageDetectUrl + url );

            JsonObject jsonObject = CoreHelper.ParseGoogleAjaxAPIResponse(responseData);

            // Translation response validation
            // Get 'responseData'
            if (jsonObject.ContainsKey("responseData") == false)
                throw new GapiException("Invalid response - no responseData: " + responseData);

            if (!(jsonObject["responseData"] is JsonObject))
                throw new GapiException("Invalid response - responseData is not JsonObject: " + responseData);

            // Get 'translatedText'
            JsonObject responseContent = (JsonObject)jsonObject["responseData"];
            if (responseContent.ContainsKey("language") == false)
                throw new GapiException("Invalid response - no language: " + responseData);

            if (!(responseContent["language"] is JsonString))
                throw new GapiException("Invalid response - language is not JsonString: " + responseData);

            string language = ((JsonString)responseContent["language"]).Value;

            isReliable = false;
            confidence = 0;

            if ((responseContent.ContainsKey("isReliable") == true) &&
                 (responseContent["isReliable"] is JsonBoolean))
            {
                isReliable = ((JsonBoolean)responseContent["isReliable"]).Value;
            }

            if ((responseContent.ContainsKey("confidence") == true) &&
                 (responseContent["confidence"] is JsonNumber))
            {
                confidence = ((JsonNumber)responseContent["confidence"]).DoubleValue;
            }

            return LanguageHelper.GetLanguage(language);
        }
    }
}
