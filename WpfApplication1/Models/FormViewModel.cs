using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
            
            var pagination = Parser.Qs(document, "ul.toggle-menu_pagination", true);

            Pages = "Your upload: " + _pageCounter + ", total: " +
                    Parser.QsAttr(pagination, "a.toggle-menu__item-link_bordered", "href")
                          ?.Substring(8).TrimEnd('/');
        
            foreach(var item in Parser.QsAll(document, "article.post"))
            {
                var postHeader = Parser.Qs(item, "header.post__meta", true);
                var postText = Parser.QsAll(Parser.Qs(item, "div.post__text", true), "p");
                var description = postText.Aggregate("", (current, node) => current + "\n" + node.InnerHtml);
                
                News.Add(new NewsViewModel
                {
                    Author = Parser.QsAttr(postHeader, "a.post__user-info", "href"),
                    Date = Parser.Qs(postHeader, "span.post__time"),
                    Title = Parser.Qs(item, "a.post__title_link"),
                    Description = description,
                    Link = Parser.QsAttr(Parser.Qs(item, "div.post__body", true), "a.btn", "href")
                });
            }
        }

        public RelayCommand NextPage => new RelayCommand(obj => { LoadData(); });
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}