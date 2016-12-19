using System;
using System.Collections.Generic;
using System.Text;
using Gapi.Json;

namespace Gapi.Search
{
    public enum SearchResultType
    {
        Unknown,
        Web,
        Video,
        Local,
        Blog,
        News,
        Book,
        Image,
        Patent
    }

    public class SearchResult
    {
        SearchResultType _resultType;
        JsonObject _jsonObject;

        public string Url
        {
            get { return GetStringAttribute("unescapedUrl"); }
        }

        public string Title
        {
            get { return GetStringAttribute("title"); }
        }

        public string Content
        {
            get { return GetStringAttribute("content"); }
        }

        public SearchResultType ResultType
        {
            get { return _resultType; }
        }

        public JsonObject JsonObject
        {
            get { return _jsonObject; }
        }

        public SearchResult(JsonObject jsonObject)
        {
            _jsonObject = jsonObject;

            if (jsonObject.ContainsKey("GsearchResultClass"))
                _resultType = ParseSearchResultType(((JsonString)jsonObject["GsearchResultClass"]).Value);
        }

        protected string GetStringAttribute(string key)
        {
            return JsonHelper.GetJsonStringAsString(this.JsonObject, key);
        }

        public static SearchResult Parse(JsonObject jsonObject)
        {
            SearchResultType resultType = SearchResultType.Unknown;

            if (jsonObject.ContainsKey("GsearchResultClass"))
                resultType = ParseSearchResultType(((JsonString)jsonObject["GsearchResultClass"]).Value);

            switch (resultType)
            {
                case SearchResultType.Image:
                    return new ImageSearchResult(jsonObject);
                case SearchResultType.Video:
                    return new VideoSearchResult(jsonObject);
                case SearchResultType.Web:
                    return new WebSearchResult(jsonObject);
                default:
                    return new SearchResult(jsonObject);
            }
        }

        private static SearchResultType ParseSearchResultType(string str)
        {
            switch (str)
            {
                case "GwebSearch":
                    return SearchResultType.Web;
                case "GvideoSearch":
                    return SearchResultType.Video;
                case "GlocalSearch":
                    return SearchResultType.Local;
                case "GblogSearch":
                    return SearchResultType.Blog;
                case "GnewsSearch":
                    return SearchResultType.News;
                case "GbookSearch":
                    return SearchResultType.Book;
                case "GimageSearch":
                    return SearchResultType.Image;
                case "GpatentSearch":
                    return SearchResultType.Patent;
            }

            return SearchResultType.Unknown;
        }
    }

    public class WebSearchResult : SearchResult 
    {
        public string CacheUrl
        {
            get { return GetStringAttribute("cacheUrl"); }
        }

        public WebSearchResult(JsonObject jsonObject) 
            : base(jsonObject)
        {
        }
    }

    public class ThumbnailSearchResult : SearchResult
    {
        public int ThumbnailWidth
        {
            get { return int.Parse(GetStringAttribute("tbWidth")); }
        }

        public int ThumbnailHeight
        {
            get { return int.Parse(GetStringAttribute("tbHeight")); }
        }

        public string ThumbnailUrl
        {
            get { return GetStringAttribute("tbUrl"); }
        }

        protected ThumbnailSearchResult(JsonObject jsonObject) 
            : base(jsonObject)
        {
        }
    }

    public class ImageSearchResult : ThumbnailSearchResult
    {
        public int Width
        {
            get { return int.Parse(GetStringAttribute("tbWidth")); }
        }

        public int Height
        {
            get { return int.Parse(GetStringAttribute("tbHeight")); }
        }

        public ImageSearchResult(JsonObject jsonObject) 
            : base(jsonObject)
        {
        }
    }

    public class VideoSearchResult : ThumbnailSearchResult
    {
        public int Duration
        {
            get { return int.Parse(GetStringAttribute("duration")); }
        }

        public string PlayUrl
        {
            get { return GetStringAttribute("playUrl"); }            
        }

        public string Publisher
        {
            get { return GetStringAttribute("publisher"); }
        }

        public DateTime Published
        {
            get { return Gapi.Core.Rfc822DateTime.Parse(GetStringAttribute("published")); }
        }

        public int Rating
        {
            get { return int.Parse(GetStringAttribute("rating")); }
        }

        /// <summary>
        /// Known values: 'YouTube'
        /// </summary>
        public string VideoType
        {
            get { return GetStringAttribute("videoType"); }
        }

        public VideoSearchResult(JsonObject jsonObject)
            : base(jsonObject)
        {
        }
    }
}
