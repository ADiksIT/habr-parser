using System.Collections.Generic;
using System.Collections.ObjectModel;
using HtmlAgilityPack;
using WpfApplication1.Models;

namespace WpfApplication1.Services
{
    public class HabrParser
    {
        private const string BaseUrl = @"https://habr.com/ru/";
        
        private readonly Parser _parser;
        
        public HabrParser()
        {
            _parser = new Parser(BaseUrl);
        }

        public HtmlNode GetPage()
        {
            _parser.Load($"page{++_pageCounter}/");
            return _parser.Document;
        }

        public ObservableCollection<NewsViewModel> GetPosts()
        {
            ObservableCollection<NewsViewModel> posts = new ObservableCollection<NewsViewModel>();
          
            var document = GetPage();
            
            // Pages = "Your upload: " + _pageCounter;

            foreach (var item in Parser.QsAll(document, "article.post"))
            {
                // PostCount++;
                var postHeader = Parser.Qs(item, "header.post__meta", true);
                
                posts.Add(new NewsViewModel
                {
                    Author = Parser.QsAttr(postHeader, "a.post__user-info", "href"),
                    Date = Parser.Qs(postHeader, "span.post__time"),
                    Title = Parser.Qs(item, "a.post__title_link"),
                    Description = Parser.Qs(item, "div.post__text", true)?.InnerText ?? "text",
                    Link = Parser.QsAttr(Parser.Qs(item, "div.post__body", true), "a.btn", "href")
                });
            }
            return posts;
        }
    }
}