using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;


namespace WpfApplication1.Models
{
    public class FormViewModel : INotifyPropertyChanged
    {
        private const string URL = @"https://cashessentials.org/publications/";
        private int _pageCounter = 1;

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

        private void LoadData(string url = URL)
        {
            HtmlWeb web = new HtmlWeb();
            var html = web.Load(url);
            var document = html.DocumentNode;
            
            foreach(var item in document.QuerySelectorAll("a.quicklink"))
            {
                News.Add(new NewsViewModel
                {
                    Author = item.QuerySelector("div.quicklink__author")?.InnerHtml ?? "starts",
                    Date = item.QuerySelector("div.quicklink__date")?.InnerText ?? "null",
                    Description = item.QuerySelector("div.quicklink__catchphrase")?.InnerHtml ?? "null",
                    Link = item.Attributes["href"]?.Value ?? "null",
                    Title = item.QuerySelector("div.quicklink__title")?.InnerText ?? "null"
                });
            }
        }

        public FormViewModel()
        {
            LoadData();
        }
        
        public RelayCommand NextPage =>
            new RelayCommand(obj => { LoadData(URL + $"page/{++_pageCounter}/"); });


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}