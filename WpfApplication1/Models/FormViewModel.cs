using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WpfApplication1.Annotations;
using WpfApplication1.Services;


namespace WpfApplication1.Models
{
    public class FormViewModel : INotifyPropertyChanged
    {

        
        private int _pageCounter;

        private int _postCount;

        public int PostCount
        {
            get => _postCount;
            set
            {
                _postCount = value;
                OnPropertyChanged();
            }
        }

        private string _input;

        public string Input
        {
            get => _input;
            set
            {
                _input = value;
                OnPropertyChanged();
            }
        }

        private string _pages;

        public string Pages
        {
            get => _pages;
            set
            {
                _pages = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        private readonly Parser _parser;

        public FormViewModel()
        {
            _parser = new Parser(BaseUrl);
            LoadAsync();
        }

        private ObservableCollection<NewsViewModel> LoadData()
        {
            var collection = new ObservableCollection<NewsViewModel>();

            _parser.Load($"page{++_pageCounter}/");
            var document = _parser.Document;
            
            Pages = "Your upload: " + _pageCounter;

            foreach (var item in Parser.QsAll(document, "article.post"))
            {
                PostCount++;
                var postHeader = Parser.Qs(item, "header.post__meta", true);
                
                collection.Add(new NewsViewModel
                {
                    Author = Parser.QsAttr(postHeader, "a.post__user-info", "href"),
                    Date = Parser.Qs(postHeader, "span.post__time"),
                    Title = Parser.Qs(item, "a.post__title_link"),
                    Description = Parser.Qs(item, "div.post__text", true)?.InnerText ?? "text",
                    Link = Parser.QsAttr(Parser.Qs(item, "div.post__body", true), "a.btn", "href")
                });
            }

            return collection;
        }

        private async void LoadAsync()
        {
            while (true)
            {
                ObservableCollection<NewsViewModel> collection = await Task.Run(LoadData);
                foreach (var newsViewModel in collection) News.Add(newsViewModel);
                if (_pageCounter != 50) continue;
                break;
            }
        }

        public RelayCommand NextPage => new RelayCommand(obj => { LoadAsync(); });

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}