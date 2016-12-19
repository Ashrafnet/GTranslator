using System;
using System.Text;

using Gapi.Language;
using Gapi.Search;
using Gapi.Charting;

namespace GAPI.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            

            //// Sample: Language API
            string phrase = "Ciao mondo";
            Language targetLanguage = Language.English;

            System.Console.WriteLine("Source Phrase: {0}", phrase);
            System.Console.WriteLine("Detected Language: {0}", Translator.Detect("Ciao mondo"));
            System.Console.WriteLine("Translation: {0}", Translator.Translate(phrase, targetLanguage));

            // Sample: Search API
            string searchPhrase = "Paris Hilton";
            SearchResults searchResults = Searcher.Search(SearchType.Web, searchPhrase);
            System.Console.WriteLine("{0}", searchResults.Items.Length);

            SearchResults searchResults2 = Searcher.Search(SearchType.Local, searchPhrase);
            foreach (SearchResult searchResult in searchResults2.Items)
            {
                if (searchResult.JsonObject.ContainsKey("lat"))
                    System.Console.WriteLine(searchResult.JsonObject["lat"].ToString());
            }

            
            System.Console.ReadLine();
        }
    }
}
