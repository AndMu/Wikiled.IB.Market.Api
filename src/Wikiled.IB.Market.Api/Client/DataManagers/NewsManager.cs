using System;
using System.Collections.Generic;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public class NewsManager
    {
        private const int TICK_NEWS_ID_BASE = 90000000;
        private const int TICK_NEWS_ID = TICK_NEWS_ID_BASE;

        private const int NEWS_ARTICLE_ID = TICK_NEWS_ID_BASE + 10000;
        private const int HISTORICAL_NEWS_ID = TICK_NEWS_ID_BASE + 20000;

        private int rowCountHistoricalNewsGrid;

        private int rowCountTickNewsGrid;

        public NewsManager(IBClient ibClient)
        {
            IbClient = ibClient;
        }

        public IBClient IbClient { get; set; }

        public void UpdateUI(HistoricalNewsMessage historicalNewsMessage)
        {
            if (historicalNewsMessage.RequestId == HISTORICAL_NEWS_ID)
            {
                rowCountHistoricalNewsGrid++;
            }
        }

        public void UpdateUI(HistoricalNewsEndMessage historicalNewsEndMessage)
        {
            if (historicalNewsEndMessage.RequestId == HISTORICAL_NEWS_ID)
            {
                if (historicalNewsEndMessage.HasMore)
                {
                }
            }
        }

        public void UpdateUI(TickNewsMessage tickNewsMessage)
        {
            if (tickNewsMessage.TickerId == TICK_NEWS_ID)
            {
                rowCountTickNewsGrid++;
            }
        }

        public void HandleNewsProviders(NewsProvidersMessage newsProvidersMessage)
        {
        }

        public void UpdateUI(NewsArticleMessage newsArticleMessage)
        {
            if (newsArticleMessage.ArticleType == 0)
            {
                var text = newsArticleMessage.ArticleText;
            }
            else if (newsArticleMessage.ArticleType == 1)
            {
                var bytes = Convert.FromBase64String(newsArticleMessage.ArticleText);
            }
        }

        public void RequestNewsArticle(string providerCode, string articleId, string path)
        {
            IbClient.ClientSocket.ReqNewsArticle(NEWS_ARTICLE_ID, providerCode, articleId, new List<TagValue>());
        }

        public void ClearArticleText()
        {
        }

        public void RequestNewsTicks(Contract contract)
        {
            ClearTickNews();
            IbClient.ClientSocket.ReqMktData(TICK_NEWS_ID, contract, "mdoff,292", false, false, new List<TagValue>());
        }

        public void ClearTickNews()
        {
            rowCountTickNewsGrid = 0;
        }

        public void CancelTickNews()
        {
            IbClient.ClientSocket.CancelMktData(TICK_NEWS_ID);
            ClearTickNews();
        }

        public void ClearNewsProviders()
        {
        }

        public void RequestNewsProviders()
        {
            ClearNewsProviders();
            IbClient.ClientSocket.ReqNewsProviders();
        }

        public void RequestHistoricalNews(int conId,
                                          string providerCodes,
                                          string startDateTime,
                                          string endDateTime,
                                          int totalResults)
        {
            ClearHistoricalNews();
            IbClient.ClientSocket.ReqHistoricalNews(HISTORICAL_NEWS_ID,
                                                    conId,
                                                    providerCodes,
                                                    startDateTime,
                                                    endDateTime,
                                                    totalResults,
                                                    new List<TagValue>());
        }

        public void ClearHistoricalNews()
        {
            rowCountHistoricalNewsGrid = 0;
        }
    }
}