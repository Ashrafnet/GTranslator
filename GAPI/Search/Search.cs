using System;
using System.Collections.Generic;
using System.Text;
using Gapi.Json;
using Gapi.Core;

namespace Gapi.Search
{
    public enum SearchType
    {
        Web,
        Video,
        Local,
        Blog,
        News,
        Book,
        Image,
        Patent
    }

    public class Searcher
    {
        const string SearchWebUrl = "http://ajax.googleapis.com/ajax/services/search/web?v={0}&q={1}";
        const string SearchVideoUrl = "http://ajax.googleapis.com/ajax/services/search/video?v={0}&q={1}";
        const string SearchLocalUrl = "http://ajax.googleapis.com/ajax/services/search/local?v={0}&q={1}";
        const string SearchBlogUrl = "http://ajax.googleapis.com/ajax/services/search/blogs?v={0}&q={1}";
        const string SearchNewsUrl = "http://ajax.googleapis.com/ajax/services/search/news?v={0}&q={1}";
        const string SearchBookUrl = "http://ajax.googleapis.com/ajax/services/search/books?v={0}&q={1}";
        const string SearchImageUrl = "http://ajax.googleapis.com/ajax/services/search/images?v={0}&q={1}";
        const string SearchPatentUrl = "http://ajax.googleapis.com/ajax/services/search/patent?v={0}&q={1}";

        const string SearchApiVersion = "1.0";

        public static SearchResults Search(SearchType searchType, string phrase)
        {
            return Search(searchType, phrase, 0);
        }

        public static SearchResults Search(SearchType searchType, string phrase, int pageIndex)
        {
            string searchUrl = null;

            switch (searchType)
            {
                case SearchType.Web:
                    searchUrl = SearchWebUrl;
                    break;
                case SearchType.Video:
                    searchUrl = SearchVideoUrl;
                    break;
                case SearchType.Patent:
                    searchUrl = SearchPatentUrl;
                    break;
                case SearchType.News:
                    searchUrl = SearchNewsUrl;
                    break;
                case SearchType.Local:
                    searchUrl = SearchLocalUrl;
                    break;
                case SearchType.Image:
                    searchUrl = SearchImageUrl;
                    break;
                case SearchType.Book:
                    searchUrl = SearchBookUrl;
                    break;
                case SearchType.Blog:
                    searchUrl = SearchBlogUrl;
                    break;
                default:
                    throw new NotSupportedException("SearchType: " + searchType.ToString());
            }

            return DoSearch(phrase, searchUrl, pageIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchOptions">Use specific search options class: WebSearchOptions, VideoSearchOptions,
        /// LocalSearchOptions, BlogSearchOptions, NewsSearchOptions, BookSearchOptions, ImageSearchOptions, 
        /// PatentSearchOptions, etc...
        /// </param>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public static SearchResults Search(SearchOptions searchOptions, string phrase)
        {
            return Search(searchOptions, phrase, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchOptions">Use specific search options class: WebSearchOptions, VideoSearchOptions,
        /// LocalSearchOptions, BlogSearchOptions, NewsSearchOptions, BookSearchOptions, ImageSearchOptions, 
        /// PatentSearchOptions, etc...
        /// <param name="phrase"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public static SearchResults Search(SearchOptions searchOptions, string phrase, int pageIndex)
        {
            if (searchOptions == null)
                throw new NullReferenceException("searchOptions");

            string searchUrl = null;

            if (searchOptions is WebSearchOptions)
                searchUrl = SearchWebUrl;
            else if (searchOptions is VideoSearchOptions)
                searchUrl = SearchVideoUrl;
            else if (searchOptions is PatentSearchOptions)
                searchUrl = SearchPatentUrl;
            else if (searchOptions is NewsSearchOptions)
                searchUrl = SearchNewsUrl;
            else if (searchOptions is LocalSearchOptions)
                searchUrl = SearchLocalUrl;
            else if (searchOptions is ImageSearchOptions)
                searchUrl = SearchImageUrl;
            else if (searchOptions is BookSearchOptions)
                searchUrl = SearchBookUrl;
            else if (searchOptions is BlogSearchOptions)
                searchUrl = SearchBlogUrl;
            else
                throw new NotSupportedException("SearchType: " + searchOptions.GetType().ToString());

            searchUrl += searchOptions.ToString();

            return DoSearch(phrase, searchUrl, pageIndex);
        }


        private static SearchResults DoSearch(string phrase, string searchUrl, int pageIndex)
        {
            if ((phrase == null)||(phrase == ""))
                return new SearchResults();

            string url = string.Format(
                searchUrl, 
                SearchApiVersion, 
                System.Web.HttpUtility.UrlEncode(phrase));

            // Append parameters
            url += "&rsz=large&start=" + pageIndex;

            string responseData = Core.CoreHelper.PerformRequest(url);
            JsonObject jsonObject = Core.CoreHelper.ParseGoogleAjaxAPIResponse(responseData);

            // Translation response validation
            // Get 'responseData'
            JsonHelper.ValidateJsonField(jsonObject, "responseData", typeof(JsonObject));

            // Get 'translatedText'
            JsonObject responseContent = (JsonObject)jsonObject["responseData"];

            SearchResults searchResults = SearchResults.Parse(responseContent);

            return searchResults;
        }
    }
}
