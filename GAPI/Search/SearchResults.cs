using System;
using System.Collections;
using System.Text;
using Gapi.Json;
using Gapi.Core;

namespace Gapi.Search
{
    public class SearchResults
    {
        ArrayList _items;
        long _estimatedResultCount;
        string _moreResultsUrl;
        int _currentPageIndex;
        ArrayList _optionalPages;

        public int[] Pages
        {
            get { return (int[])_optionalPages.ToArray(typeof(int)); }
        }

        public SearchResult[] Items
        {
            get { return (SearchResult[])_items.ToArray(typeof(SearchResult)); }
        }

        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
        }

        public long EstimatedResultCount
        {
            get { return _estimatedResultCount; }
        }

        internal SearchResults()
        {
            _items = new ArrayList();
            _optionalPages = new ArrayList();

            _estimatedResultCount = 0;
            _currentPageIndex = 0;
            _moreResultsUrl = null;
        }

        private void Add(SearchResult searchResult)
        {
            _items.Add(searchResult);
        }

        public static SearchResults Parse(JsonObject responseContent)
        {
            SearchResults searchResults = new SearchResults();

            // Parse cursor
            JsonHelper.ValidateJsonField(responseContent, "cursor", typeof(JsonObject));
            JsonObject cursor = (JsonObject)responseContent["cursor"];

            searchResults._estimatedResultCount = long.Parse(JsonHelper.GetJsonStringAsString(cursor, "estimatedResultCount", "0"));
            searchResults._currentPageIndex = int.Parse(JsonHelper.GetJsonStringAsString(cursor, "currentPageIndex", "0"));
            searchResults._moreResultsUrl = JsonHelper.GetJsonStringAsString(cursor, "moreResultsUrl");

            if (cursor.ContainsKey("pages") == true)
            {
                JsonHelper.ValidateJsonField(cursor, "pages", typeof(JsonArray));
                JsonArray pages = (JsonArray)cursor["pages"];
                foreach (JsonValue pageValue in pages.Items)
                {
                    JsonObject pageValueObject = (JsonObject)pageValue;
                    JsonHelper.ValidateJsonField(pageValueObject, "start", typeof(JsonString));

                    searchResults._optionalPages.Add(
                        int.Parse(JsonHelper.GetJsonStringAsString(pageValueObject, "start", "-1")));
                }
            }

            // Parse results
            JsonHelper.ValidateJsonField(responseContent, "results", typeof(JsonArray));
            JsonArray jsonResults = (JsonArray)(responseContent["results"]);

            foreach (JsonValue jsonResult in jsonResults.Items)
            {
                SearchResult searchResult = SearchResult.Parse((JsonObject)jsonResult);
                searchResults.Add(searchResult);
            }

            return searchResults;
        }
    }
}
