using DTO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace SiteParcer
{
    class Program
    {
        private static int index = 1;
        [Obsolete]
        public static IEnumerable<NewsDTO> Crawl()
        {
            string homeUrl = "http://edition.cnn.com/world";

            //options skip errors
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--ignore-certificate-errors-spki-list");
            chromeOptions.AddArgument("--ignore-ssl-errors");
            chromeOptions.AddArgument("test-type");
            chromeOptions.AddArgument("no-sandbox");
            chromeOptions.AddArgument("-incognito");
            chromeOptions.AddArgument("--start-maximized");


            IWebDriver driver = new ChromeDriver(@"D:\Chrome", chromeOptions);
            driver.Navigate().GoToUrl(homeUrl);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);


            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            wait.Until(d => d.FindElement(By.CssSelector(".headline__text")));

            //get all news
            var elements = driver.FindElements(By.CssSelector(".container__link"));

            List<NewsDTO> news = elements
            .Select(el => new NewsDTO
            {
                ID = (index++).ToString(),
                Title = el.Text,
                Url = el.GetAttribute("href")
            }).ToList();
            news.RemoveAll(x => String.IsNullOrEmpty(x.Title));

            for (int i = 0; i < news.Count; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));

                var n = news[i];
                try
                {
                    driver.Navigate().GoToUrl(n.Url);

                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".pg-headline")));

                    n.Author = driver.FindElement(By.CssSelector("meta[name^=author]")).GetAttribute("content");
                    n.Description = driver.FindElement(By.CssSelector("meta[name^=description]")).GetAttribute("content");
                    n.DateOfPublication = DateTime.Parse(driver.FindElement(By.CssSelector("meta[name^=pubdate]")).GetAttribute("content"));
                    n.Like = false;
                    n.Dislike = false;
                }
                catch (Exception e) { }
                yield return n;
            }

            driver.Close();
        }

        [Obsolete]
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = ConnectionFactory.DefaultUser;
            factory.Password = ConnectionFactory.DefaultPass;
            factory.VirtualHost = ConnectionFactory.DefaultVHost;
            factory.HostName = "localhost";
            factory.Port = AmqpTcpEndpoint.UseDefaultPort;


            using (IConnection conn = factory.CreateConnection())
            using (var model = conn.CreateModel())
            {
                model.QueueDeclare("news", false, false, false, null);
                using (StreamWriter sw = new StreamWriter("text.txt", false, System.Text.Encoding.Default))
                {
                    foreach (var x in Crawl())
                    {
                        //sw.WriteLine(x.GetStr());
                        Console.WriteLine(x.GetStr());
                        var properties = model.CreateBasicProperties();
                        properties.Persistent = true;
                        model.BasicPublish(string.Empty, "news", basicProperties: properties, body: BinaryConverter.ObjectToByteArray(x));
                    }
                }
            }
        }
    }
}