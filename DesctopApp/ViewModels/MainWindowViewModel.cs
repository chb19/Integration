using DAL;
using DesctopApp.Models;
using GalaSoft.MvvmLight.Command;
using Microsoft.ML;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesctopApp.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel
    {
        private static INewsRepository newsRepository = new NewsRepository(new MongoConfig()
        {
            ConnectionString = "mongodb://localhost",
            Database = "news2"
        });
        public ObservableCollection<News> News { get; set; } = new ObservableCollection<News>();
        public News SelectedNews { get; set; }
        public RelayCommand LoadCommand { get; }
        public RelayCommand DeleteAllCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand LikeCommand { get; }
        public RelayCommand DislikeCommand { get; }
        public RelayCommand LoadMLCommand { get; }
        public MainWindowViewModel()
        {
            LoadCommand = new RelayCommand(() => LoadNews());
            DeleteAllCommand = new RelayCommand(() => DeleteAll());
            DeleteCommand = new RelayCommand(() => DeleteSelected());
            LikeCommand = new RelayCommand(() => Like());
            DislikeCommand = new RelayCommand(() => Dislike());
            LoadMLCommand = new RelayCommand(() => LoadML());

        }

        public async Task LoadNews()
        {
            var response = await GraphqlClient.GetAllNews();
            News = new ObservableCollection<News>();

            foreach (var news in response ?? new List<News>())
            {
                News.Add(news);
            }

            var t = "";

            await LoadML();
        }

        public async Task Like()
        {
            SelectedNews.Like = !SelectedNews.Like;
            if(SelectedNews.Like)
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

        public async Task LoadML()
        {
            MLContext mlContext = new MLContext();
            var trainedModel = LoadModel(mlContext);
            await UseModelForSinglePrediction(mlContext, trainedModel);
        }

        public static ITransformer LoadModel(MLContext mlContext)
        {
            var modelPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Data", "NewsRecommenderModel.zip");
            DataViewSchema dataViewSchema;

            return mlContext.Model.Load(modelPath, out dataViewSchema);
        }

        public async Task UseModelForSinglePrediction(MLContext mlContext, ITransformer model)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<NewsRating, NewsRatingPrediction>(model);

            List<Models.News> rrecoNews = new List<News>();

            foreach (var item in News)
            {
                var testInput = new NewsRating { userId = 1, NewsId = float.Parse(item.ID) };

                var NewsRatingPrediction = predictionEngine.Predict(testInput);
                if (Math.Round(NewsRatingPrediction.Score, 1) > 0.5)
                {
                    rrecoNews.Add(item);
                }
            }

            foreach (var item in rrecoNews)
            {
                MessageBox.Show($"Recomended News: {Environment.NewLine}{Environment.NewLine} " +
                    $"Title: {item.Title} {Environment.NewLine}{Environment.NewLine} " +
                    $"Author: {item.Author} {Environment.NewLine} {Environment.NewLine} " +
                    $"Description: {item.Description} {Environment.NewLine}{Environment.NewLine} ");
            }
        }
    }
}
