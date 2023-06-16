using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private static INewsRepository newsRepository = new NewsRepository(new MongoConfig()
        {
            ConnectionString = "mongodb://localhost",
            Database = "news"
        });

        public ObservableCollection<News> News { get; set; } = new ObservableCollection<News>();
        public News SelectedNews { get; set; }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
      
        }

        public IActionResult Index()
        {
            var response = GraphqlClient.GetAllNews();
            News = new ObservableCollection<News>();
            var model = new HomeModel()
            {
                News = new List<News>()
            };

            foreach (var news in response.GetAwaiter().GetResult())
            {
                model.News.Add(news);
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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

        public async Task Like()
        {
            SelectedNews.Like = !SelectedNews.Like;
            if (SelectedNews.Like)
            {
                SelectedNews.Dislike = false;
            }
            await newsRepository.LikeNewsByIdAsync(SelectedNews.ID, SelectedNews.Like);
        }

        public async Task Dislike()
        {
            SelectedNews.Dislike = !SelectedNews.Dislike;
            if (SelectedNews.Dislike)
            {
                SelectedNews.Like = false;
            }
            await newsRepository.DislikeNewsByIdAsync(SelectedNews.ID, SelectedNews.Dislike);
        }

        public async Task DeleteAll()
        {
            await newsRepository.DeleteAllAsync();
            News.Clear();
        }

        public async Task DeleteSelected()
        {
            await newsRepository.DeleteByTitleAsync(SelectedNews.Title);
            News.Remove(SelectedNews);
        }
    }
}
