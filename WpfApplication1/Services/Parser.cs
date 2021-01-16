using HtmlAgilityPack;

namespace WpfApplication1.Services
{
    public class Parser
    {
        private readonly HtmlWeb _web;
        private readonly string _baseUrl;
        public HtmlNode Document { get; set; }

        public Parser(string baseUrl)
        {
            _web = new HtmlWeb();
            _baseUrl = baseUrl;
        }

        /// <summary>
        /// Receives a new html page and puts her the root node into a variable
        /// </summary>
        /// <param name="urlParams">Additional parameters for request</param>
        public void Load(string urlParams = "")
        {
            var html = _web.Load(_baseUrl + urlParams);
            Document = html.DocumentNode;
        }
    }
}