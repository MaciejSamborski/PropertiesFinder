using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using AngleSharp;
using AngleSharp.Html.Parser;
using System.Net.Http;
using AngleSharp.Dom;
using System.Threading.Tasks;

namespace SprzedawaczIntegration
{
    public class SprzedawaczIntegration : IWebSiteIntegration
    {
        const string mainUrl = "https://sprzedawacz.pl/nieruchomosci/";
        const string sellFlatsPath = "mieszkania/sprzedaz/";
        const string rentFlatsPath = "mieszkania/wynajem/";
        const string sellHousesPath = "domy/sprzedaz/";
        const string rentHousesPath = "domy/wynajem/";
        // "Ilość prezentowanych ogłoszeń została ograniczona do 100 pierwszych podstron.
        // Jeśli chcesz przejrzeć pozostałe ogłoszenia skorzystaj z wyszukiwarki, filtrów
        // lub ogranicz lokalizację."
        const int maxPages = 100;

        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }
        public IEqualityComparer<Entry> EntriesComparer { get; }

        private readonly HttpClient client;

        public SprzedawaczIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://sprzedawacz.pl",
                Name = "Sprzedawacz.pl Integration",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = true,
                    HouseSale = true,
                    HouseRental = true
                }
            };
            client = new HttpClient();
            client.BaseAddress = new Uri(WebPage.Url);
        }

        public Dump GenerateDump()
        {
            var categories = new List<string>
            {
                $"{mainUrl}{rentFlatsPath}",
                $"{mainUrl}{rentHousesPath}",
                $"{mainUrl}{sellFlatsPath}",
                $"{mainUrl}{sellHousesPath}"
            };
            var entries = new List<Entry>();
            return new Dump
            {
                DateTime = DateTime.Now,
                WebPage = WebPage,
                Entries = entries
            };
        }

        private async Task<IDocument> GetParsedHtmlFromUrl(string url)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            return await context.OpenAsync(new Url(url));
        }
    }
}
