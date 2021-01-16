using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Fizzler.Systems.HtmlAgilityPack;
using WpfApplication1.Services;


namespace WpfApplication1.Models
{
    public class FormViewModel : INotifyPropertyChanged
    {
        private const string BaseUrl = @"https://habr.com/ru/";
        private int _pageCounter;

        private string _input;
        
        public string Input
        {
            get => _input;
            set
            {
                _input = value;
                OnPropertyChanged("Input");
            }
        }

        private string _pages;

        public string Pages
        {
            get => _pages;
            set
            {
                _pages = value;
                OnPropertyChanged("Pages");
            }
        }


        public ObservableCollection<NewsViewModel> News { get; set; } = new ObservableCollection<NewsViewModel>();

        private NewsViewModel _selectedNews;
        
        public NewsViewModel SelectedNews
        {
            get => _selectedNews;
            set
            {
                _selectedNews = value;
                OnPropertyChanged("SelectedNews");
            }
        }

        private readonly Parser _parser;
    
        public FormViewModel()
        {
            _parser =  new Parser(BaseUrl);;
            LoadData();
        }
        
        private void LoadData()
        {
            _parser.Load($"page{++_pageCounter}/");
            var document = _parser.Document;

            var pagination = document.QuerySelector("ul.toggle-menu_pagination");
            
            Pages = "Your upload: " + _pageCounter + ", total: " + pagination
                ?.QuerySelector("a.toggle-menu__item-link_bordered")
                ?.Attributes["href"]
                ?.Value
                ?.Substring(8).TrimEnd('/');
        
            foreach(var item in document.QuerySelectorAll("article.post"))
            {
                var postHeader = item.QuerySelector("header.post__meta");
                var postText = item.QuerySelector("div.post__text").QuerySelectorAll("p");
                var description = postText.Aggregate("", (current, node) => current + "\n" + node.InnerHtml);
                
                News.Add(new NewsViewModel
                {
                    Author = postHeader.QuerySelector("a.post__user-info").Attributes["href"]?.Value ?? "null",
                    Date = postHeader.QuerySelector("span.post__time")?.InnerHtml ?? "null",
                    Title = item.QuerySelector("a.post__title_link")?.InnerHtml ?? "null",
                    Description = description,
                    Link = item.QuerySelector("div.post__body")?.QuerySelector("a.btn")?.Attributes["href"]?.Value ?? "null"
                });
            }
        }

        public RelayCommand NextPage => new RelayCommand(obj => { LoadData(); });
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}