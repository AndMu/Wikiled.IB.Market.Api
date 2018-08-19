using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Wikiled.IB.Market.Api.Client.Messages;

namespace Wikiled.IB.Market.Api.Client.DataManagers
{
    public class NewsManager : SingleBaseDataManager<NewsArticleMessage>
    {
        public NewsManager(IBClient ibClient, ILoggerFactory loggerFactory)
            : base(ibClient, loggerFactory)
        {
            ibClient.NewsArticle += OnCompleted;
        }

        protected override int RequestOffset => MessageIdConstants.News;

        public IObservable<NewsArticleMessage> Request(string providerCode, string articleId)
        {
            var stream = Construct();
            //{
            //    if (newsArticleMessage.ArticleType == 0)
            //    {
            //        var text = newsArticleMessage.ArticleText;
            //    }
            //    else if (newsArticleMessage.ArticleType == 1)
            //    {
            //        var bytes = Convert.FromBase64String(newsArticleMessage.ArticleText);
            //    }
            IbClient.ClientSocket.ReqNewsArticle(RequestId, providerCode, articleId, new List<TagValue>());
            return stream;
        }

        public override void Dispose()
        {
            IbClient.NewsArticle -= OnCompleted;
            base.Dispose();
        }
    }
}