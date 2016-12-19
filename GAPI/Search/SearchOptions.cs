using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Gapi.Search
{
    public enum Scoring
    {
        /// <summary>
        /// Order items by relevance
        /// </summary>
        Relevance,

        /// <summary>
        /// Ordering by date, newest first
        /// </summary>
        Date,

        /// <summary>
        /// Ordering by ascending date where the oldest result is first
        /// </summary>
        AscendingDate
    }

    public enum NewsTopic
    {
        None,
        TopHeadlines,
        World,
        Business,
        Nation,
        ScienceAndTechnology,
        Election,
        Politics,
        Entertainment,
        Sports,
        Health
    }

    public enum ImageSize
    {
        /// <summary>
        /// Restrict to small images
        /// </summary>
        Small,
 
        /// <summary>
        /// Restrict to medium images
        /// </summary>
        Medium,

        /// <summary>
        /// Restrict to large images
        /// </summary>
        Large,

        /// <summary>
        /// Restrict to extra large images
        /// </summary>
        Huge
    }

    public enum ImageColorization
    {
        BlackWhite,
        GrayScale,
        Color
    }

    public enum ImageFileType
    {
        None,
        Jpg,
        Gif,
        Bmp,
        Png
    }

    public enum ImageType
    {
        None,

        /// <summary>
        /// Restrict to images of faces.
        /// </summary>
        Face
    }

    public abstract class SearchOptions
    {
        private Dictionary<string, string> _options;

        protected SearchOptions()
        {
            _options = new Dictionary<string,string>();
        }

        protected void SetOption(string key, string value)
        {
            if (key == null)
                return;

            if (value == null)
            {
                if (_options.ContainsKey(key))
                    _options.Remove(key);

                return;
            }

            if (_options.ContainsKey(key))
                _options[key] = value;
            else
                _options.Add(key, value);
        }

        #region EnumToString helper functions
        protected static string ImageFileTypeToString(ImageFileType value)
        {
            switch (value)
            {
                case ImageFileType.Bmp:
                    return "bmp";
                case ImageFileType.Gif:
                    return "gif";
                case ImageFileType.Jpg:
                    return "jpg";
                case ImageFileType.Png:
                    return "png";
            }

            return null;
        }

        protected static string ImageColoringToString(ImageColorization value)
        {
            switch (value)
            {
                case ImageColorization.BlackWhite:
                    return "mono";
                case ImageColorization.GrayScale:
                    return "gray";
                case ImageColorization.Color:
                    return "color";
            }

            return null;
        }

        protected static string ImageTypeToString(ImageType value)
        {
            switch (value)
            {
                case ImageType.Face:
                    return "face";
            }

            return null;
        }

        protected static string ImageSizeToString(ImageSize value)
        {
            switch (value)
            {
                case ImageSize.Small:
                    return "icon";
                case ImageSize.Medium:
                    return "medium";
                case ImageSize.Large:
                    return "xxlarge";
                case ImageSize.Huge:
                    return "huge";
            }

            return null;
        }

        protected static string SearchSafetyToString(SearchSafety value)
        {
            string val = null;
            switch (value)
            {
                case SearchSafety.Active:
                    val = "active";
                    break;
                case SearchSafety.Moderate:
                    val = "moderate";
                    break;
                case SearchSafety.Off:
                    val = "off";
                    break;
            }

            return val;
        }

        protected static string NewsTopicToString(NewsTopic topic)
        {
            switch (topic)
            {
                case NewsTopic.TopHeadlines:
                    return "h";
                case NewsTopic.World:
                    return "w";
                case NewsTopic.Business:
                    return "b";
                case NewsTopic.Nation:
                    return "n";
                case NewsTopic.ScienceAndTechnology:
                    return "t";
                case NewsTopic.Election:
                    return "el";
                case NewsTopic.Politics:
                    return "p";
                case NewsTopic.Entertainment:
                    return "e";
                case NewsTopic.Sports:
                    return "s";
                case NewsTopic.Health:
                    return "m";
            }

            return null;
        }

        protected static string ScoringToString(Scoring scoring)
        {
            string val = null;

            switch (scoring)
            {
                case Scoring.Relevance:
                    val = null;
                    break;
                case Scoring.Date:
                    val = "d";
                    break;
                case Scoring.AscendingDate:
                    val = "ad";
                    break;
            }

            return val;
        }
        #endregion

        protected string GetOption(string key)
        {
            if (key == null)
                return null;

            if (_options.ContainsKey(key))
                return _options[key];

            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> kvp in _options)
            {
                sb.AppendFormat("&{0}={1}", kvp.Key, HttpUtility.UrlEncode(kvp.Value));
            }

            return sb.ToString();
        }
    }

    public enum SearchSafety
    {
        /// <summary>
        /// Enables the highest level of safe search filtering
        /// </summary>
        Active,

        /// <summary>
        /// Enables moderate safe search filtering (default)
        /// </summary>
        Moderate,
        
        /// <summary>
        /// Disables safe search filtering
        /// </summary>
        Off        
    }

    public class WebSearchOptions : SearchOptions 
    {
        /// <summary>
        /// This optional argument supplies the unique id for the Custom Search Engine that should be 
        /// used for this request, e.g., cx=000455696194071821846:reviews  
        /// </summary>
        public string CustomSearchEngineUniqueId
        {
            set { SetOption("cx", value); }
        }

        /// <summary>
        /// This optional argument supplies the url of a linked Custom Search Engine specification that 
        /// should be used to satisfy this request
        /// </summary>
        public string CustomSearchEngineRef
        {
            set { SetOption("cref", value); }
        }

        /// <summary>
        /// This optional argument supplies the search safety level
        /// </summary>
        public SearchSafety Safety
        {
            set
            {
                SetOption("safe", SearchSafetyToString(value));
            }
        }

        /// <summary>
        /// Valid values: http://www.google.com/coop/docs/cse/resultsxml.html#languageCollections
        /// </summary>
        public string LRestrict
        {
            set { this.SetOption("lr", value); }
        }
    }

    public enum ListingType
    {
        /// <summary>
        /// request KML, Local Business Listings, and Geocode results
        /// </summary>
        Blended,

        /// <summary>
        /// request KML and Geocode results
        /// </summary>
        KmlOnly,

        /// <summary>
        /// request Local Business Listings and Geocode results
        /// </summary>
        LocalOnly
    }

    public class LocalSearchOptions : SearchOptions
    {
        /// <summary>
        /// This optional argument specifies which type of listing the user is interested in. 
        /// Valid values include:
        /// * blended - request KML, Local Business Listings, and Geocode results
        /// * kmlonly - request KML and Geocode results
        /// * localonly - request Local Business Listings and Geocode results
        ///
        /// If this argument is not supplied, the default value of localonly is used. 
        /// </summary>
        /// <param name="lt"></param>
        void SetListingType(ListingType lt)
        {
            string val = null;
            switch (lt)
            {
                case ListingType.Blended:
                    val = "blended";
                    break;
                case ListingType.KmlOnly:
                    val = "kmlonly";
                    break;
                case ListingType.LocalOnly:
                    val = "localonly";
                    break;
            }

            SetOption("mrt", val);
        }

        /// <summary>
        /// This argument supplies the search center point for a local search.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        void SetSearchCenterPoint(double latitude, double longitude)
        {
            string val = string.Format("{0},{1}", latitude, longitude);

            SetOption("sll", val);
        }

        /// <summary>
        /// This optional argument supplies a bounding box for the local search should be relative to. 
        /// When using a Google Map, the sspn value can be computed using: 
        /// myMap.getBounds().toSpan().toUrlValue();  e.g., sspn=0.065169,0.194149
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void SetBoundingBox(double x, double y)
        {
            string val = string.Format("{0},{1}", x, y);

            SetOption("sspn", val);
        }
    }

    public class VideoSearchOptions : SearchOptions
    {
        /// <summary>
        /// This optional argument tells the search system how to order results. 
        /// Results may be ordered by relevance (which is the default), or by date. 
        /// To select ordering by relevance, do not supply this argument.
        /// </summary>
        public Scoring Scoring
        {
            set
            {
                SetOption("scoring", ScoringToString(value));
            }
        }
    }

    public class BlogSearchOptions : SearchOptions
    {
        /// <summary>
        /// This optional argument tells the search system how to order results. 
        /// Results may be ordered by relevance (which is the default), or by date. 
        /// To select ordering by relevance, do not supply this argument.
        /// </summary>
        public Scoring Scoring
        {
            set
            {
                SetOption("scoring", ScoringToString(value));
            }
        }
    }

    public class NewsSearchOptions : SearchOptions
    {
        /// <summary>
        /// This optional argument tells the search system how to order results. 
        /// Results may be ordered by relevance (which is the default), or by date. 
        /// To select ordering by relevance, do not supply this argument.
        /// </summary>
        public Scoring Scoring
        {
            set
            {
                SetOption("scoring", ScoringToString(value));
            }
        }

        /// <summary>
        /// This optional argument tells the news search system to scope search results to a particular 
        /// location. With this argument present, the query argument (q) becomes optional. Note, 
        /// this is a very new feature and locally scoped query coverage is somewhat sparse. 
        /// You must supply either a city, state, country, or zip code as in geo=Santa%20Barbara 
        /// or geo=British%20Columbia or geo=Peru or geo=93108. 
        /// </summary>
        public string Geo
        {
            set
            {
                SetOption("geo", value);
            }
        }

        /// <summary>
        /// This optional argument tells the news search system to scope search results to include only 
        /// quote typed results (rather than classic news article style results). With this argument 
        /// present, the query argument (q) becomes optional. The value of this argument designates 
        /// a prominent individual whose quotes have been reckognized and classified by the Google 
        /// News search service. For instance, Barack Obama has a qsid value of tPjE5CDNzMicmM, 
        /// John McCain has a value of lE61RnznhxvadM. Note, this is a very new feature and we 
        /// currently do not have a good search or descovery mechanism for these qsid values.. 
        /// </summary>
        public string Qsid
        {
            set
            {
                SetOption("qsid", value);
            }
        }

        /// <summary>
        /// This optional argument tells the news search system to scope search results to a particular topic.
        /// Topics vary slightly from edition to edition.
        /// </summary>
        public NewsTopic Topic
        {
            set
            {
                SetOption("topic", NewsTopicToString(value));
            }
        }

        /// <summary>
        /// This optional argument tells the news search system which edition of news to pull results from. 
        /// Values include:
        /// * us - specifies the US edition
        /// * uk - specifies the UK edition
        /// * fr_ca - specifies the French Canadian edition
        /// * etc.
        ///
        /// The best way to understand the available set of editions is to look at the edition links at 
        /// the bottom of Google News. After clicking on an edition, note the value of &topic argument 
        /// in the browser's address bar. 
        /// </summary>
        public string Edition
        {
            set
            {
                SetOption("ned", value);
            }
        }
    }

    public class BookSearchOptions : SearchOptions
    {
        /// <summary>
        /// This optional argument tells the book search system to restrict the search to "full view" 
        /// books, or all books. A value of as_brr=1 restricts the search to only those books 
        /// that are viewable in full. The default case is all books and that is indicated by 
        /// not specifying this argument. 
        /// </summary>
        public bool FullViewOnly
        {
            set
            {
                if (value == true)
                    SetOption("as_brr", "1");
                else
                    SetOption("as_brr", null);
            }
        }

        /// <summary>
        /// This optional argument tells the book search system to restrict the search to the 
        /// specified user-defined library. 
        /// </summary>
        public string UserDefinedLibrary
        {
            set { SetOption("as_list", value); }
        }
    }

    public class ImageSearchOptions : SearchOptions
    {
        /// <summary>
        /// This optional argument supplies the search safety level
        /// </summary>
        public SearchSafety Safety
        {
            set { SetOption("safe", SearchSafetyToString(value)); }
        }

        /// <summary>
        /// This optional argument tells the image search system to restrict the search to images of 
        /// the specified size
        /// </summary>
        public ImageSize Size
        {
            set { SetOption("imgsz", ImageSizeToString(value)); }
        }

        /// <summary>
        /// This optional argument tells the image search system to restrict the search to images of 
        /// the specified colorization
        /// </summary>
        public ImageColorization Colorization
        {
            set { SetOption("imgc", ImageColoringToString(value)); }
        }

        /// <summary>
        /// This optional argument tells the image search system to restrict the search to images 
        /// of the specified type
        /// </summary>
        public ImageType ImageType
        {
            set { SetOption("imgtype", ImageTypeToString(value)); }
        }

        /// <summary>
        /// This optional argument tells the image search system to restrict the search to images of 
        /// the specified filetype
        /// </summary>
        public ImageFileType FileType
        {
            set { SetOption("as_filetype", ImageFileTypeToString(value)); }
        }

        /// <summary>
        /// This optional argument tells the image search system to restrict the search to images 
        /// within the specified domain
        /// </summary>
        public string SiteSearch
        {
            set { SetOption("as_sitesearch", value); }
        }
    }

    public class PatentSearchOptions : SearchOptions
    {
        /// <summary>
        /// This optional argument tells the patent search system to restrict the search to ONLY 
        /// patents that having been issued, skiping all patents that have only been filed
        /// </summary>
        public bool OnlyIssued
        {
            set
            {
                if (value == true)
                    SetOption("as_psrg", "1");
                else
                    SetOption("as_psrg", null);
            }
        }

        /// <summary>
        /// This optional argument tells the patent search system to restrict the search to ONLY
        /// patents that only been filed, skipping over all patents that have been issued
        /// </summary>
        public bool OnlyFiled
        {
            set
            {
                if (value == true)
                    SetOption("as_psra", "1");
                else
                    SetOption("as_psra", null);
            }
        }

        /// <summary>
        /// This optional argument tells the search system how to order results. 
        /// Results may be ordered by relevance (which is the default), or by date. 
        /// To select ordering by relevance, do not supply this argument.
        /// </summary>
        public Scoring Scoring
        {
            set
            {
                SetOption("scoring", ScoringToString(value));
            }
        }
    }
}
