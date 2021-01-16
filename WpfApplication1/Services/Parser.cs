using Fizzler.Systems.HtmlAgilityPack;
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

        /// <summary>
        /// Helper function, wrapper on QuerySelector
        /// </summary>
        /// <param name="node">Node being searched</param>
        /// <param name="selector">The selector we're looking for</param>
        /// <returns>Html content of node or "without data"</returns>
        public static string Qs(HtmlNode node, string selector) =>
            node.QuerySelector(selector)?.InnerHtml ?? "without data";

        /// <summary>
        /// Helper function, wrapper on QuerySelector
        /// </summary>
        /// <param name="node">Node being searched</param>
        /// <param name="selector">The selector we're looking for</param>
        /// <param name="isNode">checker for overload methods</param>
        /// <returns>HtmlNode element</returns>
        public static HtmlNode Qs(HtmlNode node, string selector, bool isNode) =>
            node.QuerySelector(selector);
        
        /// <summary>
        /// Methods find of node special attribute
        /// </summary>
        /// <param name="node">Node being searched</param>
        /// <param name="selector">The selector we're looking for</param>
        /// <param name="attr">html attr key</param>
        /// <returns>Html content of Attribute or "without data"</returns>
        public static string QsAttr(HtmlNode node, string selector, string attr) =>
            Qs(node, selector, true)?.Attributes[attr]?.Value ?? "without data";
        
    }
}