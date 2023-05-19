using CNNDesktop.Models;
using PropertyChanged;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;

namespace CNNDesktop.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<News> News { get; set; } = new ObservableCollection<News>();

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        public MainWindowViewModel()
        {
            LoadCommand = ReactiveCommand.CreateFromTask(LoadNews);
        }

        public async Task LoadNews()
        {
            var response = await GraphqlClient.GetAllNews();
            News = new ObservableCollection<News>();

            foreach (var news in response ?? new List<News>())
            {
                News.Add(news);
            }
        }
    }
}
